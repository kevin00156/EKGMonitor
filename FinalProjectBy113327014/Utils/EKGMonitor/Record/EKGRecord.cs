using ScottPlot.MultiplotLayouts;
using ScottPlot.Plottables;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyForms.Utils.EKGMonitor.Base;
using System.Threading;
using Newtonsoft.Json;

namespace MyForms.Utils.EKGMonitor.Record
{
    public class EKGRecord : RecordBase, IDisposable
    {
        //constants
        public const int LEADS_COUNT = 6;
        public const int SHOW_SAMPLE_WIDTH = 1000;
        public const int BUFFER_THRESHOLD = 50; // 緩存閾值，達到此數量時自動保存
        public static readonly string[] LEADS_NAME = new string[LEADS_COUNT]
        {
            "I", "II", "III", "aVR", "aVL", "aVF"
        };
        
        [JsonIgnore] // 不序列化顏色陣列
        public readonly Color[] LEADS_COLOR = new Color[LEADS_COUNT]
        {
            Colors.Green,
            Colors.Green,
            Colors.DarkCyan,
            Colors.Red,
            Colors.CornflowerBlue,
            Colors.Orange,
        };

        //internal vars
        [JsonIgnore] // 不序列化內部變數
        int _sampleCount = 0;
        [JsonIgnore]
        Random _random = new Random();

        // 資料緩存
        [JsonIgnore] // 不序列化緩存資料
        private Dictionary<string, Queue<float>> _signals = new();
        [JsonIgnore]
        private readonly object _dataLock = new object(); // 用於保護 _signals 的鎖

        // 優化：重用List以避免頻繁的記憶體分配
        [JsonIgnore]
        private List<float> _reusableLogDatas = new List<float>();

        // 檔案相關
        [JsonIgnore] // 不序列化檔案相關物件
        private StreamWriter _csvWriter;
        [JsonIgnore]
        private readonly object _fileLock = new object(); // 用於保護檔案寫入的鎖
        
        [JsonIgnore] // 不序列化寫入狀態
        private bool _isWritingToFile = false;
        
        [JsonIgnore] // 不序列化讀取模式狀態
        private bool _isReadOnlyMode = false; // 新增：標記是否為唯讀模式

        // 添加取消令牌支持
        [JsonIgnore] // 不序列化取消令牌
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        [JsonIgnore]
        private bool _disposed = false;

        //plots
        [JsonIgnore] // 不序列化繪圖物件
        public Multiplot multiplot = new Multiplot();
        [JsonIgnore]
        private List<DataLogger> dataLoggers = new List<DataLogger>();

        //輸出數據
        [JsonIgnore] // 不序列化生理數據
        private Dictionary<string, float> _physiologicalDatas;
        
        [JsonIgnore] // 不序列化生理數據
        public Dictionary<string, float> PhysiologicalDatas { 
            get {
                return _physiologicalDatas; 
            } 
            private set
            {
                _physiologicalDatas = value;
            } 
        }

        /// <summary>
        /// 獲取當前緩存中的資料數量
        /// </summary>
        [JsonIgnore] // 不序列化緩存數據計數
        public int BufferedDataCount
        {
            get
            {
                if (_signals == null) return 0;
                lock (_dataLock)
                {
                    return _signals.Values.FirstOrDefault()?.Count ?? 0;
                }
            }
        }

        /// <summary>
        /// 檢查是否正在寫入檔案
        /// </summary>
        [JsonIgnore] // 不序列化寫入狀態
        public bool IsWritingToFile => _isWritingToFile;

        /// <summary>
        /// 檢查是否為唯讀模式
        /// </summary>
        [JsonIgnore] // 不序列化讀取模式狀態
        public bool IsReadOnlyMode => _isReadOnlyMode;

        void _InitializeMutiPlot()
        {
            multiplot.AddPlots(LEADS_COUNT);
            multiplot.Layout = new Rows();
            multiplot.SharedAxes.ShareX(multiplot.GetPlots());

            for (int i = 0; i < LEADS_COUNT; i++)
            { 
                Plot plot = multiplot.GetPlot(i);
                Color plotColor = LEADS_COLOR[i];
                plot.Axes.Left.Label.Text = LEADS_NAME[i];

                // 先添加 DataLogger，稍後再設定 grid
                dataLoggers.Add(plot.Add.DataLogger());
            }
            
            // 在 multiplot 設定完成後，統一設定所有 plot 的 grid
            for (int i = 0; i < LEADS_COUNT; i++)
            {
                Plot plot = multiplot.GetPlot(i);
                
                // 強制設定 grid 樣式
                plot.Grid.MajorLineColor = Colors.Red.WithOpacity(0.3);
                plot.Grid.MajorLineWidth = 2;
                plot.Grid.MinorLineColor = Colors.Black.WithOpacity(0.1);
                plot.Grid.MinorLineWidth = 1;
                
            }
            
            // 設定 DataLogger 樣式
            for (int i = 0; i < LEADS_COUNT; i++)
            {
                DataLogger dataLogger = dataLoggers[i];
                Color color = LEADS_COLOR[i];
                // 移除 ViewJump 調用以防止自動重置行為
                dataLogger.ViewJump(SHOW_SAMPLE_WIDTH);
                
                dataLogger.LineColor = color;
            }

            // 初始化 signals dictionary
            foreach (string plotName in LEADS_NAME)
            {
                _signals.Add(plotName, new Queue<float>());
            }
            
        }

        void _InitializePhysiologicalDatas()
        {
            _physiologicalDatas = new();
            foreach (string leadName in LEADS_NAME)
            {
                _physiologicalDatas.Add(leadName, 0);
            }
        }


        public EKGRecord(string savedFilePath, string name = "")
        {
            _savedFilePath = savedFilePath;
            _name = name;

            // 創建目錄（如果不存在）
            string directoryPath = Path.GetDirectoryName(savedFilePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // 開啟CSV檔案並保持開啟狀態
            try
            {
                _csvWriter = new StreamWriter(savedFilePath, false, Encoding.UTF8);
                _csvWriter.AutoFlush = true; // 確保數據立即寫入硬碟

                // 寫入CSV表頭
                _WriteHeader();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"無法開啟檔案 {savedFilePath}: {ex.Message}");
                throw;
            }

            _InitializeMutiPlot();
            _InitializePhysiologicalDatas();
        }

        /// <summary>
        /// 用於讀取模式的私有建構子
        /// </summary>
        /// <param name="savedFilePath">檔案路徑</param>
        /// <param name="name">記錄名稱</param>
        /// <param name="isReadOnly">是否為唯讀模式</param>
        private EKGRecord(string savedFilePath, string name, bool isReadOnly)
        {
            _savedFilePath = savedFilePath;
            _name = name;
            _isReadOnlyMode = isReadOnly;

            if (!isReadOnly)
            {
                // 如果不是唯讀模式，執行原本的寫入模式初始化
                // 創建目錄（如果不存在）
                string directoryPath = Path.GetDirectoryName(savedFilePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // 開啟CSV檔案並保持開啟狀態
                try
                {
                    _csvWriter = new StreamWriter(savedFilePath, false, Encoding.UTF8);
                    _csvWriter.AutoFlush = true; // 確保數據立即寫入硬碟

                    // 寫入CSV表頭
                    _WriteHeader();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"無法開啟檔案 {savedFilePath}: {ex.Message}");
                    throw;
                }
            }

            _InitializeMutiPlot();
            _InitializePhysiologicalDatas();
        }

        /// <summary>
        /// 用於 JSON 反序列化的建構函式
        /// </summary>
        [JsonConstructor]
        public EKGRecord(
            [JsonProperty("Name")] string name,
            [JsonProperty("SavedFilePath")] string savedFilePath,
            [JsonProperty("TimeStamp")] DateTime timeStamp)
        {
            _name = name;
            _savedFilePath = savedFilePath;
            _timeStamp = timeStamp;
            
            // 設定為唯讀模式（從 JSON 反序列化的記錄預設為唯讀）
            _isReadOnlyMode = true;
            _isWritingToFile = false;

            // 初始化必要的物件（但不開啟檔案寫入）
            _signals = new Dictionary<string, Queue<float>>();
            _dataLock = new object();
            _fileLock = new object();
            _cancellationTokenSource = new CancellationTokenSource();
            
            // 初始化繪圖相關物件
            _InitializeMutiPlot();
            
            // 初始化生理數據
            _InitializePhysiologicalDatas();
        }

        public void AddData(Dictionary<string, float> datas)
        {
            // 如果是唯讀模式，不允許添加數據
            if (_isReadOnlyMode)
            {
                throw new InvalidOperationException("無法在唯讀模式下添加數據");
            }

            _sampleCount++;

            // 優化：重用List，避免每次創建新物件
            _reusableLogDatas.Clear();

            // 線程安全地添加數據到緩存
            lock (_dataLock)
            {
                // 按照 LEADS_NAME 的順序添加數據到 dataLoggers 和緩存
                foreach (string leadName in LEADS_NAME)
                {
                    float setValue;
                    if (datas.TryGetValue(leadName, out float value))
                    {
                        setValue = value;
                    }
                    else
                    {
                        setValue = 0;
                    }

                    // 添加到緩存隊列
                    _signals[leadName].Enqueue(setValue);
                    _reusableLogDatas.Add(setValue);
                }
            }

            _AddDataToPlots(_reusableLogDatas);

            // 檢查是否需要自動保存
            if (BufferedDataCount >= BUFFER_THRESHOLD && !IsWritingToFile)
            {
                // 異步保存，不阻塞當前線程，並添加異常處理
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await SaveBufferedDataToFileAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"自動保存時發生錯誤: {ex.Message}");
                    }
                });
            }
        }

        public void ChangeAutoAxisManage(bool autoManage)
        {
            foreach (var dataLogger in dataLoggers)
            {
                dataLogger.ManageAxisLimits = autoManage;
            }
        }

        private void _AddDataToPlots(List<float> data)
        {
            for (int i = 0; i < data.Count && i < dataLoggers.Count; i++)
            {
                dataLoggers[i].Add(data[i]);
            }
            _CalculatePhysiologicalData();
        }

        private void _WriteHeader()
        {
            try
            {
                // 建立表頭：Sample + 各通道名稱
                List<string> headers = new List<string>();
                headers.Add($"Start Time: {TimeStamp}");
                headers.AddRange(LEADS_NAME);

                _csvWriter.WriteLine(string.Join(",", headers));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"無法寫入表頭到檔案: {ex.Message}");
            }
        }

        public static void ReadDataFromFile(string savedFilePath, out EKGRecord ekgRecord)
        {
            ekgRecord = null;

            try
            {
                // 檢查檔案是否存在
                if (!File.Exists(savedFilePath))
                {
                    throw new FileNotFoundException($"檔案不存在: {savedFilePath}");
                }

                // 創建唯讀模式的EKGRecord物件
                string fileName = Path.GetFileNameWithoutExtension(savedFilePath);
                ekgRecord = new EKGRecord(savedFilePath, fileName, true);

                // 讀取並解析檔案
                using (StreamReader reader = new StreamReader(savedFilePath, Encoding.UTF8))
                {
                    // 讀取表頭
                    string headerLine = reader.ReadLine();
                    if (string.IsNullOrEmpty(headerLine))
                    {
                        throw new InvalidDataException("檔案格式錯誤：無法讀取表頭");
                    }

                    // 解析表頭並設定開始時間
                    DateTime startTime = ekgRecord._ParseHeader(headerLine);
                    ekgRecord._timeStamp = startTime;

                    // 讀取數據行
                    string dataLine;
                    int lineNumber = 2; // 從第二行開始計算
                    List<Dictionary<string, float>> allData = new List<Dictionary<string, float>>();

                    while ((dataLine = reader.ReadLine()) != null)
                    {
                        try
                        {
                            var parsedData = ekgRecord._ParseDataLine(dataLine, lineNumber);
                            if (parsedData != null)
                            {
                                allData.Add(parsedData);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"解析第 {lineNumber} 行時發生錯誤: {ex.Message}");
                            // 繼續處理下一行，不中斷整個讀取過程
                        }
                        lineNumber++;
                    }

                    // 將所有數據添加到圖表中
                    ekgRecord._LoadDataToPlots(allData);
                    
                    Console.WriteLine($"成功讀取檔案 {savedFilePath}");
                    Console.WriteLine($"開始時間: {startTime:yyyy-MM-dd HH:mm:ss}");
                    Console.WriteLine($"總計讀取 {allData.Count} 筆數據");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"讀取檔案時發生錯誤: {ex.Message}");
                ekgRecord?.Dispose();
                ekgRecord = null;
                throw;
            }
        }

        /// <summary>
        /// 解析CSV表頭，提取開始時間
        /// </summary>
        /// <param name="headerLine">表頭行</param>
        /// <returns>開始時間</returns>
        private DateTime _ParseHeader(string headerLine)
        {
            try
            {
                // 表頭格式: "Start Time: yyyy-MM-dd HH:mm:ss,I,II,III,aVR,aVL,aVF"
                string[] parts = headerLine.Split(',');
                if (parts.Length < LEADS_COUNT + 1)
                {
                    throw new InvalidDataException($"表頭格式錯誤：期望 {LEADS_COUNT + 1} 個欄位，實際 {parts.Length} 個");
                }

                // 提取時間部分
                string timePart = parts[0];
                if (!timePart.StartsWith("Start Time: "))
                {
                    throw new InvalidDataException("表頭格式錯誤：找不到開始時間");
                }

                string timeString = timePart.Substring("Start Time: ".Length);
                if (DateTime.TryParse(timeString, out DateTime startTime))
                {
                    return startTime;
                }
                else
                {
                    throw new InvalidDataException($"無法解析開始時間: {timeString}");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidDataException($"解析表頭時發生錯誤: {ex.Message}");
            }
        }

        /// <summary>
        /// 解析數據行
        /// </summary>
        /// <param name="dataLine">數據行</param>
        /// <param name="lineNumber">行號</param>
        /// <returns>解析後的數據字典</returns>
        private Dictionary<string, float> _ParseDataLine(string dataLine, int lineNumber)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dataLine))
                {
                    return null;
                }

                string[] values = dataLine.Split(',');
                if (values.Length < LEADS_COUNT + 1)
                {
                    throw new InvalidDataException($"第 {lineNumber} 行格式錯誤：期望 {LEADS_COUNT + 1} 個值，實際 {values.Length} 個");
                }

                Dictionary<string, float> data = new Dictionary<string, float>();

                // 跳過第一個值（Sample number），從第二個值開始解析
                for (int i = 0; i < LEADS_COUNT; i++)
                {
                    if (i + 1 < values.Length)
                    {
                        if (float.TryParse(values[i + 1], out float value))
                        {
                            data[LEADS_NAME[i]] = value;
                        }
                        else
                        {
                            Console.WriteLine($"第 {lineNumber} 行第 {i + 2} 個值無法解析為浮點數: {values[i + 1]}，使用 0 代替");
                            data[LEADS_NAME[i]] = 0f;
                        }
                    }
                    else
                    {
                        data[LEADS_NAME[i]] = 0f;
                    }
                }

                return data;
            }
            catch (Exception ex)
            {
                throw new InvalidDataException($"解析第 {lineNumber} 行時發生錯誤: {ex.Message}");
            }
        }

        /// <summary>
        /// 將讀取的數據載入到圖表中
        /// </summary>
        /// <param name="allData">所有數據</param>
        private void _LoadDataToPlots(List<Dictionary<string, float>> allData)
        {
            try
            {
                foreach (var data in allData)
                {
                    _sampleCount++;
                    List<float> logDatas = new List<float>();

                    // 按照 LEADS_NAME 的順序添加數據
                    foreach (string leadName in LEADS_NAME)
                    {
                        float value = data.ContainsKey(leadName) ? data[leadName] : 0f;
                        logDatas.Add(value);
                    }

                    // 添加到圖表
                    for (int i = 0; i < logDatas.Count && i < dataLoggers.Count; i++)
                    {
                        dataLoggers[i].Add(logDatas[i]);
                    }
                }

                // 重新計算生理數據
                _CalculatePhysiologicalData();

                Console.WriteLine($"成功載入 {allData.Count} 筆數據到圖表中");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"載入數據到圖表時發生錯誤: {ex.Message}");
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            try
            {
                // 標記為已釋放，防止新的操作
                _disposed = true;

                // 取消所有進行中的異步操作
                _cancellationTokenSource?.Cancel();

                // 如果不是唯讀模式，才需要保存剩餘的緩存數據
                if (!_isReadOnlyMode)
                {
                    // 使用同步方法保存剩餘的緩存數據，避免死鎖
                    Console.WriteLine("正在保存剩餘的緩存數據...");
                    SaveBufferedDataToFile();

                    // 關閉CSV檔案
                    lock (_fileLock)
                    {
                        try
                        {
                            _csvWriter?.Flush();
                            _csvWriter?.Close();
                            _csvWriter?.Dispose();
                            _csvWriter = null;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"關閉CSV檔案時發生錯誤: {ex.Message}");
                        }
                    }
                }

                // 清理其他資源
                lock (_dataLock)
                {
                    _signals.Clear();
                }

                dataLoggers.Clear();

                // 釋放取消令牌
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;

                if (_isReadOnlyMode)
                {
                    Console.WriteLine($"EKGRecord '{_name}' 資源已釋放（唯讀模式）");
                }
                else
                {
                    Console.WriteLine($"EKGRecord '{_name}' 資源已釋放，所有數據已保存到 {_savedFilePath}");
                }
                Console.WriteLine($"記錄期間：{TimeStamp:yyyy-MM-dd HH:mm:ss} ~ {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                Console.WriteLine($"總計 sample 數：{_sampleCount}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"釋放資源時發生錯誤: {ex.Message}");
            }
        }

        /// <summary>
        /// 異步將緩存的數據寫入到CSV檔案
        /// </summary>
        /// <returns></returns>
        public async Task SaveBufferedDataToFileAsync()
        {
            // 如果是唯讀模式，不允許寫入檔案
            if (_isReadOnlyMode)
            {
                Console.WriteLine("唯讀模式下無法寫入檔案");
                return;
            }

            if (_isWritingToFile || _disposed)
            {
                Console.WriteLine("檔案寫入正在進行中或物件已釋放，跳過此次寫入");
                return;
            }

            try
            {
                await Task.Run(() =>
                {
                    // 檢查取消令牌
                    _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                    lock (_fileLock)
                    {
                        if (_isWritingToFile || _disposed)
                        {
                            Console.WriteLine("檔案寫入正在進行中或物件已釋放，跳過此次寫入");
                            return;
                        }

                        _isWritingToFile = true;

                        try
                        {
                            // 複製當前緩存的數據（避免長時間鎖定數據）
                            Dictionary<string, Queue<float>> dataToWrite;
                            int currentSampleCount;

                            lock (_dataLock)
                            {
                                // 檢查是否有數據需要寫入
                                if (_signals.Values.All(q => q.Count == 0))
                                {
                                    return;
                                }

                                // 複製數據到臨時變數
                                dataToWrite = new Dictionary<string, Queue<float>>();
                                foreach (var kvp in _signals)
                                {
                                    dataToWrite[kvp.Key] = new Queue<float>(kvp.Value);
                                    kvp.Value.Clear(); // 清空原始隊列
                                }
                                currentSampleCount = _sampleCount;
                            }

                            // 再次檢查取消令牌
                            _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                            // 寫入數據到檔案（不持有數據鎖）
                            _WriteDataToFile(dataToWrite, currentSampleCount);

                            Console.WriteLine($"成功寫入 {dataToWrite.Values.FirstOrDefault()?.Count ?? 0} 筆資料到檔案");
                        }
                        catch (OperationCanceledException)
                        {
                            Console.WriteLine("檔案寫入操作已取消");
                            throw;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"異步寫入檔案時發生錯誤: {ex.Message}");
                        }
                        finally
                        {
                            _isWritingToFile = false;
                        }
                    }
                }, _cancellationTokenSource.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("異步保存操作已取消");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"異步保存操作發生錯誤: {ex.Message}");
            }
        }

        /// <summary>
        /// 同步將緩存的數據寫入到CSV檔案（供 Dispose 使用）
        /// </summary>
        private void SaveBufferedDataToFile()
        {
            // 如果是唯讀模式，不允許寫入檔案
            if (_isReadOnlyMode || _disposed)
            {
                return;
            }

            lock (_fileLock)
            {
                if (_isWritingToFile || _disposed)
                {
                    return;
                }

                _isWritingToFile = true;

                try
                {
                    // 複製當前緩存的數據（避免長時間鎖定數據）
                    Dictionary<string, Queue<float>> dataToWrite;
                    int currentSampleCount;

                    lock (_dataLock)
                    {
                        // 檢查是否有數據需要寫入
                        if (_signals.Values.All(q => q.Count == 0))
                        {
                            return;
                        }

                        // 複製數據到臨時變數
                        dataToWrite = new Dictionary<string, Queue<float>>();
                        foreach (var kvp in _signals)
                        {
                            dataToWrite[kvp.Key] = new Queue<float>(kvp.Value);
                            kvp.Value.Clear(); // 清空原始隊列
                        }
                        currentSampleCount = _sampleCount;
                    }

                    // 寫入數據到檔案（不持有數據鎖）
                    _WriteDataToFile(dataToWrite, currentSampleCount);

                    Console.WriteLine($"同步寫入 {dataToWrite.Values.FirstOrDefault()?.Count ?? 0} 筆資料到檔案");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"同步寫入檔案時發生錯誤: {ex.Message}");
                }
                finally
                {
                    _isWritingToFile = false;
                }
            }
        }

        private void _CalculatePhysiologicalData()
        {
            //TODO: 替換成真實計算生理數據使用的公式
            for (int i = 0; i < LEADS_COUNT; i++)
            {
                // 優化：直接使用float運算，避免Convert.ToSingle
                _physiologicalDatas[LEADS_NAME[i]] = (float)(_random.NextDouble() * 150);
            }
        }

        /// <summary>
        /// 將指定的數據寫入到CSV檔案
        /// </summary>
        /// <param name="dataToWrite">要寫入的數據</param>
        /// <param name="endSampleCount">結束的樣本數</param>
        private void _WriteDataToFile(Dictionary<string, Queue<float>> dataToWrite, int endSampleCount)
        {
            try
            {
                if (_csvWriter == null)
                {
                    Console.WriteLine("CSV檔案未開啟，無法寫入數據");
                    return;
                }

                // 計算起始樣本數
                int dataCount = dataToWrite.Values.FirstOrDefault()?.Count ?? 0;
                int startSampleCount = endSampleCount - dataCount + 1;

                // 逐行寫入數據
                for (int i = 0; i < dataCount; i++)
                {
                    List<string> values = new();
                    values.Add((startSampleCount + i).ToString()); // Sample number

                    // 按照 LEADS_NAME 的順序添加數據
                    foreach (string leadName in LEADS_NAME)
                    {
                        if (dataToWrite.ContainsKey(leadName) && dataToWrite[leadName].Count > 0)
                        {
                            values.Add(dataToWrite[leadName].Dequeue().ToString("F3"));
                        }
                        else
                        {
                            values.Add("0.000");
                        }
                    }

                    _csvWriter.WriteLine(string.Join(",", values));
                }

                _csvWriter.Flush(); // 確保數據寫入磁盤
            }
            catch (Exception ex)
            {
                Console.WriteLine($"寫入CSV檔案時發生錯誤: {ex.Message}");
            }
        }
    }
}

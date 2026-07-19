using System;

namespace MyForms.Utils.EKGMonitor.Signal
{
    /// <summary>
    /// 訊號獲取委派
    /// 定義外部實作訊號獲取的方法簽章
    /// </summary>
    /// <param name="currentTime">目前時間</param>
    /// <returns>訊號值</returns>
    public delegate double SignalProvider(double currentTime);

    /// <summary>
    /// 真實訊號類別
    /// 允許外部透過委派方法提供訊號實作
    /// </summary>
    public class RealSignal : SignalBase
    {
        private readonly SignalProvider _signalProvider;
        private readonly string _signalName;

        /// <summary>
        /// 訊號名稱
        /// </summary>
        public string SignalName => _signalName;

        /// <summary>
        /// 訊號提供者委派
        /// </summary>
        public SignalProvider Provider => _signalProvider;

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="signalProvider">訊號提供者委派</param>
        /// <param name="signalName">訊號名稱</param>
        /// <exception cref="ArgumentNullException">當 signalProvider 為 null 時拋出</exception>
        public RealSignal(SignalProvider signalProvider, string signalName = "RealSignal")
        {
            _signalProvider = signalProvider ?? throw new ArgumentNullException(nameof(signalProvider), "訊號提供者不能為 null");
            _signalName = signalName ?? "RealSignal";
        }

        /// <summary>
        /// 產生下一個訊號值
        /// 透過委派方法獲取訊號值
        /// </summary>
        /// <returns>訊號值</returns>
        /// <exception cref="InvalidOperationException">當委派方法執行失敗時拋出</exception>
        public override double Next()
        {
            try
            {
                double signal = _signalProvider(_time);
                _time++;
                return signal;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"訊號提供者 '{_signalName}' 執行失敗: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 建立靜態工廠方法 - 從函數建立 RealSignal
        /// </summary>
        /// <param name="signalFunction">訊號函數</param>
        /// <param name="signalName">訊號名稱</param>
        /// <returns>RealSignal 實例</returns>
        public static RealSignal FromFunction(Func<double, double> signalFunction, string signalName = "FunctionSignal")
        {
            if (signalFunction == null)
                throw new ArgumentNullException(nameof(signalFunction));

            return new RealSignal(time => signalFunction(time), signalName);
        }

        /// <summary>
        /// 建立靜態工廠方法 - 從常數值建立 RealSignal
        /// </summary>
        /// <param name="constantValue">常數值</param>
        /// <param name="signalName">訊號名稱</param>
        /// <returns>RealSignal 實例</returns>
        public static RealSignal FromConstant(double constantValue, string signalName = "ConstantSignal")
        {
            return new RealSignal(time => constantValue, signalName);
        }

        /// <summary>
        /// 建立靜態工廠方法 - 從陣列資料建立 RealSignal
        /// </summary>
        /// <param name="data">資料陣列</param>
        /// <param name="signalName">訊號名稱</param>
        /// <param name="loopWhenEnd">當資料結束時是否循環</param>
        /// <returns>RealSignal 實例</returns>
        public static RealSignal FromArray(double[] data, string signalName = "ArraySignal", bool loopWhenEnd = true)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentException("資料陣列不能為空", nameof(data));

            return new RealSignal(time =>
            {
                int index = (int)time;
                if (loopWhenEnd)
                {
                    index = index % data.Length;
                }
                else
                {
                    if (index >= data.Length)
                        return 0.0; // 超出範圍時返回 0
                }
                return data[index];
            }, signalName);
        }
    }
} 
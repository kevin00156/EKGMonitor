using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using MyForms.Utils.EKGMonitor.Signal;
using MyForms.Utils.EKGMonitor.Record;
using MyForms.Utils.EKGMonitor.Forms;
using MyForms.Utils.EKGMonitor.Person;

namespace MyForms.TabControls.Projects
{
    public partial class PortableEKGMonitor : UserControl
    {
        #region 全局變數
        private int SCAN_TIME = 10;//ms 
        private int REFRESH_LABEL_INTERVAL = 1000;
        private string _personFilePath = "config/PersonData.json";
        private SignalType[] _signalTypes =
        {
                SignalType.NOISE,
                SignalType.SINWAVEWITHNOISE,
                SignalType.SINWAVE,
                SignalType.SINWAVEWITHNOISE,
                SignalType.SINWAVE,
                SignalType.NOISE
            };
        private List<Label> labelsPhysiologicalData = new();
        MedicalDataContainer _medicalDataContainer = new MedicalDataContainer();
        MedicalDataContainer medicalDataContainer => _medicalDataContainer;
        private User _currentUser;
        private Doctor _currentDoctor;

        List<SignalBase> _signals = new();
        private EKGRecord _ekgRecord;

        // 優化：重用Dictionary以避免頻繁的記憶體分配
        private Dictionary<string, float> _reusableSignals = new Dictionary<string, float>();

        // 優化：重用List以避免在AddData中頻繁分配
        private List<float> _reusableLogDatas = new List<float>();

        private bool _isEKGRecordExist = false;
        public EKGRecord ekgRecord
        {
            get => _ekgRecord;
            set
            {
                _ekgRecord = value;
                formsPlotEKG.Multiplot = _ekgRecord?.multiplot;
                formsPlotEKG.Update();
                _UpdateStatusStrip();
                _isEKGRecordExist = true;
            }
        }

        Timer _timer = new Timer();
        Timer _refreshLabelTimer = new Timer();

        #endregion

        #region 初始化行為
        public PortableEKGMonitor()
        {
            InitializeComponent();
            InitializeControlState();
            InitializePersonData();
        }

        private void InitializePersonData()
        {
            MedicalDataContainer.LoadFromFile(_personFilePath, out _medicalDataContainer);
        }

        private void InitializeControlState()
        {
            startRecordToolStripMenuItem.Enabled = false;
            stopRecordToolStripMenuItem.Enabled = false;
            newRecordToolStripMenuItem.Enabled = false;
            userLoadRecordToolStripMenuItem.Enabled = false;
            patientRecordManagementToolStripMenuItem.Enabled = false;
        }

        private void EKGMonitor_Load(object sender, EventArgs e)
        {
            _ekgRecord = new("log/mock.csv");
            InitializeSignals();
            InitializeScanTimer();
            InitializePhysiologicalDataLabels();
            InitializeRefreshLabelTimer();
            InitializeUIControls();
        }

        private void InitializeUIControls()
        {
            checkBoxAutoManageAxisLimit.Checked = true;
        }

        private void InitializeSignals()
        {
            for (int i = 0; i < EKGRecord.LEADS_COUNT; i++)
            {
                SignalBase signal = SignalFactory.CreateSignal(
                    signalType: _signalTypes[i],
                    randomSeed: i,
                    noiseRange: i + 1,
                    amplitude: 2,
                    frequency: i
                );
                _signals.Add(signal);
            }
        }

        private void InitializePhysiologicalDataLabels()
        {
            labelsPhysiologicalData.Add(labelPhysiologicalData1);
            labelsPhysiologicalData.Add(labelPhysiologicalData2);
            labelsPhysiologicalData.Add(labelPhysiologicalData3);
            labelsPhysiologicalData.Add(labelPhysiologicalData4);
            labelsPhysiologicalData.Add(labelPhysiologicalData5);
            labelsPhysiologicalData.Add(labelPhysiologicalData6);

            for (int i = 0; i < labelsPhysiologicalData.Count; i++)
            {
                var label = labelsPhysiologicalData[i];
                var color = ekgRecord.LEADS_COLOR[i].ToSDColor();
                label.ForeColor = color;
                label.AutoSize = true;
                label.Font = new Font("Arial", 24);
            }
        }

        private void InitializeScanTimer()
        {
            _timer.Interval = SCAN_TIME;
            _timer.Tick += TimerScanning;
        }

        private void InitializeRefreshLabelTimer()
        {
            RefreshLabelTimer_Tick(null, null);//起始時先刷新一次Timer
            _refreshLabelTimer.Interval = REFRESH_LABEL_INTERVAL;
            _refreshLabelTimer.Tick += RefreshLabelTimer_Tick;
        }

        #endregion

        #region Timer_Tick
        private void RefreshLabelTimer_Tick(object sender, EventArgs e)
        {
            var physiologicalDatas = ekgRecord.PhysiologicalDatas;
            for (int i = 0; i < EKGRecord.LEADS_NAME.Length && i < labelsPhysiologicalData.Count; i++)
            {
                string leadName = EKGRecord.LEADS_NAME[i];
                var label = labelsPhysiologicalData[i];

                if (physiologicalDatas.TryGetValue(leadName, out float value))
                {
                    label.Text = $"{leadName}: {value:F2}"; // 使用格式化字串而非ToString
                }
                else
                {
                    label.Text = $"{leadName}: 0.00";
                }
            }
        }

        void TimerScanning(object sender, EventArgs e)
        {
            // 優化：重用Dictionary，避免每次創建新物件
            _reusableSignals.Clear();

            for (int i = 0; i < _signals.Count; i++)
            {
                var signal = _signals[i];
                double signalValue = signal.Next();
                _reusableSignals[EKGRecord.LEADS_NAME[i]] = (float)signalValue;
            }

            ekgRecord.AddData(_reusableSignals);
            formsPlotEKG.Refresh();
        }

        /// <summary>
        /// 手動觸發保存緩存數據到檔案
        /// </summary>
        public async Task ManualSaveAsync()
        {
            try
            {
                int bufferedCount = ekgRecord.BufferedDataCount;
                if (bufferedCount > 0)
                {
                    Console.WriteLine($"手動保存 {bufferedCount} 筆緩存資料...");
                    await ekgRecord.SaveBufferedDataToFileAsync();
                    Console.WriteLine("手動保存完成！");
                }
                else
                {
                    Console.WriteLine("沒有需要保存的緩存資料");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"手動保存時發生錯誤: {ex.Message}");
            }
        }

        /// <summary>
        /// 獲取當前緩存的資料數量
        /// </summary>
        public int GetBufferedDataCount()
        {
            return ekgRecord?.BufferedDataCount ?? 0;
        }
        #endregion

        #region ToolStripMenu相關功能
        private void LoadRecordFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV files (*.csv)|*.csv";
            DialogResult result = openFileDialog.ShowDialog();
            if (result != DialogResult.OK)
            {
                MessageBox.Show("未選擇任何檔案。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string filePath = openFileDialog.FileName;
            EKGRecord newEKGRecord;
            EKGRecord.ReadDataFromFile(filePath, out newEKGRecord);
            ekgRecord = newEKGRecord;

            formsPlotEKG.Multiplot = ekgRecord.multiplot;
            formsPlotEKG.Refresh();
            _AfterLoadRecord(sender, e);
        }

        private void StartRecord(object sender, EventArgs e)
        {
            formsPlotEKG.Multiplot = ekgRecord.multiplot;

            _timer.Enabled = true;
            _timer.Start();
            _refreshLabelTimer.Enabled = true;
            _refreshLabelTimer.Start();
        }

        private void StopRecord(object sender, EventArgs e)
        {
            _timer.Stop();
            _refreshLabelTimer.Stop();
        }

        private void NewRecord(object sender, EventArgs e)
        {
            if (_isEKGRecordExist == true)
            {
                DialogResult result = MessageBox.Show("目前紀錄將被保存後清除。是否繼續？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.No)
                {
                    return;
                }
            }


            string recordName = $"{_currentUser}_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}";
            string csvFilePath = $"log/{recordName}.csv";
            ekgRecord = new(csvFilePath, recordName);
            _currentUser.NewRecord(ekgRecord);
            CheckCurrentEKGRecordIsReadOnly(sender, e);
            _AfterNewRecord(sender, e);
            StartRecord(sender, e);
        }

        private void UserRegister(object sender, EventArgs e)
        {
            PersonRegisterForm<User>.CreatePersonDelegate userFactory =
                (firstName, lastName, id, sexual) => new User(firstName, lastName, id, sexual);
            using (PersonRegisterForm<User> userRegisterForm = new(userFactory))
            {
                DialogResult result = userRegisterForm.ShowDialog();

                if (result == DialogResult.OK)
                {
                    medicalDataContainer.AddUser(userRegisterForm.NewPerson);
                }
            }
        }

        private void DoctorRegister(object sender, EventArgs e)
        {
            PersonRegisterForm<Doctor>.CreatePersonDelegate doctorFactory =
                (firstName, lastName, id, sexual) => new Doctor(firstName, lastName, id, sexual);
            using (PersonRegisterForm<Doctor> doctorRegisterForm = new(doctorFactory))
            {
                DialogResult result = doctorRegisterForm.ShowDialog();

                if (result == DialogResult.OK)
                {
                    medicalDataContainer.AddDoctor(doctorRegisterForm.NewPerson);
                }
            }
        }

        private void SelectUser(object sender, EventArgs e)
        {

            using (SelectPersonForm<User> selectUserForm = new(medicalDataContainer.Users))
            {
                DialogResult result = selectUserForm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    _currentUser = selectUserForm.SelectedPerson;
                    _AfterSelectUser(sender, e);
                }
            }
        }

        private void SelectDoctor(object sender, EventArgs e)
        {

            using (SelectPersonForm<Doctor> selectDoctorForm = new(medicalDataContainer.Doctors))
            {
                DialogResult result = selectDoctorForm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    _currentDoctor = selectDoctorForm.SelectedPerson;
                    _AfterSelectDoctor(sender, e);
                }
            }
        }

        private void DoctorAddPatient(object sender, EventArgs e)
        {

            using (SelectPersonForm<User> selectUserForm = new(medicalDataContainer.Users))
            {
                DialogResult result = selectUserForm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    _currentDoctor.AddPatient(selectUserForm.SelectedPerson);
                }
            }
        }

        private void DoctorDeletePatient(object sender, EventArgs e)
        {
            using (SelectPersonForm<User> selectUserForm = new(_currentDoctor.Patients))
            {
                DialogResult result = selectUserForm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    _currentDoctor.DeletePatient(selectUserForm.SelectedPerson);
                }
            }
        }

        private void PatientRecordManagement(object sender, EventArgs e)
        {
            using (
                PatientRecordManagementForm patientRecordManagementForm = new(
                _medicalDataContainer.Users,
                _currentDoctor)
                )
            {
                DialogResult result = patientRecordManagementForm.ShowDialog();
            }
        }

        private void UserLoadEKGRecord(object sender, EventArgs e)
        {
            using (SelectUserEKGRecordsForm selectUserEKGRecordsForm = new SelectUserEKGRecordsForm(_currentUser.Records))
            {
                DialogResult result = selectUserEKGRecordsForm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    EKGRecord newEKGRecord;
                    EKGRecord.ReadDataFromFile(
                        selectUserEKGRecordsForm.SelectedEKGRecord.SavedFilePath,
                        out newEKGRecord
                        );
                    ekgRecord = newEKGRecord;
                    CheckCurrentEKGRecordIsReadOnly(sender, e);
                    _AfterLoadRecord(sender, e);
                }
            }
        }
        #endregion


        #region StatusStrip相關功能
        private void _UpdateStatusStrip()
        {
            string userString = "";
            if (_currentUser != null)
            {
                userString = $"{_currentUser.FirstName} {_currentUser.LastName}";
            }
            toolStripStatusLabelUser.Text = $"使用者: {userString}";

            string timeStampString = "";
            if (ekgRecord != null)
            {
                timeStampString = ekgRecord.TimeStamp.ToString("yyyyMMdd_HHmmss");
            }
            toolStripStatusLabelRecordTime.Text = $"開始紀錄時間: {timeStampString}";

            string doctorString = "";
            if (_currentDoctor != null)
            {
                doctorString = $"{_currentDoctor.FirstName} {_currentDoctor.LastName}";
            }
            toolStripStatusLabelDoctor.Text = $"操作醫生: {doctorString}";
        }

        #endregion

        #region 使用者選擇相關功能
        private void _AfterSelectUser(object sender, EventArgs e)
        {
            newRecordToolStripMenuItem.Enabled = true;
            userLoadRecordToolStripMenuItem.Enabled = true;
            _UpdateStatusStrip();
        }
        private void _AfterNewRecord(object sender, EventArgs e)
        {
            startRecordToolStripMenuItem.Enabled = true;
            stopRecordToolStripMenuItem.Enabled = true;
            _UpdateStatusStrip();
        }

        private void _AfterSelectDoctor(object sender, EventArgs e)
        {
            patientRecordManagementToolStripMenuItem.Enabled = true;
            _UpdateStatusStrip();
        }

        private void _AfterLoadRecord(object sender, EventArgs e)
        {
            startRecordToolStripMenuItem.Enabled = false;
            stopRecordToolStripMenuItem.Enabled = false;
            _UpdateStatusStrip();
        }
        #endregion


        #region 其他控件行為
        private void checkBoxAutoManageAxisLimit_CheckedChanged(object sender, EventArgs e)
        {
            ekgRecord.ChangeAutoAxisManage(checkBoxAutoManageAxisLimit.Checked);
        }

        private void CheckCurrentEKGRecordIsReadOnly(object sender, EventArgs e)
        {
            if (ekgRecord.IsReadOnlyMode)
            {
                startRecordToolStripMenuItem.Enabled = false;
                stopRecordToolStripMenuItem.Enabled = false;
            }
            else
            {
                startRecordToolStripMenuItem.Enabled = true;
                stopRecordToolStripMenuItem.Enabled = true;
            }
        }
        #endregion

        #region 解構行為

        protected override void OnHandleDestroyed(EventArgs e)
        {
            try
            {
                // 停止計時器
                _timer?.Stop();
                _refreshLabelTimer?.Stop();

                // 釋放EKG監控器資源
                ekgRecord?.Dispose();

                medicalDataContainer.SaveToFile(_personFilePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"清理資源時發生錯誤: {ex.Message}");
            }
            finally
            {
                base.OnHandleDestroyed(e);
            }
        }
        #endregion
    }
}

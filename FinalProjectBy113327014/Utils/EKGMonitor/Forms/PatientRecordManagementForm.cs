using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MyForms.Utils.EKGMonitor.Base;
using MyForms.Utils.EKGMonitor.Person;
using MyForms.Utils.EKGMonitor.Record;

namespace MyForms.Utils.EKGMonitor.Forms
{
    public partial class PatientRecordManagementForm : Form 
    {
        private User _selectedPatient;
        private List<User> _allPatients;
        private Doctor _doctor;

        public User SelectedPatient
        {
            get
            {
                return _selectedPatient;
            }
            set
            {
                if (_selectedPatient != value)
                {
                    _selectedPatient = value;
                }
                _RefreshListBoxRecord();
            }
        }
        public RecordBase SelectedRecord { get; private set; }

        public PatientRecordManagementForm(List<User> allPatients, Doctor doctor)
        {
            InitializeComponent();

            _allPatients = allPatients;
            _doctor = doctor;
            _RefreshListBoxPatient();
        }

        private void _RefreshListBoxRecord()
        {
            listBoxRecord.Items.Clear();
            foreach (RecordBase record in SelectedPatient.Records)
            {
                listBoxRecord.Items.Add(record);
            }
        }

        private void _RefreshListBoxPatient()
        {
            listBoxPatient.Items.Clear();
            foreach (User patient in _doctor.Patients)
            {
                listBoxPatient.Items.Add(patient);
            }
        }

        private void SelectPatient(object sender, EventArgs e)
        {
            if (listBoxPatient.SelectedItem == null) { return; }
            
            SelectedPatient = listBoxPatient.SelectedItem as User;
        }

        private void AddRecord(object sender, EventArgs e)
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
            SelectedPatient += newEKGRecord;
            _RefreshListBoxRecord();
        }

        private void DeleteRecord(object sender, EventArgs e)
        {
            SelectedPatient.DeleteRecord(listBoxRecord.SelectedItem as RecordBase);
            _RefreshListBoxRecord();
        }

        private void AddPatient(object sender, EventArgs e)
        {
            List<User> remainingPatient = _allPatients
                .Where(user => !_doctor.Patients.Contains(user))
                .ToList();

            using (SelectPersonForm<User> selectUserForm = new(remainingPatient))
            {
                DialogResult result = selectUserForm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    _doctor += selectUserForm.SelectedPerson;
                }
            }
            _RefreshListBoxPatient();
        }

        private void DeletePatient(object sender, EventArgs e)
        {
            var user = listBoxPatient.SelectedItem as User;
            _doctor.DeletePatient(user);
            _RefreshListBoxPatient();
        }
    }
}

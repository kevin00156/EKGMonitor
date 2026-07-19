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
    public partial class SelectUserEKGRecordsForm : Form 
    { 
        public EKGRecord SelectedEKGRecord { get; private set; }

        public SelectUserEKGRecordsForm(List<RecordBase> records)
        {
            InitializeComponent();

            foreach (RecordBase record in records)
            {
                if (record is not EKGRecord) { continue; }

                listBoxEKGRecord.Items.Add(record);
            }
        }

        private void SelectItem(object sender, EventArgs e)
        {
            if (listBoxEKGRecord.SelectedItem == null) return;
            try
            {
                SelectedEKGRecord = listBoxEKGRecord.SelectedItem as EKGRecord;
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}

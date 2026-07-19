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

namespace MyForms.Utils.EKGMonitor.Forms
{
    public partial class SelectPersonForm<T> : Form where T : PersonBase
    { 
        public T SelectedPerson { get; private set; }

        public SelectPersonForm(List<T> persons)
        {
            InitializeComponent();

            foreach (T person in persons)
            {
                listBoxPerson.Items.Add(person);
            }
        }

        private void SelectItem(object sender, EventArgs e)
        {
            if (listBoxPerson.SelectedItem == null) return;
            try
            {
                SelectedPerson = listBoxPerson.SelectedItem as T;
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

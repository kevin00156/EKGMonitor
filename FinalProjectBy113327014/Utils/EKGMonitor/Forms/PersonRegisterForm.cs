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
    public partial class PersonRegisterForm<T> : Form where T : PersonBase
    {
        T person;
        public T NewPerson { get; private set; }

        /// <summary>
        /// 用於創建新 Person 物件的委託
        /// </summary>
        public delegate T CreatePersonDelegate(string firstName, string lastName, string id, sexual sexual);
        
        /// <summary>
        /// 創建 Person 物件的工廠方法
        /// </summary>
        public CreatePersonDelegate CreatePersonFactory { get; set; }

        public PersonRegisterForm(CreatePersonDelegate createPersonFactory)
        {
            InitializeComponent();
            CreatePersonFactory = createPersonFactory ?? throw new ArgumentNullException(nameof(createPersonFactory));
            
            foreach (var item in Enum.GetValues(typeof(sexual)))
            {
                comboBoxSexual.Items.Add(item);
            }
            comboBoxSexual.SelectedItem = comboBoxSexual.Items[0];
        }



        private void buttonRegister_Click(object sender, EventArgs e)
        {
            try
            {
                person = CreatePersonFactory(
                    firstName: textBoxFirstName.Text,
                    lastName: textBoxLastName.Text,
                    id: textBoxID.Text,
                    sexual: (sexual)comboBoxSexual.SelectedItem
                );

                NewPerson = person;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void PersonRegisterForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    buttonRegister_Click(sender, e);
                    break;
            }
        }
    }
}

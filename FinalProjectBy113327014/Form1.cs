using MyForms.TabControls.Projects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FinalProjectBy113327014
{
    public partial class Form1 : Form
    {
        PortableEKGMonitor PortableEKGMonitor = new();
        public Form1()
        {
            InitializeComponent();
            PortableEKGMonitor.Dock = DockStyle.Fill;
            this.Controls.Add(PortableEKGMonitor);
            PortableEKGMonitor.Show();
        }
    }
}

namespace MyForms.Utils.EKGMonitor.Forms
{
    partial class PatientRecordManagementForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.listBoxRecord = new System.Windows.Forms.ListBox();
            this.contextMenuStripRecord = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addRecordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteRecordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listBoxPatient = new System.Windows.Forms.ListBox();
            this.contextMenuStripPatient = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addPatientToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deletePatientToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.contextMenuStripRecord.SuspendLayout();
            this.contextMenuStripPatient.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panel1.Controls.Add(this.tableLayoutPanel2);
            this.panel1.Location = new System.Drawing.Point(165, 45);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(358, 301);
            this.panel1.TabIndex = 6;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.listBoxRecord, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.listBoxPatient, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(358, 301);
            this.tableLayoutPanel2.TabIndex = 8;
            // 
            // listBoxRecord
            // 
            this.listBoxRecord.ContextMenuStrip = this.contextMenuStripRecord;
            this.listBoxRecord.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxRecord.FormattingEnabled = true;
            this.listBoxRecord.ItemHeight = 12;
            this.listBoxRecord.Location = new System.Drawing.Point(182, 3);
            this.listBoxRecord.Name = "listBoxRecord";
            this.listBoxRecord.Size = new System.Drawing.Size(173, 295);
            this.listBoxRecord.TabIndex = 7;
            // 
            // contextMenuStripRecord
            // 
            this.contextMenuStripRecord.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addRecordToolStripMenuItem,
            this.deleteRecordToolStripMenuItem});
            this.contextMenuStripRecord.Name = "contextMenuStrip1";
            this.contextMenuStripRecord.Size = new System.Drawing.Size(153, 48);
            // 
            // addRecordToolStripMenuItem
            // 
            this.addRecordToolStripMenuItem.Name = "addRecordToolStripMenuItem";
            this.addRecordToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.addRecordToolStripMenuItem.Text = "AddRecord";
            this.addRecordToolStripMenuItem.Click += new System.EventHandler(this.AddRecord);
            // 
            // deleteRecordToolStripMenuItem
            // 
            this.deleteRecordToolStripMenuItem.Name = "deleteRecordToolStripMenuItem";
            this.deleteRecordToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.deleteRecordToolStripMenuItem.Text = "DeleteRecord";
            this.deleteRecordToolStripMenuItem.Click += new System.EventHandler(this.DeleteRecord);
            // 
            // listBoxPatient
            // 
            this.listBoxPatient.ContextMenuStrip = this.contextMenuStripPatient;
            this.listBoxPatient.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxPatient.FormattingEnabled = true;
            this.listBoxPatient.ItemHeight = 12;
            this.listBoxPatient.Location = new System.Drawing.Point(3, 3);
            this.listBoxPatient.Name = "listBoxPatient";
            this.listBoxPatient.Size = new System.Drawing.Size(173, 295);
            this.listBoxPatient.TabIndex = 7;
            this.listBoxPatient.SelectedIndexChanged += new System.EventHandler(this.SelectPatient);
            // 
            // contextMenuStripPatient
            // 
            this.contextMenuStripPatient.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addPatientToolStripMenuItem,
            this.deletePatientToolStripMenuItem});
            this.contextMenuStripPatient.Name = "contextMenuStripPatient";
            this.contextMenuStripPatient.Size = new System.Drawing.Size(181, 70);
            // 
            // addPatientToolStripMenuItem
            // 
            this.addPatientToolStripMenuItem.Name = "addPatientToolStripMenuItem";
            this.addPatientToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.addPatientToolStripMenuItem.Text = "AddPatient";
            this.addPatientToolStripMenuItem.Click += new System.EventHandler(this.AddPatient);
            // 
            // deletePatientToolStripMenuItem
            // 
            this.deletePatientToolStripMenuItem.Name = "deletePatientToolStripMenuItem";
            this.deletePatientToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.deletePatientToolStripMenuItem.Text = "DeletePatient";
            this.deletePatientToolStripMenuItem.Click += new System.EventHandler(this.DeletePatient);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(689, 392);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // PatientRecordManagementForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(689, 392);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "PatientRecordManagementForm";
            this.Text = "Patient Record Management";
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.contextMenuStripRecord.ResumeLayout(false);
            this.contextMenuStripPatient.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ListBox listBoxRecord;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.ListBox listBoxPatient;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripRecord;
        private System.Windows.Forms.ToolStripMenuItem deleteRecordToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addRecordToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripPatient;
        private System.Windows.Forms.ToolStripMenuItem addPatientToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deletePatientToolStripMenuItem;
    }
}
namespace MyForms.TabControls.Projects
{
    partial class PortableEKGMonitor
    {
        /// <summary> 
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.formsPlotEKG = new ScottPlot.WinForms.FormsPlot();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.labelPhysiologicalData6 = new System.Windows.Forms.Label();
            this.labelPhysiologicalData5 = new System.Windows.Forms.Label();
            this.labelPhysiologicalData4 = new System.Windows.Forms.Label();
            this.labelPhysiologicalData3 = new System.Windows.Forms.Label();
            this.labelPhysiologicalData2 = new System.Windows.Forms.Label();
            this.labelPhysiologicalData1 = new System.Windows.Forms.Label();
            this.checkBoxAutoManageAxisLimit = new System.Windows.Forms.CheckBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.ToolStripMenuItem_Option = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_UserRegister = new System.Windows.Forms.ToolStripMenuItem();
            this.selectUserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.userLoadRecordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.doctorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_DoctorRegister = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.patientRecordManagementToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadRecordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startRecordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopRecordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newRecordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelUser = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelRecordTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelSpring = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelDoctor = new System.Windows.Forms.ToolStripStatusLabel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // formsPlotEKG
            // 
            this.formsPlotEKG.DisplayScale = 1F;
            this.formsPlotEKG.Dock = System.Windows.Forms.DockStyle.Fill;
            this.formsPlotEKG.Location = new System.Drawing.Point(0, 0);
            this.formsPlotEKG.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.formsPlotEKG.Name = "formsPlotEKG";
            this.formsPlotEKG.Size = new System.Drawing.Size(841, 536);
            this.formsPlotEKG.TabIndex = 0;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(4, 4);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.formsPlotEKG);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel2);
            this.splitContainer1.Size = new System.Drawing.Size(1088, 536);
            this.splitContainer1.SplitterDistance = 841;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 1;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.labelPhysiologicalData6, 0, 6);
            this.tableLayoutPanel2.Controls.Add(this.labelPhysiologicalData5, 0, 5);
            this.tableLayoutPanel2.Controls.Add(this.labelPhysiologicalData4, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.labelPhysiologicalData3, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.labelPhysiologicalData2, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.labelPhysiologicalData1, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.checkBoxAutoManageAxisLimit, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 7;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66611F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66611F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66611F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66611F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66611F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66944F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(242, 536);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // labelPhysiologicalData6
            // 
            this.labelPhysiologicalData6.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelPhysiologicalData6.AutoSize = true;
            this.labelPhysiologicalData6.Location = new System.Drawing.Point(49, 484);
            this.labelPhysiologicalData6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelPhysiologicalData6.Name = "labelPhysiologicalData6";
            this.labelPhysiologicalData6.Size = new System.Drawing.Size(143, 15);
            this.labelPhysiologicalData6.TabIndex = 5;
            this.labelPhysiologicalData6.Text = "labelPhysiologicalData6";
            // 
            // labelPhysiologicalData5
            // 
            this.labelPhysiologicalData5.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelPhysiologicalData5.AutoSize = true;
            this.labelPhysiologicalData5.Location = new System.Drawing.Point(49, 399);
            this.labelPhysiologicalData5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelPhysiologicalData5.Name = "labelPhysiologicalData5";
            this.labelPhysiologicalData5.Size = new System.Drawing.Size(143, 15);
            this.labelPhysiologicalData5.TabIndex = 4;
            this.labelPhysiologicalData5.Text = "labelPhysiologicalData5";
            // 
            // labelPhysiologicalData4
            // 
            this.labelPhysiologicalData4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelPhysiologicalData4.AutoSize = true;
            this.labelPhysiologicalData4.Location = new System.Drawing.Point(49, 317);
            this.labelPhysiologicalData4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelPhysiologicalData4.Name = "labelPhysiologicalData4";
            this.labelPhysiologicalData4.Size = new System.Drawing.Size(143, 15);
            this.labelPhysiologicalData4.TabIndex = 3;
            this.labelPhysiologicalData4.Text = "labelPhysiologicalData4";
            // 
            // labelPhysiologicalData3
            // 
            this.labelPhysiologicalData3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelPhysiologicalData3.AutoSize = true;
            this.labelPhysiologicalData3.Location = new System.Drawing.Point(49, 235);
            this.labelPhysiologicalData3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelPhysiologicalData3.Name = "labelPhysiologicalData3";
            this.labelPhysiologicalData3.Size = new System.Drawing.Size(143, 15);
            this.labelPhysiologicalData3.TabIndex = 2;
            this.labelPhysiologicalData3.Text = "labelPhysiologicalData3";
            // 
            // labelPhysiologicalData2
            // 
            this.labelPhysiologicalData2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelPhysiologicalData2.AutoSize = true;
            this.labelPhysiologicalData2.Location = new System.Drawing.Point(49, 153);
            this.labelPhysiologicalData2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelPhysiologicalData2.Name = "labelPhysiologicalData2";
            this.labelPhysiologicalData2.Size = new System.Drawing.Size(143, 15);
            this.labelPhysiologicalData2.TabIndex = 1;
            this.labelPhysiologicalData2.Text = "labelPhysiologicalData2";
            // 
            // labelPhysiologicalData1
            // 
            this.labelPhysiologicalData1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelPhysiologicalData1.AutoSize = true;
            this.labelPhysiologicalData1.Location = new System.Drawing.Point(49, 71);
            this.labelPhysiologicalData1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelPhysiologicalData1.Name = "labelPhysiologicalData1";
            this.labelPhysiologicalData1.Size = new System.Drawing.Size(143, 15);
            this.labelPhysiologicalData1.TabIndex = 0;
            this.labelPhysiologicalData1.Text = "labelPhysiologicalData1";
            // 
            // checkBoxAutoManageAxisLimit
            // 
            this.checkBoxAutoManageAxisLimit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.checkBoxAutoManageAxisLimit.AutoSize = true;
            this.checkBoxAutoManageAxisLimit.Location = new System.Drawing.Point(35, 9);
            this.checkBoxAutoManageAxisLimit.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxAutoManageAxisLimit.Name = "checkBoxAutoManageAxisLimit";
            this.checkBoxAutoManageAxisLimit.Size = new System.Drawing.Size(172, 19);
            this.checkBoxAutoManageAxisLimit.TabIndex = 6;
            this.checkBoxAutoManageAxisLimit.Text = "Auto Manage Axis Limit";
            this.checkBoxAutoManageAxisLimit.UseVisualStyleBackColor = true;
            this.checkBoxAutoManageAxisLimit.CheckedChanged += new System.EventHandler(this.checkBoxAutoManageAxisLimit_CheckedChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_Option,
            this.doctorToolStripMenuItem,
            this.loadRecordToolStripMenuItem,
            this.startRecordToolStripMenuItem,
            this.stopRecordToolStripMenuItem,
            this.newRecordToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1096, 27);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // ToolStripMenuItem_Option
            // 
            this.ToolStripMenuItem_Option.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_UserRegister,
            this.selectUserToolStripMenuItem,
            this.userLoadRecordToolStripMenuItem});
            this.ToolStripMenuItem_Option.Name = "ToolStripMenuItem_Option";
            this.ToolStripMenuItem_Option.Size = new System.Drawing.Size(53, 23);
            this.ToolStripMenuItem_Option.Text = "User";
            // 
            // ToolStripMenuItem_UserRegister
            // 
            this.ToolStripMenuItem_UserRegister.Name = "ToolStripMenuItem_UserRegister";
            this.ToolStripMenuItem_UserRegister.Size = new System.Drawing.Size(168, 24);
            this.ToolStripMenuItem_UserRegister.Text = "UserRegister";
            this.ToolStripMenuItem_UserRegister.Click += new System.EventHandler(this.UserRegister);
            // 
            // selectUserToolStripMenuItem
            // 
            this.selectUserToolStripMenuItem.Name = "selectUserToolStripMenuItem";
            this.selectUserToolStripMenuItem.Size = new System.Drawing.Size(168, 24);
            this.selectUserToolStripMenuItem.Text = "SelectUser";
            this.selectUserToolStripMenuItem.Click += new System.EventHandler(this.SelectUser);
            // 
            // userLoadRecordToolStripMenuItem
            // 
            this.userLoadRecordToolStripMenuItem.Name = "userLoadRecordToolStripMenuItem";
            this.userLoadRecordToolStripMenuItem.Size = new System.Drawing.Size(168, 24);
            this.userLoadRecordToolStripMenuItem.Text = "LoadRecord";
            this.userLoadRecordToolStripMenuItem.Click += new System.EventHandler(this.UserLoadEKGRecord);
            // 
            // doctorToolStripMenuItem
            // 
            this.doctorToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_DoctorRegister,
            this.ToolStripMenuItem2,
            this.patientRecordManagementToolStripMenuItem});
            this.doctorToolStripMenuItem.Name = "doctorToolStripMenuItem";
            this.doctorToolStripMenuItem.Size = new System.Drawing.Size(68, 23);
            this.doctorToolStripMenuItem.Text = "Doctor";
            // 
            // ToolStripMenuItem_DoctorRegister
            // 
            this.ToolStripMenuItem_DoctorRegister.Name = "ToolStripMenuItem_DoctorRegister";
            this.ToolStripMenuItem_DoctorRegister.Size = new System.Drawing.Size(269, 24);
            this.ToolStripMenuItem_DoctorRegister.Text = "DoctorRegister";
            this.ToolStripMenuItem_DoctorRegister.Click += new System.EventHandler(this.DoctorRegister);
            // 
            // ToolStripMenuItem2
            // 
            this.ToolStripMenuItem2.Name = "ToolStripMenuItem2";
            this.ToolStripMenuItem2.Size = new System.Drawing.Size(269, 24);
            this.ToolStripMenuItem2.Text = "SelectDoctor";
            this.ToolStripMenuItem2.Click += new System.EventHandler(this.SelectDoctor);
            // 
            // patientRecordManagementToolStripMenuItem
            // 
            this.patientRecordManagementToolStripMenuItem.Name = "patientRecordManagementToolStripMenuItem";
            this.patientRecordManagementToolStripMenuItem.Size = new System.Drawing.Size(269, 24);
            this.patientRecordManagementToolStripMenuItem.Text = "PatientRecordManagement";
            this.patientRecordManagementToolStripMenuItem.Click += new System.EventHandler(this.PatientRecordManagement);
            // 
            // loadRecordToolStripMenuItem
            // 
            this.loadRecordToolStripMenuItem.Name = "loadRecordToolStripMenuItem";
            this.loadRecordToolStripMenuItem.Size = new System.Drawing.Size(167, 23);
            this.loadRecordToolStripMenuItem.Text = "LoadRecordFromFile";
            this.loadRecordToolStripMenuItem.Click += new System.EventHandler(this.LoadRecordFile);
            // 
            // startRecordToolStripMenuItem
            // 
            this.startRecordToolStripMenuItem.Name = "startRecordToolStripMenuItem";
            this.startRecordToolStripMenuItem.Size = new System.Drawing.Size(104, 23);
            this.startRecordToolStripMenuItem.Text = "StartRecord";
            this.startRecordToolStripMenuItem.Click += new System.EventHandler(this.StartRecord);
            // 
            // stopRecordToolStripMenuItem
            // 
            this.stopRecordToolStripMenuItem.Name = "stopRecordToolStripMenuItem";
            this.stopRecordToolStripMenuItem.Size = new System.Drawing.Size(103, 23);
            this.stopRecordToolStripMenuItem.Text = "StopRecord";
            this.stopRecordToolStripMenuItem.Click += new System.EventHandler(this.StopRecord);
            // 
            // newRecordToolStripMenuItem
            // 
            this.newRecordToolStripMenuItem.Name = "newRecordToolStripMenuItem";
            this.newRecordToolStripMenuItem.Size = new System.Drawing.Size(103, 23);
            this.newRecordToolStripMenuItem.Text = "NewRecord";
            this.newRecordToolStripMenuItem.Click += new System.EventHandler(this.NewRecord);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelUser,
            this.toolStripStatusLabelRecordTime,
            this.toolStripStatusLabelSpring,
            this.toolStripStatusLabelDoctor});
            this.statusStrip.Location = new System.Drawing.Point(0, 571);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.statusStrip.Size = new System.Drawing.Size(1096, 24);
            this.statusStrip.TabIndex = 2;
            this.statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabelUser
            // 
            this.toolStripStatusLabelUser.Name = "toolStripStatusLabelUser";
            this.toolStripStatusLabelUser.Size = new System.Drawing.Size(57, 19);
            this.toolStripStatusLabelUser.Text = "使用者:";
            // 
            // toolStripStatusLabelRecordTime
            // 
            this.toolStripStatusLabelRecordTime.Name = "toolStripStatusLabelRecordTime";
            this.toolStripStatusLabelRecordTime.Size = new System.Drawing.Size(102, 19);
            this.toolStripStatusLabelRecordTime.Text = "開始紀錄時間:";
            // 
            // toolStripStatusLabelSpring
            // 
            this.toolStripStatusLabelSpring.Name = "toolStripStatusLabelSpring";
            this.toolStripStatusLabelSpring.Size = new System.Drawing.Size(845, 19);
            this.toolStripStatusLabelSpring.Spring = true;
            // 
            // toolStripStatusLabelDoctor
            // 
            this.toolStripStatusLabelDoctor.Name = "toolStripStatusLabelDoctor";
            this.toolStripStatusLabelDoctor.Size = new System.Drawing.Size(72, 19);
            this.toolStripStatusLabelDoctor.Text = "操作醫生:";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.splitContainer1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 27);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1096, 544);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // PortableEKGMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.statusStrip);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "PortableEKGMonitor";
            this.Size = new System.Drawing.Size(1096, 595);
            this.Load += new System.EventHandler(this.EKGMonitor_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ScottPlot.WinForms.FormsPlot formsPlotEKG;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Option;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_UserRegister;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelUser;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelRecordTime;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ToolStripMenuItem selectUserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem doctorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startRecordToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadRecordToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label labelPhysiologicalData1;
        private System.Windows.Forms.Label labelPhysiologicalData2;
        private System.Windows.Forms.Label labelPhysiologicalData3;
        private System.Windows.Forms.Label labelPhysiologicalData4;
        private System.Windows.Forms.Label labelPhysiologicalData5;
        private System.Windows.Forms.Label labelPhysiologicalData6;
        private System.Windows.Forms.ToolStripMenuItem stopRecordToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelSpring;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelDoctor;
        private System.Windows.Forms.ToolStripMenuItem newRecordToolStripMenuItem;
        private System.Windows.Forms.CheckBox checkBoxAutoManageAxisLimit;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_DoctorRegister;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem userLoadRecordToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem patientRecordManagementToolStripMenuItem;
    }
}

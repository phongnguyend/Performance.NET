namespace HeapInspector.Components
{
    partial class SelectProcess
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.processesGrid = new System.Windows.Forms.DataGridView();
            this.clIcon = new System.Windows.Forms.DataGridViewImageColumn();
            this.hdName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hdPID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Platform = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.processesGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // processesGrid
            // 
            this.processesGrid.AllowUserToAddRows = false;
            this.processesGrid.AllowUserToDeleteRows = false;
            this.processesGrid.AllowUserToResizeRows = false;
            this.processesGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.processesGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.processesGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clIcon,
            this.hdName,
            this.hdPID,
            this.Platform,
            this.Description});
            this.processesGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.processesGrid.Location = new System.Drawing.Point(0, 0);
            this.processesGrid.MultiSelect = false;
            this.processesGrid.Name = "processesGrid";
            this.processesGrid.RowHeadersVisible = false;
            this.processesGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.processesGrid.Size = new System.Drawing.Size(800, 450);
            this.processesGrid.TabIndex = 0;
            this.processesGrid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.ProcessesGrid_CellDoubleClick);
            // 
            // clIcon
            // 
            this.clIcon.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.clIcon.FillWeight = 10F;
            this.clIcon.HeaderText = "";
            this.clIcon.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Stretch;
            this.clIcon.Name = "clIcon";
            this.clIcon.ReadOnly = true;
            this.clIcon.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.clIcon.Width = 18;
            // 
            // hdName
            // 
            this.hdName.FillWeight = 105.7894F;
            this.hdName.HeaderText = "Name";
            this.hdName.Name = "hdName";
            // 
            // hdPID
            // 
            this.hdPID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.hdPID.DefaultCellStyle = dataGridViewCellStyle1;
            this.hdPID.FillWeight = 105.7894F;
            this.hdPID.HeaderText = "PID";
            this.hdPID.Name = "hdPID";
            this.hdPID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.hdPID.Width = 80;
            // 
            // Platform
            // 
            this.Platform.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.Platform.DefaultCellStyle = dataGridViewCellStyle2;
            this.Platform.FillWeight = 105.7894F;
            this.Platform.HeaderText = "Platform";
            this.Platform.Name = "Platform";
            this.Platform.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Platform.Width = 80;
            // 
            // Description
            // 
            this.Description.FillWeight = 105.7894F;
            this.Description.HeaderText = "Description";
            this.Description.Name = "Description";
            this.Description.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // SelectProcess
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.processesGrid);
            this.Name = "SelectProcess";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Process";
            this.Load += new System.EventHandler(this.SelectProcess_Load);
            ((System.ComponentModel.ISupportInitialize)(this.processesGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView processesGrid;
        private System.Windows.Forms.DataGridViewImageColumn clIcon;
        private System.Windows.Forms.DataGridViewTextBoxColumn hdName;
        private System.Windows.Forms.DataGridViewTextBoxColumn hdPID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Platform;
        private System.Windows.Forms.DataGridViewTextBoxColumn Description;
    }
}
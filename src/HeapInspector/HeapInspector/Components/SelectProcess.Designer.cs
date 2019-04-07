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
            this.processesGrid = new System.Windows.Forms.DataGridView();
            this.hdName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hdPID = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.hdName,
            this.hdPID});
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
            // hdName
            // 
            this.hdName.HeaderText = "Name";
            this.hdName.Name = "hdName";
            // 
            // hdPID
            // 
            this.hdPID.HeaderText = "PID";
            this.hdPID.Name = "hdPID";
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
        private System.Windows.Forms.DataGridViewTextBoxColumn hdName;
        private System.Windows.Forms.DataGridViewTextBoxColumn hdPID;
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeapInspector.Components
{
    public partial class SelectProcess : Form
    {
        public int SelectedProcess { get; private set; }

        public SelectProcess()
        {
            InitializeComponent();
        }

        private void SelectProcess_Load(object sender, EventArgs e)
        {
            var processes = Process.GetProcesses().OrderBy(x => x.ProcessName);

            int rowcount = 0;
            foreach (var process in processes)
            {
                DataGridViewRow gridrow = new DataGridViewRow();
                gridrow.DefaultCellStyle.SelectionBackColor = Color.Black;
                if (rowcount % 2 > 0)
                {
                    gridrow.DefaultCellStyle.BackColor = Color.AliceBlue;
                }
                rowcount++;

                DataGridViewTextBoxCell name = new DataGridViewTextBoxCell();
                name.Value = process.ProcessName;
                gridrow.Cells.Add(name);

                DataGridViewTextBoxCell pid = new DataGridViewTextBoxCell();
                pid.Value = process.Id;
                gridrow.Cells.Add(pid);

                processesGrid.Rows.Add(gridrow);
            }
        }

        private void ProcessesGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                SelectedProcess = Convert.ToInt32(processesGrid.Rows[e.RowIndex].Cells[1].Value.ToString());
                Close();
            }
        }
    }
}

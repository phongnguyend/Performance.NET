using HeapInspector.Components;
using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeapInspector
{
    public partial class MainForm : Form
    {
        private int _selectedProcessId;
        public MainForm()
        {
            InitializeComponent();
        }

        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectProcessForm = new SelectProcess();
            selectProcessForm.ShowDialog();

            var selectedProcess = selectProcessForm.SelectedProcess;

            if (selectedProcess <= 0)
            {
                return;
            }
            _selectedProcessId = selectedProcess;
            InspectProcess(selectedProcess);
        }

        private void InspectProcess(int pId)
        {
            using (DataTarget target = DataTarget.AttachToProcess(pId, 10000))
            {
                ClrRuntime runtime = target.ClrVersions.First().CreateRuntime();

                Dictionary<string, TypeEntry> types = new Dictionary<string, TypeEntry>();
                ClrHeap heap = runtime.Heap;
                foreach (ulong obj in heap.EnumerateObjects())
                {
                    ClrType type = heap.GetObjectType(obj);
                    var size = type.GetSize(obj);
                    if (types.ContainsKey(type.Name))
                    {
                        types[type.Name].Count++;
                        types[type.Name].MinSize = Math.Min(types[type.Name].MinSize, size);
                        types[type.Name].MaxSize = Math.Max(types[type.Name].MaxSize, size);
                        types[type.Name].TotalSize += size;
                    }
                    else
                    {
                        types[type.Name] = new TypeEntry
                        {
                            Name = type.Name,
                            Count = 1,
                            MinSize = size,
                            MaxSize = size,
                            TotalSize = size
                        };
                    }
                }

                var sortOrder = heapObjectsGrid.SortOrder == SortOrder.Ascending ?
            ListSortDirection.Ascending : ListSortDirection.Descending;
                var sortCol = heapObjectsGrid.SortedColumn;

                int rowcount = 0;
                heapObjectsGrid.Rows.Clear();
                foreach (var val in types)
                {
                    var infor = val.Value;
                    DataGridViewRow gridrow = new DataGridViewRow();
                    gridrow.DefaultCellStyle.SelectionBackColor = Color.Black;
                    if (rowcount % 2 > 0)
                    {
                        gridrow.DefaultCellStyle.BackColor = Color.AliceBlue;
                    }
                    rowcount++;

                    DataGridViewTextBoxCell name = new DataGridViewTextBoxCell();
                    name.Value = infor.Name;
                    gridrow.Cells.Add(name);

                    DataGridViewTextBoxCell count = new DataGridViewTextBoxCell();
                    count.Style.Alignment = DataGridViewContentAlignment.BottomRight;
                    count.Style.Format = "n0";
                    count.Value = infor.Count;
                    gridrow.Cells.Add(count);

                    DataGridViewTextBoxCell minSize = new DataGridViewTextBoxCell();
                    minSize.Style.Alignment = DataGridViewContentAlignment.BottomRight;
                    minSize.Style.Format = "n0";
                    if (infor.MinSize >= 85000)
                    {
                        minSize.Style.ForeColor = Color.Orange;
                        minSize.ToolTipText = "Large Object Heap";
                    }
                    minSize.Value = infor.MinSize;
                    gridrow.Cells.Add(minSize);

                    DataGridViewTextBoxCell maxSize = new DataGridViewTextBoxCell();
                    maxSize.Style.Alignment = DataGridViewContentAlignment.BottomRight;
                    maxSize.Style.Format = "n0";
                    if (infor.MaxSize >= 85000)
                    {
                        maxSize.Style.ForeColor = Color.Orange;
                        maxSize.ToolTipText = "Large Object Heap";
                    }
                    maxSize.Value = infor.MaxSize;
                    gridrow.Cells.Add(maxSize);

                    DataGridViewTextBoxCell totalSize = new DataGridViewTextBoxCell();
                    totalSize.Style.Alignment = DataGridViewContentAlignment.BottomRight;
                    totalSize.Style.Format = "n0";
                    totalSize.Value = infor.TotalSize;
                    gridrow.Cells.Add(totalSize);

                    heapObjectsGrid.Rows.Add(gridrow);
                }

                heapObjectsGrid.Columns[1].HeaderCell.ToolTipText = $"Total: { types.Values.Sum(x => x.Count).ToString("N0")} objects";
                heapObjectsGrid.Columns[4].HeaderCell.ToolTipText = $"Total: {types.Values.Sum(x => (long)x.TotalSize).ToString("N0")} bytes";

                if (sortCol != null)
                {
                    heapObjectsGrid.Sort(sortCol, sortOrder);
                }
            }
        }

        private void RefreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_selectedProcessId <= 0)
            {
                return;
            }
            InspectProcess(_selectedProcessId);
        }

        private void AutoRefreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var menuItem = sender as ToolStripMenuItem;
            menuItem.Checked = !menuItem.Checked;

            if (menuItem.Checked)
            {
                autoRefreshTimer.Start();
            }
            else
            {
                autoRefreshTimer.Stop();
            }
        }

        private void AutoRefreshTimer_Tick(object sender, EventArgs e)
        {
            if (_selectedProcessId <= 0)
            {
                return;
            }
            InspectProcess(_selectedProcessId);
        }
    }
}

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
                ShowCLRRuntimeInformation(target);

                ClrRuntime runtime = target.ClrVersions.First().CreateRuntime();

                Dictionary<string, TypeEntry> types = new Dictionary<string, TypeEntry>();
                Dictionary<string, StringInfor> stringCount = new Dictionary<string, StringInfor>();

                ClrHeap heap = runtime.Heap;

                var finalizerQueueObjects = runtime.EnumerateFinalizerQueueObjectAddresses().ToList();
                var pinnedObjects = runtime.EnumerateHandles().Where(h => h.IsPinned).Select(h => h.Object);
                var blockingObjects = heap.EnumerateBlockingObjects().Select(x => x.Object);

                foreach (ulong obj in heap.EnumerateObjects())
                {
                    ClrType type = heap.GetObjectType(obj);
                    var size = type.GetSize(obj);
                    var gen = heap.GetGeneration(obj);
                    var isInFinalizerQueue = finalizerQueueObjects.Contains(obj);
                    var isPinned = pinnedObjects.Contains(obj);
                    var isBlocking = blockingObjects.Contains(obj);

                    if (types.ContainsKey(type.Name))
                    {
                        types[type.Name].Count++;
                        types[type.Name].MinSize = Math.Min(types[type.Name].MinSize, size);
                        types[type.Name].MaxSize = Math.Max(types[type.Name].MaxSize, size);
                        types[type.Name].TotalSize += size;
                        types[type.Name].Generation0 += gen == 0 ? 1 : 0;
                        types[type.Name].Generation1 += gen == 1 ? 1 : 0;
                        types[type.Name].Generation2 += gen == 2 ? 1 : 0;
                        types[type.Name].FinalizerQueueCount += isInFinalizerQueue ? 1 : 0;
                        types[type.Name].PinnedCount += isPinned ? 1 : 0;
                        types[type.Name].BlockingCount += isBlocking ? 1 : 0;
                    }
                    else
                    {
                        types[type.Name] = new TypeEntry
                        {
                            Name = type.Name,
                            Count = 1,
                            MinSize = size,
                            MaxSize = size,
                            TotalSize = size,
                            Generation0 = gen == 0 ? 1 : 0,
                            Generation1 = gen == 1 ? 1 : 0,
                            Generation2 = gen == 2 ? 1 : 0,
                            FinalizerQueueCount = isInFinalizerQueue ? 1 : 0,
                            PinnedCount = isPinned ? 1 : 0,
                            BlockingCount = isBlocking ? 1 : 0,
                        };
                    }

                    if (type.IsString)
                    {
                        var text = (string)type.GetValue(obj);

                        if (stringCount.ContainsKey(text))
                        {
                            stringCount[text].Count++;
                            stringCount[text].Generation0 += gen == 0 ? 1 : 0;
                            stringCount[text].Generation1 += gen == 1 ? 1 : 0;
                            stringCount[text].Generation2 += gen == 2 ? 1 : 0;
                        }
                        else
                        {
                            stringCount[text] = new StringInfor
                            {
                                Text = text,
                                Count = 1,
                                Generation0 = gen == 0 ? 1 : 0,
                                Generation1 = gen == 1 ? 1 : 0,
                                Generation2 = gen == 2 ? 1 : 0,
                            };
                        }
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

                    DataGridViewTextBoxCell generation0 = new DataGridViewTextBoxCell();
                    generation0.Value = infor.Generation0;
                    generation0.Style.Alignment = DataGridViewContentAlignment.BottomRight;
                    generation0.Style.Format = "n0";
                    gridrow.Cells.Add(generation0);

                    DataGridViewTextBoxCell generation1 = new DataGridViewTextBoxCell();
                    generation1.Value = infor.Generation1;
                    generation1.Style.Alignment = DataGridViewContentAlignment.BottomRight;
                    generation1.Style.Format = "n0";
                    gridrow.Cells.Add(generation1);

                    DataGridViewTextBoxCell generation2 = new DataGridViewTextBoxCell();
                    generation2.Value = infor.Generation2;
                    generation2.Style.Alignment = DataGridViewContentAlignment.BottomRight;
                    generation2.Style.Format = "n0";
                    gridrow.Cells.Add(generation2);

                    DataGridViewTextBoxCell finalizerQueue = new DataGridViewTextBoxCell();
                    finalizerQueue.Value = infor.FinalizerQueueCount;
                    finalizerQueue.Style.Alignment = DataGridViewContentAlignment.BottomRight;
                    finalizerQueue.Style.Format = "n0";
                    gridrow.Cells.Add(finalizerQueue);

                    DataGridViewTextBoxCell pinned = new DataGridViewTextBoxCell();
                    pinned.Value = infor.PinnedCount;
                    pinned.Style.Alignment = DataGridViewContentAlignment.BottomRight;
                    pinned.Style.Format = "n0";
                    gridrow.Cells.Add(pinned);

                    DataGridViewTextBoxCell blocking = new DataGridViewTextBoxCell();
                    blocking.Value = infor.BlockingCount;
                    blocking.Style.Alignment = DataGridViewContentAlignment.BottomRight;
                    blocking.Style.Format = "n0";
                    gridrow.Cells.Add(blocking);

                    heapObjectsGrid.Rows.Add(gridrow);
                }

                heapObjectsGrid.Columns[1].HeaderCell.ToolTipText = $"Total: { types.Values.Sum(x => x.Count).ToString("N0")} objects";
                heapObjectsGrid.Columns[4].HeaderCell.ToolTipText = $"Total: {types.Values.Sum(x => (long)x.TotalSize).ToString("N0")} bytes";

                if (sortCol != null)
                {
                    heapObjectsGrid.Sort(sortCol, sortOrder);
                }

                // Strings Grid View:
                rowcount = 0;
                stringsGrid.Rows.Clear();
                foreach (var str in stringCount)
                {
                    DataGridViewRow gridrow = new DataGridViewRow();
                    gridrow.DefaultCellStyle.SelectionBackColor = Color.Black;
                    if (rowcount % 2 > 0)
                    {
                        gridrow.DefaultCellStyle.BackColor = Color.AliceBlue;
                    }
                    rowcount++;

                    DataGridViewTextBoxCell name = new DataGridViewTextBoxCell();
                    name.Value = str.Key;
                    gridrow.Cells.Add(name);

                    DataGridViewTextBoxCell length = new DataGridViewTextBoxCell();
                    length.Style.Alignment = DataGridViewContentAlignment.BottomRight;
                    length.Style.Format = "n0";
                    length.Value = str.Key.Length;
                    gridrow.Cells.Add(length);

                    DataGridViewTextBoxCell count = new DataGridViewTextBoxCell();
                    count.Style.Alignment = DataGridViewContentAlignment.BottomRight;
                    count.Style.Format = "n0";
                    count.Value = str.Value.Count;
                    gridrow.Cells.Add(count);

                    DataGridViewTextBoxCell generation0 = new DataGridViewTextBoxCell();
                    generation0.Value = str.Value.Generation0;
                    generation0.Style.Alignment = DataGridViewContentAlignment.BottomRight;
                    generation0.Style.Format = "n0";
                    gridrow.Cells.Add(generation0);

                    DataGridViewTextBoxCell generation1 = new DataGridViewTextBoxCell();
                    generation1.Value = str.Value.Generation1;
                    generation1.Style.Alignment = DataGridViewContentAlignment.BottomRight;
                    generation1.Style.Format = "n0";
                    gridrow.Cells.Add(generation1);

                    DataGridViewTextBoxCell generation2 = new DataGridViewTextBoxCell();
                    generation2.Value = str.Value.Generation2;
                    generation2.Style.Alignment = DataGridViewContentAlignment.BottomRight;
                    generation2.Style.Format = "n0";
                    gridrow.Cells.Add(generation2);

                    stringsGrid.Rows.Add(gridrow);
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

        private void ShowCLRRuntimeInformation(DataTarget target)
        {
            rtxtCLRInfor.Clear();
            foreach (var clr in target.ClrVersions)
            {
                AppendText($"Version:\t{clr.Version.ToString()}");
                AppendText($"Module: \t{clr.ModuleInfo.ToString()}");
            }
            AppendText("");

            ClrRuntime runtime = target.ClrVersions.First().CreateRuntime();

            AppendText("App Domains:");
            AppendText("");

            foreach (ClrAppDomain domain in runtime.AppDomains)
            {
                AppendText($"  ID:                {domain.Id}");
                AppendText($"  Name:              {domain.Name}");
                AppendText($"  Address:           {domain.Address}");
                AppendText($"  ApplicationBase:   {domain.ApplicationBase}");
                AppendText($"  ConfigurationFile: {domain.ConfigurationFile}");
                AppendText("");

                AppendText("    Modules:" + Environment.NewLine);
                foreach (ClrModule module in domain.Modules)
                {
                    AppendText($"       Module:       { module.Name}");
                    AppendText($"       AssemblyName: { module.AssemblyName}");
                    AppendText("");
                }
            }
        }

        private void AppendText(string text)
        {
            rtxtCLRInfor.AppendText($"{text}{Environment.NewLine}");
        }
    }
}

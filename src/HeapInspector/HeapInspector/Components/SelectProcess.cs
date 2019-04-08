using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeapInspector.Components
{
    public partial class SelectProcess : Form
    {
        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool IsWow64Process([In] IntPtr process, [Out] out bool wow64Process);

        public int SelectedProcess { get; private set; }

        public SelectProcess()
        {
            InitializeComponent();
            Text += $" ({GetPlatform(Process.GetCurrentProcess())})";
            processesGrid.Columns["hdPID"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomRight;
            processesGrid.Columns["Platform"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomRight;
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

                DataGridViewImageCell icon = new DataGridViewImageCell();
                icon.Value = GetIcon(process);
                gridrow.Cells.Add(icon);

                DataGridViewTextBoxCell name = new DataGridViewTextBoxCell();
                name.Value = process.ProcessName;
                gridrow.Cells.Add(name);

                DataGridViewTextBoxCell pid = new DataGridViewTextBoxCell();
                pid.Value = process.Id;
                pid.Style.Alignment = DataGridViewContentAlignment.BottomRight;
                gridrow.Cells.Add(pid);

                DataGridViewTextBoxCell platform = new DataGridViewTextBoxCell();
                platform.Value = GetPlatform(process);
                platform.Style.Alignment = DataGridViewContentAlignment.BottomRight;
                gridrow.Cells.Add(platform);

                DataGridViewTextBoxCell description = new DataGridViewTextBoxCell();
                description.Value = GetDescription(process);
                gridrow.Cells.Add(description);

                processesGrid.Rows.Add(gridrow);
            }
        }

        private void ProcessesGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                SelectedProcess = Convert.ToInt32(processesGrid.Rows[e.RowIndex].Cells[2].Value.ToString());
                Close();
            }
        }

        private static string GetPlatform(Process process)
        {
            if (!Environment.Is64BitOperatingSystem)
                return "32 bit";

            try
            {
                if ((Environment.OSVersion.Version.Major > 5)
                || ((Environment.OSVersion.Version.Major == 5) && (Environment.OSVersion.Version.Minor >= 1)))
                {
                    bool retVal;

                    return IsWow64Process(process.Handle, out retVal) && retVal ? "32 bit" : "64 bit";
                }
            }
            catch (Win32Exception ex)
            {
                if (ex.NativeErrorCode != 0x00000005)
                {
                    throw;
                }
            }

            return "";
        }

        private static string GetDescription(Process process)
        {
            try
            {
                var desc = process.MainModule.FileVersionInfo.FileDescription;
                if (string.IsNullOrEmpty(desc))
                    return process.MainModule.FileVersionInfo.FileName;
                return desc;
            }
            catch
            {
                return "";
            }
        }

        private static object GetIcon(Process process)
        {
            try
            {
                var icon = Icon.ExtractAssociatedIcon(process.MainModule.FileName);
                return new Bitmap(icon.ToBitmap(), 16, 16);
            }
            catch (Exception)
            {

                return null;
            }
        }
    }
}

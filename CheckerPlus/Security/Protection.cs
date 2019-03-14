using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;

namespace CheckerPlus.Security
{
    // Token: 0x0200001C RID: 28
    public class Protection
    {
        [DllImport("Kernel32.dll", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CheckRemoteDebuggerPresent(IntPtr hProcess, [MarshalAs(UnmanagedType.Bool)] ref bool isDebuggerPresent);

        [DllImport("kernel32.dll")]
        private static extern bool IsDebuggerPresent();

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string module);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, IntPtr ZeroOnly);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern uint GetFileAttributes(string lpFileName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        public static string GetWindowClassName(IntPtr hWnd)
        {
            StringBuilder buffer = new StringBuilder(128);
            GetClassName(hWnd, buffer, buffer.Capacity);
            return buffer.ToString();
        }

        static int timeout = 100;

        static List<string> apps = new List<string>()
        {
            "dnspy",
            "procmon",
            "procexpl",
            "378734a", // MegaDumper
            "ollydbg"
        };

        // Token: 0x0600011F RID: 287 RVA: 0x000072F0 File Offset: 0x000054F0
        static public void Start()
        {
            if (isProcessVirtualized())
            {
                spoofCrash();
            }
#if !DEBUG
            if (isDebugged())
            {
                endlessLoop();
            }
#endif
            if (isProcessInSandbox(Application.ExecutablePath))
            {
                spoofCrash();
            }
            if (isEmulated())
            {
                endlessLoop();
            }
            checkSnooping();
#if !DEBUG
            AntiDebuggingThread = new Thread(new ThreadStart(debuggerThread));
            AntiDebuggingThread.Start();
#endif
            AntiSnooperThread = new Thread(new ThreadStart(snooperThread));
            AntiSnooperThread.Start();
        }

        // Token: 0x06000120 RID: 288 RVA: 0x000073D5 File Offset: 0x000055D5
        static public void Stop()
        {
            if (AntiDebuggingThread != null)
            {
                AntiDebuggingThread = null;
            }
            if (AntiSnooperThread != null)
            {
                AntiSnooperThread = null;
            }
        }

        // Token: 0x06000121 RID: 289 RVA: 0x00007405 File Offset: 0x00005605
        static private void spoofCrash()
        {
#if !DEBUG
            GC.Collect();
            Environment.FailFast(null);
#elif DEBUG
            Debug.WriteLine("spoofcrash");
#endif
        }

        // Token: 0x06000122 RID: 290 RVA: 0x00007412 File Offset: 0x00005612
        static private void endlessLoop()
        {
            GC.Collect();
            Environment.Exit(0);
        }

        // Token: 0x06000123 RID: 291 RVA: 0x00007420 File Offset: 0x00005620
        static private bool isDebugged()
        {
            bool flag = false;
            if (Debugger.IsAttached)
            {
                flag = true;
            }
            if (Debugger.IsLogging())
            {
                flag = true;
            }
            bool remotedbg = false;
            Protection.CheckRemoteDebuggerPresent(Process.GetCurrentProcess().Handle, ref remotedbg);
            if (remotedbg)
            {
                flag = true;
            }
            if (Protection.IsDebuggerPresent())
            {
                flag = true;
            }
            return flag;
        }

        // Token: 0x06000124 RID: 292 RVA: 0x00007468 File Offset: 0x00005668
        static private bool isProcessInSandbox(string startupPath)
        {
            if ((int)Protection.GetModuleHandle("SbieDLL.dll") != 0)
            {
                return true;
            }
            if (Process.GetCurrentProcess().ProcessName == "mlwr_smpl")
            {
                return true;
            }
            if (Environment.MachineName.StartsWith("placehol-"))
            {
                return true;
            }
            string a = WindowsIdentity.GetCurrent().Name.ToString().ToUpper();
            if (a == "USER")
            {
                return true;
            }
            if (a == "SANDBOX")
            {
                return true;
            }
            if (a == "VIRUS")
            {
                return true;
            }
            if (a == "MALWARE")
            {
                return true;
            }
            if (a == "SCHMIDTI")
            {
                return true;
            }
            if (!(a == "CURRENTUSER"))
            {
                string sPath = startupPath.ToUpper();
                return sPath == "C:\\FILE.EXE" || sPath.Contains("\\VIRUS") || sPath.Contains("SANDBOX") || sPath.Contains("SAMPLE") || (int)Protection.FindWindow("Afx:400000:0", (IntPtr)0) != 0;
            }
            return true;
        }

        // Token: 0x06000125 RID: 293 RVA: 0x00007584 File Offset: 0x00005784
        static private bool isEmulated()
        {
            long tickCount = (long)Environment.TickCount;
            Thread.Sleep(500);
            return (long)Environment.TickCount - tickCount < 500L;
        }

        // Token: 0x06000126 RID: 294 RVA: 0x000075B8 File Offset: 0x000057B8
        static private bool isProcessVirtualized()
        {
            if (readRegistryKey("HARDWARE\\DEVICEMAP\\Scsi\\Scsi Port 0\\Scsi Bus 0\\Target Id 0\\Logical Unit Id 0", "Identifier").ToUpper().Contains("VBOX"))
            {
                return true;
            }
            if (readRegistryKey("HARDWARE\\Description\\System", "SystemBiosVersion").ToUpper().Contains("VBOX"))
            {
                return true;
            }
            if (readRegistryKey("HARDWARE\\Description\\System", "VideoBiosVersion").ToUpper().Contains("VIRTUALBOX"))
            {
                return true;
            }
            if (readRegistryKey("SOFTWARE\\Oracle\\VirtualBox Guest Additions", "") == "noValueButYesKey")
            {
                return true;
            }
            if (Protection.GetFileAttributes("C:\\WINDOWS\\system32\\drivers\\VBoxMouse.sys") != 4294967295u)
            {
                return true;
            }
            if (readRegistryKey("HARDWARE\\DEVICEMAP\\Scsi\\Scsi Port 0\\Scsi Bus 0\\Target Id 0\\Logical Unit Id 0", "Identifier").ToUpper().Contains("VMWARE"))
            {
                return true;
            }
            if (readRegistryKey("SOFTWARE\\VMware, Inc.\\VMware Tools", "") == "noValueButYesKey")
            {
                return true;
            }
            if (readRegistryKey("HARDWARE\\DEVICEMAP\\Scsi\\Scsi Port 1\\Scsi Bus 0\\Target Id 0\\Logical Unit Id 0", "Identifier").ToUpper().Contains("VMWARE"))
            {
                return true;
            }
            if (readRegistryKey("HARDWARE\\DEVICEMAP\\Scsi\\Scsi Port 2\\Scsi Bus 0\\Target Id 0\\Logical Unit Id 0", "Identifier").ToUpper().Contains("VMWARE"))
            {
                return true;
            }
            if (readRegistryKey("SYSTEM\\ControlSet001\\Services\\Disk\\Enum", "0").ToUpper().Contains("vmware".ToUpper()))
            {
                return true;
            }
            if (readRegistryKey("SYSTEM\\ControlSet001\\Control\\Class\\{4D36E968-E325-11CE-BFC1-08002BE10318}\\0000", "DriverDesc").ToUpper().Contains("VMWARE"))
            {
                return true;
            }
            if (readRegistryKey("SYSTEM\\ControlSet001\\Control\\Class\\{4D36E968-E325-11CE-BFC1-08002BE10318}\\0000\\Settings", "Device Description").ToUpper().Contains("VMWARE"))
            {
                return true;
            }
            if (readRegistryKey("SOFTWARE\\VMware, Inc.\\VMware Tools", "InstallPath").ToUpper().Contains("C:\\PROGRAM FILES\\VMWARE\\VMWARE TOOLS\\"))
            {
                return true;
            }
            if (Protection.GetFileAttributes("C:\\WINDOWS\\system32\\drivers\\vmmouse.sys") != 4294967295u)
            {
                return true;
            }
            if (Protection.GetFileAttributes("C:\\WINDOWS\\system32\\drivers\\vmhgfs.sys") != 4294967295u)
            {
                return true;
            }
            if (Protection.GetProcAddress(Protection.GetModuleHandle("kernel32.dll"), "wine_get_unix_file_name") != (IntPtr)0)
            {
                return true;
            }
            if (readRegistryKey("HARDWARE\\DEVICEMAP\\Scsi\\Scsi Port 0\\Scsi Bus 0\\Target Id 0\\Logical Unit Id 0", "Identifier").ToUpper().Contains("QEMU"))
            {
                return true;
            }
            if (readRegistryKey("HARDWARE\\Description\\System", "SystemBiosVersion").ToUpper().Contains("QEMU"))
            {
                return true;
            }
            ManagementScope scope = new ManagementScope("\\\\.\\ROOT\\cimv2");
            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_VideoController");
            foreach (ManagementBaseObject managementBaseObject in new ManagementObjectSearcher(scope, query).Get())
            {
                ManagementObject i = (ManagementObject)managementBaseObject;
                if (i["Description"].ToString() == "VM Additions S3 Trio32/64")
                {
                    return true;
                }
                if (i["Description"].ToString() == "S3 Trio32/64")
                {
                    return true;
                }
                if (i["Description"].ToString() == "VirtualBox Graphics Adapter")
                {
                    return true;
                }
                if (i["Description"].ToString() == "VMware SVGA II")
                {
                    return true;
                }
                if (i["Description"].ToString().ToUpper().Contains("VMWARE"))
                {
                    return true;
                }
                if (i["Description"].ToString() == "")
                {
                    return true;
                }
            }
            return false;
        }

        // Token: 0x06000127 RID: 295 RVA: 0x00007928 File Offset: 0x00005B28
        static private string readRegistryKey(string key, string value)
        {
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(key, false);
            if (registryKey == null)
            {
                return "noKey";
            }
            object rkey = registryKey.GetValue(value, "noValueButYesKey");
            if (rkey.GetType() == typeof(string))
            {
                return rkey.ToString();
            }
            if (registryKey.GetValueKind(value) == RegistryValueKind.String || registryKey.GetValueKind(value) == RegistryValueKind.ExpandString)
            {
                return rkey.ToString();
            }
            if (registryKey.GetValueKind(value) == RegistryValueKind.DWord)
            {
                return Convert.ToString((int)rkey);
            }
            if (registryKey.GetValueKind(value) == RegistryValueKind.QWord)
            {
                return Convert.ToString((long)rkey);
            }
            if (registryKey.GetValueKind(value) == RegistryValueKind.Binary)
            {
                return Convert.ToString((byte[])rkey);
            }
            if (registryKey.GetValueKind(value) == RegistryValueKind.MultiString)
            {
                return string.Join("", (string[])rkey);
            }
            return "noValueButYesKey";
        }

        // Token: 0x06000128 RID: 296 RVA: 0x000079F8 File Offset: 0x00005BF8
        static private void checkSnooping()
        {
            foreach (Process process in Process.GetProcesses())
            {
                string fixedname = GetWindowClassName(process.MainWindowHandle).ToLower();
#if DEBUG
                Debug.WriteLine(fixedname);
#endif
                foreach (string name in apps)
                {
                    if (fixedname.Contains(name))
                    {
                        try
                        {
                            process.Kill();
                        }
                        catch
                        {
                            spoofCrash();
                        }
                    }
                }
            }

        }

        // Token: 0x06000129 RID: 297 RVA: 0x00007A80 File Offset: 0x00005C80
        static private void snooperThread()
        {
            while (true)
            {
                try
                {
                    checkSnooping();
                }
                catch
                {
                }
                Thread.Sleep(timeout);
            }
        }

        // Token: 0x0600012A RID: 298 RVA: 0x00007AC8 File Offset: 0x00005CC8
        static private void debuggerThread()
        {
            while (true)
            {
                if (isDebugged())
                {
                    spoofCrash();
                }
                Thread.Sleep(timeout);
            }
        }

        // Token: 0x0400009D RID: 157
        static private Thread AntiDebuggingThread;

        // Token: 0x0400009E RID: 158
        static private Thread AntiSnooperThread;
    }
}

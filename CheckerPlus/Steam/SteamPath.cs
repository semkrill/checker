using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;

namespace CheckerPlus.Steam
{

    public class SteamPath
    {
        private static readonly string SteamPath_x64 = @"SOFTWARE\Wow6432Node\Valve\Steam";
        private static readonly string SteamPath_x32 = @"Software\Valve\Steam";
        private static readonly bool True = true, False = false;

        public static string GetLocationSteam(string Inst = "InstallPath", string Source = "SourceModInstallPath")
        {
            using (var BaseSteam = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, (Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32)))
            {
                using (RegistryKey Key = BaseSteam.OpenSubKey(SteamPath_x64, (Environment.Is64BitOperatingSystem ? True : False)))
                {
                    using (RegistryKey Key2 = BaseSteam.OpenSubKey(SteamPath_x32, (Environment.Is64BitOperatingSystem ? True : False)))
                    {
                        return Key?.GetValue(Inst)?.ToString() ?? Key2?.GetValue(Source)?.ToString();
                    }
                }
            }
        }
    }
}

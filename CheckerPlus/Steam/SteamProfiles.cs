using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace CheckerPlus.Steam
{
    public class SteamProfiles
    {
        private static readonly string LoginFile = Path.Combine(SteamPath.GetLocationSteam(), @"config\loginusers.vdf");
        private static StringBuilder SB = new StringBuilder();

        public static string GetSteamID()
        {
            try
            {
                if (!File.Exists(LoginFile))
                {
                    return null;
                }
                else
                {
                    var textlogin = File.ReadAllText(LoginFile);
                    var result = Regex.Matches(textlogin, "\\\"76(.*?)\\\"").Cast<Match>().Select(x => "76" + x.Groups[1].Value).ToList();
                    var result2 = Regex.Matches(textlogin, "\\\"mostrecent\\\"		\\\"(.*?)\\\"").Cast<Match>().Select(x => x.Groups[1].Value).ToList();
                    for(int d = 0;d<result2.Count();d++)
                        if(result2[d]=="1")
                            return result[d];
                    return result.ToList()[result.Count-1];
                }
            }
            catch { return null; }
        }

        public static List<string> GetAllProfiles()
        {
            if (!File.Exists(LoginFile))
            {
                return null;
            }
            else
            {
                var textlogin = File.ReadAllText(LoginFile);
                var result = Regex.Matches(textlogin, "\\\"76(.*?)\\\"").Cast<Match>().Select(x => "76" + x.Groups[1].Value).ToList();
                string exclude = GetSteamID();
                List<string> lists = new List<string>();
                for (int d = 0; d < result.Count(); d++)
                {
                    if (result[d] != exclude)
                        lists.Add(result[d]);
                }


                return lists;
            }
        }
    }
}

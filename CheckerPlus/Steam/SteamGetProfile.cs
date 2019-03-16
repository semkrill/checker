using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace CheckerPlus.Steam
{
    class SteamGetProfile
    {
        public class FullState
        {
            public bool ban;
            public string name;
            public string id;
            public string url;
        }

        public static FullState GetProfile(string id)
        {
            return Parse(id);
        }

        public static List<FullState> GetProfiles()
        {
            List<FullState> fullStates = new List<FullState>();
            string steamalls = string.Empty;
            var list = SteamProfiles.GetAllProfiles();
            if (list.Count() == 0)
                return fullStates;

            foreach (var a in list)
            {
                var prs = Parse(a);
                if (prs != null)
                    fullStates.Add(prs);
            }

            return fullStates;
        }

        public static FullState Parse(string id)
        {
            try
            {
                long steamid = SteamConverter.FromSteam64ToSteam32(long.Parse(id));
                WebBrowser web = new WebBrowser();
                FullState fullState = new FullState();
                web.Navigate("https://steamid.xyz/" + steamid);
                while (web.ReadyState != WebBrowserReadyState.Complete)
                {
                    Application.DoEvents();
                }
                string name = "User";
                try
                {
                    var lines = Regex.Split(web.DocumentText, @"\n");
                    for (int d = 0; d < lines.Count(); d++)
                        if (lines[d].Contains("Nick Name"))
                        {
                            name = Regex.Match(lines[d + 1], "value=\"(.+)\"").Value;
                            name = name.Substring(7, name.Length - 7);
                            name = name.Remove(name.Length - 1);
                        }
                }
                catch { }
                string url = "https://steamcdn-a.akamaihd.net/steamcommunity/public/images/avatars/fe/fef49e7fa7e1997310d705b2a6158ff8dc1cdfeb_full.jpg";
                try
                {
                    url = Regex.Match(web.DocumentText, @"https://steamcdn-a.akamaihd.net/steamcommunity/public/images/(.+)").Value.Replace("\">", string.Empty);
                }
                catch { }

                if (web.DocumentText.Contains("User is VAC Banned"))
                    fullState.ban = true;

                fullState.name = name;
                fullState.id = id;
                fullState.url = url;

                return fullState;
            }
            catch { }
            return null;
        }
    }
}

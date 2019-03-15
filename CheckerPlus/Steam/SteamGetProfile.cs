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
            public BAN Ban { get; }
            public Player player { get; }
            public FullState(BAN ban,Player player)
            {
                this.Ban = ban;
                this.player = player;
            }
        }

        public class FullStates
        {
            public List<BAN> Ban { get; }
            public List<Player> player { get; }
            public FullStates(List<BAN> ban, List<Player> player)
            {
                this.Ban = ban;
                this.player = player;
            }
        }

        public class BAN
        {
            public string SteamId { get; set; }
            public bool CommunityBanned { get; set; }
            public bool VACBanned { get; set; }
            public int NumberOfVACBans { get; set; }
            public int DaysSinceLastBan { get; set; }
            public int NumberOfGameBans { get; set; }
            public string EconomyBan { get; set; }
        }

        public class RootObject
        {
            public List<BAN> players { get; set; }
        }

        public class Player
        {
            public string steamid { get; set; }
            public int communityvisibilitystate { get; set; }
            public int profilestate { get; set; }
            public string personaname { get; set; }
            public int lastlogoff { get; set; }
            public string profileurl { get; set; }
            public string avatar { get; set; }
            public string avatarmedium { get; set; }
            public string avatarfull { get; set; }
            public int personastate { get; set; }
            public string primaryclanid { get; set; }
            public int timecreated { get; set; }
            public int personastateflags { get; set; }
        }

        public class Response
        {
            public List<Player> players { get; set; }
        }

        public class RootObjectPlayer
        {
            public Response response { get; set; }
        }

        readonly static private string steamkey = "";

        public static FullState GetProfile(string id)
        {
            Player newplayer = null;
            BAN newban = null;
            try
            {
                WebClient webClient = new WebClient();
                string json_profile = webClient.DownloadString("http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=" + steamkey + "&steamids=" + id);
                newplayer = JsonConvert.DeserializeObject<RootObjectPlayer>(json_profile).response.players[0];
                string json_bans = webClient.DownloadString("http://api.steampowered.com/ISteamUser/GetPlayerBans/v1/?key=" + steamkey + "&steamids=" + id);
                newban = JsonConvert.DeserializeObject<RootObject>(json_bans).players[0];
            }
            catch { }
            return new FullState(newban, newplayer);
        }

        public static List<FullState> GetProfiles()
        {
            List<FullState> fullStates = new List<FullState>();
            string steamalls = string.Empty;
            var d = SteamProfiles.GetAllProfiles();
            if (d.Count()==0)
                return fullStates;
            for(int a = 0;a<d.Count();a++)
            {
                if (a + 1 % 2 == 0)
                    steamalls += ",";

                steamalls += d[a];
            }
            List<Player> newplayer = null;
            List<BAN> newban = null;
            try
            {
                WebClient webClient = new WebClient();
                string json_profile = webClient.DownloadString("http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=" + steamkey + "&steamids=" + steamalls);
                newplayer = JsonConvert.DeserializeObject<RootObjectPlayer>(json_profile).response.players;
                string json_bans = webClient.DownloadString("http://api.steampowered.com/ISteamUser/GetPlayerBans/v1/?key=" + steamkey + "&steamids=" + steamalls);
                newban = JsonConvert.DeserializeObject<RootObject>(json_bans).players;
            }
            catch {
                return fullStates;
            }
            for(int a = 0;a<newplayer.Count();a++)
            {
                fullStates.Add(new FullState(newban[a], newplayer[a]));
            }
            return fullStates;
        }

    }
}

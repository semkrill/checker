using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Threading;

namespace CheckerPlus
{
    class CheckBans
    {
        public class User_Banned
        {
            public string nickname { get; set; }
            public string steamid { get; set; }
            public string reason { get; set; }
            public string time { get; set; }
            public string banip { get; set; }
        }

        static WebClient webClient = new WebClient();

        public static User_Banned GetBanMagicow(string id)
        {
            string json = string.Empty;
            List<User_Banned> players = null;
            try
            {
                json = webClient.DownloadString("https://api.magicrust.ru/players/getBanList.php");
                players = JsonConvert.DeserializeObject<List<User_Banned>>(json);
            }
            catch
            {
                return null;
            }
#if DEBUG
            players.Add(new User_Banned()
            {
                banip = "",
                nickname = "",
                reason = "cheater",
                steamid = "76561198874259939",
                time = "24.2.12"
            });
#endif
            var result = (from x in players where x.steamid == id select x);
            if (result.Count()>=1)
            {
                return result.FirstOrDefault();
            }
            return null;
        }
    }
}

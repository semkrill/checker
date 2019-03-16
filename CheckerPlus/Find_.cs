using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Windows;
using CheckerPlus.Properties;
using FastSearchLibrary;
using CheckerPlus;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace CheckerPlus
{
    static class Find_
    {

        #region Classes

        public class typecheat
        {
            public string namefile { get; set; }
            public typecheat(string namefile)
            {
                this.namefile = namefile;
            }
        }

        public class foundedfile
        {
            public string path { get; set; }
            public string namefile { get; set; }
            public string lastchange { get; set; }
            public double height { get; set; }
            public int danger { get; set; }

            public foundedfile(FileInfo file, int dg = 1)
            {
                this.path = file.FullName;
                this.namefile = file.Name;
                this.lastchange = file.LastWriteTime.ToString();
                this.height = (double)file.Length / 1024;
                this.danger = dg;
            }
        }

        #endregion

        static public List<string> dirinclude = new List<string>()
        {
            "Steam",
            "Rust",
            "HurtWorld",
            "Download",
            "Desktop"
            //(dir.Contains("Steam") || dir.Contains("Rust") || dir.Contains("HurtWorld") || dir.Contains("Downloads"))
        };

        static public List<string> warringtext = new List<string>()
        {   
            "recoil",
            "hitbox"
        };

        static public List<string> warringbat = new List<string>()
        {
            ".amc",
            "ak-47",
            "ak47",
            "ак-47",
        };

        static public List<string> warringsearchfile = new List<string>()
        {
            "cheat",
            "bloody",
            "macros",
            "чит",
            "rustcheat",
            "haksclub",
            "hax3s",
            "hack",
            ".ahk",
            ".mefx",
            ".bwp",
            ".amc",
            ".ini",
            ".bat"
        };

        static private List<foundedfile> foundedfiles = new List<foundedfile>();

        static public List<foundedfile> getfiles()
        {
            return foundedfiles;
        }

        static public bool checkinclude(string text)
        {
            foreach (string ad in dirinclude)
                if (text.Contains(ad))
                    return true;

            return false;
        }

        static public bool CheckText(string path,List<string> warrings)
        {
            string text = File.ReadAllText(path).ToLower();
            foreach (string d in warrings)
                if (text.Contains(d))
                    return true;

            return false;
        }

        static public bool checkext(string file)
        {
            string ext = System.IO.Path.GetExtension(file);
            if (!string.IsNullOrEmpty(ext) && (ext.Contains("dll") || ext.Contains("ext")))
                return true;

            return false;
        }

        public static byte[] HexToBytes(this string str)
        {
            if (str.Length == 0 || str.Length % 2 != 0)
                return new byte[0];

            byte[] buffer = new byte[str.Length / 2];
            char c;
            for (int bx = 0, sx = 0; bx < buffer.Length; ++bx, ++sx)
            {
                c = str[sx];
                buffer[bx] = (byte)((c > '9' ? (c > 'Z' ? (c - 'a' + 10) : (c - 'A' + 10)) : (c - '0')) << 4);

                c = str[++sx];
                buffer[bx] |= (byte)(c > '9' ? (c > 'Z' ? (c - 'a' + 10) : (c - 'A' + 10)) : (c - '0'));
            }

            return buffer;
        }

        public static int CheckFileInfo(FileInfo file)
        {
            byte[] he = AdvancedFind.FindSignature2(file.FullName);
            for (int d = 0; d < AdvancedFind.signa.Count(); d++)
            {
                var ada = AdvancedFind.signa[d];
                if (CheckInt(AdvancedFind.Locate(he, ada.sign), ada.pos, ada.pos2))
                {
                    return 3;
                }
            }
            return 0;
        }

        public static bool CheckInt(int[] ee, int pos, int pos2)
        {
            foreach (int d in ee)
            {
                if ((pos == 0 || d < pos) && (d > pos2 || pos2 == 0))
                {
                    return true;
                }
            }

            return false;
        }
    }
}

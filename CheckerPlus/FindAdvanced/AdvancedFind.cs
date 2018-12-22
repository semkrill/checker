using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace CheckerPlus
{
    static class AdvancedFind
    {

        // ENIGMA                     // VMP
        //if (AdvancedFind.FindSignature(@"C:\Soft\Reverse\Softes\Quest\UnpackMe_protected.exe", "454E49474D41")) // 60000060
        //   MaterialMessageBox.Show("Найдена сигнатура!");                                                          E0000060
        //                                                                                                           E0000040 68........E9 68........E8 9c9cc74424
        //
        //Find.GetDanger(Find.PeGet(@"C:\Users\Kaidoz\Downloads\pphudcheat\pphud.beta.dll")); (.*?) Regex.Matches(textlogin, "\\\"76(.*?)\\\"").Cast<Match>().Select(x => "76" + x.Groups[1].Value).ToList();

        public class Sign
        {
            public byte[] sign { get; set; }
            public int pos { get; set; }
            public int pos2 { get; set; }
            public Sign(byte[] sign,int pos = 0, int pos2 = 0)
            {
                this.sign = sign;
                this.pos = pos;
                this.pos2 = pos2;
            }
        }

        #region Lists
        // 60000060
        public static List<Sign> signa = new List<Sign>()
        {
            new Sign(new byte[]{0x45, 0x4E, 0x49, 0x47, 0x4D, 0x41 }),//"454E49474D41"),
            new Sign(new byte[]{ 0x60, 0x00, 0x00, 0x60 }, 600, 800),
            new Sign(new byte[]{ 0xE0, 0x00, 0x00, 0x60 }, 600, 800),
            new Sign(new byte[]{ 0xE0, 0x00, 0x00, 0x40 }, 600, 800),
            new Sign(new byte[]{ 0x9C, 0x9C, 0xC7, 0x47, 0x44, 0x24 }),
            new Sign(new byte[]{0x52, 0x66, 0x68, 0x6E, 0x20, 0x4D, 0x18, 0x22, 0x76, 0xB5, 0x33, 0x11,
    0x12, 0x33, 0x0C, 0x6D, 0x0A, 0x20, 0x4D, 0x18, 0x22, 0x9E, 0xA1, 0x29,
    0x61, 0x1C, 0x76, 0xB5, 0x05, 0x19, 0x01, 0x58}),
            new Sign(new byte[]{0x59, 0x61, 0x6E, 0x6F, 0x41, 0x74, 0x74, 0x72, 0x69, 0x62, 0x75, 0x74,
    0x65})
            //"4424")
        };

        #endregion

        static public bool FindSignature(string path, string sign)
        {
            byte[] d = File.ReadAllBytes(path);
            string hex = BitConverter.ToString(d).Replace("-", string.Empty);
            if (hex.Contains(sign.ToUpper()))
                return true;

            return false;

        }

        static public byte[] FindSignature2(string path)
        {
            return File.ReadAllBytes(path);
        }

        public static string ConvertHex(String hexString)
        {
            string ascii = string.Empty;

            for (int i = 0; i < hexString.Length; i += 2)
            {
                String hs = string.Empty;

                hs = hexString.Substring(i, 2);
                uint decval = System.Convert.ToUInt32(hs, 16);
                char character = System.Convert.ToChar(decval);
                ascii += character;

            }

            return ascii;
        }

        static readonly int[] Empty = new int[0];

        public static int[] Locate(this byte[] self, byte[] candidate)
        {
            if (IsEmptyLocate(self, candidate))
                return Empty;

            var list = new List<int>();

            for (int i = 0; i < self.Length; i++)
            {
                if (!IsMatch(self, i, candidate))
                    continue;

                list.Add(i);
            }

            return list.Count == 0 ? Empty : list.ToArray();
        }

        static bool IsMatch(byte[] array, int position, byte[] candidate)
        {
            if (candidate.Length > (array.Length - position))
                return false;

            for (int i = 0; i < candidate.Length; i++)
                if (array[position + i] != candidate[i])
                    return false;

            return true;
        }

        static bool IsEmptyLocate(byte[] array, byte[] candidate)
        {
            return array == null
                || candidate == null
                || array.Length == 0
                || candidate.Length == 0
                || candidate.Length > array.Length;
        }
    }
}

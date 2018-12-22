// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
#pragma warning disable IDE1901
#pragma warning disable IDE1006

namespace CheckerPlus
{
    public class KeyCheck
    {

        #region Mouse
        [DllImport("user32.dll", SetLastError = true)]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);

        void CheckMouse()
        {
            mouse_event((uint)MouseEventFlags.RIGHTDOWN, 0, 0, 0, 0);
            mouse_event((uint)MouseEventFlags.LEFTDOWN, 0, 0, 0, 0);

            Thread.Sleep(2000);
            mouse_event((uint)MouseEventFlags.LEFTUP, 0, 0, 0, 0);
            mouse_event((uint)MouseEventFlags.RIGHTUP, 0, 0, 0, 0);
        }


        [Flags]
        public enum MouseEventFlags
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010,
            XDOWN = 0x00000080,
            XUP = 0x00000100
        }

        public enum MouseEventDataXButtons : uint
        {
            XBUTTON1 = 0x00000001,
            XBUTTON2 = 0x00000002
        }

        private void xbuttonspress(MouseEventDataXButtons msa)
        {
            mouse_event((uint)msa, 0, 0, 0, 0);

            Thread.Sleep(100);

            mouse_event((uint)MouseEventFlags.XUP, 0, 0, 0, 0);
        }

        private void middlepress()
        {
            mouse_event((uint)MouseEventFlags.MIDDLEDOWN, 0, 0, 0, 0);

            Thread.Sleep(100);
            mouse_event((uint)MouseEventFlags.MIDDLEUP, 0, 0, 0, 0);
        }

        #endregion

        public class KeyComb
        {

            public byte key1 { get; }
            public byte key2 { get; }
            public KeyComb(byte key1, byte key2)
            {
                this.key1 = key1;
                this.key2 = key2;
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);


        private static void SendKey(byte key1, byte Key2, byte key3 = 0x00, int th = 0)
        {
            if (key1 != 0x00) keybd_event(key1, 0, 0, 0);
            keybd_event(Key2, 0, 0, 0);
            if (key3 != 0x00) keybd_event(key3, 0, 0, 0);

            if (th != 0)
                Thread.Sleep(th);
            else
                Thread.Sleep(100);

            if (key1 != 0x00) keybd_event(key1, 0, 0x2, 0);
            keybd_event(Key2, 0, 0x2, 0);
            if (key3 != 0x00) keybd_event(key3, 0, 0x2, 0);
        }

        // CONTROL 0x11

        List<KeyComb> keys = new List<KeyComb>()
        {
            new KeyComb(0x11,0x70), // F1
            new KeyComb(0x11,0x71),
            new KeyComb(0x11,0x72),
            new KeyComb(0x11,0x73),
            new KeyComb(0x11,0x74),
            new KeyComb(0x11,0x75),
            new KeyComb(0x11,0x76),
            new KeyComb(0x11,0x77),
            new KeyComb(0x11,0x78),
            new KeyComb(0x11,0x79),
            new KeyComb(0x11,0x7A),
            new KeyComb(0x11,0x7B), // F12
            new KeyComb(0x11,0x2D), // Ins
            new KeyComb(0x11,0x2E), // del
            new KeyComb(0x11,0x24),
            new KeyComb(0x00,0x05), // X1 Mouse
            new KeyComb(0x00,0x06), // X2 Mouse
            new KeyComb(0x00,0x04),
            new KeyComb(0x00,0x60), // Num1 // 0x90
            new KeyComb(0x00,0x61),
            new KeyComb(0x00,0x62),
            new KeyComb(0x00,0x63),
            new KeyComb(0x00,0x64),
            new KeyComb(0x00,0x65),
            new KeyComb(0x00,0x66),
            new KeyComb(0x00,0x67),
            new KeyComb(0x00,0x68),
            new KeyComb(0x00,0x69), // num2
            new KeyComb(0x00,0x02) /// mouse pricel
            // keys
        };

        List<KeyComb> mouse_keys = new List<KeyComb>()
        {
            new KeyComb(0x00,0x05), // X1 Mouse
            new KeyComb(0x00,0x06), // X2 Mouse
            new KeyComb(0x00,0x04)
        };

        List<string> cmds = new List<string>()
        {
            "debugcamera"
        };

        public static void PasteCommand(string command)
        {
            try
            {
                Clipboard.SetText(command);
                SendKey(0x11, 0x56);
                SendKey(0x00, 0x0D);
            }
            catch { }
        }

        public bool startfind(bool game)
        {
            Thread th = new Thread(key_start);
            th.IsBackground = true;
            th.Start(game);
            th.Join();
            return true;
        }

        public bool startpress_mouse()
        {
            Thread th = new Thread(mouse_start);
            th.IsBackground = true;
            th.Start();
            th.Join();
            return true;
        }

        private void mouse_start()
        {
            for (int a = 0; a < mouse_keys.Count(); a++)
            {
                SendKey(keys[a].key1, keys[a].key2);
            }
        }


        public void StartCheck(object game)
        {
            key_start(game);
            Command_Start();
        }

        private void key_start(object game)
        {
            for (int a = 0; a < keys.Count(); a++)
            {
                if (keys[a].key2 == 0x60)
                {
                    if (!IsNumLockOn())
                        SendKey(0, 0x90);
                }
                if (keys[a].key2 == 0x02)
                {
                    if ((bool)game)
                        CheckMouse();
                }
                else
                SendKey(keys[a].key1, keys[a].key2);
#if DEBUG
                Debug.WriteLine((char)keys[a].key1 + " " + (char)keys[a].key2);
#endif
            }
        }


        private void Command_Start()
        {
            SendKey(0x00, 0xC0);
            foreach (string d in cmds)
                PasteCommand(d);
        }


        [DllImport("user32.dll")]
        extern static short GetKeyState(byte key);

        int IsKeyPressedEx(byte key)
        {
            short result = GetKeyState(key);

            switch (result)
            {
                case -1:            // Not pressed and not toggled on.
                    return -1;
                case 1:            // Not pressed, but toggled on                    
                    return 1;
                default:    // Pressed (and may be toggled on)                    
                    return (result & 128) == 128 ? 0 : -1;
            }
        }

        public bool IsNumLockOn()
        {
            return IsKeyPressedEx(0x90) == 1;
        }
    }
}

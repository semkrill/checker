using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Diagnostics;

namespace WindowsHookLiba
{
    [DebuggerNonUserCode]
    internal static class UnsafeNativeMethods
    {
        #region ' Structures '

        [DebuggerNonUserCode]
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct KeyboardData
        {
            public UInt32 vkCode;
            public UInt32 scanCode;
            public UInt32 flags;
            public UInt32 time;
            public IntPtr dwExtraInfo;
        }

        [DebuggerNonUserCode]
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct MouseInfo
        {
            public Point pt;
            public UInt32 mouseData;
            public UInt32 dwFlags;
            public UInt32 time;
            public IntPtr dwExtraInfo;
        }

        [DebuggerNonUserCode]
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct MsInput
        {
            public UInt32 dwType;
            public MouseInfo xi;
        }

        #endregion

        #region ' Constants '

        public const Int32 HC_ACTION = 0;
        public const UInt32 INPUT_HARDWARE = 2;

        #region ' Keyboard '

        public const UInt32 INPUT_KEYBOARD = 1;

        public const Int32 WH_KEYBOARD = 2;
        public const Int32 WH_KEYBOARD_LL = 13;
        public const Int32 WM_KEYDOWN = 0x100;
        public const Int32 WM_KEYUP = 0x101;
        public const Int32 WM_SYSKEYDOWN = 0x104;
        public const Int32 WM_SYSKEYUP = 0x105;

        #endregion

        #region ' Mouse '

        public const UInt32 INPUT_MOUSE = 0;

        public const Int32 WH_MOUSE = 7;
        public const Int32 WH_MOUSE_LL = 14;
        public const Int32 WM_MOUSEMOVE = 0x200;
        public const Int32 WM_LBUTTONDOWN = 0x201;
        public const Int32 WM_LBUTTONUP = 0x202;
        public const Int32 WM_LBUTTONDBLCLK = 0x203;
        public const Int32 WM_RBUTTONDOWN = 0x204;
        public const Int32 WM_RBUTTONUP = 0x205;
        public const Int32 WM_RBUTTONDBLCLK = 0x206;
        public const Int32 WM_MBUTTONDOWN = 0x207;
        public const Int32 WM_MBUTTONUP = 0x208;
        public const Int32 WM_MBUTTONDBLCLK = 0x209;
        public const Int32 WM_MOUSEWHEEL = 0x20A;
        public const Int32 WM_MOUSEHWHEEL = 0x20E;
        public const Int32 WM_XBUTTONDOWN = 0x20B;
        public const Int32 WM_XBUTTONUP = 0x20C;
        public const Int32 WM_XBUTTONDBLCLK = 0x20D;
        public const Int32 WM_NCXBUTTONDOWN = 0xAB;
        public const Int32 WM_NCXBUTTONUP = 0xAC;
        public const Int32 WM_NCXBUTTONDBLCLK = 0xAD;

        // These are SynthesizeMouse constants
        public const UInt32 MOUSEEVENTF_ABSOLUTE = 0x8000;
        public const UInt32 MOUSEEVENTF_LEFTDOWN = 0x2;
        public const UInt32 MOUSEEVENTF_LEFTUP = 0x4;
        public const UInt32 MOUSEEVENTF_MIDDLEDOWN = 0x20;
        public const UInt32 MOUSEEVENTF_MIDDLEUP = 0x40;
        public const UInt32 MOUSEEVENTF_MOVE = 0x1;
        public const UInt32 MOUSEEVENTF_RIGHTDOWN = 0x8;
        public const UInt32 MOUSEEVENTF_RIGHTUP = 0x10;
        public const UInt32 MOUSEEVENTF_VIRTUALDESK = 0x4000;
        public const UInt32 MOUSEEVENTF_WHEEL = 0x800;
        public const UInt32 MOUSEEVENTF_XDOWN = 0x80;
        public const UInt32 MOUSEEVENTF_XUP = 0x100;
        public const UInt32 XBUTTON1 = 0x1;
        public const UInt32 XBUTTON2 = 0x2;
        public const UInt32 WHEEL_DELTA = 120;

        #endregion

        #region ' Clipboard '

        public const Int32 WM_DRAWCLIPBOARD = 0x308;
        public const Int32 WM_CHANGECBCHAIN = 0x30D;

        #endregion

        #endregion

        #region ' Methods '

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(
            Int32 idHook,
            Delegate lpfn,
            IntPtr hMod,
            UInt32 dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(
            IntPtr hook);

        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(
            IntPtr hhk,
            Int32 nCode,
            IntPtr wParam,
            ref KeyboardData lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(
            IntPtr hhk,
            Int32 nCode,
            IntPtr wParam,
            ref MouseInfo lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern UInt32 SendInput(
            UInt32 cInputs,
            ref MsInput pInputs,
            Int32 cbSize);

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(
            Point pt);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetClipboardViewer(
            IntPtr hWndNewViewer);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ChangeClipboardChain(
            IntPtr hWndRemove,
            IntPtr hWndNewNext);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SendMessage(
            IntPtr hWnd,
            UInt32 Msg,
            IntPtr wParam,
            IntPtr lParam);

        [DllImport("kernel32.dll")]
        public static extern void SetLastError(
            UInt32 errorCode);

        [DllImport("kernel32.dll")]
        public static extern UInt32 GetLastError();

        #endregion
    }
}

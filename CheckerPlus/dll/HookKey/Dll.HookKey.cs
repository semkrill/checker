using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
//using WindowsHookLib;
using System.Windows.Threading;
using DesktopWPFAppLowLevelKeyboardHook;
using System.Reflection;
using System.Windows.Input;

namespace CheckerPlus.Hooks
{
    class Main
    {
        private LowLevelKeyboardListener _listener;
        //private KeyboardHook keyboardHook;

        public void Start()
        {
            _listener = new LowLevelKeyboardListener();
            _listener.OnKeyPressed += _listener_OnKeyPressed;

            //keyboardHook = new KeyboardHook();
            //keyboardHook.KeyUp += KeyboardHook_KeyUp;
        }

        void _listener_OnKeyPressed(object sender, KeyPressedArgs e)
        {
            Dll.MessageKey.KeyShow.Show((System.Windows.Forms.Keys)KeyInterop.VirtualKeyFromKey(e.KeyPressed));
        }

        public void Uninstall()
        {
            _listener.UnHookKeyboard();
            //keyboardHook.RemoveHook();
        }

        public void Install()
        {
            _listener.HookKeyboard();
            //keyboardHook.InstallHook();
        }

    }
}

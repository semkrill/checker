// Author: Arman Ghazanchyan
// Created: 11/02/2006
// Modified: 09/13/2010

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Reflection;

namespace WindowsHookLiba
{
    /// <summary>
    /// Provides functionality to hook the keyboard system wide (low level).
    /// </summary>
    [DebuggerNonUserCode]
    [DefaultEvent("KeyDown"), ToolboxBitmap(typeof(KeyboardHook), "Resources.keyboard"),
    Description("Component that hooks the keyboard system wide and raises some useful events.")]
    public partial class KeyboardHook : Component
    {
        #region ' Event Handlers and Delegates '

        /// <summary>
        /// Occurs when the KeyboardHook state changed.
        /// </summary>
        [Description("Occurs when the KeyboardHook state changed.")]
        public event System.EventHandler<WindowsHookLiba.StateChangedEventArgs> StateChanged;
        /// <summary>
        /// Occurs when a key is first pressed.
        /// </summary>
        [Description("Occurs when a key is first pressed.")]
        public event System.EventHandler<WindowsHookLiba.KeyboardEventArgs> KeyDown;
        /// <summary>
        /// Occurs when a key is released.
        /// </summary>
        [Description("Occurs when a key is released.")]
        public event System.EventHandler<WindowsHookLiba.KeyboardEventArgs> KeyUp;
        /// <summary>
        /// Represents the method that will handle the keyboard message event.
        /// </summary>
        delegate IntPtr KeyboardMessageEventHandler(Int32 nCode, IntPtr wParam, ref UnsafeNativeMethods.KeyboardData lParam);

        #endregion

        #region ' Members '

        // Holds a method pointer to KeyboardProc for callback.
        // Needed for InstallHook method.
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [MarshalAs(UnmanagedType.FunctionPtr)]
        private KeyboardHook.KeyboardMessageEventHandler _keyboardProc;
        // Holds the keyboard hook handle. Needed 
        // for RemoveHook and KeyboardProc methods.
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IntPtr _hKeyboardHook;
        // Holds a bitwise combination of the keys that are up or down.
        // Needed for key down and key up events’ KeyEventArgs object.
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Keys _keyData;

        #endregion

        #region ' Properties '

        /// <summary>
        /// Gets the component's assembly information.
        /// </summary>
        public static Assembly AssemblyInfo
        {
            get
            {
                return Assembly.GetExecutingAssembly();
            }
        }

        /// <summary>
        /// Gets a Boolean value indicating if the ALT key is down.
        /// </summary>
        public bool AltKeyDown
        {
            get
            {
                return (this._keyData & Keys.Alt) == Keys.Alt;
            }
        }

        /// <summary>
        /// Gets a Boolean value indicating if the CTRL key is down.
        /// </summary>
        public bool CtrlKeyDown
        {
            get
            {
                return (this._keyData & Keys.Control) == Keys.Control;
            }
        }

        /// <summary>
        /// Gets a Boolean value indicating if the SHIFT key is down.
        /// </summary>
        public bool ShiftKeyDown
        {
            get
            {
                return (this._keyData & Keys.Shift) == Keys.Shift;
            }
        }

        /// <summary>
        /// Gets the state of the hook.
        /// </summary>
        public HookState State
        {
            get
            {
                if (this._hKeyboardHook != IntPtr.Zero)
                    return HookState.Installed;
                else
                    return HookState.Uninstalled;
            }
        }

        #endregion

        #region ' Methods '

        /// <summary>
        /// Default constructor.
        /// </summary>
        public KeyboardHook()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Installs the keyboard hook for this application. 
        /// </summary>
        public void InstallHook()
        {
            if (this._hKeyboardHook == IntPtr.Zero)
            {
                this._keyboardProc = new KeyboardHook.KeyboardMessageEventHandler(KeyboardProc);
                IntPtr hinstDLL = Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]);
                this._hKeyboardHook = UnsafeNativeMethods.SetWindowsHookEx(UnsafeNativeMethods.WH_KEYBOARD_LL, this._keyboardProc, hinstDLL, 0);
                if (this._hKeyboardHook == IntPtr.Zero)
                {
                    // Failed to hook. Throw a HookException
                    int eCode = Marshal.GetLastWin32Error();
                    this._keyboardProc = null;
                    //throw new WindowsHookException(new Win32Exception(eCode).Message);
                }
                else
                {
                    this.OnStateChanged(new WindowsHookLiba.StateChangedEventArgs(this.State));
                }
            }
        }

        /// <summary>
        /// Removes the keyboard hook for this application.
        /// </summary>
        public void RemoveHook()
        {
            if (this._hKeyboardHook != IntPtr.Zero)
            {
                if (!UnsafeNativeMethods.UnhookWindowsHookEx(this._hKeyboardHook))
                {
                    // Failed to remove the hook. Throw a HookException
                    int eCode = Marshal.GetLastWin32Error();
                    throw new WindowsHookException(new Win32Exception(eCode).Message);
                }
                else
                {
                    this._keyboardProc = null;
                    this._hKeyboardHook = IntPtr.Zero;
                    this._keyData = Keys.None;
                    this.OnStateChanged(new WindowsHookLiba.StateChangedEventArgs(this.State));
                }
            }
        }

        /// <summary>
        /// Safely removes the hook without throwing exception.
        /// </summary>
        private void SafeRemove()
        {
            if (this._hKeyboardHook != IntPtr.Zero)
            {
                UnsafeNativeMethods.UnhookWindowsHookEx(this._hKeyboardHook);
                this._keyboardProc = null;
                this._hKeyboardHook = IntPtr.Zero;
                this._keyData = Keys.None;
                this.OnStateChanged(new WindowsHookLiba.StateChangedEventArgs(this.State));
            }
        }

        // This sub processes all the keyboard messages and passes to the other windows
        private IntPtr KeyboardProc(int nCode, IntPtr wParam, ref UnsafeNativeMethods.KeyboardData lParam)
        {
            if (nCode >= UnsafeNativeMethods.HC_ACTION)
            {
                WindowsHookLiba.KeyboardEventArgs e;
                Keys keyCode = (Keys)lParam.vkCode;
                if ((int)wParam == UnsafeNativeMethods.WM_KEYDOWN | (int)wParam == UnsafeNativeMethods.WM_SYSKEYDOWN)
                {
                    if (keyCode == Keys.LMenu | keyCode == Keys.RMenu)
                    {
                        this._keyData = (this._keyData | Keys.Alt);
                        e = new WindowsHookLiba.KeyboardEventArgs(this._keyData | Keys.Menu, keyCode);
                    }
                    else if (keyCode == Keys.LControlKey | keyCode == Keys.RControlKey)
                    {
                        this._keyData = (this._keyData | Keys.Control);
                        e = new WindowsHookLiba.KeyboardEventArgs(this._keyData | Keys.ControlKey, keyCode);
                    }
                    else if (keyCode == Keys.LShiftKey | keyCode == Keys.RShiftKey)
                    {
                        this._keyData = (this._keyData | Keys.Shift);
                        e = new WindowsHookLiba.KeyboardEventArgs(this._keyData | Keys.ShiftKey, keyCode);
                    }
                    else
                        e = new WindowsHookLiba.KeyboardEventArgs(this._keyData | keyCode, keyCode);

                    this.OnKeyDown(e);
                    if (e.Handled)
                        return new IntPtr(1);
                }
                else if ((int)wParam == UnsafeNativeMethods.WM_KEYUP | (int)wParam == UnsafeNativeMethods.WM_SYSKEYUP)
                {
                    if (keyCode == Keys.LMenu | keyCode == Keys.RMenu)
                    {
                        this._keyData = (this._keyData & ~Keys.Alt);
                        e = new WindowsHookLiba.KeyboardEventArgs(this._keyData | Keys.Menu, keyCode);
                    }
                    else if (keyCode == Keys.LControlKey | keyCode == Keys.RControlKey)
                    {
                        this._keyData = (this._keyData & ~Keys.Control);
                        e = new WindowsHookLiba.KeyboardEventArgs(this._keyData | Keys.ControlKey, keyCode);
                    }
                    else if (keyCode == Keys.LShiftKey | keyCode == Keys.RShiftKey)
                    {
                        this._keyData = (this._keyData & ~Keys.Shift);
                        e = new WindowsHookLiba.KeyboardEventArgs(this._keyData | Keys.ShiftKey, keyCode);
                    }
                    else
                        e = new WindowsHookLiba.KeyboardEventArgs(this._keyData | keyCode, keyCode);

                    this.OnKeyUp(e);
                    if (e.Handled)
                        return new IntPtr(1);
                }
            }
            return UnsafeNativeMethods.CallNextHookEx(this._hKeyboardHook, nCode, wParam, ref lParam);
        }

        #endregion

        #region ' On Event '

        /// <summary>
        /// Raises the WindowsHookLiba.KeyboardHook.StateChanged event.
        /// </summary>
        /// <param name="e">A WindowsHookLiba.StateChangedEventArgs
        /// that contains the event data.</param>
        protected virtual void OnStateChanged(WindowsHookLiba.StateChangedEventArgs e)
        {
            if (StateChanged != null)
                StateChanged(this, e);
        }

        /// <summary>
        /// Raises the WindowsHookLiba.KeyboardHook.KeyUp event.
        /// </summary>
        /// <param name="e">A WindowsHookLiba.KeyBoardEventArgs
        /// that contains the event data.</param>
        protected virtual void OnKeyUp(WindowsHookLiba.KeyboardEventArgs e)
        {
            if (KeyUp != null)
                KeyUp(this, e);
        }

        /// <summary>
        /// Raises the WindowsHookLiba.KeyboardHook.KeyDown event.
        /// </summary>
        /// <param name="e">A WindowsHookLiba.KeyBoardEventArgs
        /// that contains the event data.</param>
        protected virtual void OnKeyDown(WindowsHookLiba.KeyboardEventArgs e)
        {
            if (KeyDown != null)
                KeyDown(this, e);
        }

        #endregion
    }
}

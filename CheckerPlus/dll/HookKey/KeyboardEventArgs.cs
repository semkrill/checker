using System.Windows.Forms;
using System.Diagnostics;
using System.ComponentModel;

namespace WindowsHookLiba
{
    /// <summary>
    /// Provides data for the WindowsHookLib.KeyboardHook.KeyDown and 
    /// WindowsHookLib.KeyboardHook.KeyUp events. 
    /// </summary>
    [DebuggerNonUserCode]
    public class KeyboardEventArgs : System.Windows.Forms.KeyEventArgs
    {
        #region ' Members '

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Keys _vkCode;

        #endregion

        #region ' Properties '

        /// <summary>
        /// Gets or sets a value indicating whether the event was handled.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never),
        DebuggerBrowsable(DebuggerBrowsableState.Never)]
        new public bool SuppressKeyPress
        {
            get
            {
                return base.SuppressKeyPress;
            }
            set
            {
                base.SuppressKeyPress = value;
            }
        }

        /// <summary>
        /// Gets the virtual key code for a KeyDown or KeyUp event.
        /// </summary>
        public Keys VirtualKeyCode
        {
            get
            {
                return this._vkCode;
            }
        }

        #endregion

        #region ' Methods '

        /// <param name="keyData">A System.Windows.Forms.Keys representing 
        /// the key that was pressed, combined with any modifier flags that 
        /// indicate which CTRL, SHIFT, and ALT keys were pressed at the same time. 
        /// Possible values are obtained by applying bitwise OR (|) operator 
        /// to constants from the System.Windows.Forms.Keys enumeration.</param>
        /// <param name="virtualKeyCode">The virtual key code.</param>
        public KeyboardEventArgs(Keys keyData, Keys virtualKeyCode)
            : base(keyData)
        {
            this._vkCode = virtualKeyCode;
        }

        #endregion
    }
}

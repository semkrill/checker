using System;
using System.Diagnostics;

namespace WindowsHookLiba
{
    #region ' Enumarations '

    /// <summary>
    /// Specifies constants that define the state of the hook.
    /// </summary>
    public enum HookState
    {
        /// <summary>
        /// The unistalled state.
        /// </summary>
        Uninstalled = 0,
        /// <summary>
        /// The installed state.
        /// </summary>
        Installed = 1
    }

    #endregion

    /// <summary>
    /// Provides data for the WindowsHookLib.MouseHook.StateChanged, 
    /// WindowsHookLib.KeyboardHook.StateChanged and WindowsHookLib.ClipboardHook.StateChanged events.
    /// </summary>
    [DebuggerNonUserCode]
    [DebuggerDisplay("State = {_state}")]
    public class StateChangedEventArgs : System.EventArgs
    {
        #region ' Members '

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly HookState _state;

        #endregion

        #region ' Properties '

        /// <summary>
        /// Gets a value indicating whether the hook is installed. 
        /// </summary>
        public HookState State
        {
            get
            {
                return this._state;
            }
        }

        #endregion

        #region ' Methods '

        /// <param name="hookState">A WindowsHookLib.HookState enumeration 
        /// value representing the state of the hook.</param>
        public StateChangedEventArgs(HookState hookState)
        {
            this._state = hookState;
        }

        #endregion
    }
}

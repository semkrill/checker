using System;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.Globalization;
using System.Security.Permissions;

namespace WindowsHookLiba
{
    /// <summary>
    /// Represents errors that occur in WindowsHookLib.ClipboardHook, 
    /// WindowsHookLib.MouseHook and WindowsHookLib.KeyboardHook.
    /// </summary>
    [Serializable]
    [DebuggerNonUserCode]
    public class WindowsHookException : ArgumentException
    {
        #region ' Members '

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private object _actualValue = null;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private const string DEFAULT_MESSAGE = "Exception of type 'WindowsHookLib.WindowsHookException' was thrown.";

        #endregion

        #region ' Properties '

        /// <summary>
        /// Gets the actural value of the parameter that causes this exception.
        /// </summary>
        public object ActualValue
        {
            get
            {
                return this._actualValue;
            }
        }

        /// <summary>
        /// Gets the error message of the exception.
        /// </summary>
        public override string Message
        {
            get
            {
                string message = base.Message;
                if (this._actualValue == null)
                {
                    return message;
                }
                string value = "Actual value was " + this._actualValue.ToString() + ".";
                if (string.IsNullOrEmpty(message))
                {
                    return value;
                }
                return (message + Environment.NewLine + value);
            }
        }

        #endregion

        #region ' Methods '

        /// <summary>
        /// Default constructor.
        /// </summary>
        public WindowsHookException()
            : base(DEFAULT_MESSAGE) { }

        /// <param name="message">The exception message.</param>
        public WindowsHookException(string message)
            : base(message = (string.IsNullOrEmpty(message) ? DEFAULT_MESSAGE : message)) { }

        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public WindowsHookException(string message, Exception innerException)
            : base(message = (string.IsNullOrEmpty(message) ? DEFAULT_MESSAGE : message), innerException) { }

        /// <param name="message">The exception message.</param>
        /// <param name="parameterName">The invalid parameter name.</param>
        public WindowsHookException(string message, string parameterName)
            : base(message = (string.IsNullOrEmpty(message) ? DEFAULT_MESSAGE : message), parameterName) { }

        /// <param name="message">The exception message.</param>
        /// <param name="parameterName">The invalid parameter name.</param>
        /// <param name="actualValue">The parameter value.</param>
        public WindowsHookException(string message, string parameterName, object actualValue)
            : base(message = (string.IsNullOrEmpty(message) ? DEFAULT_MESSAGE : message), parameterName)
        {
            this._actualValue = actualValue;
        }

        /// <param name="info">The serialization information.</param>
        /// <param name="context"></param>
        protected WindowsHookException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        /// <summary>
        /// Sets the System.Runtime.Serialization.SerializationInfo object 
        /// with the parameter name, actual value and additional exception information.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            base.GetObjectData(info, context);
            info.AddValue("ActualValue", this._actualValue, typeof(object));
        }

        #endregion
    }
}

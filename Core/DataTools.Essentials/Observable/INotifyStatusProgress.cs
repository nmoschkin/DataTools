using System;

// ' The INotifyStatusProgress framework.
// ' Copyright (C) 2015 Nathan Moschkin
// ' All Rights Reserved.

namespace DataTools.Essentials.Observable
{
    /// <summary>
    /// Progress event handler delegate
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ProgressEventHandler(object sender, StatusProgressEventArgs e);

    
    /// <summary>
    /// Progress finished delegate
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void FinishedEventHandler(object sender, StatusProgressEventArgs e);

    /// <summary>
    /// Progress starting delegate
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void StartingEventHandler(object sender, StatusProgressEventArgs e);

    /// <summary>
    /// Status display characteristics.
    /// </summary>
    [Flags]
    public enum StatusModes
    {
        /// <summary>
        /// Static display
        /// </summary>
        StaticDisplay = 1,

        /// <summary>
        /// Progress bar
        /// </summary>
        ProgressBar = 2,

        /// <summary>
        /// Static progress bar
        /// </summary>
        ProgressStatic = 4,

        /// <summary>
        /// Marquis progress bar
        /// </summary>
        Marquis = 8
    }

    /// <summary>
    /// Work progress status codes.
    /// </summary>
    public enum StatusCodes
    {
        /// <summary>
        /// The process was aborted or failed
        /// </summary>
        Aborted = -1,

        /// <summary>
        /// The process has finished
        /// </summary>
        Stopped = 0,

        /// <summary>
        /// The process is starting
        /// </summary>
        Starting = 1,

        /// <summary>
        /// The process is running
        /// </summary>
        Running = 2,

        /// <summary>
        /// The process is paused
        /// </summary>
        Paused = 3
    }

    /// <summary>
    /// Provides an common interface for classes that do long-running out-of-process work.
    /// </summary>
    public interface INotifyStatusProgress
    {
        /// <summary>
        /// Raised while the work is progressing.
        /// </summary>
        event ProgressEventHandler Progress;

        /// <summary>
        /// Raised when work is completed.
        /// </summary>
        event FinishedEventHandler Finished;

        /// <summary>
        /// Raised when work is starting.
        /// </summary>
        event StartingEventHandler Starting;

        /// <summary>
        /// Call this while the work is progressing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnProgress(object sender, StatusProgressEventArgs e);

        /// <summary>
        /// Call this when the work is complete.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnFinished(object sender, StatusProgressEventArgs e);

        /// <summary>
        /// Call this when the work is starting.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnStarting(object sender, StatusProgressEventArgs e);

        /// <summary>
        /// Call this to invoke a method on another thread or the Dispatcher thread.
        /// The implementing class must provide for the Dispatcher or other means of invoking a method.
        /// </summary>
        /// <param name="func">The delegate function to invoke.</param>
        void Invoke(Delegate func);

        /// <summary>
        /// Call this to begin the process of invoking a method on another thread or the Dispatcher thread.
        /// The implementing class must provide for the Dispatcher or other means of invoking a method.
        /// </summary>
        /// <param name="func">The delegate function to invoke.</param>
        void BeginInvoke(Delegate func);
    }

    /// <summary>
    /// Arguments to be used when calling events implemented in <see cref="INotifyStatusProgress" />
    /// </summary>
    public class StatusProgressEventArgs : EventArgs
    {
        private int _pos;
        private int _count;
        private string _msg;
        private object _data;
        private int _msgcode;
        private string _detail;
        private StatusModes _mode;
        private StatusCodes _scode = StatusCodes.Stopped;

        /// <summary>
        /// Gets or sets a value indicating a desire to abort the current process.
        /// </summary>
        /// <returns></returns>
        public bool Abort { get; set; }

        /// <summary>
        /// Gets the current status display characteristics.
        /// </summary>
        /// <returns></returns>
        public StatusModes Mode
        {
            get
            {
                return _mode;
            }

            set
            {
                _mode = value;
            }
        }

        /// <summary>
        /// Gets the current status code.
        /// </summary>
        /// <returns></returns>
        public StatusCodes StatusCode
        {
            get
            {
                return _scode;
            }

            set
            {
                _scode = value;
            }
        }

        /// <summary>
        /// Gets the numeric message code.
        /// </summary>
        /// <returns></returns>
        public int MessageCode
        {
            get
            {
                return _msgcode;
            }

            set
            {
                _msgcode = value;
            }
        }

        /// <summary>
        /// Gets the current position.
        /// </summary>
        /// <returns></returns>
        public int Position
        {
            get
            {
                return _pos;
            }

            set
            {
                _pos = value;
            }
        }

        /// <summary>
        /// Gets the total number of elements.
        /// </summary>
        /// <returns></returns>
        public int Count
        {
            get
            {
                return _count;
            }

            set
            {
                _count = value;
            }
        }

        /// <summary>
        /// Gets the status message text.
        /// </summary>
        /// <returns></returns>
        public string Message
        {
            get
            {
                return _msg;
            }

            set
            {
                _msg = value;
            }
        }

        /// <summary>
        /// Gets the status detail text.
        /// </summary>
        /// <returns></returns>
        public string Detail
        {
            get
            {
                return _detail;
            }

            set
            {
                _detail = value;
            }
        }

        /// <summary>
        /// Gets or sets miscellaneous data.
        /// </summary>
        /// <returns></returns>
        public object Data
        {
            get
            {
                return _data;
            }

            set
            {
                _data = value;
            }
        }

        /// <summary>
        /// Create a new <see cref="StatusProgressEventArgs"/> instance
        /// </summary>
        public StatusProgressEventArgs()
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="mode">A StatusModes value indicating the desired display characteristics.</param>
        /// <param name="msg">The textual status message.</param>
        /// <param name="count">The total number of elements.</param>
        /// <param name="detail">Optional detailed textual status message.</param>
        /// <param name="code">Optional numeric message code.</param>
        /// <param name="data">Optional miscellaneous data.</param>
        public StatusProgressEventArgs(StatusModes mode, string msg, int count, string detail = null, int code = 0, object data = null)
        {
            _scode = StatusCodes.Starting;
            _mode = mode;
            _msg = msg;
            _pos = 0;
            _count = count;
            _data = data;
            _msgcode = code;
            _detail = detail;
        }

        /// <summary>
        /// Creates a new StatusProgressEventArgs object with the specified parameters.
        /// </summary>
        /// <param name="status">A StatusCodes value indicating the type of status update.</param>
        /// <param name="msg">The textual status message.</param>
        /// <param name="pos">The numeric position.</param>
        /// <param name="count">The total number of elements.</param>
        /// <param name="detail">Optional detailed textual status message.</param>
        /// <param name="code">Optional numeric message code.</param>
        /// <param name="data">Optional miscellaneous data.</param>
        /// <param name="mode">Optional StatusModes value indicating the desired display characteristics.</param>
        public StatusProgressEventArgs(StatusCodes status, string msg, int pos, int count, string detail = null, int code = 0, object data = null, StatusModes mode = StatusModes.ProgressBar)
        {
            _scode = status;
            _mode = mode;
            _msg = msg;
            _pos = pos;
            _count = count;
            _data = data;
            _msgcode = code;
            _detail = detail;
        }
    }
}
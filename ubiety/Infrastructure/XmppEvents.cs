// XmppEvents.cs
//
//Ubiety XMPP Library Copyright (C) 2015 Dieter Lunn
//
//This library is free software; you can redistribute it and/or modify it under
//the terms of the GNU Lesser General Public License as published by the Free
//Software Foundation; either version 3 of the License, or (at your option)
//any later version.
//
//This library is distributed in the hope that it will be useful, but WITHOUT
//ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
//FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
//
//You should have received a copy of the GNU Lesser General Public License along
//with this library; if not, write to the Free Software Foundation, Inc., 59
//Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;
using Ubiety.Common;

namespace Ubiety.Infrastructure
{
    /// <summary>
    /// </summary>
    public class TagEventArgs : EventArgs
    {
        /// <summary>
        /// </summary>
        /// <param name="tag"></param>
        public TagEventArgs(Tag tag)
        {
            Tag = tag;
        }

        /// <summary>
        /// </summary>
        public Tag Tag { get; private set; }
    }

    /// <summary>
    /// </summary>
    public class ErrorEventArgs : EventArgs
    {
        private readonly ErrorLevel _level;
        private readonly string _message;
        private readonly ErrorType _type;

        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        /// <param name="level"></param>
        public ErrorEventArgs(string message, ErrorType type, ErrorLevel level)
        {
            _message = message;
            _type = type;
            _level = level;
        }

        /// <value>
        ///     The default error message.
        /// </value>
        public string Message
        {
            get { return _message; }
        }

        /// <value>
        ///     The type of error that is being returned.
        /// </value>
        public ErrorType Type
        {
            get { return _type; }
        }

        /// <summary>
        /// </summary>
        public ErrorLevel Level
        {
            get { return _level; }
        }
    }

    /// <summary>
    /// </summary>
    public class XmppEvents
    {
        #region Internal Connect

        /// <summary>
        /// </summary>
        public event EventHandler<EventArgs> OnConnect;

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void Connect(object sender, EventArgs args = default (EventArgs))
        {
            if (OnConnect != null)
            {
                OnConnect(sender, args);
            }
        }

        #endregion

        #region Internal Disconnect

        /// <summary>
        /// </summary>
        public event EventHandler<EventArgs> OnDisconnect;

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void Disconnect(object sender, EventArgs args = default (EventArgs))
        {
            if (OnDisconnect != null)
            {
                OnDisconnect(sender, args);
            }
        }

        #endregion

        #region Internal Send

        /// <summary>
        /// </summary>
        public event EventHandler<TagEventArgs> OnSend;

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void Send(object sender, TagEventArgs args)
        {
            if (OnSend != null)
            {
                OnSend(sender, args);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="tag"></param>
        public void Send(object sender, Tag tag)
        {
            Send(sender, new TagEventArgs(tag));
        }

        #endregion

        #region New Tag

        /// <summary>
        /// </summary>
        public event EventHandler<TagEventArgs> OnNewTag;

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void NewTag(object sender, TagEventArgs args)
        {
            if (OnNewTag != null)
            {
                OnNewTag(sender, args);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="tag"></param>
        public void NewTag(object sender, Tag tag)
        {
            NewTag(sender, new TagEventArgs(tag));
        }

        #endregion

        /// <summary>
        /// </summary>
        public event EventHandler<ErrorEventArgs> OnError;

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void Error(object sender, ErrorEventArgs args)
        {
            if (OnError != null)
            {
                OnError(sender, args);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="type"></param>
        /// <param name="level"></param>
        /// <param name="message"></param>
        public void Error(object sender, ErrorType type, ErrorLevel level, String message)
        {
            Error(sender, new ErrorEventArgs(message, type, level));
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="type"></param>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="parameters"></param>
        public void Error(object sender, ErrorType type, ErrorLevel level, String message, params object[] parameters)
        {
            Error(sender, type, level, String.Format(message, parameters));
        }
    }
}
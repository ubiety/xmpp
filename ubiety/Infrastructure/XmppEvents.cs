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
    public class XmppEvents
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public delegate void InternalConnect(object sender, EventArgs args);

        /// <summary>
        /// 
        /// </summary>
        public event InternalConnect OnConnect;

        /// <summary>
        /// 
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

        #region New Tag

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public delegate void ExternalNewTag(object sender, TagEventArgs args);

        /// <summary>
        /// </summary>
        public event ExternalNewTag OnNewTag;

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
            if (OnNewTag != null)
            {
                OnNewTag(sender, new TagEventArgs(tag));
            }
        }

        #endregion
    }
}
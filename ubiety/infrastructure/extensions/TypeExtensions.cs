// TypeExtensions.cs
//
//Ubiety XMPP Library Copyright (C) 2006 - 2015 Dieter Lunn
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ubiety.infrastructure.extensions
{
    /// <summary>
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// </summary>
        /// <param name="type"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T[] GetCustomAttributes<T>(this Type type) where T : Attribute
        {
            return GetCustomAttributes(type, typeof (T), false).Select(arg => (T) arg).ToArray();
        }

        /// <summary>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="inherit"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T[] GetCustomAttributes<T>(this Type type, bool inherit) where T : Attribute
        {
            return GetCustomAttributes(type, typeof (T), inherit).Select(arg => (T) arg).ToArray();
        }

        private static IEnumerable<object> GetCustomAttributes(Type type, Type attributeType, bool inherit)
        {
            if (!inherit)
            {
                return type.GetCustomAttributes(attributeType, false);
            }

            var attributeCollection = new Collection<object>();
            Type baseType = type;

            do
            {
                baseType.GetCustomAttributes(attributeType, true).Apply(attributeCollection.Add);
                baseType = baseType.BaseType;
            } while (baseType != null);

            foreach (Type interfaceType in type.GetInterfaces())
            {
                GetCustomAttributes(interfaceType, attributeType, true).Apply(attributeCollection.Add);
            }

            var attributeArray = new object[attributeCollection.Count];
            attributeCollection.CopyTo(attributeArray, 0);
            return attributeArray;
        }
    }
}
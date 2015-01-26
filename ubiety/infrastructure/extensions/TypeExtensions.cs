using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ubiety.common.extensions
{
	public static class TypeExtensions
	{
		public static T[] GetCustomAttributes<T>(this Type type) where T : Attribute
		{
			return GetCustomAttributes(type, typeof (T), false).Select(arg => (T) arg).ToArray();
		}

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
			var baseType = type;

			do
			{
				baseType.GetCustomAttributes(attributeType, true).Apply(attributeCollection.Add);
				baseType = baseType.BaseType;
			} while (baseType != null);

			foreach (var interfaceType in type.GetInterfaces())
			{
				GetCustomAttributes(interfaceType, attributeType, true).Apply(attributeCollection.Add);
			}

			var attributeArray = new object[attributeCollection.Count];
			attributeCollection.CopyTo(attributeArray, 0);
			return attributeArray;
		}
	}
}

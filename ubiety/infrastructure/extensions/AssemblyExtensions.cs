using System;
using System.Collections.Generic;
using System.Reflection;

namespace ubiety.common.extensions
{
    public static class AssemblyExtensions
    {
        public static IEnumerable<T> GetAttributes<T>(this Assembly assembly) where T : Attribute
        {
            var tags = new List<T>();
            var types = assembly.GetTypes();
            
            foreach (var type in types)
            {
                type.GetCustomAttributes<T>(true).Apply(tags.Add);
            }
            
            return tags;

        }
    }
}


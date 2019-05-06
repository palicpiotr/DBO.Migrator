using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DBO.DataTransport.HelpersStandard.Reflection
{
    public static class ReflectionExtensions
    {
        /// <summary> 
        /// Check if property is indexer 
        /// </summary> 
        private static bool IsIndexer(PropertyInfo property)
        {
            var parameters = property.GetIndexParameters();

            if (parameters.Length > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary> 
        /// Check if property implements IEnumerable 
        /// </summary> 
        private static bool IsEnumerable(PropertyInfo property)
        {
            return property.PropertyType.GetInterfaces().Any(x => x == typeof(IEnumerable));
        }

        public static List<FieldInfo> GetConstants(this Type type)
        {
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Public |
                 BindingFlags.Static | BindingFlags.FlattenHierarchy);

            return fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToList();
        }

        public static TAttribute GetAttribute<TAttribute>(this Enum enumValue)
           where TAttribute : Attribute
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<TAttribute>();
        }

        /// <summary> 
        /// Gets properties of T 
        /// </summary> 
        public static IEnumerable<PropertyInfo> GetProperties<T>(BindingFlags binding, PropertyReflectionOptions options = PropertyReflectionOptions.All)
        {
            var properties = typeof(T).GetProperties(binding);

            bool all = (options & PropertyReflectionOptions.All) != 0;
            bool ignoreIndexer = (options & PropertyReflectionOptions.IgnoreIndexer) != 0;
            bool ignoreEnumerable = (options & PropertyReflectionOptions.IgnoreEnumerable) != 0;
            bool ignoreClasses = (options & PropertyReflectionOptions.IgnoreClasses) != 0;

            foreach (var property in properties)
            {
                if (!all)
                {
                    if (ignoreIndexer && IsIndexer(property))
                    {
                        continue;
                    }

                    if (ignoreEnumerable && property.PropertyType != typeof(string) && IsEnumerable(property))
                    {
                        continue;
                    }
                    if (ignoreClasses && property.PropertyType != typeof(string) && property.PropertyType.IsClass)
                    {
                        continue;
                    }
                }

                yield return property;
            }
        }
    }

    [Flags]
    public enum PropertyReflectionOptions : int
    {
        /// <summary> 
        /// Take all. 
        /// </summary> 
        All = 0,

        /// <summary> 
        /// Ignores indexer properties. 
        /// </summary> 
        IgnoreIndexer = 1,

        /// <summary> 
        /// Ignores all other IEnumerable properties 
        /// except strings. 
        /// </summary> 
        IgnoreEnumerable = 2,
        IgnoreClasses = 4
    }
}

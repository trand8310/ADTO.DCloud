using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ADTOSharp.Reflection
{
    /// <summary>
    /// Some simple type-checking methods used internally.
    /// </summary>
    public static class TypeHelper
    {
        private static readonly HashSet<Type> FloatingTypes = new HashSet<Type>
        {
            typeof(float),
            typeof(double),
            typeof(decimal)
        };

        private static readonly HashSet<Type> NonNullablePrimitiveTypes = new HashSet<Type>
        {
            typeof(byte),
            typeof(short),
            typeof(int),
            typeof(long),
            typeof(sbyte),
            typeof(ushort),
            typeof(uint),
            typeof(ulong),
            typeof(bool),
            typeof(float),
            typeof(decimal),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(TimeSpan),
            typeof(Guid)
        };

        private static readonly IReadOnlyList<string> SystemAssemblyNames = new List<string> { "mscorlib", "System.Private.CoreLib" };

        public static bool IsNonNullablePrimitiveType(Type type)
        {
            return NonNullablePrimitiveTypes.Contains(type);
        }
        public static bool IsFunc(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var type = obj.GetType();
            if (!type.GetTypeInfo().IsGenericType)
            {
                return false;
            }

            return type.GetGenericTypeDefinition() == typeof(Func<>);
        }

        public static bool IsFunc<TReturn>(object obj)
        {
            return obj != null && obj.GetType() == typeof(Func<TReturn>);
        }

        public static bool IsPrimitiveExtendedIncludingNullable(Type type, bool includeEnums = false)
        {
            if (IsPrimitiveExtended(type, includeEnums))
            {
                return true;
            }

            if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return IsPrimitiveExtended(type.GenericTypeArguments[0], includeEnums);
            }

            return false;
        }

        private static bool IsPrimitiveExtended(Type type, bool includeEnums)
        {
            if (type.GetTypeInfo().IsPrimitive)
            {
                return true;
            }

            if (includeEnums && type.GetTypeInfo().IsEnum)
            {
                return true;
            }

            return type == typeof (string) ||
                   type == typeof (decimal) ||
                   type == typeof (DateTime) ||
                   type == typeof (DateTimeOffset) ||
                   type == typeof (TimeSpan) ||
                   type == typeof (Guid);
        }
        
        public static StringBuilder SerializeType(Type type, bool withAssemblyName = true, StringBuilder typeNameBuilder = null)
        {
            typeNameBuilder = typeNameBuilder ?? new StringBuilder();

            if (type.DeclaringType != null)
            {
                SerializeType(type.DeclaringType, false, typeNameBuilder).Append('+');
            }
            else if (type.Namespace != null)
            {
                typeNameBuilder.Append(type.Namespace).Append('.');
            }

            typeNameBuilder.Append(type.Name);

            if (type.GenericTypeArguments.Length > 0)
            {
                SerializeTypes(type.GenericTypeArguments, '[', ']', typeNameBuilder);
            }

            if (!withAssemblyName)
            {
                return typeNameBuilder;
            }

            var assemblyName = type.GetTypeInfo().Assembly.GetName().FullName;

            if (!SystemAssemblyNames.Contains(assemblyName))
            {
                typeNameBuilder.Append(", ").Append(assemblyName);
            }

            return typeNameBuilder;
        }

        private static StringBuilder SerializeTypes(Type[] types, char beginTypeDelimiter = '"', char endTypeDelimiter = '"', StringBuilder typeNamesBuilder = null)
        {
            if (types == null)
            {
                return null;
            }

            if (typeNamesBuilder == null)
            {
                typeNamesBuilder = new StringBuilder();
            }

            typeNamesBuilder.Append('[');

            for (int i = 0; i < types.Length; i++)
            {
                typeNamesBuilder.Append(beginTypeDelimiter);
                SerializeType(types[i], true, typeNamesBuilder);
                typeNamesBuilder.Append(endTypeDelimiter);

                if (i != types.Length - 1)
                {
                    typeNamesBuilder.Append(',');
                }
            }

            return typeNamesBuilder.Append(']');
        }



        public static object? GetDefaultValue(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            return null;
        }


        public static bool IsDefaultValue(object? obj)
        {
            if (obj == null)
            {
                return true;
            }

            return obj.Equals(GetDefaultValue(obj.GetType()));
        }



        public static TProperty? ChangeTypePrimitiveExtended<TProperty>(object? value)
        {
            if (value == null)
            {
                return default;
            }

            if (IsPrimitiveExtended(typeof(TProperty), includeEnums: true))
            {
                var conversionType = typeof(TProperty);
                if (IsNullable(conversionType))
                {
                    conversionType = conversionType.GetFirstGenericArgumentIfNullable();
                }

                if (conversionType == typeof(Guid))
                {
                    return (TProperty)TypeDescriptor.GetConverter(conversionType).ConvertFromInvariantString(value.ToString()!)!;
                }

                if (conversionType.IsEnum)
                {
                    return (TProperty)Enum.Parse(conversionType, value.ToString()!);
                }

                return (TProperty)Convert.ChangeType(value, conversionType, CultureInfo.InvariantCulture);
            }

            throw new ADTOSharpException("ChangeTypePrimitiveExtended<TProperty> does not support non-primitive types. Use non-generic GetProperty method and handle type casting manually.");
        }

        public static bool IsNullable(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static bool IsNullableEnum(Type type)
        {
            return type.IsGenericType &&
                   type.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                   type.GenericTypeArguments.Length == 1 &&
                   type.GenericTypeArguments[0].IsEnum;
        }
        public static Type GetFirstGenericArgumentIfNullable(this Type t)
        {
            if (t.GetGenericArguments().Length > 0 && t.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return t.GetGenericArguments().First();
            }

            return t;
        }


    }
}

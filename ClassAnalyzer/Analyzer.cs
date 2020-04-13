using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassAnalyzer
{
    public sealed class Analyzer : IClassAnalyzer
    {
        private static readonly Dictionary<string, bool> PrimitiveTypeCheckCache = new Dictionary<string, bool>();

        private readonly Dictionary<string, string> Cache = new Dictionary<string, string>();

        public string GetStringObjRepresentation<TBaseType>(TBaseType obj)
        {
            var type = typeof(TBaseType);

            if (Cache.TryGetValue(type.FullName, out var s))
            {
                return s;
            }
            Cache[type.FullName] = s = GetStringObjectRepresentation(obj);
            return s;
        }

        private string GetStringObjectRepresentation<TBaseType>(TBaseType obj, int shift = 0, HashSet<string> history = null)
        {
            history ??= new HashSet<string>();
            var type = obj.GetType();
            history.Add(type.FullName);
            var sb = new StringBuilder();
            sb.AppendLine("");
            AddString(sb, $"Object of type {type.Name}", shift);
            sb.AppendLine(new string('-', 30 - shift));
            shift += 5;

            foreach (var p in type.GetProperties())
            {
                var value = p.GetValue(obj);

                if (IsPrimitiveType(p.PropertyType))
                {
                    AddString(sb, $"{p.Name} = {value ?? "null"}", shift);
                }
                else
                {
                    if (history.Contains(p.PropertyType.FullName))
                    {
                        AddString(sb, $"{p.Name}" + " = X \n Error! A circular dependency is detected!", shift);
                        return sb.ToString();
                    }
                    history.Add(p.PropertyType.FullName);
                    var strObj = GetStringObjectRepresentation(value, shift, history);
                    AddString(sb, strObj, shift);
                }
            }
            return sb.ToString();
        }

        private static StringBuilder AddString(StringBuilder sb, string str, int shift)
        {
            sb.Append(new string(' ', shift));
            sb.AppendLine(str);
            return sb;
        }

        private static bool IsPrimitiveType(Type type)
        {
            if (PrimitiveTypeCheckCache.TryGetValue(type.FullName, out bool isPrimitive))
            {
                return isPrimitive;
            }
            isPrimitive = type.IsPrimitive ||
                          new Type[]
                          {
                              typeof(string),
                              typeof(decimal),
                              typeof(DateTime),
                              typeof(DateTimeOffset),
                              typeof(TimeSpan),
                              typeof(Guid)
                          }.Contains(type) ||
                          type.IsEnum ||
                          Convert.GetTypeCode(type) != TypeCode.Object ||
                          (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                           IsPrimitiveType(type.GetGenericArguments()[0]))
                          || type == typeof(object);
            PrimitiveTypeCheckCache[type.FullName] = isPrimitive;
            return isPrimitive;
        }
    }
}

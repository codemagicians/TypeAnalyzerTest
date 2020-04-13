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

        /// <summary>
        /// Gets a collection of objects serialized as string
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="collection">Target collection of objects</param>
        /// <param name="limitOutput">Max objects to be serialized. Use it to prevent overflow</param>
        /// <returns>String representation of serialized collection</returns>
        public string GetStringObjRepresentations<T>(IEnumerable<T> collection, int limitOutput = 25)
        {
            if (collection == null)
            {
                return "Error. Initial object cannot be null!";
            }
            var type = typeof(T);
            var list = collection as IList<T> ?? collection.ToList();

            if (list.Count == 0)
            {
                return $"Object is an empty collection of type {type.Name}";
            }
            string result = $"Object is a collection of type {type.Name} \n\n";

            foreach (var el in collection.Take(limitOutput))
            {
                result += GetStringObjRepresentation(el);
            }
            return result;
        }

        /// <summary>
        /// Gets object serialized as string
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="obj">An instance of the object</param>
        /// <returns>String representation of serialized object</returns>
        public string GetStringObjRepresentation<T>(T obj)
        {
            if (obj == null)
            {
                return "Error. Initial object cannot be null!";
            }
            var type = typeof(T);

            if (IsPrimitiveType(type))
            {
                return $"Object is of primitive type {type.Name} with value - {obj}";
            }
            return GetStringObjectRepresentation(obj, type: obj.GetType());
        }

        private string GetStringObjectRepresentation<TBaseType>(TBaseType obj, int shift = 0, Dictionary<string, int> history = null, Type type = null)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (shift < 0)
            {
                throw new ArgumentException(nameof(shift));
            }
            history ??= new Dictionary<string, int>();
            history[type.FullName] = shift;
            var sb = new StringBuilder();
            sb.AppendLine("");
            AddString(sb, $"Object of type {type.Name}", shift);
            sb.Append(new string(' ', shift));
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
                    if (history.TryGetValue(p.PropertyType.FullName, out var layer) && layer != shift)
                    {
                        AddString(sb, $"{p.Name}" + " = X \n Error! A circular dependency is detected!", shift);
                        return sb.ToString();
                    }

                    if (value == null)
                    {
                        history[p.PropertyType.FullName] = shift;
                        AddString(sb, $"{p.Name} = null", shift);
                        continue;
                    }
                    var strObj = GetStringObjectRepresentation(value, shift, history, p.PropertyType);
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

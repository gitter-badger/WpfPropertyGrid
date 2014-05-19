namespace WpfPropertyGrid
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml.Serialization;

    public class UndoCache<T>
    {
        private static readonly ConcurrentDictionary<Type, Properties> TypeProperties = new ConcurrentDictionary<Type, Properties>();
        private T _cache;

        public T CachedValue
        {
            get
            {
                return _cache;
            }
        }

        /// <summary>
        /// Saves a snapshot of o to cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <param name="name">If many sattings of the same type T are cached a name must be provided</param>
        public void Cache(T o, string name = null)
        {
            using (MemoryStream ms = Repository.ToStream(o))
            {
                var fromSTream = Repository.FromStream<T>(ms);
                _cache = fromSTream;
            }
        }

        /// <summary>
        /// Resets o to cached value
        /// Copies values for properties not marked with [XmlIgnore] from cache to o.
        /// Traverses nested properties recursively
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <param name="name">If many sattings of the same type T are cached a name must be provided</param>
        /// <returns></returns>
        public void Reset(T o, string name = null)
        {
            if (_cache == null)
            {
                throw new ArgumentOutOfRangeException("o", "No cache for type: " + o.GetType().Name);
            }
            CopyPropertyValues(_cache, o);
        }

        /// <summary>
        /// Compares the original with the cached value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="original"></param>
        /// <param name="name">If many sattings of the same type T are cached a name must be provided</param>
        /// <returns></returns>
        public bool IsDirty(T original, string name = null)
        {
            if (_cache == null)
            {
                throw new ArgumentOutOfRangeException("o", "No cache for type: " + original.GetType().Name);
            }
            return IsDirty(original, _cache);
        }

        /// <summary>
        /// Checks if any property not marked with [XmlIgnore] differs. Traverses nested properties recursively
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="original"></param>
        /// <param name="cached"></param>
        /// <returns></returns>
        internal bool IsDirty<T1>(T1 original, T1 cached)
        {
            if (original == null && cached == null)
            {
                return false;
            }
            if (original == null || cached == null)
            {
                return true;
            }
            Properties properties = TypeProperties.GetOrAdd(original.GetType(), t => new Properties(t));
            if (properties.SimpleProperties.Any(p => !Equals(p.GetValue(original), p.GetValue(cached))))
            {
                return true;
            }
            foreach (var nested in properties.NestedProperties)
            {
                if (IsDirty(nested.GetValue(original), nested.GetValue(cached)))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Copies values for properties not marked with [XmlIgnore] from source to destination.
        /// Traverses nested properties recursively
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        protected void CopyPropertyValues<T1>(T1 source, T1 destination)
        {
            Properties properties = TypeProperties.GetOrAdd(source.GetType(), t => new Properties(t));
            foreach (var prop in properties.SimpleProperties)
            {
                prop.SetValue(destination, prop.GetValue(source));
            }
            foreach (var nested in properties.NestedProperties)
            {
                object s = nested.GetValue(source);
                if (s == null)
                {
                    nested.SetValue(destination, null);
                    continue;
                }
                object d = nested.GetValue(destination);
                if (d == null)
                {
                    d = Activator.CreateInstance(s.GetType(), true);
                    nested.SetValue(destination, d);
                }
                CopyPropertyValues(s, d);
            }
        }

        public class Properties
        {
            public Properties(Type type)
            {
                Type = type;
                SimpleProperties = GetSimpleProperties(type);
                NestedProperties = GetNestedProperties(type);
            }

            public Type Type { get; private set; }

            public PropertyInfo[] SimpleProperties { get; private set; }

            public PropertyInfo[] NestedProperties { get; private set; }

            internal PropertyInfo[] GetSimpleProperties(Type t)
            {
                return t.GetProperties()
                        .Where(x => IsSimpleType(x) && IsIncluded(x))
                        .ToArray();
            }

            internal PropertyInfo[] GetNestedProperties(Type t)
            {
                return t.GetProperties()
                        .Where(x => !IsSimpleType(x) && IsIncluded(x))
                        .ToArray();
            }

            /// <summary>
            /// Checks if the type of the property is any of { int, ..., double, enum, string }
            /// </summary>
            /// <param name="info"></param>
            /// <returns></returns>
            internal bool IsSimpleType(PropertyInfo info)
            {
                // checks if it is an indexer[i]
                if (info.GetIndexParameters().Any())
                {
                    return false;
                }
                if (info.PropertyType.IsEnum)
                {
                    return true;
                }
                if (info.PropertyType.IsPrimitive)
                {
                    return true;
                }
                if (info.PropertyType == typeof(string))
                {
                    return true;
                }
                return false;
            }

            internal bool IsIncluded(PropertyInfo info)
            {
                return !Attribute.GetCustomAttributes(info, typeof(XmlIgnoreAttribute))
                                 .Any();
            }
        }

        public class TypeAndName
        {
            public TypeAndName(Type type, string name)
            {
                Type = type;
                Name = name ?? "";
            }
            public Type Type { get; private set; }
            public string Name { get; private set; }

            public bool Equals(TypeAndName other)
            {
                return Type == other.Type && Name == other.Name;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }
                if (ReferenceEquals(this, obj))
                {
                    return true;
                }
                if (obj.GetType() != this.GetType())
                {
                    return false;
                }
                return Equals((TypeAndName)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (Type.GetHashCode() * 397) ^ Name.GetHashCode();
                }
            }
        }
    }
}

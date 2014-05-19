namespace WpfPropertyGrid
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Threading.Tasks;
    using System.Xml.Serialization;

    public class Repository : IRepository
    {
        internal static readonly ConcurrentDictionary<Type, XmlSerializer> Serializers = new ConcurrentDictionary<Type, XmlSerializer>();
        private const string SomeSettingFilename = "Meh";
        private const string SettingsPath = @"C:\XXX";

        public static T FromStream<T>(Stream stream)
        {
            var serializer = Serializers.GetOrAdd(typeof(T), x => new XmlSerializer(typeof(T)));
            var setting = (T)serializer.Deserialize(stream);
            return setting;
        }

        public static MemoryStream ToStream<T>(T o)
        {
            var serializer = Serializers.GetOrAdd(o.GetType(), x => new XmlSerializer(o.GetType()));
            var ms = new MemoryStream();
            serializer.Serialize(ms, o);
            ms.Flush();
            ms.Position = 0;
            return ms;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="creator">Create object if file is missing</param>
        /// <returns></returns>
        public static async Task<T> ReadAsync<T>(string fileName, Func<T> creator)
        {
            string fullFileName = Path.Combine(SettingsPath, fileName + ".cfg");
            if (!File.Exists(fullFileName))
            {
                T setting = creator();
                await SaveAsync(setting, fileName);
                return setting;
            }
            try
            {
                using (var ms = new MemoryStream())
                {
                    using (FileStream stream = File.OpenRead(fullFileName))
                    {
                        await stream.CopyToAsync(ms)
                                    .ConfigureAwait(false);
                    }
                    ms.Position = 0;
                    var setting = FromStream<T>(ms);
                    return setting;
                }
            }
            catch (Exception)
            {
                T setting = creator();
                SaveAsync(setting, fileName).Wait();
                return setting;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="creator">Create object if file is missing</param>
        /// <returns></returns>
        public static T Read<T>(string fileName, Func<T> creator)
        {
            string fullFileName = Path.Combine(SettingsPath, fileName + ".cfg");
            if (!File.Exists(fullFileName))
            {
                T setting = creator();
                Save(setting, fileName);
                return setting;
            }
            try
            {
                using (FileStream stream = File.OpenRead(fullFileName))
                {
                    return FromStream<T>(stream);
                }
            }
            catch (Exception)
            {
                T setting = creator();
                SaveAsync(setting, fileName).Wait();
                return setting;
            }
        }

        public static async Task SaveAsync<T>(T o, string fileName)
        {
            if (!Directory.Exists(SettingsPath))
            {
                Directory.CreateDirectory(SettingsPath);
            }
            using (var ms = ToStream(o))
            {
                string fullFileName = Path.Combine(SettingsPath, fileName + ".cfg");
                using (FileStream stream = File.OpenWrite(fullFileName))
                {
                    await ms.CopyToAsync(stream)
                            .ConfigureAwait(false);
                }
            }
        }

        public static void Save<T>(T o, string fileName)
        {
            if (!Directory.Exists(SettingsPath))
            {
                Directory.CreateDirectory(SettingsPath);
            }
            string fullFileName = Path.Combine(SettingsPath, fileName + ".cfg");
            var serializer = Serializers.GetOrAdd(typeof(T), x => new XmlSerializer(typeof(T)));
            using (FileStream stream = File.OpenWrite(fullFileName))
            {
                serializer.Serialize(stream, o);
            }
        }

        public SomeSetting SaveSomeSettingc()
        {
            return Read(SomeSettingFilename, () => new SomeSetting());
        }

        public Task SaveSomeSettingAsync(SomeSetting setting)
        {
            return SaveAsync(setting, SomeSettingFilename);
        }
    }
}
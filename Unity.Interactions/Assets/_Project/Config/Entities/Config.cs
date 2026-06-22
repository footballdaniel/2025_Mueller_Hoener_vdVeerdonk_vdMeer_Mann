using System.Collections;
using System.Collections.Generic;
using System.IO;
using Interactions.Config.Contracts;
using Newtonsoft.Json.Linq;

namespace Interactions.Config.Entities
{
    public class Config : IConfig
    {
        public Config() : this(null)
        {
        }

        public Config(string path)
        {
            _path = path;
            _content = Load(path);
        }

        public string Get(string key, string fallback = null)
        {
            return _content.TryGetValue(key, out var token) ? token.ToString() : fallback;
        }

        public void Set(string key, string value)
        {
            _content[key] = value;

            if (!string.IsNullOrEmpty(_path))
                Save(_path, _content);
        }

        public IEnumerator<(string, string)> GetEnumerator()
        {
            foreach (var property in _content.Properties())
                yield return (property.Name, property.Value.ToString());
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        static JObject Load(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                return new JObject();

            try
            {
                return JObject.Parse(File.ReadAllText(path));
            }
            catch
            {
                return new JObject();
            }
        }

        static void Save(string path, JObject content)
        {
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            File.WriteAllText(path, content.ToString());
        }

        readonly JObject _content;
        readonly string _path;
    }
}

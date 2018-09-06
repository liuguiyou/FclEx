using System;
using System.Collections.Generic;

namespace FclEx.Fw.Configuration
{
    /// <inheritdoc />
    public class DictionaryBasedConfig : IDictionaryBasedConfig
    {
        /// <summary>
        /// Dictionary of custom configuration.
        /// </summary>
        protected Dictionary<string, object> CustomSettings { get; private set; }

        /// <summary>
        /// Gets/sets a config value.
        /// Returns null if no config with given name.
        /// </summary>
        /// <param name="name">Name of the config</param>
        /// <returns>Value of the config</returns>
        public object this[string name]
        {
            get => CustomSettings.GetOrDefault(name);
            set => CustomSettings[name] = value;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected DictionaryBasedConfig()
        {
            CustomSettings = new Dictionary<string, object>();
        }

        /// <inheritdoc />
        public T Get<T>(string name)
        {
            var value = this[name];
            return value == null
                ? default
                : (T) Convert.ChangeType(value, typeof (T));
        }

        /// <inheritdoc />
        public void Set<T>(string name, T value)
        {
            this[name] = value;
        }

        /// <inheritdoc />
        public object Get(string name)
        {
            return Get(name, null);
        }

        /// <inheritdoc />
        public object Get(string name, object defaultValue)
        {
            var value = this[name];
            if (value == null)
            {
                return defaultValue;
            }

            return this[name];
        }

        /// <inheritdoc />
        public T Get<T>(string name, T defaultValue)
        {
            return (T)Get(name, (object)defaultValue);
        }

        /// <inheritdoc />
        public T GetOrCreate<T>(string name, Func<T> creator)
        {
            var value = Get(name);
            if (value == null)
            {
                value = creator();
                Set(name, value);
            }
            return (T) value;
        }
    }
}
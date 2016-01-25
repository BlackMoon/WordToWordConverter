using System.Configuration;

namespace WordToWordConverter.Configuration
{
    /// <summary>
    /// Секция (.config) настроек словаря 
    /// </summary>
    public class DictionarySettingsSection : ConfigurationSection
    {
        /// <summary>
        /// Словарь файл или база?
        /// </summary>
        [ConfigurationProperty("isDatabase", DefaultValue = "false")]
        public bool IsDatabase
        {
            get { return (bool)this["isDatabase"]; }
            set { this["isDatabase"] = value; }
        }

        /// <summary>
        /// Словарь-файл требуется сортировка?
        /// </summary>
        [ConfigurationProperty("needSort", DefaultValue = "false")]
        public bool NeedSort
        {
            get { return (bool)this["needSort"]; }
            set { this["needSort"] = value; }
        }

        /// <summary>
        /// Строка соединения словаря-базы
        /// </summary>
        [ConfigurationProperty("connectionString", DefaultValue = "false")]
        public string ConnectionString
        {
            get { return (string)this["connectionString"]; }
            set { this["connectionString"] = value; }
        }

        /// <summary>
        /// Файл словаря
        /// </summary>
        [ConfigurationProperty("fileName", DefaultValue = "false")]
        public string FileName
        {
            get { return (string) this["fileName"]; }
            set { this["fileName"] = value; }
        }

    }
}

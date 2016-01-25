using System.Configuration;

namespace WordToWordConverter.Configuration
{
    /// <summary>
    /// Секция (.config) настроек словаря 
    /// </summary>
    public class DictionarySettingsSection : ConfigurationSection
    {
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

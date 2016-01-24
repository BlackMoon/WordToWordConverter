using System.Configuration;

namespace WordToWordConverter.Configuration
{
    /// <summary>
    /// Секция (.config) настроек алгоритма 
    /// </summary>
    public class AlgorithmSettingsSection : ConfigurationSection
    {
        [ConfigurationProperty("maxSteps", DefaultValue = "100", IsRequired = true)]
        public int MaxSteps
        {
            get { return (int) this["maxSteps"]; }
            set { this["maxSteps"] = value; }
        }

        [ConfigurationProperty("maxPopulation", DefaultValue = "50", IsRequired = true)]
        public int MaxPopulation
        {
            get { return (int)this["maxPopulation"]; }
            set { this["maxPopulation"] = value; }
        }
    }
}

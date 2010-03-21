namespace Cadenza.Web.ResourceVersioning.Configuration
{
    using System.Configuration;

    public class ResourceVersioningProviderConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("providers")]
        public ProviderSettingsCollection Providers 
        { 
            get
            {
                return (ProviderSettingsCollection)base["providers"];
            }
        }

        [ConfigurationProperty("enabled")]
        public bool Enabled
        {
            get
            {
                return (bool)base["enabled"];
            }

            set
            {
                base["enabled"] = value;
            }
        }

        [ConfigurationProperty("default")]
        public string Default
        {
            get
            {
                return (string)base["default"];
            }

            set
            {
                base["default"] = value;
            }
        }
    }
}
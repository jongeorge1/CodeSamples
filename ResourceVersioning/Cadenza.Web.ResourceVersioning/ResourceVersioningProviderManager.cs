namespace Cadenza.Web.ResourceVersioning
{
    using System.Configuration;
    using System.Web.Configuration;

    using Configuration;

    public static class ResourceVersioningProviderManager
    {
        static ResourceVersioningProviderManager()
        {
            Initialize();
        }


        public static ResourceVersioningProvider DefaultProvider
        {
            get;

            set;
        }

        /// <summary>
        /// Gets or sets Providers.
        /// </summary>
        public static ResourceVersioningProviderCollection Providers
        {
            get;

            set;
        }

        public static bool Enabled
        {
            get; 
            set;
        }

        private static void Initialize()
        {
            var configuration = ConfigurationManager.GetSection("resourceVersioning") as ResourceVersioningProviderConfigurationSection;

            // If the section isn't there, we don't throw an error. This will allow the DefaultProvider and Providers collections to
            // be programatically set to allow unit testing.
            if (configuration != null)
            {
                Enabled = configuration.Enabled;

                Providers = new ResourceVersioningProviderCollection();

                ProvidersHelper.InstantiateProviders(
                    configuration.Providers,
                    Providers,
                    typeof(ResourceVersioningProvider));

                Providers.SetReadOnly();

                DefaultProvider = Providers[configuration.Default];

                if (DefaultProvider == null)
                {
                    throw new ConfigurationErrorsException(
                        "The resourceVersioning configuration section does not specify a valid default provider name.");
                }
            }
        }


    }
}
namespace Cadenza.Web.ResourceVersioning
{
    using System;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Reflection;

    public class AssemblyBasedResourceVersioningProvider : ResourceVersioningProviderBase
    {
        /// <summary>
        /// The version.
        /// </summary>
        private static string Version;

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            base.Initialize(name, config);

            //  Look for the name of the assembly to use in the config
            string assemblyName = config["assemblyName"];

            try
            {
                Assembly targetAssembly = Assembly.ReflectionOnlyLoad(assemblyName);

                Version assemblyVersion = targetAssembly.GetName().Version;

                Version = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}.{1}.{2}.{3}",
                    assemblyVersion.Major,
                    assemblyVersion.Minor,
                    assemblyVersion.Revision,
                    assemblyVersion.Build);

            }
            catch (ArgumentException)
            {
                throw new ConfigurationErrorsException("The assemblyName attribute of the AssemblyBasedResourceVersioningProvider configuration was not present or was left empty. Please provide a valid assembly name.");
            }
            catch (FileNotFoundException)
            {
                throw new ConfigurationErrorsException("The assembly specified in assemblyName attribute of the AssemblyBasedResourceVersioningProvider configuration could not be found. Please provide a valid assembly name.");
            }
            catch (FileLoadException)
            {
                throw new ConfigurationErrorsException("The assembly specified in assemblyName attribute of the AssemblyBasedResourceVersioningProvider configuration could not be loaded. Please provide a valid assembly name.");
            }
            catch (BadImageFormatException)
            {
                throw new ConfigurationErrorsException("The file specified in assemblyName attribute of the AssemblyBasedResourceVersioningProvider configuration is not a valid assembly. Please provide a valid assembly name.");
            }
        }

        protected override string GetVersionNumber()
        {
            return Version;
        }
    }
}
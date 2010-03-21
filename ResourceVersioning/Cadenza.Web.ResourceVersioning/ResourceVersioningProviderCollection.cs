namespace Cadenza.Web.ResourceVersioning
{
    using System.Configuration.Provider;

    public class ResourceVersioningProviderCollection : ProviderCollection
    {
        public new ResourceVersioningProvider this[string name]
        {
            get 
            {
                return base[name] as ResourceVersioningProvider; 
            }
        }
    }
}
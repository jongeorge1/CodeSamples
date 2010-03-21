namespace Cadenza.Web.ResourceVersioning
{
    using System.Configuration.Provider;

    public abstract class ResourceVersioningProvider : ProviderBase
    {
        public abstract string AddVersionNumberToFileName(string fileName);

        public abstract string RemoveVersionNumberFromFileName(string fileName);
    }
}
namespace Cadenza.Web.ResourceVersioning
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.IO;

    public abstract class ResourceVersioningProviderBase : ResourceVersioningProvider
    {
        /// <summary>
        /// The add version cache.
        /// </summary>
        private static Dictionary<string, string> AddVersionCache = new Dictionary<string, string>();

        /// <summary>
        /// The extensions to version.
        /// </summary>
        private static string[] ExtensionsToVersion = new[] { ".js", ".css" };

        /// <summary>
        /// The remove version cache.
        /// </summary>
        private static Dictionary<string, string> RemoveVersionCache = new Dictionary<string, string>();

        protected abstract string GetVersionNumber();

        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);

            var extensionsValue = config["extensionsToVersion"];

            if (!string.IsNullOrEmpty(extensionsValue))
            {
                var splitExtensions = extensionsValue.Split(',');

                ExtensionsToVersion = Array.ConvertAll(
                    splitExtensions,
                    x => x.Trim().ToLowerInvariant());
            }
        }

        /// <summary>
        /// The add version number to file name.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>
        /// The add version number to file name.
        /// </returns>
        public override string AddVersionNumberToFileName(string fileName)
        {
            if (!ResourceVersioningProviderManager.Enabled)
            {
                return fileName;
            }

            if (!AddVersionCache.ContainsKey(fileName))
            {
                string versionedFilename = this.AddVersionNumberToFileNameInternal(fileName);

                AddVersionCache.Add(
                    fileName,
                    versionedFilename);

                RemoveVersionCache.Add(
                    versionedFilename,
                    fileName);
            }

            return AddVersionCache[fileName];
        }

        /// <summary>
        /// The remove version number from file name.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>
        /// The remove version number from file name.
        /// </returns>
        public override string RemoveVersionNumberFromFileName(string fileName)
        {
            if (!ResourceVersioningProviderManager.Enabled)
            {
                return fileName;
            }

            if (!RemoveVersionCache.ContainsKey(fileName))
            {
                string removedFilename = this.RemoveVersionNumberFromFileNameInternal(fileName);

                // Add to both remove and add caches to save work for the incoming request.
                RemoveVersionCache.Add(
                    fileName,
                    removedFilename);

                AddVersionCache.Add(
                    removedFilename,
                    fileName);
            }

            return RemoveVersionCache[fileName];
        }

        /// <summary>
        /// Adds the version number to the given filename, if it's file type is registered.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>
        /// The add version number to file name internal.
        /// </returns>
        private string AddVersionNumberToFileNameInternal(string fileName)
        {
            string modifiedFileName = fileName;

            var file = new FileInfo(fileName);

            string extension = file.Extension.ToLowerInvariant();

            foreach (string current in ExtensionsToVersion)
            {
                if (extension == current)
                {
                    modifiedFileName = string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}.{1}{2}",
                        file.Name.Remove(file.Name.Length - extension.Length),
                        this.GetVersionNumber(),
                        extension);

                    break;
                }
            }

            return modifiedFileName;
        }

        /// <summary>
        /// Removes the current version number from the given filename if present.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>
        /// The file name without the version number.
        /// </returns>
        private string RemoveVersionNumberFromFileNameInternal(string fileName)
        {
            string modifiedFileName = fileName;

            foreach (string current in ExtensionsToVersion)
            {
                string searchString = string.Format(
                    CultureInfo.InvariantCulture,
                    ".{0}{1}",
                    this.GetVersionNumber(),
                    current);

                if (fileName.EndsWith(
                    searchString,
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    modifiedFileName = fileName.Replace(
                        searchString,
                        current);

                    break;
                }
            }

            return modifiedFileName;
        }
    }
}
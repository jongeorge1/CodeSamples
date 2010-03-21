namespace Cadenza.Web.ResourceVersioning
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// Supports adding the current assembly version number to the names of static resources. This allows you to configure far-future expiry headers
    /// and not have to manually change the name of the file with each deployment. 
    /// </summary>
    public class ResourceVersioningModule : IHttpModule
    {
        /// <summary>
        /// The incoming url cache.
        /// </summary>
        private static Dictionary<Uri, string> IncomingUrlCache = new Dictionary<Uri, string>(500);

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void Dispose()
        {
        }

        /// <summary>
        /// The init.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public void Init(HttpApplication context)
        {
            context.BeginRequest += this.context_BeginRequest;
        }

        /// <summary>
        /// The get rewritten url for.
        /// </summary>
        /// <param name="requestUri">
        /// The request uri.
        /// </param>
        /// <returns>
        /// The get rewritten url for.
        /// </returns>
        private static string GetRewrittenUrl(Uri requestUri)
        {
            if (!IncomingUrlCache.ContainsKey(requestUri))
            {
                IncomingUrlCache.Add(
                    requestUri, 
                    GetRewrittenUrlInternal(requestUri));
            }

            return IncomingUrlCache[requestUri];
        }

        /// <summary>
        /// Retrieves the rewritten Url for the incoming request. If the Url should not be rewritten, the function
        /// returns an empty string.
        /// </summary>
        /// <param name="requestUri">
        /// The request uri.
        /// </param>
        /// <returns>
        /// The Url to rewrite to.
        /// </returns>
        private static string GetRewrittenUrlInternal(Uri requestUri)
        {
            string newUrl = string.Empty;

            var urlSegments = new List<string>(requestUri.Segments);

            string fileName = urlSegments.Last();

            string modifiedFileName =
                ResourceVersioningProviderManager.DefaultProvider.RemoveVersionNumberFromFileName(fileName);

            if (!fileName.Equals(
                     modifiedFileName, 
                     StringComparison.InvariantCultureIgnoreCase))
            {
                urlSegments.RemoveAt(urlSegments.Count - 1);
                urlSegments.Add(modifiedFileName);
                newUrl = string.Join(
                    "/", 
                    urlSegments.ToArray());
            }

            return newUrl;
        }

        /// <summary>
        /// The context_ begin request.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void context_BeginRequest(
            object sender, 
            EventArgs e)
        {
            string newUrl = GetRewrittenUrl(HttpContext.Current.Request.Url);

            if (!string.IsNullOrEmpty(newUrl))
            {
                HttpContext.Current.RewritePath(newUrl);
            }
        }
    }
}
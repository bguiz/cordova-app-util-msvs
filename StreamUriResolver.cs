using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Foundation;
using Windows.Web;
using Windows.Security.Cryptography;

// Credit this goes to MSDN for the code examples on their site

namespace CordovaApp.Util
{
  public sealed class StreamUriResolver : IUriToStreamResolver
    { 
        // creates a stream of the local app data folder accessible from:
        // ms-local-stream://<packageName>_<encodedContentIdentifier>/<relativePath>
        public IAsyncOperation<IInputStream> UriToStreamAsync(System.Uri uri)
        {
            string host = uri.Host;
            int delimiter = host.LastIndexOf('_');
            string encodedContentId = host.Substring(delimiter + 1);
            IBuffer buffer = CryptographicBuffer.DecodeFromHexString(encodedContentId);

            string contentIdentifier = CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, buffer);
            string relativePath = uri.PathAndQuery;
            System.Uri appDataUri = new Uri("ms-appdata:///local/" + contentIdentifier + relativePath);

            return GetFileStreamFromApplicationUriAsync(appDataUri).AsAsyncOperation();
        }

        private async Task<IInputStream> GetFileStreamFromApplicationUriAsync(System.Uri uri)
        {
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(uri);
            IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
            return stream;
        }
    }
}

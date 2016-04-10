using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace Magnets
{
    public class MagnetCollection : ReactiveList<MagnetUri>, ISupportIncrementalLoading
    {

        private async Task<LoadMoreItemsResult> InnerLoadMoreItemsAsync()
        {
            if (NextUri == null)
            {
                return new LoadMoreItemsResult() { Count = 0 };
            }
            var actualCount = 0;
            Debug.WriteLine(NextUri);

            HttpClient client = new HttpClient();
            var eztv = await client.GetStringAsync(new Uri(NextUri));
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(eztv);
            var photos = doc.DocumentNode.Descendants("a").
                  Select(x => new MagnetUri(x.GetAttributeValue("href", ""))).
                  Where(x => x.HasNameAndInfoHash);

            if (clearItems)
            {
                ClearItems();
                clearItems = false;
            }
            foreach (var item in photos)
            {
                actualCount++;
                Add(item);
            }
            var nextPage = doc.DocumentNode.Descendants("a").FirstOrDefault(x => x.InnerText.Contains("next page"));
            HasMoreItems = nextPage != null;

            if (HasMoreItems)
            {
                NextUri = new Uri(new Uri(NextUri), nextPage.GetAttributeValue("href", "")).AbsoluteUri;
            }

            return new LoadMoreItemsResult
            {
                Count = (uint)actualCount
            };
        }

        public bool HasMoreItems { get; private set; }
        public string NextUri { get; set; }
        private bool clearItems=false;
        public async Task<MagnetCollection> SetSearchAsync(string uri)
        {
            clearItems = true;
            NextUri = uri;
            HasMoreItems = true;
            await InnerLoadMoreItemsAsync();
            return this;
        }
        public MagnetCollection(string uri)
        {
            SetSearchAsync(uri);
        }
        public MagnetCollection()
        {
            NextUri = null;
            HasMoreItems = false;
        }
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return InnerLoadMoreItemsAsync().AsAsyncOperation();
        }
    }

}

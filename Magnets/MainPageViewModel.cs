
using Newtonsoft.Json;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Web.Http;
namespace Magnets
{
    [JsonObject(MemberSerialization.OptIn)]
    [DataContract]
    public class MainPageViewModel : BaseViewModel
    {
        public MagnetCollection SearchResults { get; set; } = new MagnetCollection();
        public MagnetCollection NewItemsResults { get; set; } = new MagnetCollection();

        private string _SearchQuery;
        [DataMember, JsonProperty]
        public string SearchQuery { get { return _SearchQuery; } set { this.RaiseAndSetIfChanged(ref _SearchQuery, value); } }

        private bool _OpenHere;
        [DataMember, JsonProperty]
        public bool OpenHere { get { return _OpenHere; } set { this.RaiseAndSetIfChanged(ref _OpenHere, value); } }

        private string _OpenHereUri;
        [DataMember, JsonProperty]
        public string OpenWebpageUri { get { return _OpenHereUri; } set { this.RaiseAndSetIfChanged(ref _OpenHereUri, value); } }

        private ReactiveList<string> _AutoloadList;
        [DataMember, JsonProperty]
        public ReactiveList<string> AutoloadList { get { return _AutoloadList; } set { this.RaiseAndSetIfChanged(ref _AutoloadList, value); } }

        public ReactiveCommand<bool> Search { get; set; }
        public ReactiveCommand<bool> Update { get; set; }


        public MainPageViewModel() : base("MainPage")
        {
            var canSearch = this.WhenAny(x => x.SearchQuery, x => !String.IsNullOrWhiteSpace(x.Value) && x.Value.Length > 3);

            Search = ReactiveCommand.CreateAsyncTask(canSearch, async x =>
            {
                await SearchResults.SetSearchAsync("http://eztv.ag/search/" + this.SearchQuery);
                return true;
            });
            Update = ReactiveCommand.CreateAsyncTask(async x =>
            {
                await NewItemsResults.SetSearchAsync("http://eztv.ag/page_0");
                return true;
            }); 
            Update.Execute(null);
            this.WhenAnyValue(x => x.SearchQuery)
                .Throttle(TimeSpan.FromSeconds(2), RxApp.MainThreadScheduler)
                .InvokeCommand(this, x => x.Search);
        }
    }
}

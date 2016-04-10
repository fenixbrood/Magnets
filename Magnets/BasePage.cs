using Newtonsoft.Json;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Magnets
{

    [JsonObject(MemberSerialization.OptIn)]
    [DataContract]
    public abstract class BaseViewModel : ReactiveUI.ReactiveObject
    {
        public ApplicationDataContainer Settings { get; private set; }
        public ReactiveCommand<Unit> AutoLoad { get; private set; }
        public ReactiveCommand<Unit> AutoSave { get; private set; }
        protected void SetupAutoSaveLoad(string settingsName)
        {
            Settings = ApplicationData.Current.RoamingSettings.CreateContainer("Settings" + settingsName, ApplicationDataCreateDisposition.Always);
            AutoLoad = ReactiveCommand.CreateAsyncTask(async x =>
            {
                await Task.Run(() =>
                {
                    if (!string.IsNullOrEmpty(Settings.Values["json"] + ""))
                    {
                        JsonConvert.PopulateObject(Settings.Values["json"] + "", this);
                    }
                    return true;
                });
            });
            AutoSave = ReactiveCommand.CreateAsyncTask(async x =>
            {
                await Task.Run(() =>
                {
                    var str = JsonConvert.SerializeObject(this);
                    Settings.Values["json"] = str;
                    return true;
                });
            });
            this.AutoPersist(x =>
            {
                return Observable.Create<Unit>((y) => AutoSave.ExecuteAsyncTask());
            });
            Settings.WhenAnyValue(x => x.Values).InvokeCommand(this, x => x.AutoLoad);

        }
        public BaseViewModel(string settingsName)
        {
            SetupAutoSaveLoad(settingsName);
        }
    }
}

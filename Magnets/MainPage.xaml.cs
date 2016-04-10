using ReactiveUI;
using Splat;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

namespace Magnets
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.DataContext = ViewModel;
            this.InitializeComponent();
        }
        public MainPageViewModel ViewModel { get; set; } = new MainPageViewModel();

        private async void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                var item = (MagnetUri)e.ClickedItem;
                if (ViewModel.OpenHere)
                {
                    await Launcher.LaunchUriAsync(new Uri(item.OrginalUri));
                }
                else
                {
                    var http = new HttpClient();
                    var result = (await http.PostAsync(new Uri(ViewModel.OpenWebpageUri), new HttpStringContent(item.OrginalUri)));
                    if (result.IsSuccessStatusCode)
                    {
                        var str = await result.Content.ReadAsStringAsync();
                        //str = "Added " + item.Name;
                        var toastXmlString = $"<toast><visual version='1'><binding template='ToastText01'><text id='1'>{str}</text></binding></visual></toast>";
                        Windows.Data.Xml.Dom.XmlDocument toastDOM = new Windows.Data.Xml.Dom.XmlDocument();
                        toastDOM.LoadXml(toastXmlString);
                        ToastNotification toast = new ToastNotification(toastDOM);
                        ToastNotificationManager.CreateToastNotifier().Show(toast);
                    }
                }
            }
            catch (Exception)
            {

              
            }
          
        }

        private void AutoLoad_ItemClick(object sender, ItemClickEventArgs e)
        {

        }
    }


}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magnets
{
    public class MagnetUri : MultiValueDictionary<string, string>
    {
        private void magnetURIDecode(string uri)
        {
            var result = this;
            var dataSplit = uri.Split(new string[] { "magnet:?" }, StringSplitOptions.None);
            if (dataSplit.Length < 2)
            {
                return;
            }
            var data = dataSplit[1];

            var parameters = (data != null && data.Length >= 0) ? data.Split('&') : new string[0];
            foreach (var item in parameters)
            {
                var keyval = item.Split('=');
                if (keyval.Length != 2) continue;
                var key = keyval[0];
                var val = keyval[1];
                if (key == "dn")
                    val = Uri.UnescapeDataString(val).Replace("+", " ");
                if (key == "tr" || key == "xs" || key == "as" || key == "ws")
                {
                    val = Uri.UnescapeDataString(val);
                }
                if (key == "kt")
                {
                    var valMany = Uri.UnescapeDataString(val).Split('+');
                    result.AddRange(key, valMany);
                }
                else
                {
                    result.Add(key, val);
                }
            }

            foreach (var xt in result.All("xt"))
            {
                var spl = xt.Split(new string[] { "urn:btih:" }, StringSplitOptions.None);
                if (spl.Length != 2)
                {

                }
                else if (spl[1].Length == 40)
                {
                    result.Add("infoHash", spl[1].ToLowerInvariant());
                }
                else if (spl[1].Length == 32)
                {
                    result.Add("infoHash", Base32Encoding.ToBytes(spl[1]).ToHex().ToLowerInvariant());
                }
            }
        }

        public MagnetUri(string uri)
        {
            OrginalUri = uri;
            magnetURIDecode(uri);
        }
        public string Name => this.All("dn").FirstOrDefault();
        public IEnumerable<string> Keywords => this.All("kt").Distinct();
        public IEnumerable<string> Announce => this.All("tr").Distinct();
        public string InfoHash => this.All("infoHash").FirstOrDefault();
        public IEnumerable<string> UrlList => this.All("as").Concat(this.All("ws"));
        public string    OrginalUri { get;private set; }
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (var item in this)
            {
                builder.AppendLine(item.Key + " => " + string.Join(",", item.Value));
            }
            return builder.ToString();
        }
        public bool HasNameAndInfoHash => !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(InfoHash);
    }
}

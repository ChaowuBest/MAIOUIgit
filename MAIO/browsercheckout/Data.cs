using System;
using System.Collections.Generic;
using System.Text;

namespace MAIO.browsercheckout
{
    class Data
    {
		public Dictionary<string, string> headers { get; set; }
		public string url { get; set; }
		public string method { get; set; }
		public string data { get; set; }
		public string proxy { get; set; }
		public List<AddBrowserCookie> cookies { get; set; }
		public string id { get; set; }
	}
}

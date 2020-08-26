using System;
using System.Collections.Generic;
using System.Text;

namespace MAIO.browsercheckout
{
	[Serializable]
	class BrowserRequest
    {
		public Data data { get; set; }
		public string type { get; set; }	
	}
}

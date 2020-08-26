
using System;
using System.Collections.Generic;
using System.Text;

namespace MAIO.browsercheckout
{
    class AddBrowserCookie
    {
		public string Name { get; set; }
		public string TimeStamp { get; set; }
		public string Value { get; set; }
		public string Comment { get; set; } = "";
		public string CommentUri { get; set; }
		public bool HttpOnly { get; set; }
		public bool Discard { get; set; }
		public bool Expired { get; set; }
		public bool Secure { get; set; }
		public string Domain { get; set; } = ".nike.com";
		public string Expires { get; set; } = "0001-01-01T00:00:00";
		public string Path { get; set; } = "/";
		public string Port { get; set; } = "";
		public int Version { get; set; }
	}
}

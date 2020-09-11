
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MAIO.ASOS
{
    class ASOSAPI
    {
        Random ran = new Random();
        public string setAtccookie = "";
        public bool getsession = false;
    }
}

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MAIO.JDUS
{
    class JDUS
    {
        public bool randomsize = false;
        public string profile = "";
        public string link = "";
        public string size = "";
        public string sizeid = "";
        public Main.taskset tk = null;
        string _dynSessConf = null;
        public Dictionary<string, string> allsize = new Dictionary<string, string>();
        public JDUSAPI jdusapi = new JDUSAPI();
        private static DateTime timeStampStartTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public async void StartTask(CancellationToken ct, CancellationTokenSource cts)
        {
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            string url = "https://www.footlocker.com/api/session";
            jdusapi.GetHtmlsource(url, tk, ct);
            tk.Status = "Get Session";
            url = "https://www.footlocker.com/api/products/pdp/" + tk.Sku;
            string sourcecode = jdusapi.GetHtmlsource(url, tk, ct);
            JObject jsize = JObject.Parse(sourcecode);
            string skucode = null;
            foreach (var i in jsize["variantAttributes"])
            {
                Thread.Sleep(1);
                if (i["sku"].ToString().ToUpper() == tk.Sku.ToUpper())
                {
                    skucode=i["code"].ToString();
                    break;
                }
            }
            foreach (var i in jsize["sellableUnits"])
            {
                Thread.Sleep(1);
                var size = i["attributes"][0]["value"].ToString();
                if (i["attributes"][1]["id"].ToString() == skucode)
                {            
                    if (size[0] == '0' && size.Substring(size.Length - 1, 1) == "0")
                    {
                        size = size.Substring(0, size.Length - 2);
                        size = size.Substring(1);    
                    }
                    else if (size.Substring(size.Length - 1, 1) == "0"&& size[0] != '0')
                    {
                        size = size.Substring(0, size.Length - 2);
                    }
                    else if (size[0] == '0'&& size.Substring(size.Length - 1, 1) != "0")
                    {
                        size = size.Substring(1);
                    }
                    allsize.Add(size, i["attributes"][0]["id"].ToString());
                }
            }
            JObject jo = JObject.Parse(profile);
            string subemailapi = "https://www.footlocker.com/api/users/carts/current/email/"+jo["EmailAddress"]+"?timestamp="+ (long)(DateTime.Now.ToUniversalTime() - timeStampStartTime).TotalMilliseconds;
            jdusapi.PUT(subemailapi,tk,ct);
            string subshippapi = "https://www.footlocker.com/api/users/carts/current/addresses/shipping?timestamp="+ (long)(DateTime.Now.ToUniversalTime() - timeStampStartTime).TotalMilliseconds;
            string subshippinfo = "{\"shippingAddress\":{\"setAsDefaultBilling\":false,\"setAsDefaultShipping\":false,\"firstName\":\""+ jo["FirstName"].ToString() + "\",\"lastName\":\""+ jo["LastName"].ToString() + "\",\"email\":\""+ jo["EmailAddress"].ToString() + "\",\"phone\":\""+ jo["Tel"].ToString() + "\",\"billingAddress\":false,\"country\":{\"isocode\":\"US\",\"name\":\"United States\"},\"defaultAddress\":false,\"id\":null,\"line1\":\""+ jo["Address1"].ToString() + "\",\"postalCode\":\""+ jo["Zipcode"].ToString() + "\",\"region\":{\"countryIso\":\"US\",\"isocode\":\"US-"+ jo["State"].ToString() + "\",\"isocodeShort\":\""+ jo["State"].ToString() + "\"},\"setAsBilling\":true,\"shippingAddress\":true,\"town\":\""+ jo["City"].ToString() + "\",\"visibleInAddressBook\":false,\"type\":\"default\",\"LoqateSearch\":\"\",\"line2\":\""+ jo["Address2"].ToString() + "\",\"recordType\":\"S\"}}";
            jdusapi.subship(subemailapi,tk,ct,subshippinfo);

          //  string shippinginfo = "{\"setAsDefaultBilling\":false,\"setAsDefaultShipping\":false,\"firstName\":\"wu\",\"lastName\":\"chao\",\"email\":false,\"phone\":\"4439869823\",\"id\":null,\"setAsBilling\":false,\"region\":{\"countryIso\":\"US\",\"isocode\":\"US-NJ\",\"isocodeShort\":\"NJ\",\"name\":\"New Jersey\"},\"country\":{\"isocode\":\"US\",\"name\":\"United States\"},\"type\":\"default\",\"LoqateSearch\":\"\",\"line1\":\""+ jo["Address1"].ToString() + "\",\"postalCode\":\"08904-2447\",\"town\":\"HIGHLAND PARK\",\"regionFPO\":null,\"shippingAddress\":true,\"recordType\":\"S\"}";
            string ATCUrl = "https://www.footlocker.com/api/users/carts/current/entries?timestamp=" + (long)(DateTime.Now.ToUniversalTime() - timeStampStartTime).TotalMilliseconds;
            string ATCinfo = "{\"productQuantity\":1,\"productId\":\"22510914\"}";
            jdusapi.ATC(ATCUrl, tk,ct,ATCinfo);
        }
    }
}

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace MAIO
{
    class TheNorthFaceUS
    {
        public bool randomsize = false;
        public string link = "";
        public string profile = "";
        public string size = "";
        public string sizeid = "";
        public string categoryId = "";
        public string productid = "";
        public Main.taskset tk = null;
        TheNorthFaceAPI tnfAPI = new TheNorthFaceAPI();
        public void StartTask(CancellationToken ct, CancellationTokenSource cts)
        {
         A: JObject joprofile = JObject.Parse(profile);
            try
            {
                GetSkuid(ct);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch(NullReferenceException)
            {
                goto A;
            }
        }
        public void GetSkuid(CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            string url = tk.Sku;
            bool isstocknull = true;
            JArray ja=(JArray)tnfAPI.GetHtmlsource(url,tk,ct);
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            categoryId = ja[0]["PDPProduct"]["categoryId"].ToString();
            productid= ja[0]["PDPProduct"]["parentId"].ToString();
            var  sizegroup = ja[0]["PDPProduct"]["eids"].ToString();
            string eid = "";
            foreach (var i in sizegroup)
            {
                if (i.ToString().Contains(size))
                {
                     eid=i.ToString();
                    break;
                }
            }
            string monitorurl = "https://www.thenorthface.com/webapp/wcs/stores/servlet/VFAjaxProductAvailabilityView?langId=-1&storeId=7001&productId="+ productid + "&requestype=ajax&requesttype=ajax";
            JObject jo=(JObject)tnfAPI.GetHtmlsource(monitorurl, tk,ct);
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            foreach (var i in jo["partNumbers"].ToArray())
            {
                if (i.ToString().Contains(eid))
                {
                    Regex rex = new Regex(@"\d{6}");
                    sizeid=rex.Match(i.ToString()).ToString();
                    break;
                }
            }
            foreach (var i in jo["stock"].ToArray())
            {
                if (i.ToString().Contains(sizeid))
                {
                    var st=i.ToString().Replace(sizeid, "").Replace(",", "").Replace(" ", "").Replace("\"","").Replace(":","");
                    int stock=int.Parse(st);
                    if (stock > 0)
                    {
                        isstocknull = false;
                    }
                    break;
                }
            }
            if (isstocknull==false)
            {
                
            }
        }
    }
}

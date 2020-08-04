using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MAIO
{
    class tnfmonitor
    {
        public bool randomsize = false;
        public Main.Monitor mn = null;
        tnfmonitorAPI tnfapi = new tnfmonitorAPI();
        public void start(CancellationToken ct)
        {
        Retry: string url = mn.Sku;
            string monitorurl = "";
            JArray ja = (JArray)tnfapi.GetHtmlsource(url);
            string productid = ja[0]["PDPProduct"]["parentId"].ToString();
            var sizegroup = ja[0]["PDPProduct"]["eids"].ToString();
            JArray ja2 = JArray.Parse(sizegroup);
            string eid = "";
            ArrayList skuslist = new ArrayList();
       
            if (mn.Region == "TheNorthFaceUS")
            {
                monitorurl = "https://www.thenorthface.com/webapp/wcs/stores/servlet/VFAjaxProductAvailabilityView?langId=-1&storeId=7005&productId=" + productid + "&requestype=ajax&requesttype=ajax";
            }
            else
            {
                monitorurl = "https://www.thenorthface.com/webapp/wcs/stores/servlet/VFAjaxProductAvailabilityView?langId=-1&storeId=7001&productId=" + productid + "&requestype=ajax&requesttype=ajax";
            }

            if (Config.delay == "")
            {
                Thread.Sleep(1);
            }
            else if (Config.delay == null)
            {
                Thread.Sleep(1);
            }
            else
            {
                Thread.Sleep(int.Parse(Config.delay));
            }
            goto Retry;
        }
    }
}

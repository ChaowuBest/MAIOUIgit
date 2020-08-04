using MAIO.JDUS;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MAIO.ASOS
{
    class ASOS
    {

        public bool randomsize = false;
        public string link = "";
        public string profile = "";
        public string size = "";
        public string sizeid = "";
        public Main.taskset tk = null;
        public ASOSAPI aSOSAPI = new ASOSAPI();
        public ASOSsession session = new ASOSsession();
        public void StartTask(CancellationToken ct, CancellationTokenSource cts)
        {
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            try
            {
                string accessroken = GetauthorizeID(ct);
                string skuid = GetSkuid(ct);
                Addtocart(ct, accessroken, skuid);
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }
        public string GetauthorizeID(CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
           string url = "https://my.asos.com/identity/connect/authorize?state=9876459885940323&nonce=13191304578348823&client_id=D91F2DAA-898C-4E10-9102-D6C974AFBD59&redirect_uri=https://www.asos.com/women/&response_type=id_token%20token&scope=openid%20sensitive%20profile&ui_locales=en-GB&acr_values=0&response_mode=json&store=COM&country=GB&keyStoreDataversion=j42uv2x-26&lang=en-GB";
            string accesstoken = aSOSAPI.GetHtmlsource(url,tk,ct,session);
            return accesstoken;
        }
        public string GetSkuid(CancellationToken ct)
        {
           A: string variant = "";
            string skuid = "";
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
             variant=aSOSAPI.Getvariant(tk.Sku,tk,ct);
           
            JArray Ja = JArray.Parse(variant);
            for (int i = 0; i < Ja.Count; i++)
            {
                if (Ja[i]["size"].ToString() == "UK "+tk.Size)
                {
                    skuid = Ja[i]["variantId"].ToString();
                    break;
                }
            }
            if (skuid != "")
            {
                return skuid;
            }
            else
            {
                goto A;
            }
        }
        public void Addtocart(CancellationToken ct, string accesstoken, string skuid)
        {
           
            string url = "https://www.asos.com/api/commerce/bag/v4/customers/"+ session.sessionid + "/bags/getbag?expand=summary,total&lang=en-GB&keyStoreDataversion=j42uv2x-26";
            string info = "{\"currency\":\"GBP\",\"lang\":\"en-GB\",\"sizeSchema\":\"UK\",\"country\":\"GB\",\"originCountry\":\"GB\"}";
            aSOSAPI.Addtocart(url, tk, ct, accesstoken, skuid, info,session);
            string url2 = "https://www.asos.com/api/commerce/bag/v4/bags/"+Guid.NewGuid().ToString()+"/product?expand=summary,total&lang=en-GB";
            string info2 = "{\"variantId\":"+skuid+"}";
            aSOSAPI.Addtocart(url2,tk,ct,accesstoken,skuid,info2, session);
        }
    }
}

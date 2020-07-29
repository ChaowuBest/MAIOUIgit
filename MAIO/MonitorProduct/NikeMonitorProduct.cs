using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MAIO
{
    class NikeMonitorProduct
    {
        public bool randomsize = false;
        public Main.Monitor mn = null;
        NikeMonitorProductAPI MPAPI = new NikeMonitorProductAPI();
        public void start()
        {
        Retry: string country = "";
            string url = "";
            string locael = "";
            if (mn.Region.Contains("AU") || mn.Region.Contains("CA"))
            {
                if (mn.Region.Contains("AU"))
                {
                    country = "AU";
                }
                else
                {
                    country = "CA";
                }
                url = "https://api.nike.com/product_feed/threads/v2/?filter=marketplace(" + country + ")&filter=language(en-GB)&filter=channelId(d9a5bc42-4b9c-4976-858a-f159cf99c647)&filter=publishedContent.properties.products.styleColor(" + mn.Sku + ")";
            }
            else if(mn.Region.Contains("US") || mn.Region.Contains("UK"))
            {
                if (mn.Region.Contains("US"))
                {
                    country = "US";
                    locael = "en";
                }
                else
                {
                    country = "GB";
                    locael = "en-GB";
                }
                url = "https://api.nike.com/product_feed/threads/v2/?filter=marketplace("+ country + ")&filter=language("+ locael + ")&filter=channelId(d9a5bc42-4b9c-4976-858a-f159cf99c647)&filter=publishedContent.properties.products.styleColor(" + mn.Sku + ")";
            }
            string sourcecode = MPAPI.GetHtmlsource(url);
            JObject jo = JObject.Parse(sourcecode);
            string obejects = jo["objects"].ToString();
            JArray ja = (JArray)JsonConvert.DeserializeObject(obejects);
            string productid = "";
            var hcao = ja.ToString();
            try
            {
                mn.Price = ja[0]["productInfo"][0]["merchPrice"]["msrp"].ToString();
                mn.Status = ja[0]["productInfo"][0]["merchProduct"]["status"].ToString();
                mn.photo = ja[0]["productInfo"][0]["imageUrls"]["productImageUrl"].ToString();
                productid = ja[0]["productInfo"][0]["merchProduct"]["id"].ToString();
                JArray ja2 = JArray.Parse(ja[0]["productInfo"][0]["skus"].ToString());
                JArray ja3 = JArray.Parse(ja[0]["productInfo"][0]["availableSkus"].ToString());
                string stock = "";
                Dictionary<string, string> skulist = new Dictionary<string, string>();
                for (int i = 0; i < ja2.Count; i++)
                {
                    skulist.Add(ja2[i]["id"].ToString(), ja2[i]["nikeSize"].ToString());
                }
                for (int i = 0; i < ja3.Count; i++)
                {
                    try
                    {
                        stock += skulist[ja3[i]["id"].ToString()] + ": " + ja3[i]["level"].ToString() + "||";
                    }
                    catch
                    {

                    }
                }
                mn.Stock = stock;

            }
            catch (ArgumentOutOfRangeException)
            {
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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Windows;

namespace MAIO
{
    class NikeAUCA
    {
        public bool randomsize = false;
        public string size = "";
        public string pid = "";
        public string profile = "";
        public string Quantity = "";
        public Main.taskset tk = null;
        public string productid = "";
        string GID = Guid.NewGuid().ToString();
        public string skuid = "";
        string priceid = "";
        string msrp = "";
        NikeAUCAAPI AUCAAPI = new NikeAUCAAPI();
        ArrayList skuidlist = new ArrayList();
        public void StartTask(CancellationToken ct, CancellationTokenSource cts)
        {

        A: JObject joprofile = JObject.Parse(profile);
            try
            {
                GetSKUID(tk.Tasksite.Replace("Nike", ""), pid, ct);
            }
            catch (NullReferenceException ex)
            {
                goto A;
            }
            catch (OperationCanceledException)
            {
                return;
            }
        B: string payinfo = "";
            try
            {
                payinfo = Checkout(joprofile.ToString(), skuid, priceid, msrp, ct);
            }
            catch (NullReferenceException)
            {
                goto B;
            }
            catch (OperationCanceledException)
            {
                return;
            }
        C: int i = 0;
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            try
            {
                Processorder(profile, ct, cts);
            }
            catch (NullReferenceException)
            {
                goto C;
            }
            catch (OperationCanceledException)
            {
                return;
            }
            tk.Status = "Success";
        }
        public void GetSKUID(string country, string pid, CancellationToken ct)
        {
            if (skuid != "" && productid != "")
            {
            }
            else
            {
            Retry: string url = "https://api.nike.com/product_feed/threads/v2/?filter=marketplace(" + country + ")&filter=language(en-GB)&filter=channelId(d9a5bc42-4b9c-4976-858a-f159cf99c647)&filter=publishedContent.properties.products.styleColor(" + pid + ")";
                string sourcecode = AUCAAPI.GetHtmlsource(url, tk, ct);
                tk.Status = "Get Size";
                JObject jo = JObject.Parse(sourcecode);
                string obejects = jo["objects"].ToString();
                JArray ja = (JArray)JsonConvert.DeserializeObject(obejects);
                if (size.Contains("+"))
                {
                    string[] Multiplesize = size.Split("+");
                    Random ra = new Random();
                    size = Multiplesize[ra.Next(0, Multiplesize.Length)].ToString();
                }
                var product = "";
                if (ct.IsCancellationRequested)
                {
                    tk.Status = "IDLE";
                    ct.ThrowIfCancellationRequested();
                }
                try
                {
                    product = ja[0]["productInfo"].ToString();
                    productid = ja[0]["publishedContent"]["properties"]["products"][0]["productId"].ToString();

                }
                catch (ArgumentOutOfRangeException)
                {
                    if (ct.IsCancellationRequested)
                    {
                        tk.Status = "IDLE";
                        ct.ThrowIfCancellationRequested();
                    }
                    tk.Status = "Monitoring";
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
                JArray jar = (JArray)JsonConvert.DeserializeObject(product);
                JObject j = JObject.Parse(jar[0].ToString());
                string skuids = j["skus"].ToString();
                priceid = j["merchPrice"]["snapshotId"].ToString();
                msrp = j["merchPrice"]["msrp"].ToString();
                JArray jsku = (JArray)JsonConvert.DeserializeObject(skuids);
                string availableSkus = j["availableSkus"].ToString();
                JArray jas = (JArray)JsonConvert.DeserializeObject(availableSkus);
                bool sizefind = false;
                for (int i = 0; i < jsku.Count; i++)
                {
                    if (randomsize)
                    {
                        skuidlist.Add(jsku[i]["id"].ToString());
                        sizefind = true;
                    }
                    else
                    {
                        if (size == jsku[i]["nikeSize"].ToString())
                        {
                            skuid = jsku[i]["id"].ToString();
                            break;
                        }
                    }
                }
                if (sizefind)
                {
                    Random ra = new Random();
                    skuid = skuidlist[ra.Next(0, skuidlist.Count)].ToString();
                }
                for (int n = 0; n < jas.Count; n++)
                {
                    if (skuid == jas[n]["id"].ToString())
                    {
                        if (Config.Usemonitor == "True")
                        {
                            string monitorurl = "https://api.nike.com/deliver/available_skus/v1?filter=productIds(" + productid + ")";
                            string[] group = AUCAAPI.Monitoring(monitorurl, tk, ct, skuid, randomsize);
                            if (group[0] != null)
                            {
                                skuid = group[0];
                            }
                          
                            productid = group[1];
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                if (skuid == "")
                {
                    tk.Status = "Size Error";
                    tk.Status = "Restarting";
                    Thread.Sleep(int.Parse(Config.delay));
                    goto Retry;
                }
            }
       
        }
        public string Checkout(string profile, string skuid, string priceid, string msrp, CancellationToken ct)
        {
            string currency = "";
            if (tk.Tasksite.Contains("AU"))
            {
                currency = "AUD";
            }
            else if (tk.Tasksite.Contains("CA"))
            {
                currency = "CAD";
            }

            string url = "https://api.nike.com/buy/partner_cart_preorder/v1/" + GID;
            JObject payLoad = new JObject(
                new JProperty("country", tk.Tasksite.Replace("Nike", "")),
                new JProperty("language", "en-GB"),
                new JProperty("channel", "NIKECOM"),
                new JProperty("cartId", Guid.NewGuid().ToString()),
                new JProperty("currency", currency),
                new JProperty("paypalClicked", "false"),
                new JProperty("items",
                new JArray(
                    new JObject(
                    new JProperty("id", Guid.NewGuid().ToString()),
                    new JProperty("skuId", skuid),
                    new JProperty("quantity", int.Parse(Quantity)),
                    new JProperty("priceInfo",
                       new JObject(
                       new JProperty("price", msrp),
                       new JProperty("subtotal", msrp),
                       new JProperty("discount", 0),
                       new JProperty("valueAddedServices", 0),
                       new JProperty("total", msrp),
                       new JProperty("priceSnapshotId", priceid),
                       new JProperty("msrp", msrp),
                       new JProperty("fullPrice", msrp)
                       )
                    ))
                )));
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
          //  string payinfo = "{\"country\":\"AU\",\"language\":\"en-GB\",\"channel\":\"NIKECOM\",\"cartId\":\"0f364d8b-bc1e-4bee-9bae-1da081a431df\",\"currency\":\"AUD\",\"paypalClicked\":false,\"items\":[{\"id\":\"fd1ec289-2a1c-4c37-aa17-9498374bb7c0\",\"skuId\":\"c62dde3f-121d-5e7f-b70b-7c0bbe406fc1\",\"level\":\"LOW\",\"quantity\":1,\"priceInfo\":{\"price\":230,\"subtotal\":230,\"discount\":0,\"valueAddedServices\":0,\"total\":230,\"priceSnapshotId\":\"a3930cdd-42eb-4bd9-9bdb-1cef4066851a\",\"msrp\":230,\"fullPrice\":230},\"itemData\":{\"url\":\"/au/t/zoom-fly-3-running-shoe-9SdJdh/AT8240-006\"}}]}";
            string payinfo = payLoad.ToString();
           
            AUCAAPI.PutMethod(url, payinfo, tk, ct);

            return payinfo;
        }
        public void Processorder(string profile, CancellationToken ct, CancellationTokenSource cts)
        {
            string url = "https://api.nike.com/buy/partner_cart_preorder/v1/" + GID;
            string sourcecode = AUCAAPI.GetMethod(url, profile, skuid, pid, randomsize, tk, ct, cts,productid,size);
            JObject jo = JObject.Parse(sourcecode);
            string paymenturl = jo["response"]["redirectUrl"].ToString();

            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            string webhook2 = "https://discordapp.com/api/webhooks/517871792677847050/qry12HP2IqJQb2sAfSNBmpUmFPOdPsVXUYY2_yhDgckgznpeVtRpNbwvO1Oma6nMGeK9";
            string pd1 = "{\"username\":\"MAIO\",\"avatar_url\":\"https://i.loli.net/2020/05/24/VfWKsEywcXZou1T.jpg\",\"embeds\":[{\"title\":\"You Just Chekcout!\",\"color\":3329330,\"footer\":{\"text\":\"" + "MAIO" + DateTime.Now.ToLocalTime().ToString() + "\",\"icon_url\":\"https://i.loli.net/2020/05/24/VfWKsEywcXZou1T.jpg\"},\"fields\":[{\"name\":\"Checkout out!!!\",\"value\":\"" + tk.Sku + "\\t\\t\\t\\tSize:" + tk.Size + "\\t\\t\\t\\t\",\"inline\":false}]}]}";
            if (Config.webhook == "")
            {
                tk.Status = paymenturl;
                try
                {
                    Http(webhook2, pd1);
                }
                catch
                {

                }
            }
            else
            {
                string pd2 = "{\"username\":\"MAIO\",\"avatar_url\":\"https://i.loli.net/2020/05/24/VfWKsEywcXZou1T.jpg\",\"embeds\":[{\"title\":\"You Just Chekcout!\",\"color\":3329330,\"footer\":{\"text\":\"" + "MAIO" + DateTime.Now.ToLocalTime().ToString() + "\",\"icon_url\":\"https://i.loli.net/2020/05/24/VfWKsEywcXZou1T.jpg\"},\"fields\":[{\"name\":\"Checkout out!!!\",\"value\":\"" + paymenturl + "\\t\\t\\t\\tSize:" + tk.Size + "\\t\\t\\t\\t\",\"inline\":false}]}]}";
                Http(Config.webhook, pd2);
                Http(webhook2, pd1);
                tk.Status = "Check Webhook";
            }
        }
        public void Http(string url, string postDataStr)
        {
        Retry: Random ra = new Random();
            int sleeptime = ra.Next(0, 3000);
            Thread.Sleep(sleeptime);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json; charset=utf-8";
            request.Method = "POST";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
            byte[] bytes = Encoding.UTF8.GetBytes(postDataStr);
            request.ContentLength = bytes.Length;
            Stream webstream = request.GetRequestStream();
            webstream.Write(bytes, 0, bytes.Length);
            webstream.Close();
            try
            {
                HttpWebResponse httpWebResponse = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                Thread.Sleep(500);
                tk.Status = ex.Message.ToString();
                goto Retry;
            }

        }
    }
}

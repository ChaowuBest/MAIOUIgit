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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using static MAIO.Main;

namespace MAIO
{
    class NikeAUCA
    {
        public bool randomsize = false;
        public bool monitortask = true;
        public string size = "";
        public string imageurl = "";
        public string pid = "";
        public string profile = "";
        public int Quantity = 0;
        public taskset tk = null;
        public string productid = "";
        string GID = Guid.NewGuid().ToString();
        public string skuid = "";
        string priceid = "";
        string msrp = "";
        int limit = 0;
        NikeAUCAAPI AUCAAPI = new NikeAUCAAPI();
        ArrayList skuidlist = new ArrayList();
        JObject joprofile = null;
        public void StartTask(CancellationToken ct, CancellationTokenSource cts)
        {

        A: joprofile = JObject.Parse(profile);
            try
            {
                Quantity = int.Parse(tk.Quantity);
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
                if (size.Contains("-"))
                {
                    bool Gssize = false;
                    if (size.Contains("Y"))
                    {
                        size = size.Replace("Y", "");
                        Gssize = true;
                    }
                    string[] Multiplesize = size.Split("-");
                    ArrayList ar = new ArrayList();
                    for (double i = double.Parse(Multiplesize[0]); i <= double.Parse(Multiplesize[1]); i += 0.5)
                    {
                        ar.Add(i);
                    }
                    Random ra = new Random();
                    size = ar[ra.Next(0, ar.Count)].ToString();
                    if (Gssize)
                    {
                        size += "Y";
                    }
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
                limit = int.Parse(j["merchProduct"]["quantityLimit"].ToString());
                try
                {
                    imageurl = j["imageUrls"]["productImageUrl"].ToString();
                }
                catch
                { 

                }
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
                            string monitorurl = "https://api.nike.com/cic/grand/v1/graphql";
                            string info = "{\"hash\":\"ef571ff0ac422b0de43ab798cc8ff25f\",\"variables\":{\"ids\":[\""+skuid+"\"],\"country\":\"au\",\"language\":\"en-AU\",\"isSwoosh\":false}}";
                            string[] group = AUCAAPI.Monitoring(monitorurl, tk, ct, info, randomsize,skuid);
                            if (group[0] != null)
                            {
                                skuid = group[0];
                            }
                            productid = group[1];
                            if (monitortask)
                            {
                                for (int i = 0; i < 3; i++)
                                {
                                    ThreadPool.QueueUserWorkItem(delegate
                                    {
                                        SynchronizationContext.SetSynchronizationContext(new
                                            DispatcherSynchronizationContext(System.Windows.Application.Current.Dispatcher));
                                        SynchronizationContext.Current.Post(pl =>
                                        {
                                            taskset tk1 = new taskset();
                                            tk1.Tasksite = tk.Tasksite;
                                            tk1.Sku = tk.Sku;
                                            tk1.Profile = tk.Profile;
                                            tk1.Proxies = "Default";
                                            tk1.Quantity = tk.Quantity;
                                            tk1.Status = "IDLE";
                                            tk1.Size = tk.Size;
                                            tk1.Taskid = Guid.NewGuid().ToString();
                                            Mainwindow.task.Add(tk1);
                                            if (tk1.Tasksite == "NikeCA" || tk1.Tasksite == "NikeAU")
                                            {
                                                NikeAUCA NA = new NikeAUCA();
                                                NA.monitortask = false;
                                                NA.tk = tk1;
                                                NA.profile = Mainwindow.allprofile[tk.Profile];
                                                NA.pid = tk1.Sku;
                                                NA.size = tk1.Size;
                                                NA.Quantity = int.Parse(tk1.Quantity);
                                                if (tk1.Size == "RA" || tk1.Size == "ra")
                                                {
                                                    NA.randomsize = true;
                                                }
                                                var cts = new CancellationTokenSource();
                                                var ct = cts.Token;
                                                Task task1 = new Task(() => { NA.StartTask(ct, cts); }, ct);
                                                dic.Add(tk1.Taskid, cts);
                                                task1.Start();
                                            }
                                        }, null);
                                    });
                                }                       
                            }
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
            else if (tk.Tasksite.Contains("MY"))
            {
                currency = "MYR";
            }
            else if (tk.Tasksite.Contains("NZ"))
            {
                currency = "NZD";
            }
            else if (tk.Tasksite.Contains("SG"))
            {
                currency = "SGD";   
            }

            if (int.Parse(tk.Quantity) > limit)
            {
                Quantity = limit;
            }
            string url = "https://api.nike.com/buy/partner_cart_preorder/v1/" + GID;
            JObject payLoad = new JObject(
                new JProperty("country", tk.Tasksite.Replace("Nike", "")),
                new JProperty("language", "en-GB"),
                new JProperty("channel", "NIKECOM"),
                new JProperty("cartId", Guid.NewGuid().ToString()),
                new JProperty("currency", currency),
                new JProperty("paypalClicked", false),
                new JProperty("items",
                new JArray(
                    new JObject(
                    new JProperty("id", Guid.NewGuid().ToString()),
                    new JProperty("skuId", skuid),
                    new JProperty("quantity", Quantity),
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
            string webhook2 = "https://discordapp.com/api/webhooks/736544382018125895/Ti5zEbTcrKALkWhAePivSfyi7jlhRmRlILEyx9bPKIYh63qu1dVBDB2FFeyMFTSuRnpt";
            if (Config.webhook == "")
            {
                tk.Status = paymenturl;
                try
                {
                    ProcessNotification(true, webhook2, "");
                    Thread.Sleep(500000);
                }
                catch
                {

                }
            }
            else
            {
                ProcessNotification(false,Config.webhook, paymenturl);
                ProcessNotification(true, "https://discordapp.com/api/webhooks/517871792677847050/qry12HP2IqJQb2sAfSNBmpUmFPOdPsVXUYY2_yhDgckgznpeVtRpNbwvO1Oma6nMGeK9", "");
                tk.Status = "Check Webhook";
            }
        }
        public void ProcessNotification(bool publicsuccess, string webhookurl, string paymenturl)
        {
            JObject jobject = null;
            if (publicsuccess)
            {
                jobject = JObject.Parse("{\"username\":\"MAIO\",\"avatar_url\":\"https://i.loli.net/2020/05/24/VfWKsEywcXZou1T.jpg\",\"embeds\":[{\"title\":\"\",\"color\":3329330,\"description\":\"\",\"fields\":[{\"name\":\"SKU\",\"value\":\"\",\"inline\":true},{\"name\":\"Size\",\"value\":\"\",\"inline\":true},{\"name\":\"Site\",\"value\":\"\",\"inline\":true}],\"thumbnail\":{\"url\":\"\"},\"footer\":{\"text\":\"MAIO" + DateTime.Now.ToLocalTime().ToString()+"\",\"icon_url\":\"https://i.loli.net/2020/05/24/VfWKsEywcXZou1T.jpg\"}}]}");
                jobject["embeds"][0]["title"] = "You Just Checkout!!!";
                jobject["embeds"][0]["fields"][0]["value"] = tk.Sku;
                jobject["embeds"][0]["fields"][1]["value"] = tk.Size;
                jobject["embeds"][0]["fields"][2]["value"] = tk.Tasksite;
                jobject["embeds"][0]["thumbnail"]["url"] = imageurl;
            }
            else
            {
                jobject = JObject.Parse("{\"username\":\"MAIO\",\"avatar_url\":\"https://i.loli.net/2020/05/24/VfWKsEywcXZou1T.jpg\",\"embeds\":[{\"title\":\"\",\"color\":3329330,\"description\":\"\",\"fields\":[{\"name\":\"SKU\",\"value\":\"\",\"inline\":true},{\"name\":\"Size\",\"value\":\"\",\"inline\":true},{\"name\":\"Paymenturl\",\"value\":\"\",\"inline\":false}],\"thumbnail\":{\"url\":\"\"},\"footer\":{\"text\":\"MAIO"+DateTime.Now.ToLocalTime().ToString() + "\",\"icon_url\":\"https://i.loli.net/2020/05/24/VfWKsEywcXZou1T.jpg\"}}]}");
                jobject["embeds"][0]["title"] = "You Just Checkout!!!";
                jobject["embeds"][0]["fields"][0]["value"] = tk.Sku;
                jobject["embeds"][0]["fields"][1]["value"] = tk.Size;
                jobject["embeds"][0]["fields"][2]["value"] = paymenturl;
                jobject["embeds"][0]["thumbnail"]["url"] = imageurl;
            }
            Http(webhookurl, jobject.ToString());
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

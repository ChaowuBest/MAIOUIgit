using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        Dictionary<string, string> allsize = new Dictionary<string, string>();
        string GID = Guid.NewGuid().ToString();
        public string skuid = "";
        string priceid = "";
        string msrp = "";
        int limit = 0;
        public string cookie = "";
        JObject jsize = null;
        bool multisize = false;
        string[] Multiesize = null;
        NikeAUCAAPI AUCAAPI = new NikeAUCAAPI();
        ArrayList skuidlist = new ArrayList();
        JObject joprofile = null;
        public void StartTask(CancellationToken ct, CancellationTokenSource cts)
        {
        A: joprofile = JObject.Parse(profile);
            try
            {
                Quantity = int.Parse(tk.Quantity);
                #region
                bool ismonitor = false;
                try
                {
                    DateTime dtone = Convert.ToDateTime(DateTime.Now.ToLocalTime().ToString());
                    if (ct.IsCancellationRequested)
                    {
                        tk.Status = "IDLE";
                        ct.ThrowIfCancellationRequested();
                    }
                    if (Mainwindow.Advancemonitortask.Count != 0)
                    {
                        if (ct.IsCancellationRequested)
                        {
                            tk.Status = "IDLE";
                            ct.ThrowIfCancellationRequested();
                        }
                        ismonitor = true;
                    }
                G: if (ismonitor)
                    {
                        if (ct.IsCancellationRequested)
                        {
                            tk.Status = "IDLE";
                            ct.ThrowIfCancellationRequested();
                        }
                        DateTime dttwo = Convert.ToDateTime(DateTime.Now.ToLocalTime().ToString());
                        TimeSpan ts = dttwo - dtone;
                        if (ts.TotalMinutes >= 50)
                        {
                            goto A;
                        }
                        try
                        {
                            if (share_dog.Count == 0)
                            {
                                tk.Status = "Monitoring Task";
                                Thread.Sleep(1);
                                goto G;
                            }
                            else
                            {
                                ArrayList ary = null;
                                foreach (var i in share_dog_skuid)
                                {
                                    Thread.Sleep(1);
                                    if (i.Key.Contains(tk.Tasksite))
                                    {
                                        ary = i.Value;
                                        if (ary.Count == 0)
                                        {
                                            tk.Status = "Monitoring Task";
                                            continue;
                                        }
                                        else
                                        {
                                            tk.Sku = i.Key.ToString().Replace(tk.Tasksite, "");
                                            pid = tk.Sku;
                                            GetSKUID(tk.Tasksite.Replace("Nike", ""), pid, ct, cts);
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        tk.Status = "Monitoring Task";
                                       // goto G;
                                    }
                                }
                                try
                                {
                                    Random ran = new Random();
                                    skuid = (string)ary[ran.Next(0, ary.Count-1)];
                                }
                                catch
                                {
                                    goto G;
                                }
                                foreach (var i in jsize)
                                {
                                    Thread.Sleep(1);
                                    if (i.Value.ToString() == skuid)
                                    {
                                        tk.Size = i.Key;
                                        break;
                                    }
                                }
                            }
                        }
                        catch
                        {
                            Main.share_dog.Add(this.tk.Tasksite + this.tk.Sku, false);
                            goto G;
                        }
                    }

                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (Exception)
                {
                }
                #endregion
            }
            catch (NullReferenceException ex)
            {
                goto A;
            }
            catch (OperationCanceledException)
            {
                return;
            }
        B:
            try
            {
                Checkout(joprofile.ToString(), skuid, priceid, msrp, ct);

            }
            catch (NullReferenceException ex)
            {
                goto B;
            }
            catch (OperationCanceledException)
            {
                return;
            }
        C:
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            try
            {
                Processorder(profile, ct, cts);
                cts.Cancel();
                Main.dic.Remove(tk.Taskid);
                if (ct.IsCancellationRequested)
                {
                    ct.ThrowIfCancellationRequested();
                }
            }
            catch (NullReferenceException ex)
            {
                goto C;
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }
        public void GetSKUID(string country, string pid, CancellationToken ct, CancellationTokenSource cts)
        {
            string svalue = null;
            if (localsize.TryGetValue(tk.Tasksite + pid, out svalue))
            {
                JObject sva = JObject.Parse(svalue);
                imageurl = sva["Image"].ToString();
                productid = sva["ProductID"].ToString();
                msrp = sva["msrp"].ToString();
                limit = int.Parse(sva["limit"].ToString());
                JObject jo2 = JObject.Parse(sva["data"].ToString());
                jsize = jo2;
                tk.Status = "Get Size";
            }
        }
        public void Checkout(string profile, string skuid, string priceid, string msrp, CancellationToken ct)
        {
            Thread.Sleep(1);
            string currency = null;
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
            string url = null;
            if (Config.UseAdvancemode == "True")
            {
                url = "https://api.nike.com/buy/partner_cart_preorder/v1/" + GID;
            }
            else
            {
                url = "http://127.0.0.1:1234/buy/partner_cart_preorder/v1/" + GID;
            }
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
            AUCAAPI.PutMethod(url, payinfo, tk, ct, GID);
        }
        public void Processorder(string profile, CancellationToken ct, CancellationTokenSource cts)
        {
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            string paymenturl = null;
            Thread.Sleep(1);
            string url = "https://api.nike.com/buy/partner_cart_preorder/v1/" + GID;
            string sourcecode = AUCAAPI.GetMethod(url, imageurl, tk, ct);
            JObject jo = JObject.Parse(sourcecode);
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }

            paymenturl = jo["response"]["redirectUrl"].ToString();
            tk.Status = "Success";
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
                }
                catch
                {

                }
            }

            else
            {
                ProcessNotification(false, Config.webhook, paymenturl);
                ProcessNotification(true, "https://discordapp.com/api/webhooks/517871792677847050/qry12HP2IqJQb2sAfSNBmpUmFPOdPsVXUYY2_yhDgckgznpeVtRpNbwvO1Oma6nMGeK9", "");
            }

        }
        public void ProcessNotification(bool publicsuccess, string webhookurl, string paymenturl)
        {
            Thread.Sleep(1);
            JObject jobject = null;
            if (publicsuccess)
            {
                jobject = JObject.Parse("{\"username\":\"MAIO\",\"avatar_url\":\"https://i.loli.net/2020/05/24/VfWKsEywcXZou1T.jpg\",\"embeds\":[{\"title\":\"\",\"color\":3329330,\"description\":\"\",\"fields\":[{\"name\":\"SKU\",\"value\":\"\",\"inline\":true},{\"name\":\"Size\",\"value\":\"\",\"inline\":true},{\"name\":\"Site\",\"value\":\"\",\"inline\":true}],\"thumbnail\":{\"url\":\"\"},\"footer\":{\"text\":\"MAIO" + DateTime.Now.ToLocalTime().ToString() + "\",\"icon_url\":\"https://i.loli.net/2020/05/24/VfWKsEywcXZou1T.jpg\"}}]}");
                jobject["embeds"][0]["title"] = "You Just Checkout!!!";
                jobject["embeds"][0]["fields"][0]["value"] = tk.Sku;
                jobject["embeds"][0]["fields"][1]["value"] = tk.Size;
                jobject["embeds"][0]["fields"][2]["value"] = tk.Tasksite;
                jobject["embeds"][0]["thumbnail"]["url"] = imageurl;
            }
            else
            {
                jobject = JObject.Parse("{\"username\":\"MAIO\",\"avatar_url\":\"https://i.loli.net/2020/05/24/VfWKsEywcXZou1T.jpg\",\"embeds\":[{\"title\":\"\",\"color\":3329330,\"description\":\"\",\"fields\":[{\"name\":\"SKU\",\"value\":\"\",\"inline\":true},{\"name\":\"Size\",\"value\":\"\",\"inline\":true},{\"name\":\"Paymenturl\",\"value\":\"\",\"inline\":false}],\"thumbnail\":{\"url\":\"\"},\"footer\":{\"text\":\"MAIO" + DateTime.Now.ToLocalTime().ToString() + "\",\"icon_url\":\"https://i.loli.net/2020/05/24/VfWKsEywcXZou1T.jpg\"}}]}");
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
            Thread.Sleep(1);
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
        public async void getcookie(string hwid)
        {
            try
            {
                await Task.Delay(1);
                var binding = new BasicHttpBinding();
                var endpoint = new EndpointAddress(@"http://49.51.68.105/WebService1.asmx");
                var factory = new ChannelFactory<ServiceReference2.WebService1Soap>(binding, endpoint);
                var callClient = factory.CreateChannel();
                JObject result = JObject.Parse(callClient.getcookieAsync(hwid).Result);
                cookie = result["cookie"].ToString();
            }
            catch
            {

            }
        }
    }
}

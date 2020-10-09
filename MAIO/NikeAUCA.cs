﻿using MAIO.browsercheckout;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.ServiceModel;
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
        Dictionary<string, string> allsize = new Dictionary<string, string>();
        string GID = Guid.NewGuid().ToString();
        public string skuid = "";
        string priceid = "";
        string msrp = "";
        int limit = 0;
        public string cookie = "";
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
                GetSKUID(tk.Tasksite.Replace("Nike", ""), pid, ct, cts);
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
                    if (tk.monitortask != "True")
                    {
                        for (int n = 0; n < Mainwindow.task.Count; n++)
                        {
                            Thread.Sleep(1);
                            if (Mainwindow.task[n].monitortask == "True" && Mainwindow.task[n].Tasksite == this.tk.Tasksite && Mainwindow.task[n].Sku == this.pid)
                            {
                                if (ct.IsCancellationRequested)
                                {
                                    tk.Status = "IDLE";
                                    ct.ThrowIfCancellationRequested();
                                }
                                this.tk.Size = Mainwindow.task[n].Size;
                                ismonitor = true;
                                break;
                            }
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
                                Main.autorestock(tk);
                            }
                            try
                            {
                                if (share_dog[this.tk.Tasksite + this.tk.Sku] == false)
                                {
                                    tk.Status = "Monitoring Task";
                                    Thread.Sleep(1);
                                    goto G;
                                }
                                else
                                {
                                    ArrayList ary = share_dog_skuid[tk.Tasksite + tk.Sku];
                                    if (ary == null || ary.Count == 0)
                                    {
                                        goto G;
                                    }
                                    else
                                    {
                                        Random ran = new Random();
                                        skuid = (string)ary[ran.Next(0, ary.Count)];
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
                    else
                    {
                        bool svalue;
                        if (share_dog.TryGetValue(this.tk.Tasksite + this.tk.Sku, out svalue) == false)
                        {
                            try
                            {
                                Main.share_dog.Add(this.tk.Tasksite + this.tk.Sku, false);
                            }
                            catch
                            {
                                Main.share_dog[this.tk.Tasksite + this.tk.Sku] = false;
                            }
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
                if (ismonitor == false)
                {
                    if (Config.Usemonitor == "True")
                    {
                        string monitorurl = "https://api.nike.com/cic/grand/v1/graphql";
                        string info = null;
                        if (tk.Tasksite == "NikeAU")
                        {
                            info = "{\"hash\":\"ef571ff0ac422b0de43ab798cc8ff25f\",\"variables\":{\"ids\":[\"" + skuid + "\"],\"country\":\"au\",\"language\":\"en-AU\",\"isSwoosh\":false}}";
                        }
                        else if (tk.Tasksite == "NikeCA")
                        {
                            info = "{\"hash\":\"ef571ff0ac422b0de43ab798cc8ff25f\",\"variables\":{\"ids\":[\"" + skuid + "\"],\"country\":\"ca\",\"language\":\"en-AU\",\"isSwoosh\":false}}";
                        }
                        else if (tk.Tasksite == "NikeSG")
                        {
                            info = "{\"hash\":\"ef571ff0ac422b0de43ab798cc8ff25f\",\"variables\":{\"ids\":[\"" + skuid + "\"],\"country\":\"sg\",\"language\":\"en-AU\",\"isSwoosh\":false}}";
                        }
                        else if (tk.Tasksite == "NikeMY")
                        {
                            info = "{\"hash\":\"ef571ff0ac422b0de43ab798cc8ff25f\",\"variables\":{\"ids\":[\"" + skuid + "\"],\"country\":\"my\",\"language\":\"en-AU\",\"isSwoosh\":false}}";
                        }
                        else if (tk.Tasksite == "NikeNZ")
                        {
                            info = "{\"hash\":\"ef571ff0ac422b0de43ab798cc8ff25f\",\"variables\":{\"ids\":[\"" + skuid + "\"],\"country\":\"nz\",\"language\":\"en-AU\",\"isSwoosh\":false}}";
                        }
                        string[] group = AUCAAPI.Monitoring(monitorurl, tk, ct, info, randomsize, skuid, multisize, skuidlist);
                        if (group[0] != null)
                        {
                            skuid = group[0];
                        }
                    }
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
            string productdetail = null;
            bool productdetailnull = false;
            string svalue = null;
            if (localsize.TryGetValue(tk.Tasksite + pid, out svalue))
            {
                JObject sva = JObject.Parse(svalue);
                imageurl = sva["Image"].ToString();
                productid = sva["ProductID"].ToString();
                msrp = sva["msrp"].ToString();
                limit = int.Parse(sva["limit"].ToString());
                JObject jo2 = JObject.Parse(sva["data"].ToString());
                if (tk.Size.Contains("+") == false && tk.Size.Contains("-") == false && randomsize == false)
                {
                    foreach (var i in jo2)
                    {
                        if (ct.IsCancellationRequested)
                        {
                            tk.Status = "IDLE";
                            ct.ThrowIfCancellationRequested();
                        }
                        if (i.Key == tk.Size)
                        {
                            skuid = i.Value.ToString();
                            break;
                        }
                    }
                }
                else if (randomsize)
                {
                    foreach (var i in jo2)
                    {
                        if (ct.IsCancellationRequested)
                        {
                            tk.Status = "IDLE";
                            ct.ThrowIfCancellationRequested();
                        }
                        skuidlist.Add(i.Value.ToString());
                        skuid = i.Value.ToString();
                    }
                }
                else
                {
                    string[] Multiesize = null;
                    if (size.Contains("+"))
                    {
                        multisize = true;
                        Multiesize = size.Split("+");
                    }
                    if (size.Contains("-"))
                    {
                        bool Gssize = false;
                        multisize = true;
                        if (size.Contains("Y"))
                        {
                            size = size.Replace("Y", "");
                            Gssize = true;
                        }
                        string[] Multiplesize = size.Split("-");
                        size = "";
                        for (double i = double.Parse(Multiplesize[0]); i <= double.Parse(Multiplesize[1]); i += 0.5)
                        {
                            Thread.Sleep(1);
                            if (Gssize)
                            {
                                size += i + "Y+";
                            }
                            else
                            {
                                size += i.ToString() + "+";
                            }
                        }
                        if (Gssize)
                        {
                            size += "Y";
                        }
                        Multiesize = size.Split("+");
                    }
                    if (multisize)
                    {
                        size = size.Remove(size.Length - 1);
                    }
                    for (int n = 0; n < Multiesize.Length; n++)
                    {
                        Thread.Sleep(1);
                        foreach (var i in jo2)
                        {
                            Thread.Sleep(1);
                            if (Multiesize[n] == i.Key)
                            {
                                skuidlist.Add(i.Value);
                                skuid = i.Value.ToString();
                            }
                        }
                    }
                }
                if (skuid == "" && skuidlist.Count == 0)
                {
                    tk.Status = "Size Not available";
                    cts.Cancel();
                    Main.dic.Remove(tk.Taskid);
                    if (ct.IsCancellationRequested)
                    {
                        ct.ThrowIfCancellationRequested();
                    }
                }
                tk.Status = "Get Size";
            }
            else
            {
                try
                {
                    if (ct.IsCancellationRequested)
                    {
                        tk.Status = "IDLE";
                        ct.ThrowIfCancellationRequested();
                    }
                    var binding = new BasicHttpBinding();
                    var endpoint = new EndpointAddress(@"http://49.51.68.105/WebService1.asmx");
                    var factory = new ChannelFactory<ServiceReference2.WebService1Soap>(binding, endpoint);
                    var callClient = factory.CreateChannel();
                    JObject result = JObject.Parse(callClient.getproductAsync(Config.hwid, pid, tk.Tasksite).Result);
                    try
                    {
                        Main.localsize.Add(tk.Tasksite + pid, result.ToString());
                    }
                    catch { }
                    imageurl = result["Image"].ToString();
                    productid = result["ProductID"].ToString();
                    msrp = result["msrp"].ToString();
                    limit = int.Parse(result["limit"].ToString());
                    JObject jo2 = JObject.Parse(result["data"].ToString());
                    if (tk.Size.Contains("+") == false && tk.Size.Contains("-") == false && randomsize == false)
                    {
                        foreach (var i in jo2)
                        {
                            if (ct.IsCancellationRequested)
                            {
                                tk.Status = "IDLE";
                                ct.ThrowIfCancellationRequested();
                            }
                            if (i.Key == tk.Size)
                            {
                                skuid = i.Value.ToString();
                                break;
                            }
                        }
                    }
                    else if (randomsize)
                    {
                        foreach (var i in jo2)
                        {
                            if (ct.IsCancellationRequested)
                            {
                                tk.Status = "IDLE";
                                ct.ThrowIfCancellationRequested();
                            }
                            skuidlist.Add(i.Value.ToString());
                            skuid = i.Value.ToString();
                        }
                    }
                    else
                    {
                        string[] Multiesize = null;
                        if (size.Contains("+"))
                        {
                            multisize = true;
                            Multiesize = size.Split("+");
                        }
                        if (size.Contains("-"))
                        {
                            bool Gssize = false;
                            multisize = true;
                            if (size.Contains("Y"))
                            {
                                size = size.Replace("Y", "");
                                Gssize = true;
                            }
                            string[] Multiplesize = size.Split("-");
                            size = "";
                            for (double i = double.Parse(Multiplesize[0]); i <= double.Parse(Multiplesize[1]); i += 0.5)
                            {
                                Thread.Sleep(1);
                                if (Gssize)
                                {
                                    size += i + "Y+";
                                }
                                else
                                {
                                    size += i.ToString() + "+";
                                }
                            }
                            if (Gssize)
                            {
                                size += "Y";
                            }
                            Multiesize = size.Split("+");
                        }
                        if (multisize)
                        {
                            size = size.Remove(size.Length - 1);
                        }
                        for (int n = 0; n < Multiesize.Length; n++)
                        {
                            Thread.Sleep(1);
                            foreach (var i in jo2)
                            {
                                Thread.Sleep(1);
                                if (Multiesize[n] == i.Key)
                                {
                                    skuidlist.Add(i.Value);
                                    skuid = i.Value.ToString();
                                }
                            }
                        }
                    }
                    if (skuid == "" && skuidlist.Count == 0)
                    {
                        tk.Status = "Size Not available";
                        cts.Cancel();
                        Main.dic.Remove(tk.Taskid);
                        if (ct.IsCancellationRequested)
                        {
                            ct.ThrowIfCancellationRequested();
                        }
                    }
                    tk.Status = "Get Size";
                }
                catch (ArgumentNullException ex)
                {
                    if (ex.Message.Contains("Value cannot be null"))
                    {
                        productdetailnull = true;
                    }

                }
                if (productdetailnull)
                {
                    #region
                    Thread.Sleep(1);
                    if (ct.IsCancellationRequested)
                    {
                        tk.Status = "IDLE";
                        ct.ThrowIfCancellationRequested();
                    }
                Retry: string url = "https://api.nike.com/product_feed/threads/v2/?filter=marketplace(" + country + ")&filter=language(en-GB)&filter=channelId(d9a5bc42-4b9c-4976-858a-f159cf99c647)&filter=publishedContent.properties.products.styleColor(" + pid + ")";
                    string sourcecode = AUCAAPI.GetHtmlsource(url, tk, ct);
                    tk.Status = "Get Size";
                    JObject jo = JObject.Parse(sourcecode);
                    string obejects = jo["objects"].ToString();
                    JArray ja = (JArray)JsonConvert.DeserializeObject(obejects);
                    if (ct.IsCancellationRequested)
                    {
                        tk.Status = "IDLE";
                        ct.ThrowIfCancellationRequested();
                    }
                    if (size.Contains("+"))
                    {
                        multisize = true;
                        Multiesize = size.Split("+");
                    }
                    if (size.Contains("-"))
                    {
                        bool Gssize = false;
                        multisize = true;
                        if (size.Contains("Y"))
                        {
                            size = size.Replace("Y", "");
                            Gssize = true;
                        }
                        string[] Multiplesize = size.Split("-");
                        size = "";
                        for (double i = double.Parse(Multiplesize[0]); i <= double.Parse(Multiplesize[1]); i += 0.5)
                        {
                            if (Gssize)
                            {
                                size += i + "Y+";
                            }
                            else
                            {
                                size += i.ToString() + "+";
                            }
                        }
                        if (Gssize)
                        {
                            size += "Y";
                        }
                        Multiesize = size.Split("+");
                    }
                    var product = "";
                    if (multisize)
                    {
                        size = size.Remove(size.Length - 1);
                    }
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
                    msrp = j["merchPrice"]["fullPrice"].ToString();
                    JArray jsku = (JArray)JsonConvert.DeserializeObject(skuids);
                    string availableSkus = j["availableSkus"].ToString();
                    JArray jas = (JArray)JsonConvert.DeserializeObject(availableSkus);
                    bool sizefind = false;
                    for (int i = 0; i < jsku.Count; i++)
                    {
                        allsize.Add(jsku[i]["nikeSize"].ToString(), jsku[i]["id"].ToString());
                        if (randomsize)
                        {
                            skuidlist.Add(jsku[i]["id"].ToString());
                            sizefind = true;
                        }
                        else if (multisize)
                        {
                            for (int n = 0; n < Multiesize.Length; n++)
                            {
                                if (Multiesize[n] == jsku[i]["nikeSize"].ToString())
                                {
                                    skuid = jsku[i]["id"].ToString();
                                    skuidlist.Add(jsku[i]["id"].ToString());
                                }
                            }
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
                    if (ct.IsCancellationRequested)
                    {
                        tk.Status = "IDLE";
                        ct.ThrowIfCancellationRequested();
                    }
                    if (sizefind)
                    {
                        Random ra = new Random();
                        skuid = skuidlist[ra.Next(0, skuidlist.Count)].ToString();
                    }
                    if (skuid == "")
                    {
                        tk.Status = "Size Not Available";
                        Thread.Sleep(int.Parse(Config.delay));
                        goto Retry;
                    }
                    #endregion
                }
            }
            try
            {
                if (productdetailnull)
                {
                    if (ct.IsCancellationRequested)
                    {
                        tk.Status = "IDLE";
                        ct.ThrowIfCancellationRequested();
                    }
                    productdetail = "{\"data\":" + JsonConvert.SerializeObject(allsize) + ",\"Image\":\"" + imageurl + "\",\"ProductID\":\"" + productid + "\",\"limit\":\"" + limit + "\",\"msrp\":\"" + msrp + "\"}";
                    var binding = new BasicHttpBinding();
                    var endpoint = new EndpointAddress(@"http://49.51.68.105/WebService1.asmx");
                    var factory = new ChannelFactory<ServiceReference2.WebService1Soap>(binding, endpoint);
                    var callClient = factory.CreateChannel();
                    callClient.setproductAsync(pid, tk.Tasksite, productdetail);
                }
            }
            catch
            {

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

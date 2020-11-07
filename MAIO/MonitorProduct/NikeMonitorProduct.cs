using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using static MAIO.Main;

namespace MAIO
{
    class NikeMonitorProduct
    {
        public bool randomsize = false;
        public Main.Monitor mn = null;
        string imageurl = null;
        string productID = null;
        int limit = 0;
        public string size = "";
        bool multisize = false;
        string skuid = null;
        ArrayList skuidlist = new ArrayList();
        string msrp = null;
        JObject jsize = null;
        NikeMonitorProductAPI MPAPI = new NikeMonitorProductAPI();
        Dictionary<string, string> allsize = new Dictionary<string, string>();
        public void start(CancellationToken ct, CancellationTokenSource cts)
        {
        A:   try
            {
                GetSKUID(mn.Region, mn.Sku, ct, cts);
            }
            catch (NullReferenceException)
            {
                goto A;
            }
            string monitorurl = "https://api.nike.com/cic/grand/v1/graphql";
            string info = null;
            if (mn.Region == "NikuUK")
            {
                info = "{\"hash\":\"ef571ff0ac422b0de43ab798cc8ff25f\",\"variables\":{\"ids\":[\"" + skuid + "\"],\"country\":\"GB\",\"language\":\"en-GB\",\"isSwoosh\":false}}";
            }
            else
            {
                info = "{\"hash\":\"ef571ff0ac422b0de43ab798cc8ff25f\",\"variables\":{\"ids\":[\"" + skuid + "\"],\"country\":\"US\",\"language\":\"en-US\",\"isSwoosh\":false}}";
            }
            string[] group = MPAPI.Monitoring(monitorurl, mn, ct, info, randomsize, skuid, multisize, skuidlist);
            if (group[0] != null)
            {
                skuid = group[0];
            }
            foreach (var i in jsize)
            {
                Thread.Sleep(1);
                if (i.Value.ToString() == skuid)
                {
                    mn.Size = i.Key;
                    break;
                }
            }
        }
        protected void GetSKUID(string country, string pid, CancellationToken ct, CancellationTokenSource cts)
        {
            string productdetail = null;
            bool productdetailnull = false;
            string svalue = null;
            if (localsize.TryGetValue(mn.Region + pid, out svalue))
            {
                JObject sva = JObject.Parse(svalue);
                imageurl = sva["Image"].ToString();
                mn.imageurl= imageurl;
                productID = sva["ProductID"].ToString();
                limit = int.Parse(sva["limit"].ToString());
                msrp = sva["msrp"].ToString();
                JObject jo = JObject.Parse(sva["data"].ToString());
                jsize = jo;
                if (mn.Size.Contains("+") == false && mn.Size.Contains("-") == false && randomsize == false)
                {
                    foreach (var i in jo)
                    {
                        Thread.Sleep(1);
                        if (ct.IsCancellationRequested)
                        {
                            mn.Status = "IDLE";
                            ct.ThrowIfCancellationRequested();
                        }
                        if (i.Key == mn.Size)
                        {
                            skuid = i.Value.ToString();
                            break;
                        }
                    }
                }
                else if (randomsize)
                {
                    foreach (var i in jo)
                    {
                        Thread.Sleep(1);
                        if (ct.IsCancellationRequested)
                        {
                            mn.Status = "IDLE";
                            ct.ThrowIfCancellationRequested();
                        }
                        skuid = i.Value.ToString();
                        skuidlist.Add(i.Value.ToString());
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
                        foreach (var i in jo)
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
                    mn.Status = "Size Not available";
                    cts.Cancel();
                    Main.dic.Remove(mn.Taskid);
                    if (ct.IsCancellationRequested)
                    {
                        ct.ThrowIfCancellationRequested();
                    }
                }
                mn.Status = "Get Size";
            }
            else
            {
                try
                {
                    if (ct.IsCancellationRequested)
                    {
                        mn.Status = "IDLE";
                        ct.ThrowIfCancellationRequested();
                    }
                    var binding = new BasicHttpBinding();
                    var endpoint = new EndpointAddress(@"http://49.51.68.105/WebService1.asmx");
                    var factory = new ChannelFactory<ServiceReference2.WebService1Soap>(binding, endpoint);
                    var callClient = factory.CreateChannel();
                    JObject result = JObject.Parse(callClient.getproductAsync(Config.hwid, pid, mn.Region).Result);
                    try
                    {
                        Main.localsize.Add(mn.Region + pid, result.ToString());
                        jsize = (JObject)result["data"];
                    }
                    catch { }
                    imageurl = result["Image"].ToString();
                    mn.imageurl = imageurl;
                    productID = result["ProductID"].ToString();
                    limit = int.Parse(result["limit"].ToString());
                    msrp = result["msrp"].ToString();
                    JObject jo = JObject.Parse(result["data"].ToString());
                    if (mn.Size.Contains("+") == false && mn.Size.Contains("-") == false && randomsize == false)
                    {
                        foreach (var i in jo)
                        {
                            if (ct.IsCancellationRequested)
                            {
                                mn.Status = "IDLE";
                                ct.ThrowIfCancellationRequested();
                            }
                            if (i.Key == mn.Size)
                            {
                                skuid = i.Value.ToString();
                                break;
                            }
                        }
                    }
                    else if (randomsize)
                    {
                        foreach (var i in jo)
                        {
                            if (ct.IsCancellationRequested)
                            {
                                mn.Status = "IDLE";
                                ct.ThrowIfCancellationRequested();
                            }
                            skuid = i.Value.ToString();
                            skuidlist.Add(i.Value.ToString());
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
                            foreach (var i in jo)
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
                        mn.Status = "Size Not available";
                        cts.Cancel();
                        Main.dic.Remove(mn.Taskid);
                        if (ct.IsCancellationRequested)
                        {
                            ct.ThrowIfCancellationRequested();
                        }
                    }
                    mn.Status = "Get Size";
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
                A: try
                    {
                        Thread.Sleep(1);
                    retry: string url = "";
                        if (ct.IsCancellationRequested)
                        {
                            mn.Status = "IDLE";
                            ct.ThrowIfCancellationRequested();
                        }
                        if (country.Contains("UK"))
                        {
                            url = "https://api.nike.com/product_feed/threads/v2/?filter=marketplace(GB)&filter=language(en-GB)&filter=channelId(d9a5bc42-4b9c-4976-858a-f159cf99c647)&filter=publishedContent.properties.products.styleColor(" + mn.Sku + ")";
                        }
                        else
                        {
                            url = "https://api.nike.com/product_feed/threads/v2/?filter=marketplace(US)&filter=language(en)&filter=channelId(d9a5bc42-4b9c-4976-858a-f159cf99c647)&filter=publishedContent.properties.products.styleColor(" + mn.Sku + ")";
                        }
                        bool sizefind = false;
                        string sourcecode = MPAPI.GetHtmlsource(url, mn, ct);
                        JObject jo = JObject.Parse(sourcecode);
                        Thread.Sleep(1);
                        JArray ja = (JArray)JsonConvert.DeserializeObject(jo["objects"].ToString());
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
                        JArray jar = null;
                        try
                        {
                            jar = (JArray)JsonConvert.DeserializeObject(ja[0]["productInfo"].ToString());
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            mn.Status = "Monitoring";
                            if (Config.delay == "")
                            {
                                Thread.Sleep(1);
                            }
                            else
                            {
                                Thread.Sleep(int.Parse(Config.delay));
                            }
                            goto retry;
                        }
                        try
                        {
                            JObject j = JObject.Parse(jar[0].ToString());
                            if (mn.Region == "NikeUS")
                            {
                                msrp = j["merchPrice"]["msrp"].ToString();
                            }
                            else
                            {
                                msrp = j["merchPrice"]["fullPrice"].ToString();
                            }
                            try
                            {
                                imageurl = j["imageUrls"]["productImageUrl"].ToString();
                                mn.imageurl = imageurl;
                            }
                            catch
                            {
                            }
                            limit = int.Parse(j["merchProduct"]["quantityLimit"].ToString());
                            JArray jsku = (JArray)JsonConvert.DeserializeObject(j["skus"].ToString());
                            string availableSkus = j["availableSkus"].ToString();
                            JArray jas = (JArray)JsonConvert.DeserializeObject(availableSkus);
                            for (int i = 0; i < jsku.Count; i++)
                            {
                                Thread.Sleep(1);
                                allsize.Add(jsku[i]["nikeSize"].ToString(), jsku[i]["id"].ToString());
                                if (randomsize)
                                {
                                    skuidlist.Add(jsku[i]["id"].ToString());
                                    productID = jsku[i]["productId"].ToString();
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
                                            productID = jsku[i]["productId"].ToString();
                                        }
                                    }
                                }
                                else
                                {
                                    if (size.ToString() == jsku[i]["nikeSize"].ToString())
                                    {
                                        skuid = jsku[i]["id"].ToString();
                                        productID = jsku[i]["productId"].ToString();
                                    }
                                }
                            }
                            if (sizefind)
                            {
                                Random ra = new Random();
                                skuid = skuidlist[ra.Next(0, skuidlist.Count)].ToString();
                            }
                        }
                        catch (NullReferenceException)
                        {
                            mn.Status = "Size Error";
                            goto retry;
                        }
                        if (ct.IsCancellationRequested)
                        {
                            mn.Status = "IDLE";
                            ct.ThrowIfCancellationRequested();
                        }
                        if (skuid == "")
                        {
                            mn.Status = "Size Not available";
                            goto retry;
                        }
                    }
                    catch (NullReferenceException)
                    {
                        Thread.Sleep(1);
                        goto A;
                    }
                }
            }
            try
            {
                if (productdetailnull)
                {
                    if (ct.IsCancellationRequested)
                    {
                        mn.Status = "IDLE";
                        ct.ThrowIfCancellationRequested();
                    }
                    productdetail = "{\"data\":" + JsonConvert.SerializeObject(allsize) + ",\"Image\":\"" + imageurl + "\",\"ProductID\":\"" + productID + "\",\"limit\":\"" + limit + "\",\"msrp\":\"" + msrp + "\"}";
                    jsize = (JObject)JObject.Parse(productdetail)["data"];
                    var binding = new BasicHttpBinding();
                    var endpoint = new EndpointAddress(@"http://49.51.68.105/WebService1.asmx");
                    var factory = new ChannelFactory<ServiceReference2.WebService1Soap>(binding, endpoint);
                    var callClient = factory.CreateChannel();
                    callClient.setproductAsync(pid, mn.Region, productdetail);
                    foreach (var i in jsize)
                    {
                        Thread.Sleep(1);
                        if (i.Value.ToString() == skuid)
                        {
                            mn.Size = i.Key;
                            break;
                        }
                    }
                }
            }
            catch
            {

            }
        }
    }
}

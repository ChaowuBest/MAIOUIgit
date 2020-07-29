using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using static MAIO.Main;

namespace MAIO
{
    class NikeUSUK
    {
        public bool randomsize = false;
        public bool monitortask = false;
        public string size = "";
        public string pid = "";
        public string profile = "";
        public taskset tk = null;
        public string code = "";
        
        string GID = Guid.NewGuid().ToString();
        public string skuid = "";
        public string productID = "";
        string cardguid = "";
        string msrp = "";
        int index = 0;
        int limit = 0;
        int quantity = 0;
        public string username = "";
        public string password = "";
        public string giftcard = "";
        Dictionary<string, string> giftcard2 = new Dictionary<string, string>();
        NikeUSUKAPI USUKAPI = new NikeUSUKAPI();
        ArrayList skuidlist = new ArrayList();
        private static char[] constant = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
        private static char[] num = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
        public void StartTask(CancellationToken ct)
        {
        A:
            try
            {
                try
                {
                    quantity=int.Parse(tk.Quantity);
                    GetSKUID(tk.Tasksite.Replace("Nike", ""), pid, ct);
                }
                catch (NullReferenceException)
                {
                    goto A;
                }

            B: JObject joprofile = JObject.Parse(profile);
                string Authorization = "";
                try
                {
                    Authorization = Login(joprofile, ct);
                }
                catch (NullReferenceException)
                {
                    tk.Status = "Login Error";
                    tk.Status = "Retrying";
                    goto B;
                }
            C:
                if (giftcard == "")
                {
                    try
                    {
                        Submitcardinfo(Authorization, skuid, ct);
                    }
                    catch (NullReferenceException)
                    {
                        goto C;
                    }

                }
            D:
                try
                {
                    Checkoutpreview(Authorization, skuid, joprofile, ct);
                }
                catch (NullReferenceException)
                {
                    goto D;
                }

            E:
                try
                {
                    CheckoutpreviewStatus(Authorization, skuid, ct);

                }
                catch (NullReferenceException)
                {
                    goto E;
                }

                if (giftcard != "")
                {

                    subimitgiftcard(Authorization, skuid, ct);
                }
                try
                {
                    string id = PaymentPreviw(Authorization, skuid, joprofile, ct);
                    PreviewJob(id, Authorization, skuid, ct);
                    paymenttoken(Authorization, id, skuid, joprofile, ct);
                    finalorder(Authorization, ct, joprofile);
                }
                catch (NullReferenceException)
                {
                }
            }
            catch (OperationCanceledException)
            {
                return;
            }


        }
        protected static string GenerateRandomnum(int length)
        {
            string checkCode = string.Empty;
            Random rd = new Random();
            for (int i = 0; i < length; i++)
            {
                checkCode += num[rd.Next(10)].ToString();
            }
            return checkCode;
        }
        protected static string GenerateRandomString(int length)
        {
            string checkCode = string.Empty;
            Random rd = new Random();
            for (int i = 0; i < length; i++)
            {
                checkCode += constant[rd.Next(26)].ToString();
            }
            return checkCode;
        }
        protected void GetSKUID(string country, string pid, CancellationToken ct)
        {
            if (skuid != "" && productID != "")
            {
            }
            else
            {
            retry: string url = "";
                if (ct.IsCancellationRequested)
                {
                    tk.Status = "IDLE";
                    ct.ThrowIfCancellationRequested();
                }
                if (country.Contains("UK"))
                {
                    url = "https://api.nike.com/product_feed/threads/v2/?filter=marketplace(GB)&filter=language(en-GB)&filter=channelId(d9a5bc42-4b9c-4976-858a-f159cf99c647)&filter=publishedContent.properties.products.styleColor(" + tk.Sku + ")";
                }
                else
                {
                    url = "https://api.nike.com/product_feed/threads/v2/?filter=marketplace(US)&filter=language(en)&filter=channelId(d9a5bc42-4b9c-4976-858a-f159cf99c647)&filter=publishedContent.properties.products.styleColor(" + tk.Sku + ")";
                }
                bool sizefind = false;
                string sourcecode = USUKAPI.GetHtmlsource(url, tk, ct);
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
                try
                {
                    product = ja[0]["productInfo"].ToString();
                }
                catch (ArgumentOutOfRangeException)
                {
                    tk.Status = "Monitoring";
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

                    JArray jar = (JArray)JsonConvert.DeserializeObject(product);
                    JObject j = JObject.Parse(jar[0].ToString());
                    string skuids = j["skus"].ToString();
                    msrp = j["merchPrice"]["msrp"].ToString();
                    limit = int.Parse(j["merchProduct"]["quantityLimit"].ToString());
                    JArray jsku = (JArray)JsonConvert.DeserializeObject(skuids);
                    string availableSkus = j["availableSkus"].ToString();
                    JArray jas = (JArray)JsonConvert.DeserializeObject(availableSkus);
                    for (int i = 0; i < jsku.Count; i++)
                    {
                        if (randomsize)
                        {
                            skuidlist.Add(jsku[i]["id"].ToString());
                            productID = jsku[i]["productId"].ToString();
                            sizefind = true;

                        }
                        else
                        {
                            if (size.ToString() == jsku[i]["nikeSize"].ToString())
                            {
                                skuid = jsku[i]["id"].ToString();
                                productID = jsku[i]["productId"].ToString();
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
                                if (Config.Usemonitor == "True")
                                {
                                    string monitorurl = "https://api.nike.com/deliver/available_skus/v1?filter=productIds(" + productID + ")";
                                    string[] group = USUKAPI.Monitoring(monitorurl, tk, ct, skuid, randomsize);
                                    if (group[0] != null)
                                    {
                                        skuid = group[0];
                                    }
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
                                                    if (tk.Tasksite == "NikeUS" || tk.Tasksite == "NikeUK")
                                                    {
                                                        if (Mainwindow.tasklist[tk.Taskid] != "")
                                                        {
                                                            JObject jo = JObject.Parse(Mainwindow.tasklist[tk.Taskid]);
                                                            giftcard = jo["giftcard"].ToString();
                                                            code = jo["Code"].ToString().Replace("\r\n", "");
                                                        }
                                                        Random ran = new Random();
                                                        int random = ran.Next(0, Mainwindow.listaccount.Count);
                                                        try
                                                        {
                                                            string[] account = null;
                                                            if (Mainwindow.listaccount[random].Contains("-"))
                                                            {
                                                                account = Mainwindow.listaccount[random].Split("-");
                                                            }
                                                            else if (Mainwindow.listaccount[random].Contains(":"))
                                                            {
                                                                account = Mainwindow.listaccount[random].Split(":");
                                                            }
                                                            NikeUSUK NSK = new NikeUSUK();
                                                            NSK.monitortask = false;
                                                            NSK.giftcard = giftcard;
                                                            NSK.pid = tk1.Sku;
                                                            NSK.size = tk1.Size;
                                                            NSK.code = code;
                                                            NSK.profile = Mainwindow.allprofile[tk.Profile];
                                                            NSK.tk = tk1;
                                                            NSK.username = account[0];
                                                            NSK.password = account[1];
                                                            if (tk1.Size == "RA" || tk1.Size == "ra")
                                                            {
                                                                NSK.randomsize = true;
                                                            }
                                                            var cts = new CancellationTokenSource();
                                                            var ct = cts.Token;
                                                            Task task2 = new Task(() => { NSK.StartTask(ct); }, ct);
                                                            dic.Add(tk1.Taskid, cts);
                                                            task2.Start();
                                                        }
                                                        catch (Exception ex)
                                                        {

                                                            tk.Status = "No Account";
                                                        }
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
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                catch (NullReferenceException)
                {
                    tk.Status = "Size Error";
                    goto retry;
                }
                if (ct.IsCancellationRequested)
                {
                    tk.Status = "IDLE";
                    ct.ThrowIfCancellationRequested();
                }
                if (skuid == "")
                {
                    tk.Status = "Size Not available";
                    tk.Status = "Restarting";
                    goto retry;
                }
            }
        }
        protected string Login(JObject profile, CancellationToken ct)
        {
            string Authorization = "";
            if (profile["Address1"].ToString().Contains("%char4%"))
            {
                Regex regex = new Regex(@"%char4%");
                profile["Address1"] = regex.Replace(profile["Address1"].ToString(), GenerateRandomString(4));
            }
            if (profile["Address1"].ToString().Contains("%num4%"))
            {
                Regex regex = new Regex(@"%num4%");
                profile["Address1"] = regex.Replace(profile["Address1"].ToString(), GenerateRandomnum(4));
            }
            if (profile["Address2"].ToString().Contains("%char4%"))
            {
                Regex regex = new Regex(@"%char4%");
                profile["Address2"] = regex.Replace(profile["Address2"].ToString(), GenerateRandomString(4));
            }
            if (profile["Tel"].ToString().Contains("%num4%"))
            {
                Regex regex = new Regex(@"%num4%");
                profile["Tel"] = regex.Replace(profile["Tel"].ToString(), GenerateRandomnum(4));
            }
            if (profile["FirstName"].ToString().Contains("%fname%"))
            {
                Regex regex = new Regex(@"%fname%");
                Firstname fs = new Firstname();
                profile["FirstName"] = regex.Replace(profile["FirstName"].ToString(), fs.name());
            }
            if (profile["LastName"].ToString().Contains("%lname%"))
            {
                Regex regex = new Regex(@"%lname%");
                Firstname fs = new Firstname();
                profile["LastName"] = regex.Replace(profile["LastName"].ToString(), fs.name());
            }
            var chao = profile["EmailAddress"];
            if (profile["EmailAddress"].ToString().Contains("random"))
            {
                Regex regex = new Regex(@"random");
                Firstname fs = new Firstname();
                profile["EmailAddress"] = regex.Replace(profile["EmailAddress"].ToString(), GenerateRandomString(4));
            }
        A: if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "refreshtoken.json"))
            {
                try
                {
                    string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "refreshtoken.json";
                    FileInfo fi = new FileInfo(path);
                    if (fi.Length == 0)
                    {
                        bool isrefresh = false;
                        string loginurl = "https://unite.nike.com/login?appVersion=630&experienceVersion=528&uxid=com.nike.commerce.snkrs.web&locale=en_US&backendEnvironment=identity&browser=Google%20Inc.&os=undefined&mobile=false&native=false&visit=1&visitor=" + GID;
                        string logininfo = "{\"username\":\"" + username + "\",\"password\":\"" + password + "\",\"client_id\":\"PbCREuPr3iaFANEDjtiEzXooFl7mXGQ7\",\"ux_id\":\"com.nike.commerce.snkrs.web\",\"grant_type\":\"password\"}";
                        Authorization = USUKAPI.Postlogin(loginurl, logininfo, isrefresh, username, tk, ct);
                    }
                    else
                    {
                        FileStream fs1 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                        StreamReader sr = new StreamReader(fs1);
                        string read = sr.ReadToEnd();
                        JArray ja = JArray.Parse(read);
                        string token = "";
                        foreach (var i in ja)
                        {
                            if (i.ToString().Contains(username))
                            {
                                token = JObject.Parse(i.ToString())["Token"].ToString();
                                break;
                            }
                        }
                        if (ct.IsCancellationRequested)
                        {
                            tk.Status = "IDLE";
                            ct.ThrowIfCancellationRequested();
                        }
                        if (token != "")
                        {
                            bool isrefresh2 = true;
                            string refreshinfo = "{\"refresh_token\":\"" + token + "\",\"client_id\":\"PbCREuPr3iaFANEDjtiEzXooFl7mXGQ7\",\"grant_type\":\"refresh_token\"}";
                            string loginurl2 = "https://unite.nike.com/tokenRefresh?appVersion=630&experienceVersion=528&uxid=com.nike.commerce.snkrs.ios&locale=en_US&backendEnvironment=identity&browser=Apple%20Computer%2C%20Inc.&os=undefined&mobile=true&native=true&visit=1&visitor=" + GID;
                            Authorization = USUKAPI.Postlogin(loginurl2, refreshinfo, isrefresh2, username, tk, ct);
                        }
                        else
                        {
                            bool isrefresh = false;
                            string loginurl = "https://unite.nike.com/login?appVersion=630&experienceVersion=528&uxid=com.nike.commerce.snkrs.web&locale=en_US&backendEnvironment=identity&browser=Google%20Inc.&os=undefined&mobile=false&native=false&visit=1&visitor=" + GID;
                            string logininfo = "{\"username\":\"" + username + "\",\"password\":\"" + password + "\",\"client_id\":\"PbCREuPr3iaFANEDjtiEzXooFl7mXGQ7\",\"ux_id\":\"com.nike.commerce.snkrs.web\",\"grant_type\":\"password\"}";
                            Authorization = USUKAPI.Postlogin(loginurl, logininfo, isrefresh, username, tk, ct);
                        }

                    }
                }
                catch (IOException)
                {
                    tk.Status = "Login Error";
                    goto A;
                }
            }
            else
            {
                bool isrefresh = false;
                string loginurl = "https://unite.nike.com/login?appVersion=630&experienceVersion=528&uxid=com.nike.commerce.snkrs.web&locale=en_US&backendEnvironment=identity&browser=Google%20Inc.&os=undefined&mobile=false&native=false&visit=1&visitor=" + GID;
                string logininfo = "{\"username\":\"" + username + "\",\"password\":\"" + password + "\",\"client_id\":\"PbCREuPr3iaFANEDjtiEzXooFl7mXGQ7\",\"ux_id\":\"com.nike.commerce.snkrs.web\",\"grant_type\":\"password\"}";
                Authorization = USUKAPI.Postlogin(loginurl, logininfo, isrefresh, username, tk, ct);
            }
            return Authorization;
        }
        protected void Submitcardinfo(string Authorization, string skuid, CancellationToken ct)
        {
            cardguid = Guid.NewGuid().ToString();
            string cardurl = "";
            string cardinfo = "";

            JObject jo = JObject.Parse(profile);
            cardurl = "https://paymentcc.nike.com/creditcardsubmit/" + cardguid + "/store";
            string firstcard = jo["Cardnum"].ToString().Substring(0, 1);
            string cardtype = "";
            if (firstcard == "4")
            {
                cardtype = "VISA";
            }
            else if (firstcard == "5")
            {
                cardtype = "MASTERCARD";
            }
            else if (firstcard == "6")
            {
                cardtype = "DISCOVER";
            }
            else if (firstcard == "3")
            {
                cardtype = "AMERICANEXPRESS";
            }
            string[] expir = null;
            try
            {
                expir = jo["MMYY"].ToString().Split("/");
                expir[1] = "20" + expir[1];
            }
            catch
            {
                tk.Status = "Card Error";
            }
            cardinfo = "{\"accountNumber\":\"" + jo["Cardnum"].ToString() + "\",\"cardType\":\"" + cardtype + "\",\"expirationMonth\":\"" + expir[0] + "\",\"expirationYear\":\"" + expir[1] + "\",\"creditCardInfoId\":\"" + cardguid + "\",\"cvNumber\":\"" + jo["Cvv"].ToString() + "\"}";
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            USUKAPI.Postcardinfo(cardurl, cardinfo, Authorization, cardguid, tk, ct);
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }

        }
        protected void Checkoutpreview(string Authorization, string skuid, JObject jo, CancellationToken ct)
        {
            string checkoutsessionurl = "https://api.nike.com/buy/checkout_previews/v2/" + GID;
            string country = "";
            string currency = "";
            string locale = "";
            string shippingMethod = "";         
            if (jo["Country"].ToString() == "GB")
            {
                country = "GB";
                currency = "GBP";
                locale = "en_GB";
                shippingMethod = "GROUND_SERVICE";
            }
            else if (jo["Country"].ToString() == "US")
            {
                country = "US";
                currency = "USD";
                locale = "en_US";
                shippingMethod = "STANDARD";
            }
           if (int.Parse(tk.Quantity)>limit)
            {
                quantity = limit;
            }
            JObject payLoad = null;
            payLoad = new JObject(
            new JProperty("request",
             new JObject(
            new JProperty("country", country),
            new JProperty("currency", currency),
            new JProperty("email", jo["EmailAddress"].ToString()),
            new JProperty("locale", locale),
            new JProperty("channel", "SNKRS"),
            new JProperty("promotionCodes",
            new JArray()),
            new JProperty("items",
            new JArray(
                new JObject(
                new JProperty("id", productID),
                new JProperty("skuId", skuid),
                new JProperty("shippingMethod", shippingMethod),
                new JProperty("quantity", quantity),
                new JProperty("recipient",
                   new JObject(
                   new JProperty("firstName", jo["FirstName"].ToString()),
                   new JProperty("lastName", jo["LastName"].ToString()))),
                new JProperty("contactInfo",
                   new JObject(
                   new JProperty("phoneNumber", jo["Tel"].ToString()),
                   new JProperty("email", jo["EmailAddress"].ToString()))),
                new JProperty("shippingAddress",
                   new JObject(
                   new JProperty("address1", jo["Address1"].ToString()),
                   new JProperty("address2", jo["Address2"].ToString()),
                   new JProperty("city", jo["City"].ToString()),
                   new JProperty("postalCode", jo["Zipcode"].ToString()),
                   new JProperty("state", jo["State"].ToString()),
                   new JProperty("country", country),
                   new JProperty("email", jo["EmailAddress"].ToString()))))
                )))));
            if (code != "")
            {
                payLoad = new JObject(
                new JProperty("request",
                 new JObject(
                new JProperty("country", country),
                new JProperty("currency", currency),
                new JProperty("email", jo["EmailAddress"].ToString()),
                new JProperty("locale", locale),
                new JProperty("channel", "SNKRS"),
                new JProperty("promotionCodes",
                new JArray(code)),
                new JProperty("items",
                new JArray(
                    new JObject(
                    new JProperty("id", productID),
                    new JProperty("skuId", skuid),
                    new JProperty("shippingMethod", shippingMethod),
                    new JProperty("quantity", quantity),
                    new JProperty("recipient",
                       new JObject(
                       new JProperty("firstName", jo["FirstName"].ToString()),
                       new JProperty("lastName", jo["LastName"].ToString()))),
                    new JProperty("contactInfo",
                       new JObject(
                       new JProperty("phoneNumber", jo["Tel"].ToString()),
                       new JProperty("email", jo["EmailAddress"].ToString()))),
                    new JProperty("shippingAddress",
                       new JObject(
                       new JProperty("address1", jo["Address1"].ToString()),
                       new JProperty("address2", jo["Address2"].ToString()),
                       new JProperty("city", jo["City"].ToString()),
                       new JProperty("postalCode", jo["Zipcode"].ToString()),
                       new JProperty("state", jo["State"].ToString()),
                       new JProperty("country", country),
                       new JProperty("email", jo["EmailAddress"].ToString()))))
                    )))));
            }
            string checkoutpayload = "";
            try
            {
                checkoutpayload = payLoad.ToString();
            }
            catch (Exception)
            {
                tk.Status = "Bad Profile";
                Thread.Sleep(50000000);
            }
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            USUKAPI.CheckoutPreview(checkoutsessionurl, Authorization, checkoutpayload, GID, tk, ct);
        }
        protected void CheckoutpreviewStatus(string Authorization, string skuid, CancellationToken ct)
        {
            string url = "https://api.nike.com/buy/checkout_previews/v2/jobs/" + GID;
            bool isdiscount = false;
            if (code != "")
            {
                isdiscount = true;
            }
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            msrp = USUKAPI.CheckoutPreviewStatus(url, Authorization, isdiscount, tk, ct, profile, pid, size, code, giftcard, username, password, randomsize, productID, skuid);

            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
        }
        protected void subimitgiftcard(string Authorization, string skuid, CancellationToken ct)
        {
            int count = 0;
            double balance = 0;
            JObject jo = JObject.Parse(Mainwindow.giftcardlist[giftcard]);
            try
            {
                foreach (var i in jo)
                {
                    giftcard2.Add(i.Key, i.Value.ToString());
                    count++;
                }
            }
            catch (Exception)
            {
                tk.Status = "giftcard error";
            }
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            for (int i = 0; i < count; i++)
            {
                KeyValuePair<string, string> kv = giftcard2.ElementAt(i);
                string cardurl2 = "https://api.nike.com/payment/giftcard_balance/v1";
                string cardinfo2 = "{\"accountNumber\":\"" + kv.Key + "\",\"pin\":\"" + kv.Value + "\",\"currency\":\"USD\"}";
                balance += USUKAPI.Postcardinfo(cardurl2, cardinfo2, Authorization, cardguid, tk, ct);
                double msrpdouble = Convert.ToDouble(msrp);

                if (balance > msrpdouble)
                {
                    if (ct.IsCancellationRequested)
                    {
                        tk.Status = "IDLE";
                        ct.ThrowIfCancellationRequested();
                    }
                    index = i;
                    break;
                }
            }
        }
        ArrayList giftcardadd = new ArrayList();
        protected string PaymentPreviw(string Authorization, string skuid, JObject jo, CancellationToken ct)
        {
            string paymenturl = "https://api.nike.com/payment/preview/v2/";
            JObject payinfo = null;
            string country = "";
            string currency = "";
            string locale = "";
            string shippingMethod = "";
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            if (jo["Country"].ToString() == "GB")
            {
                country = "GB";
                currency = "GBP";
                locale = "en_GB";
                shippingMethod = "GROUND_SERVICE";
            }
            else if (jo["Country"].ToString() == "US")
            {
                country = "US";
                currency = "USD";
                locale = "en_US";
                shippingMethod = "STANDARD";
            }
            if (giftcard != "")
            {
                JObject payLoad = null;
                for (int i = 0; i < index + 1; i++)
                {
                    KeyValuePair<string, string> kv = giftcard2.ElementAt(i);
                    payLoad = new JObject(
                  new JObject(
                  new JProperty("id", Guid.NewGuid().ToString()),
                  new JProperty("type", "GiftCard"),
                  new JProperty("accountNumber", kv.Key),
                  new JProperty("giftCardPin", kv.Value),
                  new JProperty("billingInfo",
                  new JObject(
                   new JProperty("name",
                   new JObject(
                       new JProperty("firstName", jo["FirstName"].ToString()),
                       new JProperty("lastName", jo["LastName"].ToString()))),
                   new JProperty("address",
                   new JObject(
                       new JProperty("address1", jo["Address1"].ToString()),
                       new JProperty("address2", jo["Address2"].ToString()),
                       new JProperty("city", jo["City"].ToString()),
                       new JProperty("postalCode", jo["Zipcode"].ToString()),
                       new JProperty("state", jo["State"].ToString()),
                       new JProperty("country", country))),
                    new JProperty("contactInfo",
                    new JObject(
                         new JProperty("phoneNumber", jo["Tel"].ToString()),
                         new JProperty("email", jo["EmailAddress"].ToString())))))
                    ));
                    giftcardadd.Add(payLoad);
                }
                payinfo = new JObject(
            new JProperty("checkoutId", GID),
            new JProperty("total", msrp),
            new JProperty("currency", currency),
            new JProperty("country", country),
            new JProperty("items",
             new JArray(
              new JObject(
              new JProperty("productId", productID),
              new JProperty("shippingAddress",
                 new JObject(
                 new JProperty("address1", jo["Address1"].ToString()),
                 new JProperty("address2", jo["Address2"].ToString()),
                 new JProperty("city", jo["City"].ToString()),
                 new JProperty("state", jo["State"].ToString()),
                 new JProperty("postalCode", jo["Zipcode"].ToString()),
                 new JProperty("country", country),
                 new JProperty("email", jo["EmailAddress"].ToString()),
                 new JProperty("phoneNumber", jo["Tel"].ToString())))))),
              new JProperty("paymentInfo",
               new JArray(giftcardadd)));
            }
            else
            {
                payinfo = new JObject(
new JProperty("checkoutId", GID),
new JProperty("total", msrp),
new JProperty("currency", currency),
new JProperty("country", country),
new JProperty("items",
new JArray(
new JObject(
new JProperty("productId", productID),
new JProperty("shippingAddress",
new JObject(
new JProperty("address1", jo["Address1"].ToString()),
new JProperty("address2", jo["Address2"].ToString()),
new JProperty("city", jo["City"].ToString()),
new JProperty("state", jo["State"].ToString()),
new JProperty("postalCode", jo["Zipcode"].ToString()),
new JProperty("country", country),
new JProperty("email", jo["EmailAddress"].ToString()),
new JProperty("phoneNumber", jo["Tel"].ToString())))))),
new JProperty("paymentInfo",
new JArray(
new JObject(
new JProperty("id", cardguid),
new JProperty("type", "CreditCard"),
new JProperty("creditCardInfoId", cardguid),
new JProperty("billingInfo",
new JObject(
new JProperty("name",
new JObject(
   new JProperty("firstName", jo["FirstName"].ToString()),
   new JProperty("lastName", jo["LastName"].ToString()))),
new JProperty("address",
new JObject(
   new JProperty("address1", jo["Address1"].ToString()),
   new JProperty("address2", jo["Address2"].ToString()),
   new JProperty("city", jo["City"].ToString()),
   new JProperty("postalCode", jo["Zipcode"].ToString()),
   new JProperty("state", jo["State"].ToString()),
   new JProperty("country", country))),
new JProperty("contactInfo",
new JObject(
     new JProperty("phoneNumber", jo["Tel"].ToString()),
     new JProperty("email", jo["EmailAddress"].ToString())))))
))));
            }
            string paymentinfo = payinfo.ToString();
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            string id = USUKAPI.payment(paymenturl, Authorization, paymentinfo, tk, ct);
            return id;
        }
        protected void PreviewJob(string id, string Authorization, string skuid, CancellationToken ct)
        {
            string url = "https://api.nike.com/payment/preview/v2/jobs/" + id;
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            USUKAPI.paymentjob(url, Authorization, ct, tk);
        }
        protected void paymenttoken(string Authorization, string id, string skuid, JObject jo, CancellationToken ct)
        {
            string url = "https://api.nike.com/buy/checkouts/v2/" + GID;
            string country = "";
            string currency = "";
            string locale = "";
            string shippingMethod = "";
            if (jo["Country"].ToString() == "UK")
            {
                country = "GB";
                currency = "GBP";
                locale = "en_GB";
                shippingMethod = "GROUND_SERVICE";
            }
            else if (jo["Country"].ToString() == "US")
            {
                country = "US";
                currency = "USD";
                locale = "en_US";
                shippingMethod = "STANDARD";
            }
            JObject payinfo = null;
            payinfo = new JObject(
new JProperty("request",
new JObject(
new JProperty("country", country),
new JProperty("currency", currency),
new JProperty("email", jo["EmailAddress"].ToString()),
new JProperty("locale", locale),
new JProperty("paymentToken", id),
new JProperty("channel", "SNKRS"),
new JProperty("promotionCodes",
new JArray()),
new JProperty("items",
new JArray(
new JObject(
new JProperty("id", productID),
new JProperty("skuId", skuid),
new JProperty("shippingMethod", shippingMethod),
new JProperty("quantity", quantity),
new JProperty("recipient",
  new JObject(
  new JProperty("firstName", jo["FirstName"].ToString()),
  new JProperty("lastName", jo["LastName"].ToString()))),
new JProperty("contactInfo",
  new JObject(
  new JProperty("phoneNumber", jo["Tel"].ToString()),
  new JProperty("email", jo["EmailAddress"].ToString()))),
new JProperty("shippingAddress",
  new JObject(
  new JProperty("address1", jo["Address1"].ToString()),
  new JProperty("address2", jo["Address2"].ToString()),
  new JProperty("city", jo["City"].ToString()),
  new JProperty("state", jo["State"].ToString()),
  new JProperty("postalCode", jo["Zipcode"].ToString()),
  new JProperty("country", country),
  new JProperty("email", jo["EmailAddress"].ToString()))))
)))));
            if (code != "")
            {
                payinfo = new JObject(
 new JProperty("request",
 new JObject(
 new JProperty("country", country),
 new JProperty("currency", currency),
 new JProperty("email", jo["EmailAddress"].ToString()),
 new JProperty("locale", locale),
 new JProperty("paymentToken", id),
 new JProperty("channel", "SNKRS"),
 new JProperty("promotionCodes",
 new JArray(code)),
 new JProperty("items",
 new JArray(
 new JObject(
 new JProperty("id", productID),
 new JProperty("skuId", skuid),
 new JProperty("shippingMethod", shippingMethod),
 new JProperty("quantity", quantity),
 new JProperty("recipient",
 new JObject(
 new JProperty("firstName", jo["FirstName"].ToString()),
 new JProperty("lastName", jo["LastName"].ToString()))),
 new JProperty("contactInfo",
 new JObject(
 new JProperty("phoneNumber", jo["Tel"].ToString()),
 new JProperty("email", jo["EmailAddress"].ToString()))),
 new JProperty("shippingAddress",
 new JObject(
 new JProperty("address1", jo["Address1"].ToString()),
 new JProperty("address2", jo["Address2"].ToString()),
 new JProperty("city", jo["City"].ToString()),
 new JProperty("state", jo["State"].ToString()),
 new JProperty("postalCode", jo["Zipcode"].ToString()),
 new JProperty("country", country),
 new JProperty("email", jo["EmailAddress"].ToString()))))
 )))));
            }
            string paytoken = payinfo.ToString();
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            USUKAPI.final(Authorization, url, paytoken, GID, tk, ct);
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }

        }
        protected void finalorder(string Authorization, CancellationToken ct, JObject joprofile)
        {
            string url = "https://api.nike.com/buy/checkouts/v2/jobs/" + GID;
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            string status = USUKAPI.finalorder(url, Authorization, profile, tk, pid, size, code, giftcard, username, password, randomsize, ct, productID, skuid);

            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            if (status.Contains("error") == true)
            {
                JObject jo = JObject.Parse(status);
                string obejects = jo["error"].ToString();
                JObject jo2 = JObject.Parse(obejects);
                string reason = jo2["message"].ToString();
                tk.Status = reason;
            }
            else
            {
                JObject jo3 = JObject.Parse(status);
                string orderid = jo3["response"]["orderId"].ToString();
                string pd2 = "{\"username\":\"MAIO\",\"avatar_url\":\"https://i.loli.net/2020/05/24/VfWKsEywcXZou1T.jpg\",\"embeds\":[{\"title\":\"You Just Chekcout!\",\"color\":3329330,\"footer\":{\"text\":\"" + "MAIO" + DateTime.Now.ToLocalTime().ToString() + "\",\"icon_url\":\"https://i.loli.net/2020/05/24/VfWKsEywcXZou1T.jpg\"},\"fields\":[{\"name\":\"Checkout out!!!\",\"value\":\"" + pid + "\\t\\t\\t\\tSize:" + size + "\\t\\t\\t\\tOrder id:" + orderid + "Email Address:" + joprofile["EmailAddress"].ToString() + "\",\"inline\":false}]}]}";
                string webhook2 = "https://discordapp.com/api/webhooks/736544382018125895/Ti5zEbTcrKALkWhAePivSfyi7jlhRmRlILEyx9bPKIYh63qu1dVBDB2FFeyMFTSuRnpt";
                string pd3 = "{\"username\":\"MAIO\",\"avatar_url\":\"https://i.loli.net/2020/05/24/VfWKsEywcXZou1T.jpg\",\"embeds\":[{\"title\":\"You Just Chekcout!\",\"color\":3329330,\"footer\":{\"text\":\"" + "MAIO" + DateTime.Now.ToLocalTime().ToString() + "\",\"icon_url\":\"https://i.loli.net/2020/05/24/VfWKsEywcXZou1T.jpg\"},\"fields\":[{\"name\":\"Checkout out!!!\",\"value\":\"" + pid + "\\t\\t\\t\\tSize:" + size + "\\t\\t\\t\\tSite:"+tk.Tasksite+"\",\"inline\":false}]}]}";
                if (Config.webhook == "")
                {
                    tk.Status = "Success";
                    Http(webhook2, pd3, tk);
                }
                if (ct.IsCancellationRequested)
                {
                    tk.Status = "IDLE";
                    ct.ThrowIfCancellationRequested();
                }
                if (status.Contains("error") == false)
                {
                    Http(Config.webhook, pd2, tk);
                    Http(webhook2, pd3, tk);
                    tk.Status = "Check Webhook";
                }
            }

        }
        public void Http(string url, string postDataStr, Main.taskset tk)
        {
        Retry: Random ra = new Random();
            int sleeptime = ra.Next(0, 3000);
            Thread.Sleep(sleeptime);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json; charset=utf-8";
            request.Method = "post";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
            byte[] bytes = Encoding.UTF8.GetBytes(postDataStr);
            request.ContentLength = bytes.Length;
            Stream webstream = request.GetRequestStream();
            webstream.Write(bytes, 0, bytes.Length);
            webstream.Close();
            try
            {
                HttpWebResponse httpWebResponse = (HttpWebResponse)request.GetResponse();
                //   StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.UTF8);
                //   string result = streamReader.ReadToEnd();
            }
            catch (WebException ex)
            {
                Thread.Sleep(2000);
                tk.Status = ex.Message.ToString();
                goto Retry;
            }

        }

    }
}


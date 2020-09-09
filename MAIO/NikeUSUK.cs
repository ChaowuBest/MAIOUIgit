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
        string paytoken = "";
        public string username = "";
        public string imageurl = "";
        public string password = "";
        public string giftcard = "";
        public static bool subcard = false;
        Dictionary<string, string> giftcard2 = new Dictionary<string, string>();
        NikeUSUKAPI USUKAPI = new NikeUSUKAPI();
        ArrayList skuidlist = new ArrayList();
        private static char[] constant = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        private static char[] num = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
        public void StartTask(CancellationToken ct)
        {
            bool monitortask = false;
            bool ismonitor = false;
            try
            {
                if (ct.IsCancellationRequested)
                {
                    tk.Status = "IDLE";
                    ct.ThrowIfCancellationRequested();
                }
                if (tk.monitortask != "True")
                {
                D: for (int n = 0; n < Mainwindow.task.Count; n++)
                    {
                        Thread.Sleep(1);
                        if (Mainwindow.task[n].monitortask == "True" && Mainwindow.task[n].Tasksite == this.tk.Tasksite && Mainwindow.task[n].Status!="IDLE"&& Mainwindow.task[n].Sku==this.pid)
                        {
                            if (ct.IsCancellationRequested)
                            {
                                tk.Status = "IDLE";
                                ct.ThrowIfCancellationRequested();
                            }
                            tk.Status = "Monitoring Task";
                            monitortask = true;
                            if (Mainwindow.task[n].Status.Contains("WaitingRestock")==false || Mainwindow.task[n].Status.Contains("Proxy Error") ==false )
                            {
                                ismonitor = true;
                                this.tk.Sku = Mainwindow.task[n].Sku;
                                this.pid = this.tk.Sku;
                                break;
                            }
                        }
                    }
                    if (monitortask && ismonitor == false)
                    {
                        goto D;
                    }
                    else
                    {
                        Thread.Sleep(1);
                    }
                }
            }
            catch
            {
            }
        A:
            try
            {
                try
                {
                    quantity = int.Parse(tk.Quantity);
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
                try
                {
                    if (giftcard == "")
                    {
                        Task task =Task.Run(() => Submitcardinfo(Authorization, skuid, ct));
                    }
                    else
                    {
                        Task task = Task.Run(() => subimitgiftcard(Authorization, skuid, ct));
                        task.Start();
                    }
                }
                catch (NullReferenceException)
                {
                    goto C;
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

                try
                {
                F: if (subcard)
                    {
                        string id = PaymentPreviw(Authorization, skuid, joprofile, ct);
                        PreviewJob(id, Authorization, skuid, ct);
                        paymenttoken(Authorization, id, skuid, joprofile, ct);
                    }
                    else
                    {
                        goto F;
                    }
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
            Thread.Sleep(1);
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
                Thread.Sleep(1);
                JArray ja = (JArray)JsonConvert.DeserializeObject(obejects);
                bool multisize = false;
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
                if(multisize)
                {
                    size = size.Remove(size.Length - 1);
                }
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
                    if (tk.Tasksite == "NikeUS")
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
                    }
                    catch
                    {
                    }
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
                                    string monitorurl = "https://api.nike.com/cic/grand/v1/graphql";
                                    string info = "{\"hash\":\"ef571ff0ac422b0de43ab798cc8ff25f\",\"variables\":{\"ids\":[\"" + skuid + "\"],\"country\":\"US\",\"language\":\"en-US\",\"isSwoosh\":false}}";
                                    string[] group = USUKAPI.Monitoring(monitorurl, tk, ct, info, randomsize, skuid, false, multisize, skuidlist);
                                    if (group[0] != null)
                                    {
                                        skuid = group[0];
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
            Thread.Sleep(1);
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
                        string loginurl = null;
                        if (tk.Tasksite.Contains("UK"))
                        {
                            loginurl = "https://unite.nike.com/login?appVersion=805&experienceVersion=805&uxid=com.nike.commerce.snkrs.web&locale=en_GB&backendEnvironment=identity&browser=Google%20Inc.&os=undefined&mobile=false&native=false&visit=1&visitor=" + GID;
                        }
                        else
                        {
                            loginurl = "https://unite.nike.com/login?appVersion=805&experienceVersion=805&uxid=com.nike.commerce.snkrs.web&locale=en_US&backendEnvironment=identity&browser=Google%20Inc.&os=undefined&mobile=false&native=false&visit=1&visitor=" + GID;
                        }
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
                        for (int i = 0; i < ja.Count; i++)
                        {
                            if (ja[i]["Account"].ToString().ToUpper() == username.ToUpper())
                            {
                                token = ja[i]["Token"].ToString();
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
                            string loginurl2 = null;
                            if (tk.Tasksite.Contains("US"))
                            { 
                                loginurl2 = "https://unite.nike.com/tokenRefresh?appVersion=805&experienceVersion=805&uxid=com.nike.commerce.nikedotcom.web&locale=en_US&backendEnvironment=identity&browser=Google%20Computer%2C%20Inc.&os=undefined&mobile=true&native=true&visit=1&visitor=" + GID;
                            }
                            else
                            {
                                loginurl2 = "https://unite.nike.com/tokenRefresh?appVersion=805&experienceVersion=805&uxid=com.nike.commerce.nikedotcom.web&locale=en_GB&backendEnvironment=identity&browser=Google%20Computer%2C%20Inc.&os=undefined&mobile=true&native=true&visit=1&visitor=" + GID;
                            }
                            Authorization = USUKAPI.Postlogin(loginurl2, refreshinfo, isrefresh2, username, tk, ct);
                        }
                        else
                        {
                            bool isrefresh = false;
                            string loginurl = null;
                            if (tk.Tasksite.Contains("UK"))
                            {
                                loginurl = "https://unite.nike.com/login?appVersion=805&experienceVersion=805&uxid=com.nike.commerce.snkrs.web&locale=en_GB&backendEnvironment=identity&browser=Google%20Inc.&os=undefined&mobile=false&native=false&visit=1&visitor=" + GID;
                            }
                            else
                            {
                                loginurl = "https://unite.nike.com/login?appVersion=805&experienceVersion=805&uxid=com.nike.commerce.snkrs.web&locale=en_US&backendEnvironment=identity&browser=Google%20Inc.&os=undefined&mobile=false&native=false&visit=1&visitor=" + GID;
                            }
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
                string loginurl = "https://unite.nike.com/login?appVersion=805&experienceVersion=805&uxid=com.nike.commerce.snkrs.web&locale=en_US&backendEnvironment=identity&browser=Google%20Inc.&os=undefined&mobile=false&native=false&visit=1&visitor=" + GID;
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
            Thread.Sleep(1);
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
            subcard = true;
        }
        protected void Checkoutpreview(string Authorization, string skuid, JObject jo, CancellationToken ct)
        {
            Thread.Sleep(1);
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
            if (int.Parse(tk.Quantity) > limit)
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
                   new JProperty("country", country))))
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
                       new JProperty("country", country))))
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
            Thread.Sleep(1);
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
            Thread.Sleep(1);
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
                string currency = null;
                if (tk.Tasksite.Contains("UK"))
                {
                    currency = "GBP";
                }
                else
                {
                    currency = "USD";
                }
                string cardinfo2 = "{\"accountNumber\":\"" + kv.Key + "\",\"pin\":\"" + kv.Value + "\",\"currency\":\"" + currency + "\"}";
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
            subcard = true;
        }
        ArrayList giftcardadd = new ArrayList();
        protected string PaymentPreviw(string Authorization, string skuid, JObject jo, CancellationToken ct)
        {
            Thread.Sleep(1);
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
new JProperty("postalCode", jo["Zipcode"].ToString()),
new JProperty("country", country)))))),
new JProperty("paymentInfo",
new JArray(
new JObject(
new JProperty("id", Guid.NewGuid().ToString()),
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
            Thread.Sleep(1);
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
            Thread.Sleep(1);
            string url = "https://api.nike.com/buy/checkouts/v2/" + GID;
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
  new JProperty("country", country))
  ))
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
 new JProperty("country", country))))
 )))));
            }
            paytoken = payinfo.ToString();
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            string status=USUKAPI.final(Authorization, url, paytoken, GID, tk, ct, id);
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            if (status.Contains("error") == true)
            {
                JObject jo3 = JObject.Parse(status);
                string obejects = jo["error"].ToString();
                JObject jo2 = JObject.Parse(obejects);
                string reason = jo2["message"].ToString();
                tk.Status = reason;
                if (Config.webhook != "")
                {
                    failcheckout(tk, Config.webhook, jo3, reason);
                }
                Main.autorestock(tk);
                if (ct.IsCancellationRequested)
                {
                    tk.Status = "IDLE";
                    ct.ThrowIfCancellationRequested();
                }
            }
            else
            {
                JObject jo3 = JObject.Parse(status);
                string orderid = jo3["response"]["orderId"].ToString();
                string webhook2 = "https://discordapp.com/api/webhooks/736544382018125895/Ti5zEbTcrKALkWhAePivSfyi7jlhRmRlILEyx9bPKIYh63qu1dVBDB2FFeyMFTSuRnpt";
                if (Config.webhook == "")
                {
                    tk.Status = "Success";
                    ProcessNotification(true, tk, webhook2, jo, "");
                }
                if (ct.IsCancellationRequested)
                {
                    tk.Status = "IDLE";
                    ct.ThrowIfCancellationRequested();
                }
                if (status.Contains("error") == false)
                {
                    ProcessNotification(true, tk, "https://discordapp.com/api/webhooks/517871792677847050/qry12HP2IqJQb2sAfSNBmpUmFPOdPsVXUYY2_yhDgckgznpeVtRpNbwvO1Oma6nMGeK9", jo, "");
                    ProcessNotification(false, tk, Config.webhook, jo, orderid);
                    tk.Status = "Success";
                }
            }
        }
        public void failcheckout(taskset tk, string webhookurl, JObject joprofile, string reson)
        {
            Thread.Sleep(1);
            JObject jobject = null;
            jobject = JObject.Parse("{\r\n\"username\": \"MAIO\",\"avatar_url\":\"https://i.loli.net/2020/05/24/VfWKsEywcXZou1T.jpg\",\r\n\"embeds\": [\r\n{\r\n\"title\": \"\",\"color\":16711680,\r\n\"description\": \"\",\r\n\"fields\": [\r\n{\r\n\"name\": \"Style Code\",\r\n\"value\": \"\",\r\n\"inline\": true\r\n},\r\n{\r\n\"name\": \"Size\",\r\n\"value\": \"\",\r\n\"inline\": true\r\n},\r\n{\r\n\"name\": \"Email\",\r\n\"value\": \"\",\r\n\"inline\": true\r\n}\r\n,\r\n{\r\n\"name\": \"Account\",\r\n\"value\": \"\",\r\n\"inline\": true\r\n}\r\n,{\r\n\"name\": \"Reason\",\r\n\"value\": \"\",\r\n\"inline\": false\r\n},\r\n{\r\n\"name\": \"Profile\",\r\n\"value\": \"\",\r\n\"inline\": true\r\n},{\r\n\"name\": \"Code\",\r\n\"value\": \"\",\r\n\"inline\": false\r\n}\r\n],\r\n\"thumbnail\": {\r\n\"url\": \"\"\r\n},\r\n\"footer\": {\r\n\"text\": \"MAIO" + DateTime.Now.ToLocalTime().ToString() + "\",\r\n\"icon_url\": \"https://i.loli.net/2020/05/24/VfWKsEywcXZou1T.jpg\"\r\n}\r\n}\r\n]\r\n}");
            jobject["embeds"][0]["title"] = "Failed Checkout!!!";
            if (tk.Size == null || tk.Size == "")
            {
                tk.Size = "RA";
            }
            jobject["embeds"][0]["fields"][0]["value"] = tk.Sku;
            jobject["embeds"][0]["fields"][1]["value"] = tk.Size;
            jobject["embeds"][0]["fields"][2]["value"] = joprofile["EmailAddress"].ToString();
            jobject["embeds"][0]["fields"][3]["value"] = username;
            jobject["embeds"][0]["fields"][4]["value"] = reson;
            jobject["embeds"][0]["fields"][5]["value"] = tk.Profile;
            if (code == "" || code == null)
            {
                jobject["embeds"][0]["fields"][6]["value"] = "null";
            }
            else
            {
                jobject["embeds"][0]["fields"][6]["value"] = code;
            }
            jobject["embeds"][0]["thumbnail"]["url"] = imageurl;

            Http(webhookurl, jobject.ToString(), tk);
        }
        public void ProcessNotification(bool publicsuccess, taskset tk, string webhookurl, JObject joprofile, string orderid)
        {
            Thread.Sleep(1);
            JObject jobject = null;
            if (publicsuccess)
            {
                jobject = JObject.Parse("{\r\n    \"username\": \"MAIO\",\"avatar_url\":\"https://i.loli.net/2020/05/24/VfWKsEywcXZou1T.jpg\",\r\n    \"embeds\": [\r\n        {\r\n            \"title\": \"\",\"color\":3329330,\r\n            \"description\": \"\",\r\n            \"fields\": [\r\n                              {\r\n                    \"name\": \"Style Code\",\r\n                    \"value\": \"\",\r\n                    \"inline\": true\r\n                },\r\n                {\r\n                    \"name\": \"Size\",\r\n                    \"value\": \"\",\r\n                    \"inline\": true\r\n                },\r\n                {\r\n                    \"name\": \"Site\",\r\n                    \"value\": \"\",\r\n                    \"inline\": true\r\n                }],\r\n            \"thumbnail\": {\r\n                \"url\": \"\"\r\n            },\r\n            \"footer\": {\r\n                \"text\": \"MAIO" + DateTime.Now.ToLocalTime().ToString() + "\",\r\n                \"icon_url\": \"https://i.loli.net/2020/05/24/VfWKsEywcXZou1T.jpg\"\r\n            }\r\n        }\r\n    ]\r\n}");
                jobject["embeds"][0]["title"] = "You Just Checkout!!!";
                if (tk.Size == null || tk.Size == "")
                {
                    tk.Size = "RA";
                }
                jobject["embeds"][0]["fields"][0]["value"] = tk.Sku;
                jobject["embeds"][0]["fields"][1]["value"] = tk.Size;
                jobject["embeds"][0]["fields"][2]["value"] = tk.Tasksite;
                jobject["embeds"][0]["thumbnail"]["url"] = imageurl;
            }
            else
            {
                jobject = JObject.Parse("{\r\n\"username\": \"MAIO\",\"avatar_url\":\"https://i.loli.net/2020/05/24/VfWKsEywcXZou1T.jpg\",\r\n\"embeds\": [\r\n{\r\n\"title\": \"\",\"color\":3329330,\r\n\"description\": \"\",\r\n\"fields\": [\r\n{\r\n\"name\": \"Style Code\",\r\n\"value\": \"\",\r\n\"inline\": true\r\n},\r\n{\r\n\"name\": \"Size\",\r\n\"value\": \"\",\r\n\"inline\": true\r\n},\r\n{\r\n\"name\": \"Email\",\r\n\"value\": \"\",\r\n\"inline\": true\r\n}\r\n,\r\n{\r\n\"name\": \"Account\",\r\n\"value\": \"\",\r\n\"inline\": true\r\n}\r\n,{\r\n\"name\": \"Orderid\",\r\n\"value\": \"\",\r\n\"inline\": false\r\n},\r\n{\r\n\"name\": \"Profile\",\r\n\"value\": \"\",\r\n\"inline\": true\r\n},{\r\n\"name\": \"Code\",\r\n\"value\": \"\",\r\n\"inline\": false\r\n}\r\n],\r\n\"thumbnail\": {\r\n\"url\": \"\"\r\n},\r\n\"footer\": {\r\n\"text\": \"MAIO" + DateTime.Now.ToLocalTime().ToString() + "\",\r\n\"icon_url\": \"https://i.loli.net/2020/05/24/VfWKsEywcXZou1T.jpg\"\r\n}\r\n}\r\n]\r\n}");
                jobject["embeds"][0]["title"] = "You Just Checkout!!!";
                if (tk.Size == null || tk.Size == "")
                {
                    tk.Size = "RA";
                }
                jobject["embeds"][0]["fields"][0]["value"] = tk.Sku;
                jobject["embeds"][0]["fields"][1]["value"] = tk.Size;
                jobject["embeds"][0]["fields"][2]["value"] = joprofile["EmailAddress"].ToString();
                jobject["embeds"][0]["fields"][3]["value"] = username;
                jobject["embeds"][0]["fields"][4]["value"] = orderid;
                jobject["embeds"][0]["fields"][5]["value"] = tk.Profile;
                if (code == "" || code == null)
                {
                    jobject["embeds"][0]["fields"][6]["value"] = "null";
                }
                else
                {
                    jobject["embeds"][0]["fields"][6]["value"] = code;
                }
                jobject["embeds"][0]["thumbnail"]["url"] = imageurl;
            }
            Http(webhookurl, jobject.ToString(), tk);
        }
        public void Http(string url, string postDataStr, Main.taskset tk)
        {
            Thread.Sleep(1);
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
            }
            catch (WebException ex)
            {
                Thread.Sleep(1000);
                tk.Status = ex.Message.ToString();
                goto Retry;
            }

        }

    }
}


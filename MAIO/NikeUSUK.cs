using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static MAIO.Main;

namespace MAIO
{
    class NikeUSUK
    {
        public bool randomsize = false;
        public bool guest = false;
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
        public string imageurl = "";
        public string password = "";
        public string giftcard = "";
        public static bool subcard = false;
        DateTime dt = DateTime.UtcNow;
        Dictionary<string, string> giftcard2 = new Dictionary<string, string>();
        Dictionary<string, string> allsize = new Dictionary<string, string>();
        NikeUSUKAPI USUKAPI = new NikeUSUKAPI();
        ArrayList skuidlist = new ArrayList();
        bool multisize = false;
        JObject jopaymenttoken = null;
        string country = null;
        string currency = null;
        string locale = null;
        string shippingMethod = null;
        public void StartTask(CancellationToken ct, CancellationTokenSource cts)
        {
        A:
            try
            {
                dt = dt.AddDays(8);
                try
                {
                    if (tk.Tasksite == "NikeUK")
                    {
                        country = "GB";
                        currency = "GBP";
                        locale = "en_GB";
                        shippingMethod = "GROUND_SERVICE";
                    }
                    else
                    {
                        country = "US";
                        currency = "USD";
                        locale = "en_US";
                        shippingMethod = "STANDARD";
                    }
                    quantity = int.Parse(tk.Quantity);
                    GetSKUID(tk.Tasksite.Replace("Nike", ""), pid, ct, cts);
                }
                catch (NullReferenceException)
                {
                    goto A;
                }
                JObject joprofile = JObject.Parse(profile);
            B: string Authorization = "";
                try
                {
                    if (tk.monitortask != "True")
                    {
                        if (guest == false)
                        {
                            jig(joprofile, ct);
                            Authorization = Login(joprofile, ct);
                        }
                        else
                        {
                            jig(joprofile, ct);
                            USUKAPI.guest = true;
                        }
                    }
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
                            #region
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
                        #endregion
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
                    catch
                    {
                        return;
                    }
                    #endregion
                    if (ismonitor == false)
                    {
                        string monitorurl = "https://api.nike.com/cic/grand/v1/graphql";
                        string info = null;
                        if (tk.Tasksite == "NikuUK")
                        {
                            info = "{\"hash\":\"ef571ff0ac422b0de43ab798cc8ff25f\",\"variables\":{\"ids\":[\"" + skuid + "\"],\"country\":\"GB\",\"language\":\"en-GB\",\"isSwoosh\":false}}";
                        }
                        else
                        {
                            info = "{\"hash\":\"ef571ff0ac422b0de43ab798cc8ff25f\",\"variables\":{\"ids\":[\"" + skuid + "\"],\"country\":\"US\",\"language\":\"en-US\",\"isSwoosh\":false}}";
                        }
                        string[] group = USUKAPI.Monitoring(monitorurl, tk, ct, info, randomsize, skuid, multisize, skuidlist);
                        if (group[0] != null)
                        {
                            skuid = group[0];
                        }
                    }
                }
                catch (NullReferenceException)
                {
                    tk.Status = "Login Error";
                    goto B;
                }
            C:
                try
                {
                    if (ct.IsCancellationRequested)
                    {
                        tk.Status = "IDLE";
                        ct.ThrowIfCancellationRequested();
                    }
                    var cts2 = new CancellationTokenSource();
                    var ct2 = cts2.Token;
                    if ((giftcard + "").Length == 0)
                    {
                        Task task = Task.Run(() => Submitcardinfo(Authorization, ct, ct2, cts2));
                    }
                    else
                    {
                        Task task = Task.Run(() => subimitgiftcard(Authorization, ct, ct2, cts2));
                    }
                }
                catch (NullReferenceException)
                {
                    goto C;
                }
            E:
                try
                {
                    if (ct.IsCancellationRequested)
                    {
                        tk.Status = "IDLE";
                        ct.ThrowIfCancellationRequested();
                    }
                    Checkoutpreview(Authorization, skuid, joprofile, ct, cts);
                    CheckoutpreviewStatus(Authorization, ct);
                }
                catch (NullReferenceException)
                {
                    if (ct.IsCancellationRequested)
                    {
                        tk.Status = "IDLE";
                        ct.ThrowIfCancellationRequested();
                    }
                    goto E;
                }
            F: if (subcard)
                {
                    if (ct.IsCancellationRequested)
                    {
                        tk.Status = "IDLE";
                        ct.ThrowIfCancellationRequested();
                    }
                    string id = PaymentPreviw(Authorization, joprofile, ct);
                    paymenttoken(Authorization, id, joprofile, ct);
                }
                else
                {
                    tk.Status = "Waiting for Submit Card";
                    Thread.Sleep(1);
                    goto F;
                }
                cts.Cancel();
                Main.dic.Remove(tk.Taskid);
                if (ct.IsCancellationRequested)
                {
                    ct.ThrowIfCancellationRequested();
                }
            }
            catch (OperationCanceledException)
            {
                return;
            }

        }
        protected void GetSKUID(string country, string pid, CancellationToken ct, CancellationTokenSource cts)
        {
            string productdetail = null;
            bool productdetailnull = false;
            string svalue = null;
            if (localsize.TryGetValue(tk.Tasksite + pid, out svalue))
            {
                JObject sva = JObject.Parse(svalue);
                imageurl = sva["Image"].ToString();
                productID = sva["ProductID"].ToString();
                limit = int.Parse(sva["limit"].ToString());
                msrp = sva["msrp"].ToString();
                JObject jo = JObject.Parse(sva["data"].ToString());
                if (tk.Size.Contains("+") == false && tk.Size.Contains("-") == false && randomsize == false)
                {
                    foreach (var i in jo)
                    {
                        Thread.Sleep(1);
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
                    foreach (var i in jo)
                    {
                        Thread.Sleep(1);
                        if (ct.IsCancellationRequested)
                        {
                            tk.Status = "IDLE";
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
                    productID = result["ProductID"].ToString();
                    limit = int.Parse(result["limit"].ToString());
                    msrp = result["msrp"].ToString();
                    JObject jo = JObject.Parse(result["data"].ToString());
                    if (tk.Size.Contains("+") == false && tk.Size.Contains("-") == false && randomsize == false)
                    {
                        foreach (var i in jo)
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
                        foreach (var i in jo)
                        {
                            if (ct.IsCancellationRequested)
                            {
                                tk.Status = "IDLE";
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
                A: try
                    {
                        Thread.Sleep(1);
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
                            JObject j = JObject.Parse(jar[0].ToString());
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
                        tk.Status = "IDLE";
                        ct.ThrowIfCancellationRequested();
                    }
                    productdetail = "{\"data\":" + JsonConvert.SerializeObject(allsize) + ",\"Image\":\"" + imageurl + "\",\"ProductID\":\"" + productID + "\",\"limit\":\"" + limit + "\",\"msrp\":\"" + msrp + "\"}";
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
        public void jig(JObject profile, CancellationToken ct)
        {
            autojig aj = new autojig();
            if (profile["Address1"].ToString().Contains("%char4%"))
            {
                Regex regex = new Regex(@"%char4%");
                profile["Address1"] = regex.Replace(profile["Address1"].ToString(), aj.GenerateRandomString(4));
            }
            if (profile["Address1"].ToString().Contains("%num4%"))
            {
                Regex regex = new Regex(@"%num4%");
                profile["Address1"] = regex.Replace(profile["Address1"].ToString(), aj.GenerateRandomnum(4));
            }
            if (profile["Address2"].ToString().Contains("%char4%"))
            {
                Regex regex = new Regex(@"%char4%");
                profile["Address2"] = regex.Replace(profile["Address2"].ToString(), aj.GenerateRandomString(4));
            }
            if (profile["Tel"].ToString().Contains("%num4%"))
            {
                Regex regex = new Regex(@"%num4%");
                profile["Tel"] = regex.Replace(profile["Tel"].ToString(), aj.GenerateRandomnum(4));
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
            if (profile["EmailAddress"].ToString().Contains("random"))
            {
                Regex regex = new Regex(@"random");
                Firstname fs = new Firstname();
                profile["EmailAddress"] = regex.Replace(profile["EmailAddress"].ToString(), aj.GenerateRandomString(4));
            }
        }
        protected string Login(JObject profile, CancellationToken ct)
        {
        A: string Authorization = "";
            string loginurl = null;
            string logininfo = null;
            bool isrefresh = false;
            if (Config.UseAdvancemode == "True")
            {
                loginurl = "https://unite.nike.com/login?appVersion=831&experienceVersion=831&uxid=com.nike.commerce.snkrs.web&locale=" + locale + "&backendEnvironment=identity&browser=Google%20Inc.&os=undefined&mobile=false&native=false&visit=1&visitor=" + GID;
            }
            else
            {
                loginurl = "http://127.0.0.1:1234/login?appVersion=831&experienceVersion=831&uxid=com.nike.commerce.snkrs.web&locale=" + locale + "&backendEnvironment=identity&browser=Google%20Inc.&os=undefined&mobile=false&native=false&visit=1&visitor=" + GID;
            }
            try
            {
                if (Mainwindow.refreshtoken.Count == 0)
                {
                    logininfo = "{\"username\":\"" + username + "\",\"password\":\"" + password + "\",\"client_id\":\"PbCREuPr3iaFANEDjtiEzXooFl7mXGQ7\",\"ux_id\":\"com.nike.commerce.snkrs.web\",\"grant_type\":\"password\"}";
                    Authorization = USUKAPI.Postlogin(loginurl, logininfo, isrefresh, username, tk, ct);
                }
                else
                {
                    if (ct.IsCancellationRequested)
                    {
                        tk.Status = "IDLE";
                        ct.ThrowIfCancellationRequested();
                    }
                    string token = "";
                    if (Mainwindow.refreshtoken.TryGetValue(username, out token))
                    {
                        if (ct.IsCancellationRequested)
                        {
                            tk.Status = "IDLE";
                            ct.ThrowIfCancellationRequested();
                        }
                        bool isrefresh2 = true;
                        string refreshinfo = "{\"refresh_token\":\"" + token + "\",\"client_id\":\"PbCREuPr3iaFANEDjtiEzXooFl7mXGQ7\",\"grant_type\":\"refresh_token\"}";
                        string loginurl2 = null;
                        if (Config.UseAdvancemode != "True")
                        {
                            loginurl2 = "http://127.0.0.1:1234/tokenRefresh?appVersion=805&experienceVersion=805&uxid=com.nike.commerce.nikedotcom.web&locale=" + locale + "&backendEnvironment=identity&browser=Google%20Computer%2C%20Inc.&os=undefined&mobile=true&native=true&visit=1&visitor=" + GID;
                        }
                        else
                        {
                            loginurl2 = "https://unite.nike.com/tokenRefresh?appVersion=805&experienceVersion=805&uxid=com.nike.commerce.nikedotcom.web&locale=" + locale + "&backendEnvironment=identity&browser=Google%20Computer%2C%20Inc.&os=undefined&mobile=true&native=true&visit=1&visitor=" + GID;
                        }
                        Authorization = USUKAPI.Postlogin(loginurl2, refreshinfo, isrefresh2, username, tk, ct);
                    }
                    else
                    {
                        logininfo = "{\"username\":\"" + username + "\",\"password\":\"" + password + "\",\"client_id\":\"PbCREuPr3iaFANEDjtiEzXooFl7mXGQ7\",\"ux_id\":\"com.nike.commerce.snkrs.web\",\"grant_type\":\"password\"}";
                        Authorization = USUKAPI.Postlogin(loginurl, logininfo, isrefresh, username, tk, ct);
                    }
                }
            }
            catch (IOException)
            {
                tk.Status = "Login Error";
                goto A;
            }
            return Authorization;
        }
        protected void Submitcardinfo(string Authorization, CancellationToken ct, CancellationToken ct2, CancellationTokenSource cts)
        {
            cardguid = Guid.NewGuid().ToString();
            string cardinfo = "";
            JObject jo = JObject.Parse(profile);
            string cardurl = "https://paymentcc.nike.com/creditcardsubmit/" + cardguid + "/store";
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
                tk.Status = "Card Input Error";
            }
            cardinfo = "{\"accountNumber\":\"" + jo["Cardnum"].ToString() + "\",\"cardType\":\"" + cardtype + "\",\"expirationMonth\":\"" + expir[0] + "\",\"expirationYear\":\"" + expir[1] + "\",\"creditCardInfoId\":\"" + cardguid + "\",\"cvNumber\":\"" + jo["Cvv"].ToString() + "\"}";
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            USUKAPI.Postcardinfo(cardurl, cardinfo, Authorization, tk, ct, false);
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            subcard = true;
            cts.Cancel();
            try
            {
                if (ct2.IsCancellationRequested)
                {
                    ct2.ThrowIfCancellationRequested();
                }
            }
            catch { }
        }
        protected void Checkoutpreview(string Authorization, string skuid, JObject jo, CancellationToken ct, CancellationTokenSource cts)
        {
            GID = Guid.NewGuid().ToString();
            string url = null;
            if (Config.UseAdvancemode == "True")
            {
                url = "https://api.nike.com/buy/checkout_previews/v3/" + GID;
            }
            else
            {
                url = "http://127.0.0.1:1234/buy/checkout_previews/v3/" + GID;
            }
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            if (limit == 0)
            {
                quantity = 1;
            }
            if (int.Parse(tk.Quantity) > limit)
            {
                quantity = limit;
            }
            JObject payLoad = null;
            payLoad = JObject.Parse(Nike_Requestpaylod.previewpaylod);
            if (code != "")
            {
                payLoad["request"]["promotionCodes"][0] = code;
            }
            else
            {
                payLoad["request"]["promotionCodes"][0].Remove();
            }
            #region
            payLoad["request"]["country"] = country;
            payLoad["request"]["currency"] = currency;
            payLoad["request"]["email"] = jo["EmailAddress"].ToString();
            payLoad["request"]["locale"] = locale;
            payLoad["request"]["items"][0]["id"] = Guid.NewGuid().ToString();
            payLoad["request"]["items"][0]["skuId"] = skuid;
            //payLoad["request"]["items"][0]["shippingMethod"] = shippingMethod;
            payLoad["request"]["items"][0]["quantity"] = quantity;
            payLoad["request"]["items"][0]["fulfillmentDetails"]["getBy"]["maxDate"]["dateTime"] = dt.ToString("yyyy-MM-ddTHH:mm:ss.ffZ");
            payLoad["request"]["items"][0]["fulfillmentDetails"]["location"]["postalAddress"]["country"] = country;
            payLoad["request"]["items"][0]["fulfillmentDetails"]["location"]["postalAddress"]["address1"] = jo["Address1"].ToString();
            payLoad["request"]["items"][0]["fulfillmentDetails"]["location"]["postalAddress"]["address2"] = jo["Address2"].ToString();
            payLoad["request"]["items"][0]["fulfillmentDetails"]["location"]["postalAddress"]["postalCode"] = jo["Zipcode"].ToString();
            payLoad["request"]["items"][0]["fulfillmentDetails"]["location"]["postalAddress"]["city"] = jo["City"].ToString();
            payLoad["request"]["items"][0]["fulfillmentDetails"]["location"]["postalAddress"]["state"] = jo["State"].ToString();
            payLoad["request"]["items"][0]["recipient"]["firstName"] = jo["FirstName"].ToString();
            payLoad["request"]["items"][0]["recipient"]["lastName"] = jo["LastName"].ToString();
            payLoad["request"]["items"][0]["contactInfo"]["phoneNumber"] = jo["Tel"].ToString();
            payLoad["request"]["items"][0]["contactInfo"]["email"] = jo["EmailAddress"].ToString();
            payLoad["request"]["items"][0]["shippingAddress"]["address1"] = jo["Address1"].ToString();
            payLoad["request"]["items"][0]["shippingAddress"]["address2"] = jo["Address2"].ToString();
            payLoad["request"]["items"][0]["shippingAddress"]["city"] = jo["City"].ToString();
            payLoad["request"]["items"][0]["shippingAddress"]["postalCode"] = jo["Zipcode"].ToString();
            payLoad["request"]["items"][0]["shippingAddress"]["state"] = jo["State"].ToString();
            payLoad["request"]["items"][0]["shippingAddress"]["country"] = country;
            jopaymenttoken = payLoad;
            #endregion
            string checkoutpayload = null;
            try
            {
                checkoutpayload = payLoad.ToString();
            }
            catch (Exception)
            {
                tk.Status = "Bad Profile";
                cts.Cancel();
                Main.dic.Remove(tk.Taskid);
                if (ct.IsCancellationRequested)
                {
                    ct.ThrowIfCancellationRequested();
                }
            }
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            USUKAPI.CheckoutPreview(url, Authorization, checkoutpayload, GID, tk, ct);
        }
        protected void CheckoutpreviewStatus(string Authorization, CancellationToken ct)
        {
            string url = "https://api.nike.com/buy/checkout_previews_jobs/v3/" + GID;
            bool isdiscount = true;
            if (string.IsNullOrEmpty(code))
            {
                isdiscount = false;
            }
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            msrp = USUKAPI.CheckoutPreviewStatus(url, Authorization, isdiscount, tk, ct);

            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
        }
        protected void subimitgiftcard(string Authorization, CancellationToken ct, CancellationToken ct2, CancellationTokenSource cts)
        {
            int count = 0;
            double balance = 0;
            JObject jo = JObject.Parse(Mainwindow.giftcardlist[giftcard]);
            double msrpdouble = 0;
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            try
            {
                foreach (var i in jo)
                {
                    Thread.Sleep(1);
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
            string cardinfo2 = null;
            for (int i = 0; i < count; i++)
            {
                KeyValuePair<string, string> kv = giftcard2.ElementAt(i);
                string cardurl2 = "https://api.nike.com/payment/giftcard_balance/v1";
                cardinfo2 = "{\"accountNumber\":\"" + kv.Key + "\",\"pin\":\"" + kv.Value + "\",\"currency\":\"" + currency + "\"}";
                balance += USUKAPI.Postcardinfo(cardurl2, cardinfo2, Authorization, tk, ct, true);
                msrpdouble = Convert.ToDouble(msrp);
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
            if (balance < msrpdouble)
            {
                tk.Status = "Insufficient balance";
            }
            else
            {
                subcard = true;
            }
            cts.Cancel();
            try
            {
                if (ct2.IsCancellationRequested)
                {
                    ct2.ThrowIfCancellationRequested();
                }
            }
            catch { }
        }
        protected string PaymentPreviw(string Authorization, JObject jo, CancellationToken ct)
        {
            string paymenturl = "https://api.nike.com/payment/preview/v3";
            JObject payinfo = JObject.Parse(Nike_Requestpaylod.previewpayinfo);
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            payinfo["checkoutId"] = GID;
            payinfo["total"] = msrp;
            payinfo["currency"] = currency;
            payinfo["country"] = country;
            payinfo["items"][0]["productId"] = productID;
            payinfo["items"][0]["contactInfo"]["phoneNumbers"] = jo["Tel"].ToString();
            payinfo["items"][0]["contactInfo"]["email"] = jo["EmailAddress"].ToString();
            payinfo["items"][0]["recipient"]["firstName"] = jo["FirstName"].ToString();
            payinfo["items"][0]["recipient"]["lastName"] = jo["LastName"].ToString();
            payinfo["items"][0]["fulfillmentDetails"]["getBy"]["maxDate"]["dateTime"] = dt.ToString("yyyy-MM-ddTHH:mm:ss.ffZ");
            payinfo["items"][0]["fulfillmentDetails"]["location"]["postalAddress"]["country"] = country;
            payinfo["items"][0]["fulfillmentDetails"]["location"]["postalAddress"]["address1"] = jo["Address1"].ToString();
            payinfo["items"][0]["fulfillmentDetails"]["location"]["postalAddress"]["address2"] = jo["Address2"].ToString();
            payinfo["items"][0]["fulfillmentDetails"]["location"]["postalAddress"]["postalCode"] = jo["Zipcode"].ToString();
            payinfo["items"][0]["fulfillmentDetails"]["location"]["postalAddress"]["city"] = jo["City"].ToString();
            payinfo["items"][0]["fulfillmentDetails"]["location"]["postalAddress"]["state"] = jo["State"].ToString();
            payinfo["items"][0]["recipient"]["firstName"] = jo["FirstName"].ToString();
            payinfo["items"][0]["recipient"]["lastName"] = jo["LastName"].ToString();
            payinfo["items"][0]["shippingAddress"]["address1"] = jo["Address1"].ToString();
            payinfo["items"][0]["shippingAddress"]["address2"] = jo["Address2"].ToString();
            payinfo["items"][0]["shippingAddress"]["city"] = jo["City"].ToString();
            payinfo["items"][0]["shippingAddress"]["postalCode"] = jo["Zipcode"].ToString();
            payinfo["items"][0]["shippingAddress"]["state"] = jo["State"].ToString();
            payinfo["items"][0]["shippingAddress"]["country"] = country;
            if (giftcard != "")
            {
                JObject payLoad = null;
                JArray ja = new JArray();
                Thread.Sleep(1);
                for (int i = 0; i < index + 1; i++)
                {
                    Thread.Sleep(1);
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
                    ja.Add(payLoad);
                }
                payinfo["paymentInfo"] = ja;
            }
            else
            {
                payinfo["paymentInfo"][0]["id"] = Guid.NewGuid().ToString();
                payinfo["paymentInfo"][0]["creditCardInfoId"] = cardguid;
                payinfo["paymentInfo"][0]["billingInfo"]["name"]["firstName"] = jo["FirstName"].ToString();
                payinfo["paymentInfo"][0]["billingInfo"]["name"]["lastName"] = jo["LastName"].ToString();
                payinfo["paymentInfo"][0]["billingInfo"]["address"]["address1"] = jo["Address1"].ToString();
                payinfo["paymentInfo"][0]["billingInfo"]["address"]["address2"] = jo["Address2"].ToString();
                payinfo["paymentInfo"][0]["billingInfo"]["address"]["city"] = jo["City"].ToString();
                payinfo["paymentInfo"][0]["billingInfo"]["address"]["postalCode"] = jo["Zipcode"].ToString();
                payinfo["paymentInfo"][0]["billingInfo"]["address"]["state"] = jo["State"].ToString();
                payinfo["paymentInfo"][0]["billingInfo"]["address"]["country"] = country;
                payinfo["paymentInfo"][0]["billingInfo"]["contactInfo"]["phoneNumber"] = jo["Tel"].ToString();
                payinfo["paymentInfo"][0]["billingInfo"]["contactInfo"]["email"] = jo["EmailAddress"].ToString();
            }
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            var wu=payinfo.ToString();
            string id = USUKAPI.payment(paymenturl, Authorization, payinfo.ToString(), tk, ct);
            return id;
        }
        protected void paymenttoken(string Authorization, string id, JObject jo, CancellationToken ct)
        {
            string url = null;
            if (Config.UseAdvancemode == "True")
            {
                url = "https://api.nike.com/buy/checkouts/v3/" + GID;
            }
            else
            {
                url = "http://127.0.0.1:1234/buy/checkouts/v3/" + GID;
            }
            JObject jtotal = new JObject(
new JObject(
new JProperty("total", msrp),
new JProperty("fulfillment",
new JObject(
new JProperty("total", float.Parse(msrp)),
new JProperty("details",
new JObject(
new JProperty("price", 0),
new JProperty("discount", 0))
)))));
            jopaymenttoken["request"]["paymentToken"] = id;
            jopaymenttoken["request"]["totals"] = jtotal;
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            string status = USUKAPI.final(Authorization, url, jopaymenttoken.ToString(), GID, tk, ct);
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            if (status.Contains("error") == true)
            {
                JObject jo3 = JObject.Parse(status);
                tk.Status = jo3["error"]["message"].ToString();
                Thread.Sleep(3000);
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
        public void ProcessNotification(bool publicsuccess, taskset tk, string webhookurl, JObject joprofile, string orderid)
        {
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
                jobject["embeds"][0]["fields"][1]["value"] = tk.Size + "*" + quantity;
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
                jobject["embeds"][0]["fields"][1]["value"] = tk.Size+"*"+quantity;
                jobject["embeds"][0]["fields"][2]["value"] = joprofile["EmailAddress"].ToString();
                if (guest)
                {
                    jobject["embeds"][0]["fields"][3]["value"] = "Guest";
                }
                else
                {
                    jobject["embeds"][0]["fields"][3]["value"] = username;
                }
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
            if ((url + "").Length != 0)
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
                }
                catch (WebException ex)
                {
                    Thread.Sleep(1000);
                    tk.Status = "429";
                    goto Retry;
                }
            }
        }
    }
}


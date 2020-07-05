using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MAIO
{
    class NikeUSUKAPI
    {
        Random ran = new Random();
        string xb3traceid = Guid.NewGuid().ToString();
        string xnikevisitorid = Guid.NewGuid().ToString();
        public string GetHtmlsource(string url, Main.taskset tk, CancellationToken ct)
        {
        A: if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            WebProxy wp = new WebProxy();
            string SourceCode = "";
            int random = ran.Next(0, Mainwindow.proxypool.Count);
            try
            {
                string proxyg = Mainwindow.proxypool[random].ToString();
                string[] proxy = proxyg.Split(":");
                if (proxy.Length == 2)
                {
                    wp.Address = new Uri("http://" + proxy[0] + ":" + proxy[1] + "/");

                }
                else if (proxy.Length == 4)
                {
                    wp.Address = new Uri("http://" + proxy[0] + ":" + proxy[1] + "/");
                    wp.Credentials = new NetworkCredential(proxy[2], proxy[3]);
                }

            }
            catch (ArgumentOutOfRangeException)
            {
                wp = default;
            }
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Proxy = wp;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;
                if (response.ContentEncoding == "gzip")
                {
                    readStream = new StreamReader(new GZipStream(receiveStream, CompressionMode.Decompress), Encoding.GetEncoding("utf-8"));
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.UTF8);
                }
                SourceCode = readStream.ReadToEnd();
                response.Close();
                readStream.Close();
                tk.Status = "Get Size";
            }
            catch (WebException ex)
            {
                HttpWebResponse response = (HttpWebResponse)ex.Response;
                tk.Status = "Get Size Error";
                tk.Status = "Change Proxy";
                goto A;
            }
            return SourceCode;
        }
        public string Postlogin(string url, string logininfo, bool isrefresh, string account, Main.taskset tk, CancellationToken ct)
        {
        retry: int random = ran.Next(0, Mainwindow.proxypool.Count);
            WebProxy wp = new WebProxy();
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            try
            {
                string proxyg = Mainwindow.proxypool[random].ToString();
                string[] proxy = proxyg.Split(":");

                if (proxy.Length == 2)
                {
                    wp.Address = new Uri("http://" + proxy[0] + ":" + proxy[1] + "/");

                }
                else if (proxy.Length == 4)
                {
                    wp.Address = new Uri("http://" + proxy[0] + ":" + proxy[1] + "/");
                    wp.Credentials = new NetworkCredential(proxy[2], proxy[3]);
                }
            }
            catch
            {
                wp = default;
            }
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Proxy = wp;
            byte[] contentByte = Encoding.UTF8.GetBytes(logininfo);
            req.Method = "POST";
            req.Headers.Add("Accept", "*/*");
            req.Headers.Add("Accept-Language", "en-US,en;q=0.9");
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
            req.ContentType = "application/json";
            req.Headers.Add("Accept-encoding", "gzip, deflate, br");
            req.Headers.Add("Host", "unite.nike.com");
            req.ContentLength = contentByte.Length;
            if (isrefresh == false)
            {
                if (Mainwindow.iscookielistnull)
                {
                    req.Headers.Add("Cookie", "");
                }
                else
                {
                    if ((Config.cjevent != "") && (Config.cid != ""))
                    {
                    reloadcookie: Random ra = new Random();
                        int sleeptime = ra.Next(0, 500);
                        Thread.Sleep(sleeptime);
                        if (Mainwindow.lines.Count == 0)
                        {
                            tk.Status = "No Cookie";
                            Mainwindow.iscookielistnull = true;
                            Thread.Sleep(3600000);
                        }
                        int cookie = ra.Next(0, Mainwindow.lines.Count);
                        try
                        {
                            // req.Headers.Add("Cookie", @"AnalysisUserId=221.194.158.162.153781589120559642; s_ecid=MCMID%7C44822141248338726597572623382071276877; anonymousId=30DDFD27B849252DF78D1FF50DF3D5C6; _gcl_au=1.1.578438674.1589120562; _scid=aa5d7dab-0f30-4547-bb93-6f5d5db55af4; _fbp=fb.1.1589120564331.339030096; CONSUMERCHOICE_SESSION=t; RES_TRACKINGID=351348755954531; ResonanceSegment=1; guidS=9a8ecb42-8a4a-49bc-9132-b6db00b3f5cb; guidU=5bfd366a-ec0e-4876-bde5-6af83a218e59; _ga=GA1.2.288946757.1589120705; AMCV_F0935E09512D2C270A490D4D%40AdobeOrg=1994364360%7CMCMID%7C44822141248338726597572623382071276877%7CMCAID%7CNONE%7CMCOPTOUT-1589130625s%7CNONE%7CvVersion%7C3.4.0%7CMCIDTS%7C18393%7CMCAAMLH-1589728225%7C9%7CMCAAMB-1589728225%7Cj8Odv6LonN4r3an7LhD3WZrU1bUpAkFkkiY1ncBR96t2PTI%7CMCSYNCSOP%7C411-18400; bc_nike_singapore_triggermail=%7B%22distinct_id%22%3A%20%22171ff21c59541c-0fa19ef195e003-d373666-1704a0-171ff21c596760%22%2C%22cart_total%22%3A%200%7D; crl8.fpcuid=8ca8a405-115e-4f99-96f1-c06cb9783d03; bm_sz=DD19CB68E48304D86D567E801E1F5AD0~YAAQNIyUG5/5S/dyAQAASifnHAhOrHGpsiJSTOxSM4Ff+ILcnvC0PKkXxLfluKNFdaIjthPBZlktdfeBNm3fbr53NuvftpG+ctFAD9Kcas4EunwMBGBiWy7urFtxWc/+9HvQ4sQRp7QRF+rbbMTKSKBTAjhOSqcrRqeXQ6Eo/27gfpH0Z0TmT8PuJ4+E/A==; CONSUMERCHOICE=us/en_us; NIKE_COMMERCE_COUNTRY=US; NIKE_COMMERCE_LANG_LOCALE=en_US; cid=undefined%7Cundefined; geoloc=cc=CN,rc=HI,tp=vhigh,tz=GMT+8,la=20.05,lo=110.34; AKA_A2=A; optimizelyEndUserId=oeu1593923424826r0.5422267882994918; bm_mi=ED521A1D90A7DD45BC273E709DF64A39~O4xp061/dr/lesWQlJN9OxA4GKkIvK8IOU6yA6L+gw2Cmk1Rdp4q3cJgBBUg9Ok+48WVDDnHB+zMKXqEEzW9iP3fliqy8TXfkOPF/fWbKvU+auJSnJT8gf8x5RxCbiO2UWHtS6T3Q22Ej73HuaogwaObN8aGHxLr/XqXfkRxJfq3dT4gzWYQMefSnIMp8Dx09DY/0ndl12R7Pa87d+SburgQ3v3IeKRzhenNSJku/J0=; ak_bmsc=6A23BD6AE6E31C67F3CBFE4F9FF6BFFE1B948C34C31E00005141015F2494A66C~pl4HDSDOhTXin8uziTtuiYDfdiO4RiL1QsmUy016Jz9F4AZq/JPylzjz1LJyLITzcDj//RjH8cBYszLB2HlXWw3sk+vBF067mn0Bt/CnMHxm3JVf2yE39D/izAwAiw8kHGyoB9XkZp8ElC46owdBqW96hWXIPmfhTorx8rizHU3Ib3/hAOTl465IISzvQTcfwH0gZi8aRCpmO3m2/jfmq7fMans4fAz71w+L9XhzTXIeRMnmB88wYDAn132z0QKzoZ; RES_SESSIONID=501796147149294; bc_nike_triggermail=%7B%22distinct_id%22%3A%20%221731d3dddf3423-0705c9f3e7378d-4353760-1704a0-1731d3dddf429e%22%2C%22cart_total%22%3A%20160%7D; _uetsid=e3894a70-7765-4063-2300-ca70570c5dc2; _uetvid=437bd4e8-91e4-6aff-9f40-bac4d207cc4f; bm_sv=11EAE474E509FA88ABD7A17451097103~qMEHlKIHh1Vc6ExF4LqxbrnbPqtCkD5vJ4m83Xu2ZF7au2imqntiZCw1q/iSvP43QwbIANxDgDlCBSxd1zAvcnoQNQF6EJZyf5qQIRk+mWtlhgQ6tYjtheMwDwRZco15ueH9R/4h67DmRoFiT/sruu4JJOHNuRgSp6txpvTHdGs=; _abck=CBF441C137B01D80E242F3B7E0FAE93F~-1~YAAQFlCcJAoP0Q9zAQAA5rJSHQRUXYhBmasa1zlKmIW0KFniq3R89DArhNWEmOi3HF5rrWsmZXbK/dH+0Y8HYxEWkq/VWMfZnpr6IqoKPOlPk9DkBdQCg1FaGq1usR86d5k7dMFqeFhkGwm8dtBYob0AUk7EXCmjmpAcpeynleQf32vhI12lXI2nXlPvEjXe+oYadY9o4gO5lnqMBLlPhOcd7rkWOyskfh7PKc2CCFPXdi/CfWieo//G4GiKkGafGvdYDbEol/ePaz+ws2QT7MqfvVqWV0v1pARNuoGB0mc0R6wKCWiejQIN3EC7OIcE+GOPlmiXQOGwt2ol2QSd+shPWaRaorVqlqaKiTInkHs=~-1~-1~-1");
                            req.Headers.Add("Cookie", Mainwindow.lines[cookie] + "; nike_cid=" + Config.cid + "; cid=" + Config.cid + "%7C" + Config.cjevent + "");
                            Mainwindow.lines.RemoveAt(cookie);
                        }
                        catch (Exception)
                        {
                            goto reloadcookie;
                        }
                    }
                    else
                    {
                    reloadcookie: Random ra = new Random();
                        int sleeptime = ra.Next(0, 500);
                        Thread.Sleep(sleeptime);
                        if (Mainwindow.lines.Count == 0)
                        {
                            tk.Status = "No Cookie";
                            Mainwindow.iscookielistnull = true;
                            Thread.Sleep(3600000);
                        }
                        int cookie = ra.Next(0, Mainwindow.lines.Count);
                        try
                        {
                            req.Headers.Add("Cookie", Mainwindow.lines[cookie]);
                            Mainwindow.lines.RemoveAt(cookie);
                        }
                        catch (Exception)
                        {
                            goto reloadcookie;
                        }
                    }
                }
            }
            Main.updatelable();
            req.Headers.Add("Sec-Fetch-Dest", "empty");
            req.Headers.Add("Sec-Fetch-Mode", "cors");
            req.Headers.Add("Sec-Fetch-Site", "same-site");
            req.Referer = "https://www.nike.com/launch/";
            req.Headers.Add("origin", "https://unite.nike.com");
            Stream webstream = req.GetRequestStream();
            webstream.Write(contentByte, 0, contentByte.Length);
            webstream.Close();
            string token = "";
            try
            {
                HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                tk.Status = "Login Successful!";
                Stream tokenStream = response.GetResponseStream();
                StreamReader readhtmlStream = new StreamReader(tokenStream, Encoding.UTF8);
                token = readhtmlStream.ReadToEnd();
            }
            catch (WebException ex)
            {
                HttpWebResponse respgethtml = (HttpWebResponse)ex.Response;
                Stream tokenStream = respgethtml.GetResponseStream();
                StreamReader readhtmlStream = new StreamReader(tokenStream, Encoding.UTF8);
                var chao = readhtmlStream.ReadToEnd();
                tk.Status = "Login Failed";
                goto retry;
            }
            JObject jo = JObject.Parse(token);
            string Authorization = "Bearer " + jo["access_token"].ToString();
            Task task1 = new Task(() => writerefreshtoken("[{\"Token\":\"" + jo["refresh_token"].ToString() + "\",\"Account\":\"" + account + "\"}]",account));
            task1.Start();  
            return Authorization;
        }
        public void writerefreshtoken(string token,string account)
        {
            try
            {
                if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "refreshtoken.json"))
                {
                    FileStream fs1 = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "refreshtoken.json", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                    StreamWriter sw = new StreamWriter(fs1);
                    JArray ja = JArray.Parse(token);
                    sw.Write(ja.ToString().Replace("\n", "").Replace("\t", "").Replace("\r", "").Replace(" ",""));
                    sw.Close();
                    fs1.Close();
                }
                else
                {
                    string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "refreshtoken.json";
                    FileInfo fi = new FileInfo(path);
                    if (fi.Length == 0)
                    {
                        FileStream fs1 = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "refreshtoken.json", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                        StreamWriter sw = new StreamWriter(fs1);
                        JArray ja = JArray.Parse(token);
                        sw.Write(ja.ToString().Replace("\n", "").Replace("\t", "").Replace("\r", "").Replace(" ", ""));
                        sw.Close();
                        fs1.Close();
                    }
                    else
                    {
                        FileStream fs1 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                        StreamReader sr = new StreamReader(fs1);
                        string read = sr.ReadToEnd();
                        JArray ja = JArray.Parse(read);
                        foreach (var i in ja)
                        {
                            if (i.ToString().Contains(account))
                            {
                                ja.Remove(i);
                            }
                        }
                        ja.Add(JObject.Parse(token.Replace("[", "").Replace("]", "")));
                        fs1.SetLength(0);
                        FileStream fs0 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                        StreamWriter sw = new StreamWriter(fs0);
                        sw.Write(ja.ToString().Replace("\n", "").Replace("\t", "").Replace("\r", "").Replace(" ", ""));
                        sw.Close();
                        fs1.Close();
                    }
                }
            }
            catch (Exception)
            {          
            }
        }
        public double Postcardinfo(string url, string cardinfo, string Authorization, string cardguid, Main.taskset tk, CancellationToken ct)
        {
        B: if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            WebProxy wp = new WebProxy();
            try
            {

                int random = ran.Next(0, Mainwindow.proxypool.Count);
                string proxyg = Mainwindow.proxypool[random].ToString();
                string[] proxy = proxyg.Split(":");

                if (proxy.Length == 2)
                {
                    wp.Address = new Uri("http://" + proxy[0] + ":" + proxy[1] + "/");

                }
                else if (proxy.Length == 4)
                {
                    wp.Address = new Uri("http://" + proxy[0] + ":" + proxy[1] + "/");
                    wp.Credentials = new NetworkCredential(proxy[2], proxy[3]);
                }
            }
            catch
            {
                wp = default;
            }
            double balance = 0;
            byte[] contentcardinfo = Encoding.UTF8.GetBytes(cardinfo);
            HttpWebRequest reqcard = (HttpWebRequest)WebRequest.Create(url);
            reqcard.Method = "POST";
            reqcard.Proxy = wp;
            reqcard.ContentType = "application/json";
            reqcard.Accept = "*/*";
            reqcard.Headers.Add("Accept-Encoding", "gzip, deflate");
            reqcard.Headers.Add("Accept-Language", "en-US, en; q=0.9");
            reqcard.Headers.Add("Authorization", Authorization);
            reqcard.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
            reqcard.ContentLength = contentcardinfo.Length;
            reqcard.Headers.Add("sec-fetch-dest", "empty");
            reqcard.Headers.Add("sec-fetch-mode", "cors");
            reqcard.Headers.Add("sec-fetch-site", "same-site");
            reqcard.Headers.Add("appid", "com.nike.commerce.snkrs.web");
            reqcard.Headers.Add("x-b3-spanname", "CiCCart");
            reqcard.Headers.Add("x-b3-traceid", xb3traceid);
            Stream cardstream = reqcard.GetRequestStream();
            cardstream.Write(contentcardinfo, 0, contentcardinfo.Length);
            cardstream.Close();
            try
            {
                HttpWebResponse respcard = (HttpWebResponse)reqcard.GetResponse();
                Stream respcardStream = respcard.GetResponseStream();
                StreamReader readhtmlStream = new StreamReader(respcardStream, Encoding.UTF8);
                string SourceCode = readhtmlStream.ReadToEnd();
                if (SourceCode.Contains("balance"))
                {

                    JObject jo = JObject.Parse(SourceCode);
                    string bal = jo["balance"].ToString();
                    balance = Convert.ToDouble(bal);
                }
                tk.Status = "Submit Card Success";
            }
            catch (WebException ex)
            {
                tk.Status = ex.Message.ToString();
                goto B;
            }
            return balance;

        }
        public void CheckoutPreview(string url, string Authorization, string checkoutpayload, string GID, Main.taskset tk, CancellationToken ct)
        {
        B: if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            WebProxy wp = new WebProxy();
            try
            {

                int random = ran.Next(0, Mainwindow.proxypool.Count);
                string proxyg = Mainwindow.proxypool[random].ToString();
                string[] proxy = proxyg.Split(":");

                if (proxy.Length == 2)
                {
                    wp.Address = new Uri("http://" + proxy[0] + ":" + proxy[1] + "/");

                }
                else if (proxy.Length == 4)
                {
                    wp.Address = new Uri("http://" + proxy[0] + ":" + proxy[1] + "/");
                    wp.Credentials = new NetworkCredential(proxy[2], proxy[3]);
                }
            }
            catch
            {
                wp = default;
            }
            HttpWebRequest reqpayment = (HttpWebRequest)WebRequest.Create(url);
            reqpayment.Method = "PUT";
            reqpayment.Proxy = wp;
            reqpayment.ContentType = "application/json; charset=UTF-8";
            byte[] contentpaymentinfo = Encoding.UTF8.GetBytes(checkoutpayload);
            reqpayment.Accept = "application/json";
            reqpayment.Headers.Add("Accept-Encoding", "gzip, deflate");
            reqpayment.Headers.Add("Accept-Language", "en-US, en; q=0.9");
            reqpayment.Headers.Add("Authorization", Authorization);
            if (Mainwindow.iscookielistnull)
            {
                reqpayment.Headers.Add("Cookie", "");
            }
            else
            {
                if ((Config.cjevent != "") && (Config.cid != ""))
                {
                reloadcookie: Random ra = new Random();
                    int sleeptime = ra.Next(0, 500);
                    Thread.Sleep(sleeptime);
                    if (Mainwindow.lines.Count == 0)
                    {
                        tk.Status = "No Cookie";
                        Mainwindow.iscookielistnull = true;
                        Thread.Sleep(3600000);
                    }
                    int cookie = ra.Next(0, Mainwindow.lines.Count);
                    try
                    {
                        reqpayment.Headers.Add("Cookie", Mainwindow.lines[cookie] + "; nike_cid=" + Config.cid + "; cid=" + Config.cid + "%7C" + Config.cjevent + "");
                        Mainwindow.lines.RemoveAt(cookie);
                    }
                    catch (Exception)
                    {
                        goto reloadcookie;
                    }
                }
                else
                {
                reloadcookie: Random ra = new Random();
                    int sleeptime = ra.Next(0, 500);
                    Thread.Sleep(sleeptime);
                    if (Mainwindow.lines.Count == 0)
                    {
                        tk.Status = "No Cookie";
                        Mainwindow.iscookielistnull = true;
                        Thread.Sleep(3600000);
                    }
                    int cookie = ra.Next(0, Mainwindow.lines.Count);
                    try
                    {
                        reqpayment.Headers.Add("Cookie", Mainwindow.lines[cookie]);
                        Mainwindow.lines.RemoveAt(cookie);
                    }
                    catch (Exception)
                    {
                        goto reloadcookie;
                    }
                }
            }
            Main.updatelable();
            reqpayment.ContentLength = contentpaymentinfo.Length;
            reqpayment.Referer = "https://www.nike.com/us/en/checkout";
            reqpayment.Headers.Add("Origin", "https://www.nike.com");
            reqpayment.Headers.Add("Sec-Fetch-Dest", "empty");
            reqpayment.Headers.Add("Sec-Fetch-Mode", "cors");
            reqpayment.Headers.Add("Sec-Fetch-Site", "same-site");
            reqpayment.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
            reqpayment.Headers.Add("x-b3-spanname", "CiCCheckout");
            reqpayment.Headers.Add("x-b3-traceid", xb3traceid);
            Stream paymentstream = reqpayment.GetRequestStream();
            paymentstream.Write(contentpaymentinfo, 0, contentpaymentinfo.Length);
            paymentstream.Close();
            string paymentsuccesscode = "";
            try
            {
                HttpWebResponse resppayment = (HttpWebResponse)reqpayment.GetResponse();
                tk.Status = "Submit Shipping";
                Stream resppaymentStream = resppayment.GetResponseStream();
                StreamReader readpaymenthtmlStream = new StreamReader(resppaymentStream, Encoding.UTF8);
                paymentsuccesscode = readpaymenthtmlStream.ReadToEnd();
                JObject jo = null;
                jo = JObject.Parse(paymentsuccesscode);
                try
                {
                    if (jo.ToString().Contains("error"))
                    {
                        if (jo["error"]["message"].ToString().Contains("Non buyable product(s)"))
                        {
                            tk.Status = "Non buyable product(s)";
                            goto B;
                        }
                    }
                }
                catch (NullReferenceException ex)
                {
                }
                
            }
            catch (WebException ex)
            {
                HttpWebResponse resppayment = (HttpWebResponse)ex.Response;
                tk.Status = "SubmitShipping error";
                goto B;
            }

        }
        public string CheckoutPreviewStatus(string url, string Authorization, bool isdiscount, Main.taskset tk, CancellationToken ct)
        {
        A: if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            WebProxy wp = new WebProxy();
            try
            {
                int random = ran.Next(0, Mainwindow.proxypool.Count);
                string proxyg = Mainwindow.proxypool[random].ToString();
                string[] proxy = proxyg.Split(":");
                if (proxy.Length == 2)
                {
                    wp.Address = new Uri("http://" + proxy[0] + ":" + proxy[1] + "/");

                }
                else if (proxy.Length == 4)
                {
                    wp.Address = new Uri("http://" + proxy[0] + ":" + proxy[1] + "/");
                    wp.Credentials = new NetworkCredential(proxy[2], proxy[3]);
                }
            }
            catch
            {
                wp = default;
            }
            string total = "";
            HttpWebRequest reqcheckstatus = (HttpWebRequest)WebRequest.Create(url);
            reqcheckstatus.Method = "GET";
            reqcheckstatus.Proxy = wp;
            reqcheckstatus.ContentType = "application/json; charset=UTF-8";
            reqcheckstatus.Accept = "application/json";
            reqcheckstatus.Headers.Add("Accept-Encoding", "gzip, deflate");
            reqcheckstatus.Headers.Add("Accept-Language", "en-US, en; q=0.9");
            reqcheckstatus.Headers.Add("Authorization", Authorization);
            reqcheckstatus.Headers.Add("Origin", "https://www.nike.com");
            reqcheckstatus.Headers.Add("Sec-Fetch-Dest", "empty");
            reqcheckstatus.Headers.Add("Sec-Fetch-Mode", "cors");
            reqcheckstatus.Headers.Add("Sec-Fetch-Site", "same-site");
            reqcheckstatus.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
            reqcheckstatus.Headers.Add("x-b3-spanname", "CiCCheckout");
            reqcheckstatus.Headers.Add("x-b3-traceid", xb3traceid);
            try
            {
                HttpWebResponse respcheckstatus = (HttpWebResponse)reqcheckstatus.GetResponse();
                string check = "";
                if (respcheckstatus.ContentEncoding == "gzip")
                {
                    Stream respcheckstatusstream = respcheckstatus.GetResponseStream();
                    StreamReader readcheckstatus = new StreamReader(new GZipStream(respcheckstatusstream, CompressionMode.Decompress), Encoding.GetEncoding("utf-8"));
                    check = readcheckstatus.ReadToEnd();
                    JObject jo = JObject.Parse(check);
                    if (check.Contains("IN_PROGRESS"))
                    {
                        Thread.Sleep(1000);
                        goto A;
                    }
                    if (check.Contains("Non buyable product(s)"))
                    {
                        tk.Status = "PRODUCT_NOT_BUYABLE";
                        goto A;
                    }
                    total = jo["response"]["totals"]["total"].ToString();
                    if (isdiscount)
                    {
                        var discount = jo["response"]["promotionCodes"].ToString();
                        JArray jo2 = JArray.Parse(discount);
                        var discountstatus = jo2[0]["status"].ToString();
                        if (discountstatus != "PROMOTION_APPLIED")
                        {
                            tk.Status = "code applied failed";
                            goto A;
                        }
                    }
                }
                else
                {
                    Stream respcheckstatusstream = respcheckstatus.GetResponseStream();
                    StreamReader readcheckstatus = new StreamReader(respcheckstatusstream, Encoding.GetEncoding("utf-8"));
                    check = readcheckstatus.ReadToEnd();
                    if (check.Contains("IN_PROGRESS"))
                    {
                        Thread.Sleep(1000);
                        goto A;
                    }
                    if (check.Contains("Non buyable product(s)"))
                    {
                        tk.Status = "PRODUCT_NOT_BUYABLE";
                        goto A;
                    }
                    JObject jo = JObject.Parse(check);
                    total = jo["response"]["totals"]["total"].ToString();
                    if (isdiscount)
                    {

                        var discount = jo["response"]["promotionCodes"].ToString();
                        JArray jo2 = JArray.Parse(discount);
                        var discountstatus = jo2[0]["status"].ToString();
                        if (discountstatus != "PROMOTION_APPLIED")
                        {
                            tk.Status = "code applied failed";
                            goto A;
                        }
                    }
                }

                if (check.Contains("COMPLETED") == true)
                {
                }
                else
                {
                    Thread.Sleep(3000);
                    goto A;
                }
            }
            catch
            {
                goto A;
            }
            return total;
        }
        public string payment(string url, string Authorization, string paymentinfo, Main.taskset tk, CancellationToken ct)
        {
        retry: if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            WebProxy wp = new WebProxy();
            try
            {

                int random = ran.Next(0, Mainwindow.proxypool.Count);
                string proxyg = Mainwindow.proxypool[random].ToString();
                string[] proxy = proxyg.Split(":");

                if (proxy.Length == 2)
                {
                    wp.Address = new Uri("http://" + proxy[0] + ":" + proxy[1] + "/");

                }
                else if (proxy.Length == 4)
                {
                    wp.Address = new Uri("http://" + proxy[0] + ":" + proxy[1] + "/");
                    wp.Credentials = new NetworkCredential(proxy[2], proxy[3]);
                }
            }
            catch
            {
                wp = default;
            }
            HttpWebRequest reqprocess = (HttpWebRequest)WebRequest.Create(url);
            reqprocess.Proxy = wp;
            reqprocess.Accept = "application/json";
            reqprocess.Method = "POST";
            reqprocess.ContentType = "application/json; charset=UTF-8";
            byte[] processbyteinfo = Encoding.UTF8.GetBytes(paymentinfo);
            reqprocess.Headers.Add("authorization", Authorization);
            if (Mainwindow.iscookielistnull)
            {
                reqprocess.Headers.Add("Cookie", "");
            }
            else
            {
                if ((Config.cjevent != "") && (Config.cid != ""))
                {
                reloadcookie: Random ra = new Random();
                    int sleeptime = ra.Next(0, 500);
                    Thread.Sleep(sleeptime);
                    if (Mainwindow.lines.Count == 0)
                    {
                        tk.Status = "No Cookie";
                        Mainwindow.iscookielistnull = true;
                        Thread.Sleep(3600000);
                    }
                    int cookie = ra.Next(0, Mainwindow.lines.Count);
                    try
                    {
                        reqprocess.Headers.Add("Cookie", Mainwindow.lines[cookie] + "; nike_cid=" + Config.cid + "; cid=" + Config.cid + "%7C" + Config.cjevent + "");
                        Mainwindow.lines.RemoveAt(cookie);
                    }
                    catch (Exception)
                    {
                        goto reloadcookie;
                    }

                }
                else
                {
                reloadcookie: Random ra = new Random();
                    int sleeptime = ra.Next(0, 500);
                    Thread.Sleep(sleeptime);
                    if (Mainwindow.lines.Count == 0)
                    {
                        tk.Status = "No Cookie";
                        Mainwindow.iscookielistnull = true;
                        Thread.Sleep(3600000);
                    }
                    int cookie = ra.Next(0, Mainwindow.lines.Count);
                    try
                    {
                        reqprocess.Headers.Add("Cookie", Mainwindow.lines[cookie]);
                        Mainwindow.lines.RemoveAt(cookie);
                    }
                    catch (Exception)
                    {
                        goto reloadcookie;
                    }
                }
            }
            Main.updatelable();
            reqprocess.Headers.Add("Accept-Encoding", "gzip, deflate");
            reqprocess.Headers.Add("Accept-Language", "en-US, en; q=0.9");
            reqprocess.Headers.Add("Origin", "https://www.nike.com");
            reqprocess.Referer = "https://www.nike.com/us/en/checkout";
            reqprocess.Headers.Add("Sec-Fetch-Dest", "empty");
            reqprocess.Headers.Add("Sec-Fetch-Mode", "cors");
            reqprocess.Headers.Add("Sec-Fetch-Site", "same-site");
            reqprocess.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
            reqprocess.Headers.Add("x-b3-spanname", "CiCCheckout");
            reqprocess.Headers.Add("x-b3-traceid", xb3traceid);
            Stream processstream = reqprocess.GetRequestStream();
            processstream.Write(processbyteinfo, 0, processbyteinfo.Length);
            processstream.Close();
            string processcode = "";
            try
            {
                HttpWebResponse processpayment = (HttpWebResponse)reqprocess.GetResponse();
                tk.Status = "Submit Billing";
                Stream processtream = processpayment.GetResponseStream();
                StreamReader readprocessstream = new StreamReader(processtream, Encoding.UTF8);
                processcode = readprocessstream.ReadToEnd();
            }
            catch (WebException ex)
            {
                HttpWebResponse processpayment = (HttpWebResponse)ex.Response;
                Stream processtream = processpayment.GetResponseStream();
                StreamReader readprocessstream = new StreamReader(processtream, Encoding.UTF8);
                tk.Status = "SubmitBilling error";
                goto retry;
            }
            JObject joid = JObject.Parse(processcode);
            string id = joid["id"].ToString();
            return id;
        }
        public void paymentjob(string url, string Authorization, CancellationToken ct, Main.taskset tk)
        {
        C: if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            WebProxy wp = new WebProxy();
            try
            {
                int random = ran.Next(0, Mainwindow.proxypool.Count);
                string proxyg = Mainwindow.proxypool[random].ToString();
                string[] proxy = proxyg.Split(":");

                if (proxy.Length == 2)
                {
                    wp.Address = new Uri("http://" + proxy[0] + ":" + proxy[1] + "/");

                }
                else if (proxy.Length == 4)
                {
                    wp.Address = new Uri("http://" + proxy[0] + ":" + proxy[1] + "/");
                    wp.Credentials = new NetworkCredential(proxy[2], proxy[3]);
                }
            }
            catch
            {
                wp = default;
            }
            HttpWebRequest reqjob = (HttpWebRequest)WebRequest.Create(url);
            reqjob.Proxy = wp;
            reqjob.Accept = "application/json";
            reqjob.Method = "GET";
            reqjob.ContentType = "application/json; charset=UTF-8";
            reqjob.Headers.Add("authorization", Authorization);
            reqjob.Headers.Add("Accept-Encoding", "gzip, deflate");
            reqjob.Headers.Add("Accept-Language", "en-US, en; q=0.9");
            reqjob.Headers.Add("Origin", "https://www.nike.com");
            reqjob.Referer = "https://www.nike.com/us/en/checkout";
            reqjob.Headers.Add("Sec-Fetch-Dest", "empty");
            reqjob.Headers.Add("Sec-Fetch-Mode", "cors");
            reqjob.Headers.Add("Sec-Fetch-Site", "same-site");
            reqjob.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
            reqjob.Headers.Add("x-b3-spanname", "CiCCheckout");
            reqjob.Headers.Add("x-b3-traceid", xb3traceid);
            string jobstatus = "";
            try
            {
                HttpWebResponse respjob = (HttpWebResponse)reqjob.GetResponse();

                if (respjob.ContentEncoding == "gzip")
                {
                    Stream jobstream = respjob.GetResponseStream();
                    StreamReader read = new StreamReader(new GZipStream(jobstream, CompressionMode.Decompress), Encoding.GetEncoding("utf-8"));
                    jobstatus = read.ReadToEnd();
                }
                else
                {
                    Stream jobstream = respjob.GetResponseStream();
                    StreamReader readjobstream = new StreamReader(jobstream, Encoding.UTF8);
                    jobstatus = readjobstream.ReadToEnd();
                }
                if (jobstatus.Contains("COMPLETED") == true)
                {
                }
                else
                {
                    goto C;
                }
            }
            catch (WebException ex)
            {
                HttpWebResponse respjob = (HttpWebResponse)ex.Response;

                goto C;
            }
        }
        public void final(string Authorization, string url, string payload, string GID, Main.taskset tk, CancellationToken ct)
        {
        D: if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            WebProxy wp = new WebProxy();
            try
            {
                int random = ran.Next(0, Mainwindow.proxypool.Count);
                string proxyg = Mainwindow.proxypool[random].ToString();
                string[] proxy = proxyg.Split(":");

                if (proxy.Length == 2)
                {
                    wp.Address = new Uri("http://" + proxy[0] + ":" + proxy[1] + "/");

                }
                else if (proxy.Length == 4)
                {
                    wp.Address = new Uri("http://" + proxy[0] + ":" + proxy[1] + "/");
                    wp.Credentials = new NetworkCredential(proxy[2], proxy[3]);
                }
            }
            catch
            {
                wp = default;
            }
            string getstatus = "https://api.nike.com/buy/checkouts/v2/" + GID;
            HttpWebRequest reqgetstatus = (HttpWebRequest)WebRequest.Create(getstatus);
            reqgetstatus.Proxy = wp;
            reqgetstatus.Method = "PUT";
            reqgetstatus.Accept = "application/json";
            reqgetstatus.ContentType = "application/json; charset=UTF-8";
            byte[] paymenttokeninfo = Encoding.UTF8.GetBytes(payload);
            reqgetstatus.Headers.Add("authorization", Authorization);
            if (Mainwindow.iscookielistnull)
            {
                reqgetstatus.Headers.Add("Cookie", "");
            }
            else
            {
                if ((Config.cjevent != "") && (Config.cid != ""))
                {
                reloadcookie: Random ra = new Random();
                    int sleeptime = ra.Next(0, 500);
                    Thread.Sleep(sleeptime);
                    if (Mainwindow.lines.Count == 0)
                    {
                        tk.Status = "No Cookie";
                        Mainwindow.iscookielistnull = true;
                        Thread.Sleep(3600000);
                    }
                    int cookie = ra.Next(0, Mainwindow.lines.Count);
                    try
                    {
                        reqgetstatus.Headers.Add("Cookie", Mainwindow.lines[cookie] + "; nike_cid=" + Config.cid + "; cid=" + Config.cid + "%7C" + Config.cjevent + "");
                        Mainwindow.lines.RemoveAt(cookie);
                    }
                    catch (Exception)
                    {
                        goto reloadcookie;
                    }

                }
                else
                {
                reloadcookie: Random ra = new Random();
                    int sleeptime = ra.Next(0, 500);
                    Thread.Sleep(sleeptime);
                    if (Mainwindow.lines.Count == 0)
                    {
                        tk.Status = "No Cookie";
                        Mainwindow.iscookielistnull = true;
                        Thread.Sleep(3600000);
                    }
                    int cookie = ra.Next(0, Mainwindow.lines.Count);
                    try
                    {
                        reqgetstatus.Headers.Add("Cookie", Mainwindow.lines[cookie]);
                        Mainwindow.lines.RemoveAt(cookie);
                    }
                    catch (Exception)
                    {
                        goto reloadcookie;
                    }

                }
            }
            Main.updatelable();
            reqgetstatus.Headers.Add("accept-encoding", "gzip, deflate,br");
            reqgetstatus.Headers.Add("accept-language", "en-US, en; q=0.9");
            reqgetstatus.Headers.Add("appid", "com.nike.commerce.checkout.web");
            reqgetstatus.Headers.Add("Origin", "https://www.nike.com");
            reqgetstatus.Referer = "https://www.nike.com/us/en/checkout";
            reqgetstatus.ContentLength = paymenttokeninfo.Length;
            reqgetstatus.Headers.Add("sec-fetch-dest", "empty");
            reqgetstatus.Headers.Add("sec-fetch-mode", "cors");
            reqgetstatus.Headers.Add("sec-fetch-site", "same-site");
            reqgetstatus.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
            reqgetstatus.Headers.Add("x-b3-spanname", "CiCCheckout");
            reqgetstatus.Headers.Add("x-b3-traceid", xb3traceid);
            Stream paymenttokenstream = reqgetstatus.GetRequestStream();
            paymenttokenstream.Write(paymenttokeninfo, 0, paymenttokeninfo.Length);
            paymenttokenstream.Close();
            try
            {
                HttpWebResponse respgetstatus = (HttpWebResponse)reqgetstatus.GetResponse();

            }
            catch (WebException ex)
            {
                HttpWebResponse respjob = (HttpWebResponse)ex.Response;
                tk.Status = "Processing failed";
                Thread.Sleep(2000);
                goto D;
            }
        }
        public string finalorder(string url, string Authorization, string profile, Main.taskset tk, string pid, string size, string code, string giftcard, string username, string password, bool randomsize, CancellationToken ct)
        {
            string status = "";
            for (int i = 0; i < 10; i++)
            {
                if (ct.IsCancellationRequested)
                {
                    tk.Status = "IDLE";
                    ct.ThrowIfCancellationRequested();
                }
                WebProxy wp = new WebProxy();
                try
                {
                    int random = ran.Next(0, Mainwindow.proxypool.Count);
                    string proxyg = Mainwindow.proxypool[random].ToString();
                    string[] proxy = proxyg.Split(":");
                    if (proxy.Length == 2)
                    {
                        wp.Address = new Uri("http://" + proxy[0] + ":" + proxy[1] + "/");

                    }
                    else if (proxy.Length == 4)
                    {
                        wp.Address = new Uri("http://" + proxy[0] + ":" + proxy[1] + "/");
                        wp.Credentials = new NetworkCredential(proxy[2], proxy[3]);
                    }
                }
                catch
                {
                    wp = default;
                }
                HttpWebRequest reqfinal = (HttpWebRequest)HttpWebRequest.Create(url);
                reqfinal.Proxy = wp;
                reqfinal.Method = "GET";
                reqfinal.Accept = "application/json";
                reqfinal.ContentType = "application/json; charset=UTF-8";
                reqfinal.Headers.Add("authorization", Authorization);
                reqfinal.Headers.Add("Accept-Encoding", "gzip, deflate");
                reqfinal.Headers.Add("Accept-Language", "en-US, en; q=0.9");
                reqfinal.Headers.Add("Origin", "https://www.nike.com");
                reqfinal.Referer = "https://www.nike.com/us/en/checkout";
                reqfinal.Headers.Add("Sec-Fetch-Dest", "empty");
                reqfinal.Headers.Add("Sec-Fetch-Mode", "cors");
                reqfinal.Headers.Add("Sec-Fetch-Site", "same-site");
                reqfinal.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
                reqfinal.Headers.Add("x-b3-spanname", "CiCCheckout");
                reqfinal.Headers.Add("x-b3-traceid", xb3traceid);
                try
                {
                    HttpWebResponse respfinal = (HttpWebResponse)reqfinal.GetResponse();
                    Thread.Sleep(2000);
                    tk.Status = "Submit Order";
                    if (respfinal.ContentEncoding == "gzip")
                    {
                        Stream receive2Stream = respfinal.GetResponseStream();
                        StreamReader read = new StreamReader(new GZipStream(receive2Stream, CompressionMode.Decompress), Encoding.GetEncoding("utf-8"));
                        status = read.ReadToEnd();
                    }
                    else
                    {
                        Stream finalstream = respfinal.GetResponseStream();
                        StreamReader readfinalstream = new StreamReader(finalstream, Encoding.UTF8);
                        status = readfinalstream.ReadToEnd();
                    }
                }
                catch (WebException ex)
                {
                    HttpWebResponse respjob = (HttpWebResponse)ex.Response;
                    Stream jobstream = respjob.GetResponseStream();
                    StreamReader readjobstream = new StreamReader(jobstream, Encoding.UTF8);
                    status = readjobstream.ReadToEnd();
                    tk.Status = "Error";

                }
                if ((status.Contains("COMPLETED") == true) && (status.Contains("OUT_OF_STOCK")))
                {
                    /*  NikeUSUK UKUS = new NikeUSUK();
                      UKUS.pid = pid;
                      UKUS.size = size;
                      UKUS.profile = profile;
                      UKUS.code = code;
                      UKUS.giftcard = giftcard;
                      UKUS.username = username;
                      UKUS.password = password;
                      UKUS.randomsize = randomsize;
                      tk.Status = "OOS Retring";
                      UKUS.StartTask(ct);*/
                }
                if (status.Contains("IN_PROGRESS") == true)
                {
                    tk.Status = "Processing";
                }
                if (status.Contains("COMPLETED") == true)
                {
                    break;
                }
            }
            return status;
        }
    }
}

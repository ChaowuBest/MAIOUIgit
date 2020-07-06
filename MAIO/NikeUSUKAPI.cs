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
                            Mainwindow.iscookielistnull = true;
                        C: tk.Status = "No Cookie";

                            if (Mainwindow.iscookielistnull)
                            {
                                goto C;
                            }
                            else
                            {
                                goto D;
                            }
                        }
                    D:   int cookie = ra.Next(0, Mainwindow.lines.Count);
                        try
                        {
                            // req.Headers.Add("Cookie", @"AnalysisUserId=221.194.158.93.285171593333672995; s_ecid=MCMID%7C32673145006496229698121162528714794698; AMCVS_F0935E09512D2C270A490D4D%40AdobeOrg=1; _gcl_au=1.1.1325209806.1593333679; _scid=66e3828e-be05-4efd-acc6-5d5a39ba47ff; _ga=GA1.2.121498077.1593333681; _fbp=fb.1.1593333680721.1941026238; _pin_unauth=dWlkPVpHSTNZamszTUdNdE5qWmpZaTAwTURrMUxXRmlPVE10TVRCaE56ZzNNMlEyTkRjeA; RES_TRACKINGID=538324262197419; ResonanceSegment=1; guidU=c012cbd4-744a-4adc-905f-51c65708edd5; CONSUMERCHOICE_SESSION=t; cid=undefined%7Cundefined; optimizelyEndUserId=oeu1593441033066r0.43937108213315823; sq=3; siteCatalyst_sample=30; dreamcatcher_sample=17; neo_sample=31; guidS=d4828455-44d7-4d26-a324-c5391127e1dc; guidSTimestamp=1593748523504|1593748539512; crl8.fpcuid=ca61d487-38b2-4b76-837f-c58039b0703d; bc_nike_great_britain_triggermail=%7B%22distinct_id%22%3A%20%2217312c2a26420d-0eba6901676bcf-7f2f4867-1704a0-17312c2a2651e3%22%2C%22cart_total%22%3A%20990%7D; _sctr=1|1593878400000; geoloc=cc=CN,rc=JL,tp=vhigh,tz=GMT+8,la=43.89,lo=125.32; fs_uid=rs.fullstory.com#BM7A6#6266740190363648:5987240799977472/1622087170; AMCV_F0935E09512D2C270A490D4D%40AdobeOrg=1994364360%7CMCMID%7C32673145006496229698121162528714794698%7CMCAID%7CNONE%7CMCOPTOUT-1594023508s%7CNONE%7CvVersion%7C3.4.0%7CMCIDTS%7C18450%7CMCAAMLH-1594621108%7C11%7CMCAAMB-1594621108%7Cj8Odv6LonN4r3an7LhD3WZrU1bUpAkFkkiY1ncBR96t2PTI; _gid=GA1.2.1454253817.1594016308; APID=80AAA3327660C6BEBAD3233BAC049859.sin-341-app-ap-0; USID=323FEBD760B38133C5D9FF93CA217086.sin-234-app-us-0; guidA=c39ec2dd3662000055c2025ff5000000e1aa0100; utag_main=_st:1594018143653$ses_id:1594016653254%3Bexp-session; s_sess=%20c51%3Dhorizontal%3B%20s_cc%3Dtrue%3B%20tp%3D1657%3B%20s_ppv%3Dnikecom%25253Echeckout%25253Eshipping%252C86%252C54%252C1419%3B; lls=3; bc_nike_australia_triggermail=%7B%22distinct_id%22%3A%20%22172fa168340242-0b5f83865ca03a-7f2f4867-1704a0-172fa168341152%22%2C%22cart_total%22%3A%200%2C%22ch%22%3A%20%22-2143685126%22%7D; slCheck=0AglhUAFmYY0QJzHVwuhPN242VxPbd5+rgGqmNifRQoT3pycEFMHtr+qf77egHFFRuL3dDGyAZszkEL3gDf41Xst6CU9NIbdycGvc94AC548AOPpNeCoTYq/pVkVWCXg; llCheck=VRQGQ4cJGqg46IGbnQUgEU1SA/DKKWEW0/hG+PC66n2X7nsCnnOCXwd5kzGMUdlT97ywUtZxyTczpikdXd9CnQWPvmIj4UgfPDTJO9RJBkqD9nOHOxiNLwqSTZWMOutsttnGoTlstziYHXfYrpGy/N+USgznBM1k6D7Y0o7aj5Q=; sls=3; bc_nike_canada_triggermail=%7B%22distinct_id%22%3A%20%2217322da68a2221-06411c10a63e05-7f2f4867-1704a0-17322da68a3396%22%2C%22cart_total%22%3A%200%2C%22ch%22%3A%20%22237271598%22%7D; NIKE_COMMERCE_COUNTRY=US; NIKE_COMMERCE_LANG_LOCALE=en_US; nike_locale=us/en_us; CONSUMERCHOICE=us/en_us; bm_sz=62872E82BA02A08A4E5A87CD31E1DE01~YAAQZfJUuCHEqxBzAQAADKtKJAhWtHCZr97s2P+NGXCvJSru/GT8QZvdZ/SsQtO8/6l/XHx5qWbgBBC2vuQT9SxBo9VXy0/JJropyPuUyb0+7cJ9OpHUcr5VQg+FpEkTQkRgG7JhGH7ffhzXBPWVXMb+Ze/QzQsjtO3LA8TB+EAqM+FBd3+xBxAknu9HZQ==; AKA_A2=A; ak_bmsc=17826697A2445C54140B36679317C6B5DDC29EC3366200007E4A035F3DB17172~pl6vaQdDs5OnGQ8IJTwK/DUP7vwUXvrkZQOBRrLkPM5AUIIiby7oEYjt6VIv62Px8QZ8U9RBB6L1WBA1LRbNRPPIeDR9MMCbJfGakml56pAzSJYMwZQecGgVIif+ULEcNcad9DXHll+vEuroy1G4ItfV2mSQisI6Mz5bH675klkicYCjhfGMuRR1uhahr7r003crZta+ZXr5WqN1dKOoHudFZA/81rg3aVAgZsDA1cU++BTfdOd1sX/aZ8MU5dQKK5; RES_SESSIONID=86018542900823; ppd=pw|nikecom>pw>men_footwear_lifestyle; anonymousId=C9A7306EDB36C7893E331DBCA5769EEA; _uetsid=4b2a1b7d-47b2-1039-e8c5-560daf8d4989; _uetvid=bcb6a662-0ee9-f733-67d0-347ba1d3af85; bc_nike_triggermail=%7B%22distinct_id%22%3A%20%22172f89e3f24529-05ee350331064a-7f2f4867-1704a0-172f89e3f25684%22%2C%22g_search_engine%22%3A%20%22google%22%2C%22cart_total%22%3A%200%2C%22ch%22%3A%20%22237271598%22%7D; _abck=603FF8E56A8A109B9EB7D14B756EB9E3~-1~YAAQw57C3RUhShNzAQAAu3PbJATESHtqT/iIy0SnDeYCl8pDmdRlK4L+2JRXo6xVjPoPyupp3NhUuVcmlfzUoo+gMgqKEgTlRjg0vb68a/6ZJHFDVEvNcDhpOdtsW73uWpJSG7evn5CoBo2nLXOhYEdic7FeHtJk+r5a6LSJp9Ga1EmvCSEbnQ4XPTSGx0E1dVIdozQEKi2fxRfk08URDuaGyajCS6jMNVfLMl7J2Wf+Bb/o8B3Ur65o1nHu9RLc1gZkGFMV9vkeKp51/F4eUcia4CPgS/rPQbkPhk7O8Y6tGH0D/c3gN7GeCRo0AEpRwO7OQBFGAw4FCiuIGG7nFy38Whwlirb8+u2sX/OCf6U=~-1~-1~-1; bm_sv=50175EFBD50E5364B050F26620435006~mfYAnRN9KRNVrmqALERhP75xpnO/jFlmxxYZB7jI6GKGFpel8LE2mleidb2Y1TwEt77+wsTSG3pIitlijZEg3myFV7WpD4M5ozsiwjOB7bN7ZFcxhM3l3uNUlhN9HFEo+4by3fP3558MAVzcBKlwq8U+6g+UMOCJsqaashlsL98=");
                            req.Headers.Add("Cookie", Mainwindow.lines[cookie] + "; nike_cid=" + Config.cid + "; cid=" + Config.cid + "%7C" + Config.cjevent + "");
                            Main.updatelable();
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
                            Main.updatelable();
                            Mainwindow.lines.RemoveAt(cookie);
                        }
                        catch (Exception)
                        {
                            goto reloadcookie;
                        }
                    }
                }
            }        
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
                        Mainwindow.iscookielistnull = true;
                       C: tk.Status = "No Cookie";

                        if (Mainwindow.iscookielistnull)
                        {
                            goto C;
                        }
                        else
                        {
                            goto D;
                        }
                    }
                  D: int cookie = ra.Next(0, Mainwindow.lines.Count);
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
                        Mainwindow.iscookielistnull = true;
                    C: tk.Status = "No Cookie";

                        if (Mainwindow.iscookielistnull)
                        {
                            goto C;
                        }
                        else
                        {
                            goto D;
                        }
                    }
                  D:  int cookie = ra.Next(0, Mainwindow.lines.Count);
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
                        Mainwindow.iscookielistnull = true;
                    C: tk.Status = "No Cookie";

                        if (Mainwindow.iscookielistnull)
                        {
                            goto C;
                        }
                        else
                        {
                            goto E;
                        }
                    }
                  E: int cookie = ra.Next(0, Mainwindow.lines.Count);
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

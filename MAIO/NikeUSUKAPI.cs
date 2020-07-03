using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;

namespace MAIO
{
    class NikeUSUKAPI
    {
        Random ran = new Random();
        string xb3traceid = Guid.NewGuid().ToString();
        string xnikevisitorid = Guid.NewGuid().ToString();
        public string GetHtmlsource(string url, Main.taskset tk)
        {
         A: WebProxy wp = new WebProxy();
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
        public string Postlogin(string url, string logininfo, bool isrefresh, string account,Main.taskset tk, CancellationToken ct)
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
                            //  string geoloc = "";
                            //  string bm_sz = "";
                            //  string abck = "";
                        //    req.Headers.Add("Cookie",Mainwindow.lines[cookie]);
                         //    req.Headers.Add("Cookie", "_abck="+abck+ ";bm_sz="+bm_sz+ ";geoloc="+ geoloc);
                           // req.Headers.Add("Cookie", @"D77105DD1D69007F45A46BC71E7AA3B4~-1~YAAQCHTA0iNgOLhyAQAA7XUv6wQm/kJOKGFShyzI0uWavTvdVheHyuBIFsfL/X5IANnyqRhlKkvtoSNKjjWGTj+mt3f6mwFZweHQY3nMyHFycPZuFMr61O9OUscHtqKt4cXJMe+vlC86zHeNhXGCSWaBi+IIjBIA3PDuCZksuAc2HkY8lkjw0VIXLp+LaZMfF/ctCRfpEwL52DaRZkqtVTrOAqfTGBxI5BbyKxumallcvAG6Kuao6eXwzsCRf9jQt4IXrkcZAI4VfBBvnNTklO0vj0NW4FgiNHQ/1F4552QJDYWp8Wo9Hx7lQg5dQjn/nsbbeV8qeojW+np77g1PlSkTmTkSCnRuRkP4J8nUdsI=~-1~-1~-1");
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
               // MessageBox.Show("login");
                Stream tokenStream = response.GetResponseStream();
                StreamReader readhtmlStream = new StreamReader(tokenStream, Encoding.UTF8);
                token = readhtmlStream.ReadToEnd();
            }
            catch (WebException ex)
            {
                HttpWebResponse respgethtml = (HttpWebResponse)ex.Response;
                Stream tokenStream = respgethtml.GetResponseStream();
                StreamReader readhtmlStream = new StreamReader(tokenStream, Encoding.UTF8);
                var chao=readhtmlStream.ReadToEnd();
                tk.Status = "Login Failed";
                goto retry;

            }
            JObject jo = JObject.Parse(token);
            string Authorization = "Bearer " + jo["access_token"].ToString();
            refresh.Add(jo["refresh_token"].ToString() + "|" + account);
            return Authorization;
        }
        public static ArrayList refresh = new ArrayList();
        public void writerefreshtoken()
        {
            try
            {
                if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "refreshtoken.txt"))
                {

                    FileStream fs1 = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "refreshtoken.txt", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                    StreamWriter sw = new StreamWriter(fs1);
                    for (int i = 0; i < refresh.Count; i++)
                    {
                        sw.WriteLine(refresh[i]);
                    }
                    sw.Close();
                    fs1.Close();
                }
                else
                {
                    FileStream fs = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "refreshtoken.txt", FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                    StreamWriter sr = new StreamWriter(fs);
                    for (int i = 0; i < refresh.Count; i++)
                    {
                        sr.WriteLine(refresh[i]);
                    }
                    sr.Close();
                    fs.Close();
                }
            }
            catch (Exception)
            {
               // Console.ForegroundColor = ConsoleColor.Red;
               // Console.WriteLine("[" + new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() + "]" + "Failed to write refreshtoken");
            }
        }
        public double Postcardinfo(string url, string cardinfo, string Authorization, string cardguid,Main.taskset tk,CancellationToken ct)
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
        public void CheckoutPreview(string url, string Authorization, string checkoutpayload, string GID,Main.taskset tk, CancellationToken ct)
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
                        reqpayment.Headers.Add("Cookie",Mainwindow.lines[cookie] + "; nike_cid=" + Config.cid + "; cid=" + Config.cid + "%7C" + Config.cjevent + "");
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
                writerefreshtoken();
            }
            catch (WebException ex)
            {
                HttpWebResponse resppayment = (HttpWebResponse)ex.Response;
                tk.Status = "SubmitShipping error";
                goto B;
            }

        }
        public string CheckoutPreviewStatus(string url, string Authorization, bool isdiscount,Main.taskset tk,CancellationToken ct)
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
        public string payment(string url, string Authorization, string paymentinfo,Main.taskset tk,CancellationToken ct)
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
        public void paymentjob(string url, string Authorization,CancellationToken ct,Main.taskset tk)
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
        public void final(string Authorization, string url, string payload, string GID,Main.taskset tk,CancellationToken ct)
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
                        reqgetstatus.Headers.Add("Cookie",Mainwindow.lines[cookie] + "; nike_cid=" + Config.cid + "; cid=" + Config.cid + "%7C" + Config.cjevent + "");
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
                        reqgetstatus.Headers.Add("Cookie",Mainwindow.lines[cookie]);
                        Mainwindow.lines.RemoveAt(cookie);
                    }
                    catch (Exception)
                    {
                        goto reloadcookie;
                    }

                }
            }
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
        public string finalorder(string url, string Authorization, string profile,Main.taskset tk,string pid,string size,string code,string giftcard,string username,string password, bool randomsize,CancellationToken ct)
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

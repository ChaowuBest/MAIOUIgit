using MAIO.browsercheckout;
using NakedBot;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PuppeteerSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using static MAIO.Main;

namespace MAIO
{
    class NikeUSUKAPI
    {
        Random ran = new Random();
        string akbmsz = "ak_bmsc=";
        string xb3traceid = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 16);
        string xb3parentspanid = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 16);
        string xb3spanID = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 16);
        string servercookie = "";
        public int failedretry = 0;
        int failedsubshipp = 0;
        public int failedlogin = 0;
        public string GetHtmlsource(string url, Main.taskset tk, CancellationToken ct)
        {
            Thread.Sleep(1);
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
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
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
                if (ct.IsCancellationRequested)
                {
                    tk.Status = "IDLE";
                    ct.ThrowIfCancellationRequested();
                }
                response.Close();
                readStream.Close();
                tk.Status = "Get Size";
            }
            catch (WebException ex)
            {
                HttpWebResponse response = (HttpWebResponse)ex.Response;
                if (ct.IsCancellationRequested)
                {
                    tk.Status = "IDLE";
                    ct.ThrowIfCancellationRequested();
                }
                tk.Status = "Get Size Error";
                tk.Status = "Change Proxy";
                goto A;
            }
            return SourceCode;
        }
        public string Postlogin(string url, string logininfo, bool isrefresh, string account, Main.taskset tk, CancellationToken ct)
        {
        D: string token = null;
            if (Config.UseAdvancemode == "True")
            {
                string[] sendcookie = new string[2];
                if (Mainwindow.iscookielistnull)
                {
                    Thread.Sleep(1);
                    goto D;
                }
                else
                {
                    Random ra = new Random();
                C: if (Mainwindow.lines.Count == 0)
                    {
                        Thread.Sleep(1);
                        if (ct.IsCancellationRequested)
                        {
                            tk.Status = "IDLE";
                            ct.ThrowIfCancellationRequested();
                        }
                        Mainwindow.iscookielistnull = true;
                        tk.Status = "No Cookie";
                        goto C;
                    }
                    else
                    {
                        int cookie = ra.Next(0, Mainwindow.lines.Count);
                        try
                        {
                            if (isrefresh)
                            {
                                sendcookie[0] = "";
                                sendcookie[1] = "";
                            }
                            else
                            {
                                updatelable(Mainwindow.lines[cookie], false);
                                sendcookie = Mainwindow.lines[cookie].Split(";");
                                Mainwindow.lines.RemoveAt(cookie);
                            }
                        }
                        catch (Exception)
                        {
                            goto C;
                        }
                    }
                }
                string proxy = "";
                try
                {
                    if (ct.IsCancellationRequested)
                    {
                        tk.Status = "IDLE";
                        ct.ThrowIfCancellationRequested();
                    }
                    int random = ran.Next(0, Mainwindow.proxypool.Count);
                    proxy = Mainwindow.proxypool[random].ToString();
                }
                catch
                {
                    proxy = "";
                }
                try
                {
                    var chao = JsonConvert.SerializeObject(logininfo).Replace("\"{", "{").Replace("}\"", "}");
                    string json = "{\"data\":{\"headers\":{\"Content-Type\":\"application/json\",\"Origin\":\" https://www.nike.com\",\"Accept\":\"application/json\"},\"url\":\"" + url + "\",\"method\":\"POST\",\"data\":\"" + chao + "\",\"proxy\":\"\",\"cookies\":[{\"Name\":\"_abck\",\"TimeStamp\":\"" + DateTime.Now.ToLocalTime().ToString() + "\",\"Value\":\"" + sendcookie[1].Replace("_abck=", "") + "\",\"Comment\":\"\",\"CommentUri\":null,\"HttpOnly\":false,\"Discard\":false,\"Expired\":false,\"Secure\":false,\"Domain\":\".nike.com\",\"Expires\":\"0001-01-01T00:00:00\",\"Path\":\"/\",\"Port\":\"\",\"Version\":0},{\"Name\":\"bm_sz\",\"TimeStamp\":\"" + DateTime.Now.ToLocalTime().ToString() + "\",\"Value\":\"" + sendcookie[0].Replace("bm_sz=", "") + "\",\"Comment\":\"\",\"CommentUri\":null,\"HttpOnly\":false,\"Discard\":false,\"Expired\":false,\"Secure\":false,\"Domain\":\".nike.com\",\"Expires\":\"0001-01-01T00:00:00\",\"Path\":\"/\",\"Port\":\"\",\"Version\":0}],\"id\":\"" + tk.Taskid + "\"},\"type\":\"request\"}";
                    if (ct.IsCancellationRequested)
                    {
                        tk.Status = "IDLE";
                        ct.ThrowIfCancellationRequested();
                    }
                    Main.allSockets[0].Send(json);
                    bool fordidden = false;
                B: JObject sValue = null;
                    try
                    {
                        Random ra = new Random();
                        int sleep = ra.Next(0, 3);
                        Thread.Sleep(sleep);
                        if (returnstatus.TryGetValue(tk.Taskid, out sValue))
                        {
                            if (ct.IsCancellationRequested)
                            {
                                tk.Status = "IDLE";
                                ct.ThrowIfCancellationRequested();
                            }
                            else if (sValue.ToString().Contains("Failed to fetch"))
                            {
                                tk.Status = "Forbidden";
                                fordidden = true;
                                returnstatus.Remove(tk.Taskid);
                            }
                            else if (sValue["status"].ToString() == "200")
                            {
                                tk.Status = "Login Successful";
                                token = sValue["text"].ToString();
                                returnstatus.Remove(tk.Taskid);
                            }
                            else if (sValue["status"].ToString() == "403")
                            {
                                tk.Status = "Forbidden";
                                fordidden = true;
                                returnstatus.Remove(tk.Taskid);
                            }
                            else if (sValue["status"].ToString() == "401")
                            {
                                tk.Status = "User/Pw Wrong";
                                fordidden = true;
                                returnstatus.Remove(tk.Taskid);
                            }
                        }
                        else
                        {
                            Thread.Sleep(sleep);
                            goto B;
                        }
                    }
                    catch (NullReferenceException)
                    {
                        Thread.Sleep(2);
                        goto B;
                    }
                    catch (OperationCanceledException)
                    {
                        Thread.Sleep(2);
                        return "";
                    }
                    if (fordidden)
                    {
                        failedlogin++;
                        if (failedlogin > 20)
                        {
                            failedlogin = 0;
                            Main.autorestock(tk);
                            if (ct.IsCancellationRequested)
                            {
                                tk.Status = "IDLE";
                                ct.ThrowIfCancellationRequested();
                            }
                        }
                        Thread.Sleep(1500);
                        goto D;
                    }
                }
                catch (OperationCanceledException)
                {
                    return "";

                }
                catch (NullReferenceException)
                {
                    goto D;
                }
            }
            else
            {
                Thread.Sleep(1);
            retry: int random = ran.Next(0, Mainwindow.proxypool.Count);
                if (ct.IsCancellationRequested)
                {
                    tk.Status = "IDLE";
                    ct.ThrowIfCancellationRequested();
                }
                string proxyaddress = null;
                try
                {
                    string proxyg = Mainwindow.proxypool[random].ToString();
                    string[] proxy = proxyg.Split(":");
                    if (proxy.Length == 2)
                    {
                        proxyaddress = "http//" + proxy[0] + ":" + proxy[1] + "";
                    }
                    else if (proxy.Length == 4)
                    {
                        proxyaddress = "http://" + proxy[2] + ":" + proxy[3] + "@" + proxy[0] + ":" + proxy[1] + "";
                    }
                }
                catch
                {
                    proxyaddress = "";
                }
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                byte[] contentByte = Encoding.UTF8.GetBytes(logininfo);
                req.Method = "POST";
                req.Headers.Add("Server-Host", "unite.nike.com:443");
                req.Headers.Add("Proxy-Address", proxyaddress);
                req.ContentLength = contentByte.Length;
                req.Accept = "*/*";
                req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.105 Safari/537.36";
                req.ContentType = "application/json";
                req.Headers.Add("origin", "https://www.nike.com");
                req.Headers.Add("Sec-Fetch-Site", "same-site");
                req.Headers.Add("Sec-Fetch-Dest", "empty");
                req.Headers.Add("Sec-Fetch-Mode", "cors");
                req.Referer = "https://www.nike.com/launch/";
                req.Headers.Add("Accept-encoding", "gzip, deflate, br");
                req.Headers.Add("Accept-Language", "en-US,en;q=0.9");
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
                            int sleeptime = ra.Next(0, 100);
                            Thread.Sleep(sleeptime);
                            if (Mainwindow.lines.Count == 0)
                            {
                                Mainwindow.iscookielistnull = true;
                            C: if (ct.IsCancellationRequested)
                                {
                                    tk.Status = "IDLE";
                                    ct.ThrowIfCancellationRequested();
                                }
                                tk.Status = "No Cookie";
                                goto C;
                            }
                            else
                            {
                                int cookie = ra.Next(0, Mainwindow.lines.Count);
                                try
                                {
                                    Main.updatelable(Mainwindow.lines[cookie], false);
                                    req.Headers.Add("Cookie", Mainwindow.lines[cookie].Replace(";", "; ") + "; nike_cid=" + Config.cid + "; cid=" + Config.cid + "%7C" + Config.cjevent + "");
                                    Mainwindow.lines.RemoveAt(cookie);
                                    if (Mainwindow.lines.Count == 0)
                                    {
                                        Mainwindow.iscookielistnull = true;
                                    }
                                }
                                catch (Exception)
                                {
                                    goto reloadcookie;
                                }
                            }
                        }
                        else
                        {
                        reloadcookie: Random ra = new Random();
                            int sleeptime = ra.Next(0, 100);
                            Thread.Sleep(sleeptime);
                        E: if (Mainwindow.lines.Count == 0)
                            {
                                if (ct.IsCancellationRequested)
                                {
                                    tk.Status = "IDLE";
                                    ct.ThrowIfCancellationRequested();
                                }
                                tk.Status = "No Cookie";
                                Mainwindow.iscookielistnull = true;
                                goto E;
                            }
                            else
                            {
                                int cookie = ra.Next(0, Mainwindow.lines.Count);
                                try
                                {
                                    req.Headers.Add("Cookie", Mainwindow.lines[cookie].Replace(";", "; "));
                                    Main.updatelable(Mainwindow.lines[cookie], false);
                                    Mainwindow.lines.RemoveAt(cookie);
                                }
                                catch (Exception)
                                {
                                    goto reloadcookie;
                                }
                            }

                        }
                    }
                }
                Stream webstream = req.GetRequestStream();
                webstream.Write(contentByte, 0, contentByte.Length);
                webstream.Close();
                try
                {
                    HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                    var wu = response.Headers["Set-Cookie"];
                    tk.Status = "Login Successful!";
                    Stream tokenStream = response.GetResponseStream();
                    StreamReader readhtmlStream = new StreamReader(tokenStream, Encoding.UTF8);
                    token = readhtmlStream.ReadToEnd();
                }
                catch (WebException ex)
                {
                    if (ex.Message.Contains("Email"))
                    {
                        tk.Status = "Wrong Password";
                    }
                    else if (ex.Message.Contains("401"))
                    {
                        tk.Status = "Refrestoken Error";
                    }
                    else
                    {
                        tk.Status = "Login Failed";
                    }
                    HttpWebResponse respgethtml = (HttpWebResponse)ex.Response;
                    Stream tokenStream = respgethtml.GetResponseStream();
                    StreamReader readhtmlStream = new StreamReader(tokenStream, Encoding.UTF8);
                    var chao = readhtmlStream.ReadToEnd();

                    Thread.Sleep(1000);
                    goto retry;
                }
            }
            JObject jo = null;
            try
            {
                jo = JObject.Parse(token);
            }
            catch (ArgumentNullException)
            {
                returnstatus.Remove(tk.Taskid);
                Main.autorestock(tk);
                if (ct.IsCancellationRequested)
                {
                    tk.Status = "IDLE";
                    ct.ThrowIfCancellationRequested();
                }
            }
            jo.ToString();
            string Authorization = "Bearer " + jo["access_token"].ToString();
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            Task task1 = new Task(() => writerefreshtoken("[{\"Token\":\"" + jo["refresh_token"].ToString() + "\",\"Account\":\"" + account + "\"}]", account));
            task1.Start();
            return Authorization;
        }
        public void writerefreshtoken(string token, string account)
        {
            Thread.Sleep(1);
            try
            {
                if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "refreshtoken.json"))
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
            Thread.Sleep(1);
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
            reqcard.ContentLength = contentcardinfo.Length;
            reqcard.Headers.Add("sec-fetch-dest", "empty");
            reqcard.Headers.Add("sec-fetch-mode", "cors");
            reqcard.Headers.Add("sec-fetch-site", "same-site");
            reqcard.Headers.Add("appid", "com.nike.commerce.checkout.web");
            reqcard.Headers.Add("X-B3-SpanId", xb3spanID);
            reqcard.Headers.Add("X-B3-ParentSpanId", xb3parentspanid);
            reqcard.Headers.Add("X-B3-TraceId", xb3traceid);
            reqcard.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.105 Safari/537.36";
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
                if (ex.Message.Contains("429"))
                {
                    HttpWebResponse processpayment = (HttpWebResponse)ex.Response;
                    Stream processtream = processpayment.GetResponseStream();
                    StreamReader readprocessstream = new StreamReader(processtream, Encoding.UTF8);
                    string processcode = readprocessstream.ReadToEnd();
                    tk.Status = processcode;
                }
                tk.Status = "Submit Card failed";
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
            Thread.Sleep(1);
            int random = ran.Next(0, Mainwindow.proxypool.Count);
            string proxyaddress = null;
            try
            {
                string proxyg = Mainwindow.proxypool[random].ToString();
                string[] proxy = proxyg.Split(":");
                if (proxy.Length == 2)
                {
                    proxyaddress = "http//" + proxy[0] + ":" + proxy[1] + "";
                }
                else if (proxy.Length == 4)
                {
                    proxyaddress = "http://" + proxy[2] + ":" + proxy[3] + "@" + proxy[0] + ":" + proxy[1] + "";
                }
            }
            catch
            {
                proxyaddress = "";
            }
            HttpWebRequest reqpayment = (HttpWebRequest)WebRequest.Create(url);
            reqpayment.Method = "PUT";
            reqpayment.Headers.Add("Server-Host", "api.nike.com:443");
            reqpayment.Headers.Add("Proxy-Address", proxyaddress);
            reqpayment.ContentType = "application/json; charset=UTF-8";
            byte[] contentpaymentinfo = Encoding.UTF8.GetBytes(checkoutpayload);
            reqpayment.Accept = "application/json";
            reqpayment.Headers.Add("Accept-Encoding", "gzip, deflate");
            reqpayment.Headers.Add("Accept-Language", "en-US, en; q=0.9");
            reqpayment.Headers.Add("Authorization", Authorization);
            reqpayment.Headers.Add("appid", "com.nike.commerce.nikedotcom.web");
            reqpayment.ContentLength = contentpaymentinfo.Length;
            reqpayment.Referer = "https://www.nike.com/";
            reqpayment.Headers.Add("Origin", "https://www.nike.com");
            reqpayment.Headers.Add("Sec-Fetch-Dest", "empty");
            reqpayment.Headers.Add("Sec-Fetch-Mode", "cors");
            reqpayment.Headers.Add("Sec-Fetch-Site", "same-site");
            reqpayment.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.105 Safari/537.36";
            reqpayment.Headers.Add("X-B3-SpanId", xb3spanID);
            reqpayment.Headers.Add("X-B3-ParentSpanId", xb3parentspanid);
            reqpayment.Headers.Add("X-B3-TraceId", xb3traceid);
            Stream paymentstream = reqpayment.GetRequestStream();
            paymentstream.Write(contentpaymentinfo, 0, contentpaymentinfo.Length);
            paymentstream.Close();
            string paymentsuccesscode = "";
            try
            {
                HttpWebResponse resppayment = (HttpWebResponse)reqpayment.GetResponse();
                var wu = resppayment.Headers["Set-Cookie"];
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
                Thread.Sleep(1500);
                failedsubshipp++;
                if (failedsubshipp > 20)
                {
                    Main.autorestock(tk);
                }
                Stream resppaymentStream = resppayment.GetResponseStream();
                StreamReader readpaymenthtmlStream = new StreamReader(resppaymentStream, Encoding.UTF8);
                paymentsuccesscode = readpaymenthtmlStream.ReadToEnd();
                goto B;
            }

        }
        public string CheckoutPreviewStatus(string url, string Authorization, bool isdiscount, Main.taskset tk, CancellationToken ct, string profile, string pid, string size, string code, string giftcard, string username, string password, bool randomsize, string productid, string skuid)
        {

        A: if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            Thread.Sleep(1);
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
            reqcheckstatus.Headers.Add("appid", "com.nike.commerce.snkrs.web");
            reqcheckstatus.Headers.Add("Origin", "https://www.nike.com");
            reqcheckstatus.Headers.Add("Sec-Fetch-Dest", "empty");
            reqcheckstatus.Headers.Add("Sec-Fetch-Mode", "cors");
            reqcheckstatus.Headers.Add("Sec-Fetch-Site", "same-site");
            reqcheckstatus.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.105 Safari/537.36";
            reqcheckstatus.Headers.Add("X-B3-SpanId", xb3spanID);
            reqcheckstatus.Headers.Add("X-B3-ParentSpanId", xb3parentspanid);
            reqcheckstatus.Headers.Add("X-B3-TraceId", xb3traceid);
            try
            {
                HttpWebResponse respcheckstatus = (HttpWebResponse)reqcheckstatus.GetResponse();
                var wu = respcheckstatus.Headers["Set-Cookie"];
                string cookiename = "ak_bmsc";
                Regex rex3 = new Regex(@"(?<=" + cookiename + "=)([^;]+)");
                akbmsz += rex3.Match(wu).ToString();
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
                        tk.Status = "Non buyable product(s)";
                        Main.autorestock(tk);
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
                    //   MessageBox.Show(check);
                    if (check.Contains("IN_PROGRESS"))
                    {
                        Thread.Sleep(1000);
                        goto A;
                    }
                    if (check.Contains("Non buyable product(s)"))
                    {
                        tk.Status = "Non buyable product(s)";
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
            Thread.Sleep(1);
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
            reqprocess.Headers.Add("Authorization", Authorization);
            reqprocess.Headers.Add("Cookie", akbmsz);
            reqprocess.Headers.Add("appid", "com.nike.commerce.snkrs.web");
            reqprocess.Headers.Add("Accept-Encoding", "gzip, deflate");
            reqprocess.Headers.Add("Accept-Language", "en-US, en; q=0.9");
            reqprocess.Headers.Add("Origin", "https://www.nike.com");
            reqprocess.Referer = "https://www.nike.com/us/en/checkout";
            reqprocess.Headers.Add("Sec-Fetch-Dest", "empty");
            reqprocess.Headers.Add("Sec-Fetch-Mode", "cors");
            reqprocess.Headers.Add("Sec-Fetch-Site", "same-site");
            reqprocess.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.105 Safari/537.36";
            reqprocess.Headers.Add("X-B3-SpanId", xb3spanID);
            reqprocess.Headers.Add("X-B3-ParentSpanId", xb3parentspanid);
            reqprocess.Headers.Add("X-B3-TraceId", xb3traceid);
            Stream processstream = reqprocess.GetRequestStream();
            processstream.Write(processbyteinfo, 0, processbyteinfo.Length);
            processstream.Close();
            string processcode = "";
            try
            {
                HttpWebResponse processpayment = (HttpWebResponse)reqprocess.GetResponse();
                var wu = processpayment.Headers["Set-Cookie"];
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
                processcode = readprocessstream.ReadToEnd();
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
            Thread.Sleep(1);
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
            reqjob.Headers.Add("Authorization", Authorization);
            reqjob.Headers.Add("appid", "com.nike.commerce.snkrs.web");
            reqjob.Headers.Add("Accept-Encoding", "gzip, deflate");
            reqjob.Headers.Add("Accept-Language", "en-US, en; q=0.9");
            reqjob.Headers.Add("Origin", "https://www.nike.com");
            reqjob.Referer = "https://www.nike.com/us/en/checkout";
            reqjob.Headers.Add("Sec-Fetch-Dest", "empty");
            reqjob.Headers.Add("Sec-Fetch-Mode", "cors");
            reqjob.Headers.Add("Sec-Fetch-Site", "same-site");
            reqjob.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.105 Safari/537.36";
            reqjob.Headers.Add("X-B3-SpanId", xb3spanID);
            reqjob.Headers.Add("X-B3-ParentSpanId", xb3parentspanid);
            reqjob.Headers.Add("X-B3-TraceId", xb3traceid);
            string jobstatus = "";
            try
            {
                HttpWebResponse respjob = (HttpWebResponse)reqjob.GetResponse();
                var wu = respjob.Headers["Set-Cookie"];
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
        public string final(string Authorization, string url, string payload, string GID, Main.taskset tk, CancellationToken ct, string id)
        {
        D: if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            string[] sendcookie = null;
            if (Config.UseAdvancemode != "True")
            {
            E: if (ct.IsCancellationRequested)
                {
                    tk.Status = "IDLE";
                    ct.ThrowIfCancellationRequested();
                }
                Thread.Sleep(1);
                int random = ran.Next(0, Mainwindow.proxypool.Count);
                string proxyaddress = null;
                try
                {
                    string proxyg = Mainwindow.proxypool[random].ToString();
                    string[] proxy = proxyg.Split(":");
                    if (proxy.Length == 2)
                    {
                        proxyaddress = "http//" + proxy[0] + ":" + proxy[1] + "";
                    }
                    else if (proxy.Length == 4)
                    {
                        proxyaddress = "http://" + proxy[2] + ":" + proxy[3] + "@" + proxy[0] + ":" + proxy[1] + "";
                    }
                }
                catch
                {
                    proxyaddress = "";
                }
                ServicePointManager.ServerCertificateValidationCallback = ((object param0, X509Certificate param1, X509Chain param2, SslPolicyErrors param3) => true);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebRequest reqgetstatus = (HttpWebRequest)WebRequest.Create(url);
                reqgetstatus.Method = "PUT";
                reqgetstatus.Headers.Add("Server-Host", "api.nike.com:443");
                reqgetstatus.Headers.Add("Proxy-Address", proxyaddress);
                reqgetstatus.ContentType = "application/json; charset=UTF-8";
                byte[] paymenttokeninfo = Encoding.UTF8.GetBytes(payload);
                reqgetstatus.Headers.Add("Authorization", Authorization);
                if (Mainwindow.iscookielistnull)
                {
                    reqgetstatus.Headers.Add("Cookie", "");
                }
                else
                {
                reloadcookie: Random ra = new Random();
                    int sleeptime = ra.Next(0, 100);
                    Thread.Sleep(sleeptime);
                C: if (Mainwindow.lines.Count == 0)
                    {
                        if (ct.IsCancellationRequested)
                        {
                            tk.Status = "IDLE";
                            ct.ThrowIfCancellationRequested();
                        }
                        Mainwindow.iscookielistnull = true;
                        tk.Status = "No Cookie";
                        goto C;

                    }
                    else
                    {
                        int cookie = ra.Next(0, Mainwindow.lines.Count);
                        try
                        {
                            reqgetstatus.Headers.Add("Cookie", Mainwindow.lines[cookie].Replace(";", "; ") + "; " + akbmsz);
                            Main.updatelable(Mainwindow.lines[cookie], false);
                            Mainwindow.lines.RemoveAt(cookie);

                        }
                        catch (Exception)
                        {
                            goto reloadcookie;
                        }
                    }
                }
                reqgetstatus.Referer = "https://www.nike.com/us/en/checkout";
                reqgetstatus.Headers.Add("Accept-encoding", "gzip, deflate,br");
                reqgetstatus.Headers.Add("Accept-language", "en-US, en; q=0.9");
                reqgetstatus.Headers.Add("appid", "com.nike.commerce.snkrs.web");
                reqgetstatus.Headers.Add("Origin", "https://www.nike.com");
                reqgetstatus.ContentLength = paymenttokeninfo.Length;
                reqgetstatus.Headers.Add("Sec-Fetch-Dest", "empty");
                reqgetstatus.Headers.Add("Sec-Fetch-Mode", "cors");
                reqgetstatus.Headers.Add("Sec-Fetch-Site", "same-site");
                reqgetstatus.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.105 Safari/537.36";
                reqgetstatus.Headers.Add("X-B3-SpanId", xb3spanID);
                reqgetstatus.Headers.Add("X-B3-ParentSpanId", xb3parentspanid);
                reqgetstatus.Headers.Add("X-B3-TraceId", xb3traceid);
                Stream paymenttokenstream = reqgetstatus.GetRequestStream();
                paymenttokenstream.Write(paymenttokeninfo, 0, paymenttokeninfo.Length);
                paymenttokenstream.Close();
                try
                {
                    HttpWebResponse respgetstatus = (HttpWebResponse)reqgetstatus.GetResponse();
                    Stream respcheckstatusstream = respgetstatus.GetResponseStream();
                    StreamReader readcheckstatus = new StreamReader(respcheckstatusstream, Encoding.GetEncoding("utf-8"));
                    string check = readcheckstatus.ReadToEnd();
                }
                catch (WebException ex)
                {
                    HttpWebResponse respjob = (HttpWebResponse)ex.Response;
                    failedretry++;
                    if (failedretry > 20)
                    {
                        Main.autorestock(tk);
                    }
                    tk.Status = "Processing failed";
                    Thread.Sleep(500);
                    goto E;
                }
            }
            else
            {
                if (Mainwindow.iscookielistnull)
                {
                    Thread.Sleep(1);
                    goto D;
                }
                else
                {
                reloadcookie: Random ra = new Random();
                    Thread.Sleep(1);
                C: if (Mainwindow.lines.Count == 0)
                    {
                        Thread.Sleep(1);
                        if (ct.IsCancellationRequested)
                        {
                            tk.Status = "IDLE";
                            ct.ThrowIfCancellationRequested();
                        }
                        Mainwindow.iscookielistnull = true;
                        tk.Status = "No Cookie";
                        goto C;
                    }
                    else
                    {
                        int cookie = ra.Next(0, Mainwindow.lines.Count);
                        try
                        {
                            Main.updatelable(Mainwindow.lines[cookie], false);
                            sendcookie = Mainwindow.lines[cookie].Split(";");
                            Mainwindow.lines.RemoveAt(cookie);

                        }
                        catch (Exception)
                        {
                            goto reloadcookie;
                        }
                    }
                }
                string proxy = "";
                try
                {
                    int random = ran.Next(0, Mainwindow.proxypool.Count);
                    proxy = Mainwindow.proxypool[random].ToString();
                }
                catch
                {
                    proxy = "";
                }
                try
                {
                    var chao = JsonConvert.SerializeObject(payload).Replace("\"{", "{").Replace("}\"", "}");
                    string json = "{\"data\":{\"headers\":{\"Content-Type\":\"application/json\",\"Origin\":\" https://www.nike.com\",\"Accept\":\"application/json\",\"Authorization\":\"" + Authorization + "\",\"X-B3-SpanId\":\"" + xb3spanID + "\",\"X-B3-ParentSpanId\":\"" + xb3parentspanid + "\",\"appid\":\"com.nike.commerce.snkrs.web\",\"X-B3-TraceId\":\"" + xb3traceid + "\"},\"url\":\"" + url + "\",\"method\":\"PUT\",\"data\":\"" + chao + "\",\"proxy\":\"\",\"cookies\":[{\"Name\":\"_abck\",\"TimeStamp\":\"" + DateTime.Now.ToLocalTime().ToString() + "\",\"Value\":\"" + sendcookie[1].Replace("_abck=", "") + "\",\"Comment\":\"\",\"CommentUri\":null,\"HttpOnly\":false,\"Discard\":false,\"Expired\":false,\"Secure\":false,\"Domain\":\".nike.com\",\"Expires\":\"0001-01-01T00:00:00\",\"Path\":\"/\",\"Port\":\"\",\"Version\":0},{\"Name\":\"bm_sz\",\"TimeStamp\":\"" + DateTime.Now.ToLocalTime().ToString() + "\",\"Value\":\"" + sendcookie[0].Replace("bm_sz=", "") + "\",\"Comment\":\"\",\"CommentUri\":null,\"HttpOnly\":false,\"Discard\":false,\"Expired\":false,\"Secure\":false,\"Domain\":\".nike.com\",\"Expires\":\"0001-01-01T00:00:00\",\"Path\":\"/\",\"Port\":\"\",\"Version\":0}],\"id\":\"" + tk.Taskid + "\"},\"type\":\"request\"}";
                    Main.allSockets[0].Send(json);
                    bool fordidden = false;
                    tk.Status = "Submit Payment";
                B: JObject sValue = null;
                    try
                    {
                        Random ra = new Random();
                        int sleep = ra.Next(0, 3);
                        Thread.Sleep(sleep);
                        if (returnstatus.TryGetValue(tk.Taskid, out sValue))
                        {
                            if (ct.IsCancellationRequested)
                            {
                                tk.Status = "IDLE";
                                ct.ThrowIfCancellationRequested();
                            }
                            if (sValue.ToString().Contains("Failed to fetch"))
                            {
                                tk.Status = "Forbidden";
                                fordidden = true;
                                returnstatus.Remove(tk.Taskid);
                            }
                            if (sValue["status"].ToString() == "202")
                            {
                                tk.Status = "Submit Payment";
                                returnstatus.Remove(tk.Taskid);
                            }
                            else if (sValue.ToString().Contains("fail"))
                            {
                                tk.Status = "Forbidden";
                                fordidden = true;
                                returnstatus.Remove(tk.Taskid);
                            }
                            else if (sValue["status"].ToString() == "403")
                            {
                                tk.Status = "Forbidden";
                                fordidden = true;
                                returnstatus.Remove(tk.Taskid);
                            }
                            else
                            {
                                tk.Status = "Forbidden";
                                fordidden = true;
                                returnstatus.Remove(tk.Taskid);

                            }

                        }
                        else
                        {
                            Thread.Sleep(sleep);
                            goto B;
                        }
                    }
                    catch (NullReferenceException)
                    {
                        Thread.Sleep(2);
                        goto B;
                    }
                    catch (OperationCanceledException)
                    {
                        Thread.Sleep(2);
                        return "";
                    }
                    if (fordidden)
                    {
                        failedretry++;
                        if (failedretry > 20)
                        {
                            Main.autorestock(tk);
                            if (ct.IsCancellationRequested)
                            {
                                tk.Status = "IDLE";
                                ct.ThrowIfCancellationRequested();
                            }
                        }
                        Thread.Sleep(1500);
                        goto D;
                    }
                }
                catch (OperationCanceledException)
                {
                    return "";

                }
                catch (NullReferenceException)
                {
                    goto D;
                }
            }
        A: string status = "";
            for (int i = 0; i < 30; i++)
            {
                if (ct.IsCancellationRequested)
                {
                    tk.Status = "IDLE";
                    ct.ThrowIfCancellationRequested();
                }
                tk.Status = "Processing";
                Thread.Sleep(1);
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
                HttpWebRequest reqfinal = (HttpWebRequest)HttpWebRequest.Create("https://api.nike.com/buy/checkouts/v2/jobs/" + GID);
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
                reqfinal.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.105 Safari/537.36";
                reqfinal.Headers.Add("X-B3-SpanId", xb3spanID);
                reqfinal.Headers.Add("X-B3-ParentSpanId", xb3parentspanid);
                reqfinal.Headers.Add("X-B3-TraceId", xb3traceid);
                try
                {
                    HttpWebResponse respfinal = (HttpWebResponse)reqfinal.GetResponse();
                    Thread.Sleep(1500);
                    tk.Status = "Processing";
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
                    tk.Status = status;
                    goto A;
                }
                if ((status.Contains("COMPLETED") == true) && (status.Contains("error")))
                {
                    Main.autorestock(tk);
                    if (ct.IsCancellationRequested)
                    {
                        tk.Status = "IDLE";
                        ct.ThrowIfCancellationRequested();
                    }
                }
                if (status.Contains("IN_PROGRESS") == true)
                {
                    tk.Status = "Processing";
                    goto A;
                }
                if (status.Contains("COMPLETED") == true)
                {
                    break;
                }
            }
            return status;
        }
        public static long time = 0;
        private static DateTime timeStampStartTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public string[] Monitoring(string url, Main.taskset tk, CancellationToken ct, string info, bool randomsize, string skuid, bool advancemode, bool multisize, ArrayList skulist)
        {
            if (advancemode)
            {
                time = (long)(DateTime.Now.ToUniversalTime() - timeStampStartTime).TotalMilliseconds;
            }
        A: if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            Thread.Sleep(1);
            string traceid = Guid.NewGuid().ToString();
            string nikevistid = Guid.NewGuid().ToString();
            string SourceCode = "";
            string[] group = new string[2];
            int random = ran.Next(0, Mainwindow.proxypool.Count);
            WebProxy wp = new WebProxy();
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
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Proxy = wp;
            request.Method = "POST";
            request.Host = "api.nike.com";
            request.Accept = "*/*";
            request.ContentType = "application/json; charset=UTF-8";
            byte[] contentcardinfo = Encoding.UTF8.GetBytes(info);
            request.ContentLength = contentcardinfo.Length;
            request.Headers.Add("Accept-Encoding", "gzip, deflate");
            request.Headers.Add("Accept-Language", "en-US, en; q=0.9");
            request.Headers.Add("Sec-Fetch-Dest", "empty");
            request.Headers.Add("Sec-Fetch-Mode", "cors");
            request.Headers.Add("Sec-Fetch-Site", "same-site");
            request.Headers.Add("X-B3-SpanId", xb3spanID);
            request.Headers.Add("X-B3-TraceId", xb3traceid);
            request.Headers.Add("x-nike-visitid", "1");
            request.Headers.Add("x-nike-visitorid", nikevistid);
            request.Headers.Add("upgrade-insecure-requests", "1");
            request.UserAgent = "Sogou inst spider";
            Stream cardstream = request.GetRequestStream();
            cardstream.Write(contentcardinfo, 0, contentcardinfo.Length);
            cardstream.Close();
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                xb3traceid = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 16);
                xb3parentspanid = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 16);
                xb3spanID = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 16);
                tk.Status = "WaitingRestock";
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
                if (SourceCode.Contains("Product not found"))
                {
                    goto A;
                }
                JObject jo = JObject.Parse(SourceCode);
                JArray ja = JArray.Parse(jo["data"]["skus"][0]["product"]["skus"].ToString());
                for (int i = 0; i < ja.Count; i++)
                {
                    Thread.Sleep(1);
                    group[1] = jo["data"]["skus"][0]["product"]["id"].ToString();
                    if (randomsize)
                    {
                        if (ja[i]["availability"].ToString() != "False" || ja[i]["availability"].ToString() != "false")
                        {
                            if (ja[i]["availability"]["level"].ToString() == "OOS")
                            {
                            }
                            else
                            {
                                group[0] = ja[i]["id"].ToString();
                                break;
                            }
                        }
                    }
                    else if (multisize)
                    {
                        for (int n = 0; n < skulist.Count; n++)
                        {
                            Thread.Sleep(1);
                            if (ja[i]["availability"].ToString() != "False" || ja[i]["availability"].ToString() != "false")
                            {
                                if (skulist[n].ToString() == ja[i]["id"].ToString())
                                {
                                    if (ja[i]["availability"]["level"].ToString() != "OOS")
                                    {
                                        group[0] = ja[i]["id"].ToString();
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        group[0] = skuid;
                        if (ja[i]["id"].ToString() == skuid)
                        {
                            if (ja[i]["availability"].ToString() != "False" || ja[i]["availability"].ToString() != "false")
                            {
                                if (ja[i]["availability"]["level"].ToString() == "OOS")
                                {
                                    if (Config.delay == "")
                                    {
                                        Thread.Sleep(1);
                                    }
                                    else
                                    {
                                        Thread.Sleep(int.Parse(Config.delay));
                                    }
                                    goto A;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }

                }
                response.Close();
                readStream.Close();
            }
            catch (WebException ex)
            {
                HttpWebResponse resppayment = (HttpWebResponse)ex.Response;
                Stream resppaymentStream = resppayment.GetResponseStream();
                StreamReader readpaymenthtmlStream = new StreamReader(resppaymentStream, Encoding.UTF8);
                string paymentsuccesscode = readpaymenthtmlStream.ReadToEnd();
                tk.Status = "Proxy Error";
                goto A;
            }
            if (group[0] == null)
            {
                goto A;
            }
            return group;
        }
        private DateTime ConvertStringToDateTime(string timeStamp)
        {
            Thread.Sleep(1);
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
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
                servercookie = result["cookie"].ToString();
            }
            catch
            {

            }
        }
    }
}

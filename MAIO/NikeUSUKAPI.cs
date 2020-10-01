﻿using MAIO.browsercheckout;
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
        public int failedretry = 0;
        public int failedlogin = 0;
        int failedsubshipp = 0;
        public string GetHtmlsource(string url, Main.taskset tk, CancellationToken ct)
        {
            Thread.Sleep(0);
        A: if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            string SourceCode = "";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Proxy = getproxy();
            request.Timeout = 5000;
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
            }
            catch (WebException ex)
            {
                //    HttpWebResponse response = (HttpWebResponse)ex.Response;
                if (ct.IsCancellationRequested)
                {
                    tk.Status = "IDLE";
                    ct.ThrowIfCancellationRequested();
                }
                //   tk.Status = "Get Size Error";
                tk.Status = "Change Proxy";
                goto A;
            }
            return SourceCode;
        }
        public string Postlogin(string url, string logininfo, bool isrefresh, string account, Main.taskset tk, CancellationToken ct)
        {
        D: string token = null;
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            if (Config.UseAdvancemode == "True")
            {
            C: string[] sendcookie = new string[2];
                if (Mainwindow.iscookielistnull || Mainwindow.lines.Count == 0)
                {
                    try
                    {
                        var binding = new BasicHttpBinding();
                        var endpoint = new EndpointAddress(@"http://49.51.68.105/WebService1.asmx");
                        var factory = new ChannelFactory<ServiceReference2.WebService1Soap>(binding, endpoint);
                        var callClient = factory.CreateChannel();
                        JObject result = JObject.Parse(callClient.getcookieAsync(Config.hwid).Result);
                        sendcookie = result["cookie"].ToString().Split(";");
                    }
                    catch
                    {
                        Thread.Sleep(1);
                        tk.Status = "Get Cookie Error";
                        goto C;
                    }
                }
                else
                {
                    Random ra = new Random();
                    int cookie = ra.Next(0, Mainwindow.lines.Count);
                    try
                    {
                        updatelable(Mainwindow.lines[cookie], false);
                        if (isrefresh)
                        {
                            sendcookie[0] = "";
                            sendcookie[1] = "";
                        }
                        else
                        {
                            sendcookie = Mainwindow.lines[cookie].Split(";");
                        }
                        Mainwindow.lines.RemoveAt(cookie);
                    }
                    catch (Exception)
                    {
                        goto C;
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
                            Thread.Sleep(500);
                            goto B;
                        }
                    }
                    catch (NullReferenceException)
                    {
                        Thread.Sleep(500);
                        goto B;
                    }
                    catch (OperationCanceledException)
                    {
                        return "";
                    }
                    if (fordidden)
                    {
                        failedlogin++;
                        if (failedlogin > 20)
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
                catch
                {
                    goto D;
                }
            }
            else
            {
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
                if (ct.IsCancellationRequested)
                {
                    tk.Status = "IDLE";
                    ct.ThrowIfCancellationRequested();
                }
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Headers.Add("Server-Host", "unite.nike.com:443");
                req.Headers.Add("Proxy-Address", proxyaddress);
                byte[] contentByte = Encoding.UTF8.GetBytes(logininfo);
                req.Method = "POST";
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
            reloadcookie: if (isrefresh == false)
                {
                    if (Mainwindow.iscookielistnull || Mainwindow.lines.Count == 0)
                    {
                        try
                        {
                            var binding = new BasicHttpBinding();
                            var endpoint = new EndpointAddress(@"http://49.51.68.105/WebService1.asmx");
                            var factory = new ChannelFactory<ServiceReference2.WebService1Soap>(binding, endpoint);
                            var callClient = factory.CreateChannel();
                            JObject result = JObject.Parse(callClient.getcookieAsync(Config.hwid).Result);
                            req.Headers.Add("Cookie", result["cookie"].ToString());
                        }
                        catch
                        {
                            Thread.Sleep(1);
                            tk.Status = "Get Cookie Error";
                            goto reloadcookie;
                        }
                    }
                    else
                    {
                        Random ra = new Random();
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
                    HttpWebResponse processpayment = (HttpWebResponse)ex.Response;
                    Stream processtream = processpayment.GetResponseStream();
                    StreamReader readprocessstream = new StreamReader(processtream, Encoding.UTF8);
                    string processcode = readprocessstream.ReadToEnd();
                    if (processcode.Contains("incorrectly"))
                    {
                        tk.Status = "Wrong Password";
                        if (ct.IsCancellationRequested)
                        {
                            tk.Status = "IDLE";
                            ct.ThrowIfCancellationRequested();
                        }
                        goto D;
                    }
                    else if (ex.Message.Contains("401"))
                    {
                        tk.Status = "Refrestoken Error";
                        if (ct.IsCancellationRequested)
                        {
                            tk.Status = "IDLE";
                            ct.ThrowIfCancellationRequested();
                        }
                        goto D;
                    }
                    else
                    {
                        tk.Status = "Login Failed";
                        if (ct.IsCancellationRequested)
                        {
                            tk.Status = "IDLE";
                            ct.ThrowIfCancellationRequested();
                        }
                        Thread.Sleep(1000);
                        goto D;
                    }
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
        public double Postcardinfo(string url, string cardinfo, string Authorization, Main.taskset tk, CancellationToken ct, bool isgiftcard)
        {
            Thread.Sleep(0);
        B: if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            double balance = 0;
            byte[] contentcardinfo = Encoding.UTF8.GetBytes(cardinfo);
            HttpWebRequest reqcard = (HttpWebRequest)WebRequest.Create(url);
            reqcard.Method = "POST";
            reqcard.Proxy = getproxy();
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
            }
            catch (WebException ex)
            {
                HttpWebResponse processpayment = (HttpWebResponse)ex.Response;
                tk.Status = "Submit Card  " + processpayment.StatusCode;
                if (isgiftcard == false)
                {
                    goto B;
                }
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
            Thread.Sleep(0);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Proxy = getproxy();
            request.Method = "PUT";
            request.ContentType = "application/json; charset=UTF-8";
            byte[] contentpaymentinfo = Encoding.UTF8.GetBytes(checkoutpayload);
            request.Accept = "application/json";
            request.Headers.Add("Accept-Encoding", "gzip, deflate");
            request.Headers.Add("Accept-Language", "en-US, en; q=0.9");
            request.Headers.Add("Authorization", Authorization);
            request.Headers.Add("appid", "com.nike.commerce.snkrs.web");
        C: if (Mainwindow.iscookielistnull || Mainwindow.lines.Count == 0)
            {
                try
                {
                    var binding = new BasicHttpBinding();
                    var endpoint = new EndpointAddress(@"http://49.51.68.105/WebService1.asmx");
                    var factory = new ChannelFactory<ServiceReference2.WebService1Soap>(binding, endpoint);
                    var callClient = factory.CreateChannel();
                    JObject result = JObject.Parse(callClient.getcookieAsync(Config.hwid).Result);
                    request.Headers.Add("Cookie", result["cookie"].ToString().Replace(";", "; "));
                }
                catch
                {
                    Thread.Sleep(1);
                    tk.Status = "Get Cookie Error";
                    goto C;
                }
            }
            else
            {
                Random ra = new Random();
                int cookie = ra.Next(0, Mainwindow.lines.Count);
                try
                {
                    request.Headers.Add("Cookie", Mainwindow.lines[cookie].Replace(";", "; "));
                    Main.updatelable(Mainwindow.lines[cookie], false);
                    Mainwindow.lines.RemoveAt(cookie);
                }
                catch (Exception)
                {
                    goto C;
                }
            }
            request.ContentLength = contentpaymentinfo.Length;
            request.Referer = "https://www.nike.com/";
            request.Headers.Add("Origin", "https://www.nike.com");
            request.Headers.Add("Sec-Fetch-Dest", "empty");
            request.Headers.Add("Sec-Fetch-Mode", "cors");
            request.Headers.Add("Sec-Fetch-Site", "same-site");
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.105 Safari/537.36";
            request.Headers.Add("X-B3-SpanId", xb3spanID);
            request.Headers.Add("X-B3-ParentSpanId", xb3parentspanid);
            request.Headers.Add("X-B3-TraceId", xb3traceid);
            Stream paymentstream = request.GetRequestStream();
            paymentstream.Write(contentpaymentinfo, 0, contentpaymentinfo.Length);
            paymentstream.Close();
            string paymentsuccesscode = "";
            try
            {
                HttpWebResponse resppayment = (HttpWebResponse)request.GetResponse();
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
                catch (NullReferenceException)
                {
                }
            }
            catch (WebException ex)
            {
                HttpWebResponse resppayment = (HttpWebResponse)ex.Response;
                tk.Status = "Submit Shipping error";
                Thread.Sleep(1500);
                failedsubshipp++;
                if (failedsubshipp > 20)
                {
                    Main.autorestock(tk);
                }
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
            Thread.Sleep(0);
            string total = "";
            HttpWebRequest reqcheckstatus = (HttpWebRequest)WebRequest.Create(url);
            reqcheckstatus.Method = "GET";
            reqcheckstatus.Proxy = getproxy();
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
                        if (ct.IsCancellationRequested)
                        {
                            tk.Status = "IDLE";
                            ct.ThrowIfCancellationRequested();
                        }
                    }
                    total = jo["response"]["totals"]["total"].ToString();
                    if (ct.IsCancellationRequested)
                    {
                        tk.Status = "IDLE";
                        ct.ThrowIfCancellationRequested();
                    }
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
            HttpWebRequest reqprocess = (HttpWebRequest)WebRequest.Create(url);
            reqprocess.Proxy = getproxy();
            reqprocess.Timeout = 5000;
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
                tk.Status = "Submit Billing error";
                goto retry;
            }
            JObject joid = JObject.Parse(processcode);
            string id = joid["id"].ToString();

        C: if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            HttpWebRequest reqjob = (HttpWebRequest)WebRequest.Create("https://api.nike.com/payment/preview/v2/jobs/" + id);
            reqjob.Proxy = getproxy();
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
            string jobstatus = null;
            try
            {
                HttpWebResponse respjob = (HttpWebResponse)reqjob.GetResponse();
                //    var wu = respjob.Headers["Set-Cookie"];
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
                if (jobstatus.Contains("COMPLETED") != true)
                {
                    goto C;
                }

            }
            catch (WebException ex)
            {
                // HttpWebResponse respjob = (HttpWebResponse)ex.Response;
                goto C;
            }
            return id;
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
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Headers.Add("Server-Host", "api.nike.com:443");
                request.Headers.Add("Proxy-Address", proxyaddress);
                request.Method = "PUT";
                request.Accept = "application/json";
                request.ContentType = "application/json; charset=UTF-8";
                byte[] paymenttokeninfo = Encoding.UTF8.GetBytes(payload);
                request.Headers.Add("Authorization", Authorization);
            C: if (Mainwindow.iscookielistnull || Mainwindow.lines.Count == 0)
                {
                    try
                    {
                        var binding = new BasicHttpBinding();
                        var endpoint = new EndpointAddress(@"http://49.51.68.105/WebService1.asmx");
                        var factory = new ChannelFactory<ServiceReference2.WebService1Soap>(binding, endpoint);
                        var callClient = factory.CreateChannel();
                        JObject result = JObject.Parse(callClient.getcookieAsync(Config.hwid).Result);
                        request.Headers.Add("Cookie", result["cookie"].ToString());
                    }
                    catch
                    {
                        Thread.Sleep(1);
                        tk.Status = "Get Cookie Error";
                        goto C;
                    }
                }
                else
                {
                    Random ra = new Random();
                    int sleeptime = ra.Next(0, 100);
                    Thread.Sleep(sleeptime);
                    int cookie = ra.Next(0, Mainwindow.lines.Count);
                    try
                    {
                        request.Headers.Add("Cookie", Mainwindow.lines[cookie].Replace(";", "; ") + "; " + akbmsz);
                        Main.updatelable(Mainwindow.lines[cookie], false);
                        Mainwindow.lines.RemoveAt(cookie);
                    }
                    catch (Exception)
                    {
                        goto C;
                    }
                }
                request.Referer = "https://www.nike.com/us/en/checkout";
                request.Headers.Add("Accept-encoding", "gzip, deflate,br");
                request.Headers.Add("Accept-language", "en-US, en; q=0.9");
                request.Headers.Add("appid", "com.nike.commerce.snkrs.web");
                request.Headers.Add("Origin", "https://www.nike.com");
                request.ContentLength = paymenttokeninfo.Length;
                request.Host = "api.nike.com";
                request.Headers.Add("Sec-Fetch-Dest", "empty");
                request.Headers.Add("Sec-Fetch-Mode", "cors");
                request.Headers.Add("Sec-Fetch-Site", "same-site");
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.105 Safari/537.36";
                request.Headers.Add("X-B3-SpanId", xb3spanID);
                request.Headers.Add("X-B3-ParentSpanId", xb3parentspanid);
                request.Headers.Add("X-B3-TraceId", xb3traceid);
                Stream paymenttokenstream = request.GetRequestStream();
                paymenttokenstream.Write(paymenttokeninfo, 0, paymenttokeninfo.Length);
                paymenttokenstream.Close();
                try
                {
                    HttpWebResponse respgetstatus = (HttpWebResponse)request.GetResponse();
                    /*     Stream respcheckstatusstream = respgetstatus.GetResponseStream();
                         StreamReader readcheckstatus = new StreamReader(respcheckstatusstream, Encoding.GetEncoding("utf-8"));
                         string check = readcheckstatus.ReadToEnd();*/
                }
                catch (WebException ex)
                {
                    //    HttpWebResponse respjob = (HttpWebResponse)ex.Response;
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
                if (Mainwindow.iscookielistnull || Mainwindow.lines.Count == 0)
                {
                    try
                    {
                        var binding = new BasicHttpBinding();
                        var endpoint = new EndpointAddress(@"http://49.51.68.105/WebService1.asmx");
                        var factory = new ChannelFactory<ServiceReference2.WebService1Soap>(binding, endpoint);
                        var callClient = factory.CreateChannel();
                        JObject result = JObject.Parse(callClient.getcookieAsync(Config.hwid).Result);
                        sendcookie = result["cookie"].ToString().Split(";");
                    }
                    catch
                    {
                        tk.Status = "Get Cookie Error";
                        Thread.Sleep(1);
                        goto D;
                    }
                }
                else
                {
                    Random ra = new Random();
                    int cookie = ra.Next(0, Mainwindow.lines.Count);
                    try
                    {
                        Main.updatelable(Mainwindow.lines[cookie], false);
                        sendcookie = Mainwindow.lines[cookie].Split(";");
                        Mainwindow.lines.RemoveAt(cookie);
                    }
                    catch (Exception)
                    {
                        goto D;
                    }
                }
                string proxy = null;
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
                            else if (sValue["status"].ToString() == "202")
                            {
                                tk.Status = "Submit Payment";
                                returnstatus.Remove(tk.Taskid);
                            }
                            else if (sValue["status"].ToString() == "403")
                            {
                                tk.Status = "Submit Payment Forbidden";
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
                            Thread.Sleep(500);
                            goto B;
                        }
                    }
                    catch (NullReferenceException)
                    {
                        Thread.Sleep(500);
                        goto B;
                    }
                    catch (OperationCanceledException)
                    {
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
                catch
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
                HttpWebRequest reqfinal = (HttpWebRequest)HttpWebRequest.Create("https://api.nike.com/buy/checkouts/v2/jobs/" + GID);
                reqfinal.Proxy = getproxy();
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
                if ((status.Contains("error")))
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
                if (status.Contains("COMPLETED") == true && status.Contains("error") == false)
                {
                    break;
                }
            }
            return status;
        }
        public static long time = 0;
        private static DateTime timeStampStartTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public string[] Monitoring(string url, Main.taskset tk, CancellationToken ct, string info, bool randomsize, string skuid, bool multisize, ArrayList skulist)
        {
        A: if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            Thread.Sleep(1);
            string traceid = Guid.NewGuid().ToString();
            string nikevistid = Guid.NewGuid().ToString();
            string SourceCode = null;
            string[] group = new string[2];
            int random = ran.Next(0, Mainwindow.proxypool.Count);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Proxy = getproxy();
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
                if (SourceCode.Contains("Product not found") || SourceCode.Contains("errors"))
                {
                    tk.Status = "Check Stock Error";
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
                        if (ja[i]["availability"]["level"].ToString() != "OOS")
                        {
                            group[0] = ja[i]["id"].ToString();
                            break;
                        }

                    }
                    else if (multisize)
                    {
                        for (int n = 0; n < skulist.Count; n++)
                        {
                            Thread.Sleep(1);
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
                    else
                    {
                        group[0] = skuid;
                        if (ja[i]["id"].ToString() == skuid)
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
                response.Close();
                readStream.Close();
            }
            catch (WebException)
            {
                //   HttpWebResponse resppayment = (HttpWebResponse)ex.Response;
                tk.Status = "Proxy Error";
                goto A;
            }
            if (group[0] == null)
            {
                if (tk.monitortask == "True")
                {
                    share_dog[tk.Tasksite + tk.Sku] = false;
                    if (ct.IsCancellationRequested)
                    {
                        tk.Status = "IDLE";
                        ct.ThrowIfCancellationRequested();
                    }
                    goto A;
                }
                goto A;
            }
            if (tk.monitortask == "True")
            {
                tk.Status = "Get Stock";
                share_dog[tk.Tasksite + tk.Sku] = true;
                if (ct.IsCancellationRequested)
                {
                    tk.Status = "IDLE";
                    ct.ThrowIfCancellationRequested();
                }
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
        public WebProxy getproxy()
        {
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
            return wp;
        }

    }
}


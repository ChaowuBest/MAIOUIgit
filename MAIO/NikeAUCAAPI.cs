using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Windows.Shapes;
using System.Windows;
using System.Text.RegularExpressions;
using static MAIO.Main;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using NakedBot;
using PuppeteerSharp;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;
using MAIO.browsercheckout;
using Newtonsoft.Json;
using System.ServiceModel;
using System.Collections.Immutable;
using System.Collections;

namespace MAIO
{
    class NikeAUCAAPI
    {
        Random ran = new Random();
        string xb3traceid = Guid.NewGuid().ToString();
        string xnikevisitorid = Guid.NewGuid().ToString();
        public int failedretry = 0;
        public Page _page = null;
        public Browser _browser = null;
        public string GetHtmlsource(string url, Main.taskset tk, CancellationToken ct)
        {
        A: if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            Thread.Sleep(1);
            string SourceCode = "";
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
        public void PutMethod(string url, string payinfo, Main.taskset tk, CancellationToken ct, string ccookie)
        {
            if (Config.UseAdvancemode == "True")
            {
            C: Thread.Sleep(1);
                string[] sendcookie = null;
                if (Mainwindow.iscookielistnull)
                {
                    Thread.Sleep(1);
                    if (ct.IsCancellationRequested)
                    {
                        tk.Status = "IDLE";
                        ct.ThrowIfCancellationRequested();
                    }
                    tk.Status = "No Cookie";
                    goto C;
                }
                else
                {
                    Thread.Sleep(1);
                    Random ra = new Random();
                    if (ct.IsCancellationRequested)
                    {
                        tk.Status = "IDLE";
                        ct.ThrowIfCancellationRequested();
                    }
                    int cookie = ra.Next(0, Mainwindow.lines.Count);
                    try
                    {
                        Main.updatelable(Mainwindow.lines[cookie], false);
                        sendcookie = Mainwindow.lines[cookie].Split(";");
                        Mainwindow.lines.RemoveAt(cookie);
                        if (Mainwindow.lines.Count == 0)
                        {
                            Mainwindow.iscookielistnull = true;
                        }
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
                    string sendjson = null;
                    try
                    {
                        var chao = JsonConvert.SerializeObject(payinfo).Replace("\"{", "{").Replace("}\"", "}");
                        sendjson = "{\"data\":{\"headers\":{\"Content-Type\":\"application/json\",\"Origin\":\" https://www.nike.com\",\"Accept\":\"application/json\",\"x-nike-visitid\":\"1\",\"x-nike-visitorid\":\"" + xnikevisitorid + "\",\"appid\":\"com.nike.commerce.nikedotcom.web\",\"X-B3-SpanName\":\"CiCCart\",\"X-B3-TraceId\":\"" + xb3traceid + "\"},\"url\":\"" + url + "\",\"method\":\"PUT\",\"data\":\"" + chao + "\",\"proxy\":\"\",\"cookies\":[{\"Name\":\"_abck\",\"TimeStamp\":\"" + DateTime.Now.ToLocalTime().ToString() + "\",\"Value\":\"" + sendcookie[1].Replace("_abck=", "") + "\",\"Comment\":\"\",\"CommentUri\":null,\"HttpOnly\":false,\"Discard\":false,\"Expired\":false,\"Secure\":false,\"Domain\":\".nike.com\",\"Expires\":\"0001-01-01T00:00:00\",\"Path\":\"/\",\"Port\":\"\",\"Version\":0},{\"Name\":\"bm_sz\",\"TimeStamp\":\"2020/8/26 23:42:48\",\"Value\":\"" + sendcookie[0].Replace("bm_sz=", "") + "\",\"Comment\":\"\",\"CommentUri\":null,\"HttpOnly\":false,\"Discard\":false,\"Expired\":false,\"Secure\":false,\"Domain\":\".nike.com\",\"Expires\":\"0001-01-01T00:00:00\",\"Path\":\"/\",\"Port\":\"\",\"Version\":0}],\"id\":\"" + tk.Taskid + "\"},\"type\":\"request\"}";
                    }
                    catch (Exception)
                    {
                    }
                    allSockets[0].Send(sendjson);
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
                            if (sValue["status"].ToString() == "202")
                            {
                                tk.Status = "Submit Order";
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
                        return;
                    }
                    #region
                    #endregion
                    if (fordidden)
                    {
                        failedretry++;
                        if (failedretry > 20)
                        {
                            Main.autorestock(tk);
                        }
                        Thread.Sleep(1500);
                        goto C;
                    }
                }
                catch (OperationCanceledException)
                {
                    return;

                }
                catch (NullReferenceException)
                {
                    goto C;
                }
            }
            else
            {
            B: if (ct.IsCancellationRequested)
                {
                    tk.Status = "IDLE";
                    ct.ThrowIfCancellationRequested();
                }
                Thread.Sleep(1);
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
                request.Method = "PUT";
                request.Proxy = wp;
                request.ContentType = "application/json; charset=UTF-8";
                byte[] contentpaymentinfo = Encoding.UTF8.GetBytes(payinfo);
                request.Accept = "application/json";
                request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
                request.Headers.Add("cloud_stack", "buy_domain");
                request.Headers.Add("appid", "com.nike.commerce.nikedotcom.web");
                request.Headers.Add("Root-Domain", "nike.com");
            C: if (Mainwindow.iscookielistnull)
                {
                    if (ct.IsCancellationRequested)
                    {
                        tk.Status = "IDLE";
                        ct.ThrowIfCancellationRequested();
                    }
                    tk.Status = "No Cookie";
                    goto C;
                }
                else
                {
                    Random ra = new Random();
                    if (ct.IsCancellationRequested)
                    {
                        tk.Status = "IDLE";
                        ct.ThrowIfCancellationRequested();
                    }
                    int sleeptime = ra.Next(0, 100);
                    Thread.Sleep(sleeptime);
                    int cookie = ra.Next(0, Mainwindow.lines.Count);
                    try
                    {
                        Main.updatelable(Mainwindow.lines[cookie], false);
                        request.Headers.Add("Cookie", Mainwindow.lines[cookie]);
                        Mainwindow.lines.RemoveAt(cookie);
                        if (Mainwindow.lines.Count == 0)
                        {
                            Mainwindow.iscookielistnull = true;
                        }
                    }
                    catch (Exception)
                    {
                        goto C;
                    }
                }
                request.ContentLength = contentpaymentinfo.Length;
                request.Headers.Add("Origin", "https://www.nike.com");
                request.Headers.Add("Sec-Fetch-Dest", "empty");
                request.Headers.Add("Sec-Fetch-Mode", "cors");
                request.Headers.Add("Sec-Fetch-Site", "same-site");
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.105 Safari/537.36";
                request.Headers.Add("X-B3-SpanName", "CiCCart");
                request.Headers.Add("X-B3-TraceId", xb3traceid);
                request.Headers.Add("x-nike-visitid", "1");
                request.Headers.Add("x-nike-visitorid", xnikevisitorid);
                Stream paymentstream = request.GetRequestStream();
                paymentstream.Write(contentpaymentinfo, 0, contentpaymentinfo.Length);
                paymentstream.Close();
                try
                {
                    HttpWebResponse resppayment = (HttpWebResponse)request.GetResponse();
                    tk.Status = "SubmitOrder";
                    Stream receiveStream = resppayment.GetResponseStream();
                    if (ct.IsCancellationRequested)
                    {
                        tk.Status = "IDLE";
                        ct.ThrowIfCancellationRequested();
                    }
                    StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                    if (resppayment.ContentEncoding == "gzip")
                    {
                        readStream = new StreamReader(new GZipStream(receiveStream, CompressionMode.Decompress), Encoding.GetEncoding("utf-8"));
                    }
                    else
                    {
                        readStream = new StreamReader(receiveStream, Encoding.UTF8);
                    }
                    string sourcecode = readStream.ReadToEnd();
                }
                catch (WebException ex)
                {
                    HttpWebResponse response = (HttpWebResponse)ex.Response;
                    if (ct.IsCancellationRequested)
                    {
                        tk.Status = "IDLE";
                        ct.ThrowIfCancellationRequested();
                    }
                    tk.Status = "Forbidden";
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
                    goto B;
                }
            }
        }
        public string GetMethod(string url, string iamgeurl, Main.taskset tk, CancellationToken ct)
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
                if (ct.IsCancellationRequested)
                {
                    tk.Status = "IDLE";
                    ct.ThrowIfCancellationRequested();
                }
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
            request.ContentType = "application/json; charset=UTF-8";
            request.Method = "GET";
            request.Accept = "application/json";
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("appid", "com.nike.commerce.nikedotcom.web");
            request.Headers.Add("Accept-Language", "en-US, en; q=0.9");
            request.Headers.Add("Origin", "https://www.nike.com");
            request.Headers.Add("Sec-Fetch-Dest", "empty");
            request.Headers.Add("Sec-Fetch-Mode", "cors");
            request.Headers.Add("Sec-Fetch-Site", "same-site");
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.105 Safari/537.36";
            request.Headers.Add("x-b3-spanname", "CiCCart");
            request.Headers.Add("x-b3-traceid", xb3traceid);
            request.Headers.Add("x-nike-visitid", "1");
            request.Headers.Add("x-nike-visitorid", xnikevisitorid);
            string sourcecode = "";
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                tk.Status = "Processing";
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                if (response.ContentEncoding == "gzip")
                {
                    readStream = new StreamReader(new GZipStream(receiveStream, CompressionMode.Decompress), Encoding.GetEncoding("utf-8"));
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.UTF8);
                }
                sourcecode = readStream.ReadToEnd();
                if (ct.IsCancellationRequested)
                {
                    tk.Status = "IDLE";
                    ct.ThrowIfCancellationRequested();
                }
                if (sourcecode.Contains("COMPLETED") == false)
                {
                    Thread.Sleep(200);
                    goto C;
                }
                if (sourcecode.Contains("Reservation with Inventory"))
                {
                    tk.Status = "WaitingRestock";
                    Main.autorestock(tk);
                    if (ct.IsCancellationRequested)
                    {
                        tk.Status = "IDLE";
                        ct.ThrowIfCancellationRequested();
                    }
                }
                if ((sourcecode.Contains("COMPLETED") == true) && (sourcecode.Contains("error")))
                {
                    tk.Status = "WaitingRestock";
                    JObject jo = JObject.Parse(sourcecode);
                    string error = jo["error"].ToString();
                    JObject jo2 = JObject.Parse(error);
                    var reason = jo2["errors"][0].ToString();
                    JObject jo3 = JObject.Parse(reason);
                    string errormessage = jo3["code"].ToString();
                    tk.Status = errormessage;
                    Main.autorestock(tk);
                    if (ct.IsCancellationRequested)
                    {
                        tk.Status = "IDLE";
                        ct.ThrowIfCancellationRequested();
                    }
                }
            }
            catch (WebException ex)
            {

                HttpWebResponse response = (HttpWebResponse)ex.Response;
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                if (ct.IsCancellationRequested)
                {
                    tk.Status = "IDLE";
                    ct.ThrowIfCancellationRequested();
                }
                if (response.ContentEncoding == "gzip")
                {
                    readStream = new StreamReader(new GZipStream(receiveStream, CompressionMode.Decompress), Encoding.GetEncoding("utf-8"));
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.UTF8);
                }
                sourcecode = readStream.ReadToEnd();
                goto C;
            }
            return sourcecode;
        }
        public string[] Monitoring(string url, taskset tk, CancellationToken ct, string info, bool randomsize, string skuid, bool multisize, ArrayList skulist)
        {
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
            request.Headers.Add("X-B3-SpanName", "CiCCart");
            request.Headers.Add("X-B3-TraceId", traceid);
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
                traceid = Guid.NewGuid().ToString();
                nikevistid = Guid.NewGuid().ToString();
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
        public void failcheckout(taskset tk, string webhookurl, string reason, string imageurl)
        {
            Thread.Sleep(1);
            JObject jobject = null;
            jobject = JObject.Parse("{\"username\":\"MAIO\",\"avatar_url\":\"https://i.loli.net/2020/05/24/VfWKsEywcXZou1T.jpg\",\"embeds\":[{\"title\":\"\",\"color\":16711680,\"description\":\"\",\"fields\":[{\"name\":\"SKU\",\"value\":\"\",\"inline\":true},{\"name\":\"Size\",\"value\":\"\",\"inline\":true},{\"name\":\"Reason\",\"value\":\"\",\"inline\":false}],\"thumbnail\":{\"url\":\"\"},\"footer\":{\"text\":\"MAIO" + DateTime.Now.ToLocalTime().ToString() + "\",\"icon_url\":\"https://i.loli.net/2020/05/24/VfWKsEywcXZou1T.jpg\"}}]}");
            jobject["embeds"][0]["title"] = "You Just Checkout!!!";
            jobject["embeds"][0]["fields"][0]["value"] = tk.Sku;
            jobject["embeds"][0]["fields"][1]["value"] = tk.Size;
            jobject["embeds"][0]["fields"][2]["value"] = reason;
            jobject["embeds"][0]["thumbnail"]["url"] = imageurl;

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
                goto Retry;
            }

        }
    }
}
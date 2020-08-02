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
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Net.Http;
using System.Net.Http.Headers;

namespace MAIO
{
    class NikeAUCAAPI
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
        public void PutMethod(string url, string payinfo, Main.taskset tk, CancellationToken ct)
        {
        B: if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
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
            #region
            /* A: var client = new HttpClient();
               client.DefaultRequestVersion = HttpVersion.Version20;
               client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
               client.DefaultRequestHeaders.Add("appid", "com.nike.commerce.nikedotcom.web");
               client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
               client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
               client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
               client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-site");
               client.DefaultRequestHeaders.Add("X-B3-SpanName", "CiCCart");
               client.DefaultRequestHeaders.Add("X-B3-TraceId", xb3traceid);
               client.DefaultRequestHeaders.Add("x-nike-visitid", "1");
               client.DefaultRequestHeaders.Add("x-nike-visitorid", xnikevisitorid);
               client.DefaultRequestHeaders.Add("Cookie", Mainwindow.lines[10]);
               client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.105 Safari/537.36");
               HttpContent httpContent = new StringContent(payinfo);
               var chao = client.PutAsync(url, httpContent).Result;

               if (chao.ReasonPhrase != "Forbidden")
               {

               }
               else
               {
                   goto A;
               }*/
            #endregion
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);         
            request.Method = "PUT";
            request.Proxy = wp;
            request.ContentType = "application/json; charset=UTF-8";
            byte[] contentpaymentinfo = Encoding.UTF8.GetBytes(payinfo);
            request.Accept = "application/json";
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("cloud_stack", "buy_domain");
            request.Headers.Add("appid", "com.nike.commerce.nikedotcom.web");
            request.Headers.Add("Accept-Language", "en-US, en; q=0.9");
            if (Config.UseAdvancemode == "True")
            {
                string path = Environment.CurrentDirectory + "\\" + "advancecookie.txt";
                FileStream fs2 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                StreamReader sr = new StreamReader(fs2);
                string cookie = sr.ReadToEnd();
                request.Headers.Add("Cookie", cookie);
                sr.Close();
                fs2.Close();
            }
            else
            {
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
                    int sleeptime = ra.Next(0, 500);
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
            }
            request.ContentLength = contentpaymentinfo.Length;
            request.Headers.Add("Origin", "https://www.nike.com");
            request.Headers.Add("Sec-Fetch-Dest", "empty");
            request.Headers.Add("Sec-Fetch-Mode", "cors");
            request.Headers.Add("Sec-Fetch-Site", "same-site");
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
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
            }
            catch (WebException ex)
            {
                HttpWebResponse response = (HttpWebResponse)ex.Response;
                tk.Status = "Forbidden";
              //  tk.Status = response.StatusCode.ToString();
                Thread.Sleep(1500);
                goto B;
            }
        }
        public string GetMethod(string url, string profile, string skuid, string pid, bool randomsize, Main.taskset tk, CancellationToken ct, CancellationTokenSource cts, string productid, string size)
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
                tk.Status = "processing";
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
                if (sourcecode.Contains("COMPLETED") == false)
                {
                    Thread.Sleep(1500);
                    goto C;
                }
                string monitorurl = "https://api.nike.com/deliver/available_skus/v1?filter=productIds(" + productid + ")";
                if ((sourcecode.Contains("COMPLETED") == true) && (sourcecode.Contains("OUT_OF_STOCK")))
                {
                    string[] group = Monitoring(monitorurl, tk, ct, skuid, randomsize);
                    tk.Status = "WaitingRestock";
                    xb3traceid = Guid.NewGuid().ToString();
                    xnikevisitorid = Guid.NewGuid().ToString();
                    NikeAUCA NAU = new NikeAUCA();
                    NAU.skuid = group[0];
                    NAU.productid = group[1];
                    NAU.size = size;
                    NAU.pid = pid;
                    NAU.profile = profile;
                    NAU.randomsize = randomsize;
                    NAU.tk = tk;
                    NAU.Quantity = int.Parse(tk.Quantity);
                    NAU.StartTask(ct, cts);
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
                    string[] group = Monitoring(monitorurl, tk, ct, skuid, randomsize);
                    xb3traceid = Guid.NewGuid().ToString();
                    xnikevisitorid = Guid.NewGuid().ToString();
                    NikeAUCA NAU = new NikeAUCA();
                    NAU.skuid = group[0];
                    NAU.productid = group[1];
                    NAU.size = size;
                    NAU.pid = pid;
                    NAU.profile = profile;
                    NAU.randomsize = randomsize;
                    NAU.tk = tk;
                    NAU.Quantity = int.Parse(tk.Quantity);
                    NAU.StartTask(ct, cts);
                }

            }
            catch (WebException ex)
            {
                tk.Status = ex.ToString() + "Retrying";
                HttpWebResponse response = (HttpWebResponse)ex.Response;
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
                goto C;
            }
            return sourcecode;
        }
        public string[] Monitoring(string url, Main.taskset tk, CancellationToken ct, string skuid, bool randomsize)
        {
        A: if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
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
            request.Method = "GET";
            request.Host = "api.nike.com";
            request.Accept = "*/*";
            request.Headers.Add("Sec-Fetch-Dest", "document");
            request.Headers.Add("Sec-Fetch-Mode", "navigate");
            request.Headers.Add("Sec-Fetch-Site", "none");
            request.Headers.Add("Sec-Fetch-user", "?1");
            request.Headers.Add("upgrade-insecure-requests", "1");
            request.UserAgent = "Sogou inst spider";
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
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
                JObject jo = JObject.Parse(SourceCode);
                JArray ja = JArray.Parse(jo["objects"].ToString());
                foreach (var i in ja)
                {
                    group[1] = i["productId"].ToString();
                    if (randomsize)
                    {
                        if (i["available"].ToString() != "False" || i["available"].ToString() != "false")
                        {
                            if (i["level"].ToString() == "OOS")
                            {
                            }
                            else
                            {
                                group[0] = i["skuId"].ToString();
                                break;
                            }
                        }
                        else
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
                    }
                    else
                    {
                        group[0] = skuid;
                        if (i["skuId"].ToString() == skuid)
                        {
                            if (i["available"].ToString() != "False" || i["available"].ToString() != "false")
                            {
                                if (i["level"].ToString() == "OOS")
                                {
                                    
                                    goto A;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
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
                        }
                    }
                }
                response.Close();
                readStream.Close();
            }
            catch (WebException ex)
            {
                HttpWebResponse response = (HttpWebResponse)ex.Response;
                goto A;
            }
            if (group[0] == null)
            {
                goto A;
            }
            return group;
        }
        private static bool AlwaysGoodCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
        {
            return true;
        }
    }
}

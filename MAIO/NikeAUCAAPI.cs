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

namespace MAIO
{
    class NikeAUCAAPI
    {
        Random ran = new Random();
        string xb3traceid = Guid.NewGuid().ToString();
        string xnikevisitorid = Guid.NewGuid().ToString();
        public int failedretry = 0;
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
            //  var chao=cookie();
            request.Headers.Add("Cookie", "_abck=0A50E8275B5485E5AE557EFD88AAAA7E~-1~YAAQDMrdPEtOVu9zAQAAN4cF8wQoCa55AnV2mrpBVxzK8Qmlzuw59uAriLTv8mDZJjmBV2tZVYxVAEiTmZST07934/jYrBTsBCtGyTvhZ4kpO4cGemSOB9jm+HxOrRWjjzQSZ1KVlC85YJuQ2XXKVK944m3H7AZg4fUkbAhzdb56baSTCDIjOFUBxScX/sQP/bCZqC8a4ZBp51TjaXwsvhs8JDQ5nxXKfNOT41QXeTgIDDl2jeNPpd5UeSfPmTdSaUqVYnZfBXNKgJ2QurHucyHyBZWs/KE8XMH7+B+zAAs4qheBXZ4hJfVB7XKatT5m6+y9U+BMPDBUYvAS0selt2zs3r2Yuv0LjfDTc8fKI+w=~-1~-1~-1; bm_sz=462AB3745FBB9C32332F4A2BE6BD78AE~YAAQDMrdPEpOVu9zAQAAN4cF8wjY3FqVo6p6Ji0pq4VFj1+YasnIweQfkd/cT6kOygFJ4HBk+zHl4PipQZOHkeXImRbyuwE5pCU5J3Ghomq/4zACyu7PUOv9KUncdiqfNxqVN7XgCeLzmsMowNauE4OM79FEfRfaeUvhTMJIdLvVJTRth/i24ERdNJS9Sw==");
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
                failedretry++;
                if (failedretry > 20)
                {
                    Main.autorestock(tk);
                }
                Thread.Sleep(1500);
                goto B;
            }
        }
        public string cookie()
        {
            string cookie = null;
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


        A: ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.nike.com/static/42d3d1b61b5ti205e7380486e5b5d0b8f");
            request.Host = "www.nike.com";
            //   request.Method = "POST";
            request.Proxy = wp;
            request.Accept = "*/*";
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8,zh-TW;q=0.7,zh-HK;q=0.5,en-US;q=0.3,en;q=0.2");
            request.Headers.Add("Cookie", "_abck=542D944AF66225E4D2A7B7A1EF49665E~-1~YAAQgpZUaO9JttpzAQAAYi/Q7QRRvgfP2m/PmDEAAdiEKBt6xvd6SSF+RYIalygYl+nHPVvl/GR6E8WqBoFoM8nJzn5e6T+LRPgljtk+2rPkn/sxUYelknkbRT74Q9nL2PCGgnsaX5GsAGiikP6IWxxij+eXyM1eEkwSoQh7cjL+GkGlDaaUBUONWoY5iTuMq82vETZ9+0ky8kHB0tIIJS2ru5KJSKL6UyziHvWD9Nlk96+dwgGw5QyJPLz8B4uzy8dGtnMcU1uBxlkq/akUy08RepbLavX78vp6CKVSYo/VxtjZNlXkERchOVOYfHyhATchZEiLvsGETsbeB9Ogo9oMlKOCV8Za+qnc46az+7o=~-1~-1~-1");
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var chao = response.Headers["Set-Cookie"];
                Regex rex3 = new Regex(@"(?<=_abck=)([^;]+)");
                cookie = rex3.Match(chao).ToString();
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
                var wu = readStream.ReadToEnd();
            }
            catch (WebException ex)
            {
                goto A;
            }
        B: ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create("https://www.nike.com/static/42d3d1b61b5ti205e7380486e5b5d0b8f");
            request2.Host = "www.nike.com";
            request2.Accept = "*/*";
            request2.Proxy = wp;
            request2.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8,zh-TW;q=0.7,zh-HK;q=0.5,en-US;q=0.3,en;q=0.2");
            request2.Headers.Add("Cookie", "_abck=" + cookie);
            request2.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
            try
            {
                HttpWebResponse response2 = (HttpWebResponse)request2.GetResponse();
                var chao = response2.Headers["Set-Cookie"];
                Regex rex3 = new Regex(@"(?<=bm_sz=)([^;]+)");
                cookie = "_abck=542D944AF66225E4D2A7B7A1EF49665E~-1~YAAQgpZUaO9JttpzAQAAYi/Q7QRRvgfP2m/PmDEAAdiEKBt6xvd6SSF+RYIalygYl+nHPVvl/GR6E8WqBoFoM8nJzn5e6T+LRPgljtk+2rPkn/sxUYelknkbRT74Q9nL2PCGgnsaX5GsAGiikP6IWxxij+eXyM1eEkwSoQh7cjL+GkGlDaaUBUONWoY5iTuMq82vETZ9+0ky8kHB0tIIJS2ru5KJSKL6UyziHvWD9Nlk96+dwgGw5QyJPLz8B4uzy8dGtnMcU1uBxlkq/akUy08RepbLavX78vp6CKVSYo/VxtjZNlXkERchOVOYfHyhATchZEiLvsGETsbeB9Ogo9oMlKOCV8Za+qnc46az+7o=~-1~-1~-1" + "; bm_sz=" + rex3.Match(chao).ToString();
                Stream receiveStream = response2.GetResponseStream();
                StreamReader readStream = null;
                if (response2.ContentEncoding == "gzip")
                {
                    readStream = new StreamReader(new GZipStream(receiveStream, CompressionMode.Decompress), Encoding.GetEncoding("utf-8"));
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.UTF8);
                }
                var wu = readStream.ReadToEnd();
                // Console.WriteLine(cookie);
                //   goto A;
            }
            catch (WebException ex)
            {
            }

            return cookie;
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
                if (sourcecode.Contains("COMPLETED") == false)
                {
                    Thread.Sleep(1500);
                    goto C;
                }
                string monitorurl = "https://api.nike.com/deliver/available_skus/v1?filter=productIds(" + productid + ")";
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
                }
            }
            catch (WebException ex)
            {
                //   tk.Status = ex.ToString() + "Retrying";
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
        public string[] Monitoring(string url, Main.taskset tk, CancellationToken ct, string info, bool randomsize, string skuid)
        {
        A: if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
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
    }
}

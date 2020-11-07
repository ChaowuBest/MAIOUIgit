using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Threading;
using static MAIO.Main;

namespace MAIO
{
    class NikeMonitorProductAPI
    {
        Random ran = new Random();
        string xb3traceid = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 16);
        string xb3parentspanid = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 16);
        string xb3spanID = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 16);
        public string[] Monitoring(string url, Main.Monitor tk, CancellationToken ct, string info, bool randomsize, string skuid, bool multisize, ArrayList skulist)
        {
            DateTime dtone = Convert.ToDateTime(DateTime.Now.ToLocalTime().ToString());
            try
            {
                ArrayList ary = null;
                if (share_dog_skuid.TryGetValue(tk.Region + tk.Sku, out ary) == false)
                {
                    share_dog_skuid.Add(tk.Region + tk.Sku, new ArrayList());
                }
                else
                {
                    share_dog_skuid[tk.Region + tk.Sku].Clear();
                }
            }
            catch { }

        A: if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                share_dog[tk.Region + tk.Sku] = false;
                share_dog_skuid[tk.Region + tk.Sku].Clear();
                ct.ThrowIfCancellationRequested();
            }
            DateTime dttwo = Convert.ToDateTime(DateTime.Now.ToLocalTime().ToString());
            TimeSpan ts = dttwo - dtone;
            #region
            /*  if (ts.TotalMinutes >= 50)
              {
                  Random ran = new Random();
                  int sleeptime = ran.Next(0, 600);
                  Thread.Sleep(sleeptime);
                  Main.autorestock(tk);
                  if (ct.IsCancellationRequested)
                  {
                      tk.Status = "IDLE";
                      ct.ThrowIfCancellationRequested();
                  }
              }*/
            #endregion
            Thread.Sleep(1);
            string nikevistid = Guid.NewGuid().ToString();
            string SourceCode = null;
            string[] group = new string[2];
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Proxy = monitorproxy();
            request.Method = "POST";
            request.Host = "api.nike.com";
            request.Accept = "*/*";
            request.ContentType = "application/json; charset=UTF-8";
            byte[] contentcardinfo = Encoding.UTF8.GetBytes(info);
            request.ContentLength = contentcardinfo.Length;
            request.Headers.Add("Accept-Encoding", "gzip, deflate br");
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
                try
                {
                    if (share_dog_skuid.ContainsKey(tk.Region + tk.Sku))
                    {
                        share_dog_skuid[tk.Region + tk.Sku].Clear();
                    }
                }
                catch { }
                for (int i = 0; i < ja.Count; i++)
                {
                    Thread.Sleep(1);
                    group[1] = jo["data"]["skus"][0]["product"]["id"].ToString();
                    if (randomsize)
                    {
                        if (ja[i]["availability"]["level"].ToString() != "OOS")
                        {
                            share_dog_skuid[tk.Region + tk.Sku].Add(ja[i]["id"].ToString());
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
                                    share_dog_skuid[tk.Region + tk.Sku].Add(ja[i]["id"].ToString());
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
                                share_dog_skuid[tk.Region + tk.Sku].Add(ja[i]["id"].ToString());
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
                tk.Status = "Proxy Error";
                goto A;
            }
            if (group[0] == null)
            {
                if (share_dog_skuid[tk.Region + tk.Sku].Count == 0)
                {
                    share_dog[tk.Region + tk.Sku] = false;
                    if (ct.IsCancellationRequested)
                    {
                        tk.Status = "IDLE";
                        share_dog[tk.Region + tk.Sku] = false;
                        share_dog_skuid[tk.Region + tk.Sku].Clear();
                        ct.ThrowIfCancellationRequested();
                    }
                    goto A;
                }

            }
            tk.Status = "Get Stock";
            share_dog[tk.Region + tk.Sku] = true;
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                share_dog[tk.Region + tk.Sku] = false;
                share_dog_skuid[tk.Region + tk.Sku].Clear();
                ct.ThrowIfCancellationRequested();
            }
            goto A;
        }
        public WebProxy monitorproxy()
        {
            WebProxy wp = new WebProxy();
            if (Mainwindow.monitorproxypool.Count == 0)
            {
                wp = default;
            }
            else
            {
                int random = ran.Next(0, Mainwindow.monitorproxypool.Count);
                string proxyg = Mainwindow.monitorproxypool[random].ToString();
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
            return wp;
        }
        public string GetHtmlsource(string url, Main.Monitor tk, CancellationToken ct)
        {
        A: if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            string SourceCode = "";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = 5000;
            request.Proxy = monitorproxy();
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
            request.Headers.Add("Accept-Encoding", "gzip, deflate br");
            request.Headers.Add("Accept-Language", "en-US, en; q=0.9");
            request.Headers.Add("Sec-Fetch-Dest", "document");
            request.Headers.Add("cache-control", "max-age=0");
            request.Headers.Add("Sec-Fetch-Mode", "navigate");
            request.Headers.Add("Sec-Fetch-Site", "none");
            request.Headers.Add("Sec-Fetch-user", "?1");
            request.Headers.Add("upgrade-insecure-requests", "1");
            request.UserAgent = "Sogou inst spider";
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
                if (ct.IsCancellationRequested)
                {
                    tk.Status = "IDLE";
                    ct.ThrowIfCancellationRequested();
                }
                tk.Status = "Change Proxy";
                goto A;
            }
            return SourceCode;
        }
    }
}

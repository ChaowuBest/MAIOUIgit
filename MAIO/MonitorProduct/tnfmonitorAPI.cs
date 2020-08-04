using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace MAIO
{
    class tnfmonitorAPI
    {
        Random ran = new Random();
        public object GetHtmlsource(string url)
        {
      
          A: string SourceCode = "";
            JArray ja = null;
            JObject jo = null;
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
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
          
                var chao = System.Web.HttpUtility.UrlDecode(response.Headers["Set-Cookie"]);
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
                if (url.Contains("VFAjaxProductAvailabilityView"))
                {
                    jo = JObject.Parse(SourceCode);
                    response.Close();
                    readStream.Close();
                    return jo;
                }
                else
                {
                    Regex rex = new Regex(@"(dataLayer = )(?=\[).+(?=\])]");
                    ja = JArray.Parse(rex.Match(SourceCode).ToString().Replace("dataLayer = ", ""));
                    response.Close();
                    readStream.Close();
                    return ja;
                }

            }
            catch (WebException ex)
            {
                HttpWebResponse response = (HttpWebResponse)ex.Response;
                goto A;
            }

        }
      /*  public object Monitor(string url, Main.Monitor mn, string eid, string sizeid)
        {
        A:
            string SourceCode = "";
            bool isstocknull = true;
            JObject jo = null;
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
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
              //  tk.Status = "WaitingRestock";
                var chao = System.Web.HttpUtility.UrlDecode(response.Headers["Set-Cookie"]);
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
                if (url.Contains("VFAjaxProductAvailabilityView"))
                {
                    jo = JObject.Parse(SourceCode);
                    response.Close();
                    readStream.Close();
                    foreach (var i in jo["partNumbers"].ToArray())
                    {
                        if (i.ToString().Contains(eid))
                        {
                            Regex rex = new Regex(@"\d{6}");
                            tnfsize.sizeid = rex.Match(i.ToString()).ToString();
                            break;
                        }
                    }
                    foreach (var i in jo["stock"].ToArray())
                    {
                        if (i.ToString().Contains(tnfsize.sizeid))
                        {
                            var st = i.ToString().Replace(tnfsize.sizeid, "").Replace(",", "").Replace(" ", "").Replace("\"", "").Replace(":", "");
                            int stock = int.Parse(st);
                            if (stock > 0)
                            {
                                isstocknull = false;
                                break;
                            }
                            else
                            {
                                goto A;
                            }
                        }
                    }

                }
                return isstocknull;

            }
            catch (WebException ex)
            {
                HttpWebResponse response = (HttpWebResponse)ex.Response;
                goto A;
            }

        }*/
    }
}

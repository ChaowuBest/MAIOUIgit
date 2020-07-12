using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace MAIO
{
    class TheNorthFaceAPI
    { 
        Random ran = new Random();
        string setATCcookie = "";
        public object GetHtmlsource(string url, Main.taskset tk, CancellationToken ct)
        {
        A: if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();

            }
            string SourceCode = "";
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
                tk.Status = "Get Size";
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
        public string ATC(string url, Main.taskset tk, CancellationToken ct,string info)
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
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Proxy = wp;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            byte[] contentpaymentinfo = Encoding.UTF8.GetBytes(info);
            request.Accept = "application/json, text/javascript, */*; q=0.01";
            request.Headers.Add("Cookie", @"cn3=0; _abck=D15C8E4570B6B79D26453D785FFFF7EC~-1~YAAQt1sauM/0Uy1zAQAAeYkmQQTQIsBea70EtEpCbEYEsmVCZv6deQhiIjWql+MLlygndpZjFjKMZAa4bi88a/uu872LhxGXKbwtCLiNtSQqLBo2nM7apbkpovbkAT5gukF91Q/aNvUjaWkY4mUYAS86Kbw7sFj64aQZKSAxJUx0pZFsMrE0vatfeX9oR2JyQXoyrYn5wxtzTwy7Peyqc7e10Kr80kVIHT/7e6xcMRKiJOajrkkqVSFVB9VkzA+yvzZX9ouzDMOWhuR5r0bUpoP/SjX0WrcijZnmFktr8Vdgrie02wfhwIwya5nemjAi~-1~-1~-1;");
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
            request.ContentLength = contentpaymentinfo.Length;
            request.Host = "www.thenorthface.com";
            request.Referer = "https://www.thenorthface.com/shop/mens-auxiliary-new-arrivals/m-glacier-short-nf0a4cei?variationId=ECL";
            request.Headers.Add("Origin", "https://www.thenorthface.com");
            request.Headers.Add("Sec-Fetch-Dest", "empty");
            request.Headers.Add("Sec-Fetch-Mode", "cors");
            request.Headers.Add("Sec-Fetch-Site", "none");
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36 OPR/68.0.3618.173";
            request.Headers.Add("X-Requested-With", "XMLHttpRequest");
            Stream paymentstream = request.GetRequestStream();
            paymentstream.Write(contentpaymentinfo, 0, contentpaymentinfo.Length);
            paymentstream.Close();
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                tk.Status = "ATC success";
                var cc = response.Headers["Set-Cookie"];
                string[] cookiename = new string[] { "JSESSIONID", "akavpau_VP_Scheduled_Maintenance", "", "SHOPPINGCART7001", "", "WC_PERSISTENT" };
                Regex rex1 = new Regex(@"(WC_AUTHENTICATION_)\d{9}");
                Regex rex2 = new Regex(@"(WC_USERACTIVITY_)\d{9}");
                cookiename[4]=rex1.Match(cc).ToString();
                cookiename[2]= rex2.Match(cc).ToString();
                for (int i = 0; i < cookiename.Length; i++)
                {
                    Regex rex3 = new Regex(@"(?<="+cookiename[i]+"=)([^;]+)");
                    cookiename[i] += "="+rex3.Match(cc).ToString();
                    setATCcookie += cookiename[i]+"; ";
                }             
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
                JObject ja = JObject.Parse(SourceCode);
                string orderid=ja["orderId"][0].ToString();
                return orderid;
            }
            catch (WebException ex)
            {
                HttpWebResponse response = (HttpWebResponse)ex.Response;
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
                goto A;
            }
        }
        public void shipping(string url, Main.taskset tk, CancellationToken ct, string info)
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
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Proxy = wp;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            byte[] contentpaymentinfo = Encoding.UTF8.GetBytes(info);
            request.Accept = "application/json, text/javascript, */*; q=0.01";
            request.Headers.Add("Cookie", @"cn3=0; _abck=D15C8E4570B6B79D26453D785FFFF7EC~-1~YAAQt1sauM/0Uy1zAQAAeYkmQQTQIsBea70EtEpCbEYEsmVCZv6deQhiIjWql+MLlygndpZjFjKMZAa4bi88a/uu872LhxGXKbwtCLiNtSQqLBo2nM7apbkpovbkAT5gukF91Q/aNvUjaWkY4mUYAS86Kbw7sFj64aQZKSAxJUx0pZFsMrE0vatfeX9oR2JyQXoyrYn5wxtzTwy7Peyqc7e10Kr80kVIHT/7e6xcMRKiJOajrkkqVSFVB9VkzA+yvzZX9ouzDMOWhuR5r0bUpoP/SjX0WrcijZnmFktr8Vdgrie02wfhwIwya5nemjAi~-1~-1~-1;");
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
            request.ContentLength = contentpaymentinfo.Length;
            request.Host = "www.thenorthface.com";
            request.Headers.Add("Origin", "https://www.thenorthface.com");
            request.Headers.Add("Sec-Fetch-Dest", "empty");
            request.Headers.Add("Sec-Fetch-Mode", "cors");
            request.Headers.Add("Sec-Fetch-Site", "none");
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36 OPR/68.0.3618.173";
            request.Headers.Add("X-Requested-With", "XMLHttpRequest");
            Stream paymentstream = request.GetRequestStream();
            paymentstream.Write(contentpaymentinfo, 0, contentpaymentinfo.Length);
            paymentstream.Close();
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var cc = response.Headers["Set-Cookie"];
               /* string[] cookiename = new string[] { "JSESSIONID", "akavpau_VP_Scheduled_Maintenance", "", "SHOPPINGCART7001", "", "WC_PERSISTENT" };
                Regex rex1 = new Regex(@"(WC_AUTHENTICATION_)\d{9}");
                Regex rex2 = new Regex(@"(WC_USERACTIVITY_)\d{9}");
                cookiename[4] = rex1.Match(cc).ToString();
                cookiename[2] = rex2.Match(cc).ToString();
                for (int i = 0; i < cookiename.Length; i++)
                {
                    Regex rex3 = new Regex(@"(?<=" + cookiename[i] + "=)([^;]+)");
                    cookiename[i] += "=" + rex3.Match(cc).ToString();
                    setATCcookie += cookiename[i] + "; ";
                }*/
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
                
            }
            catch (WebException ex)
            {
                HttpWebResponse response = (HttpWebResponse)ex.Response;
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
                goto A;
            }
        }
    }
}

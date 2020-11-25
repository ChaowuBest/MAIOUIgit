using PuppeteerSharp;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.ServiceModel.Channels;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;

namespace MAIO.JDUS
{
    class JDUSAPI
    {
        Random ran = new Random();
        string JSESSIONID = null;
        string csrftoken = null;
        string cartguid = null;
        public string productid = null;
        string requestid = Guid.NewGuid().ToString();
        string abck = "E6152AFA0CCA11C1FAEFAD68D34B0BD5~-1~YAAQp93tzLW4N/l0AQAAYTs0GwTuonnSrTCZZczJzQjMiSSF08JcoG2LbMRvSw/qYZC2xkJie5dXKfaFslqXSbZM9XJfNgOjJsO1YXQNUvUmF5TFCWX3dnJVRIHPLEPvrWT/t3kJVRXQXc/F9TwRK5yeCyt53edee8izQVRxEtJBEAt3nZxvrAZanxza2xy77r5v9Jwv0M7Xgbdw9LJWaJK037yOm21CxE0T+L7RlTztOB/259xq2FeXr7gtPq7YFk9XG8TMu5ctPD5tQnpVP+3c1W31s/y7zI7imjGnMy8biI/h7KzQyJYJyD7dTcKqmTDjkEkQqEaBfAf6NsNSe1ASsYWm6gjo~-1~-1~-1";
        string bmsz = "0C0936F4759EEE4C6AF84E9C38039F3F~YAAQp93tzCu2N/l0AQAA0iMqGwnZ6P2HLwkyDZ+hMHWoEx1Znk6uSYajRZKZMdMJc/n2Ci/Q8dSjSy2fXvd1L2222KvaCP8blw8leSD8u+JqumSEUvpuqpHc0kZYDVv+wlUB7kAb3TLBbyyRWY7i1erUffq875Cj6rEclSLoptb0E7TPQuRW2ml/JQy5PAUTlDFWJA==";
        string abck2 = "E6152AFA0CCA11C1FAEFAD68D34B0BD5~-1~YAAQp93tzLW4N/l0AQAAYTs0GwTuonnSrTCZZczJzQjMiSSF08JcoG2LbMRvSw/qYZC2xkJie5dXKfaFslqXSbZM9XJfNgOjJsO1YXQNUvUmF5TFCWX3dnJVRIHPLEPvrWT/t3kJVRXQXc/F9TwRK5yeCyt53edee8izQVRxEtJBEAt3nZxvrAZanxza2xy77r5v9Jwv0M7Xgbdw9LJWaJK037yOm21CxE0T+L7RlTztOB/259xq2FeXr7gtPq7YFk9XG8TMu5ctPD5tQnpVP+3c1W31s/y7zI7imjGnMy8biI/h7KzQyJYJyD7dTcKqmTDjkEkQqEaBfAf6NsNSe1ASsYWm6gjo~-1~-1~-1";
        string bmsz2 = "0C0936F4759EEE4C6AF84E9C38039F3F~YAAQp93tzCu2N/l0AQAA0iMqGwnZ6P2HLwkyDZ+hMHWoEx1Znk6uSYajRZKZMdMJc/n2Ci/Q8dSjSy2fXvd1L2222KvaCP8blw8leSD8u+JqumSEUvpuqpHc0kZYDVv+wlUB7kAb3TLBbyyRWY7i1erUffq875Cj6rEclSLoptb0E7TPQuRW2ml/JQy5PAUTlDFWJA==";
        string[] cookiename;
        public string GetHtmlsource(string url, Main.taskset tk, CancellationToken ct)
        {
        A: if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            string SourceCode = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Proxy = getproxy();
            request.Method = "GET";
            request.Accept = "application/json";
            request.Headers.Add("accept-encoding", "gzip, deflate, br");
            request.Headers.Add("accept-language", "en-US,en;q=0.9");
            request.Headers.Add("cache-control", "max-age=0");
            request.Headers.Add("Sec-Fetch-Dest", "document");
            request.Headers.Add("Sec-Fetch-Mode", "navigate");
            request.Headers.Add("Sec-Fetch-Site", "none");
            request.Headers.Add("sec-fetch-user", "?1");
            request.Headers.Add("upgrade-insecure-requests", "1");
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.105 Safari/537.36";
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var cookie = response.Headers["Set-Cookie"].ToString();
                Regex rex3 = new Regex(@"(?<=JSESSIONID=)([^;]+)");
                bool token = false;
                if (rex3.IsMatch(cookie))
                {
                    token = true;
                    JSESSIONID = "JSESSIONID=" + rex3.Match(cookie);
                }
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;
                if (response.ContentEncoding == "gzip")
                {
                    readStream = new StreamReader(new GZipStream(receiveStream, CompressionMode.Decompress), Encoding.GetEncoding("utf-8"));
                }
                else { readStream = new StreamReader(receiveStream, Encoding.UTF8); }
                SourceCode = readStream.ReadToEnd();
                JObject jo = JObject.Parse(SourceCode);
                if (token)
                {
                    csrftoken = jo["data"]["csrfToken"].ToString();
                }
                response.Close();
                readStream.Close();
            }
            catch (WebException ex)
            {
                HttpWebResponse response = (HttpWebResponse)ex.Response;
                tk.Status = "Proxy Error";
                goto A;
            }
            return SourceCode;
        }
        public string ATC(string url, Main.taskset tk, CancellationToken ct, string info)
        {
        A: if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            string SourceCode = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Proxy = getproxy();
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.Headers.Add("Cookie", JSESSIONID + "; _abck=" + abck + "; bm_sz=" + bmsz + "; check=true; signal_on");
            request.Referer = "https://www.footlocker.com/product/nike-air-force-1-lv8-mens/W6999600.html";
            request.Headers.Add("pragma", "no-cache");
            byte[] contentpaymentinfo = Encoding.UTF8.GetBytes(info);
            request.ContentLength = contentpaymentinfo.Length;
            request.Headers.Add("cache-control", "no-cache");
            request.Headers.Add("accept-encoding", "gzip, deflate, br");
            request.Headers.Add("accept-language", "en-US,en;q=0.9");
            request.Headers.Add("origin", "https://www.footlocker.com");
            request.Headers.Add("Sec-Fetch-Dest", "empty");
            request.Headers.Add("Sec-Fetch-Mode", "cors");
            request.Headers.Add("Sec-Fetch-Site", "same-origin");
            request.Headers.Add("x-csrf-token", csrftoken);
            request.Headers.Add("x-fl-productid", productid);
            request.Headers.Add("x-fl-request-id", Guid.NewGuid().ToString());
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.105 Safari/537.36";
            Stream paymentstream = request.GetRequestStream();
            paymentstream.Write(contentpaymentinfo, 0, contentpaymentinfo.Length);
            paymentstream.Close();
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Regex rex3 = new Regex(@"(?<=cart-guid=)([^;]+)");
                cartguid = "cart-guid=" + rex3.Match(response.Headers["Set-Cookie"].ToString());
                var wu = response.Headers["Set-Cookie"].ToString();
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;
                if (response.ContentEncoding == "gzip")
                {
                    readStream = new StreamReader(new GZipStream(receiveStream, CompressionMode.Decompress), Encoding.GetEncoding("utf-8"));
                }
                else { readStream = new StreamReader(receiveStream, Encoding.UTF8); }
                SourceCode = readStream.ReadToEnd();
                JObject jo = JObject.Parse(SourceCode);
                response.Close();
                readStream.Close();
            }
            catch (WebException ex)
            {
                HttpWebResponse resppayment = (HttpWebResponse)ex.Response;
                if (resppayment.StatusCode.ToString() == "Unknown")
                {
                    tk.Status = "OOS Retrying";
                }
                else
                {
                    tk.Status = "ATC Error";
                }
                Stream processtream = resppayment.GetResponseStream();
                StreamReader readprocessstream = new StreamReader(processtream, Encoding.UTF8);
                string processcode = readprocessstream.ReadToEnd();
                goto A;
            }
            return SourceCode;
        }
        public string PUT(string url, Main.taskset tk, CancellationToken ct)
        {
        A: if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            string SourceCode = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
         //   request.Proxy = getproxy();
            request.Method = "PUT";
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.Headers.Add("Server-Host", "www.footlocker.com:443");
            request.Headers.Add("Proxy-Address", getAdvproxy());
            request.Headers.Add("Cookie", JSESSIONID + "; _abck=" + abck + "; bm_sz=" + bmsz + "; check=true; signal_on");
            request.Headers.Add("pragma", "no-cache");
            request.ContentLength = 0;
            request.Headers.Add("cache-control", "no-cache");
            request.Headers.Add("accept-encoding", "gzip, deflate, br");
            request.Headers.Add("accept-language", "en-US,en;q=0.9");
            request.Headers.Add("origin", "https://www.footlocker.com");
            request.Headers.Add("Sec-Fetch-Dest", "empty");
            request.Headers.Add("Sec-Fetch-Mode", "cors");
            request.Headers.Add("Sec-Fetch-Site", "same-origin");
            request.Headers.Add("x-csrf-token", csrftoken);
            request.Headers.Add("x-fl-productid", productid);
            request.Headers.Add("x-fl-request-id", Guid.NewGuid().ToString());
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.105 Safari/537.36";
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;
                if (response.ContentEncoding == "gzip")
                {
                    readStream = new StreamReader(new GZipStream(receiveStream, CompressionMode.Decompress), Encoding.GetEncoding("utf-8"));
                }
                else { readStream = new StreamReader(receiveStream, Encoding.UTF8); }
                SourceCode = readStream.ReadToEnd();
                response.Close();
                readStream.Close();
            }
            catch (WebException ex)
            {
                HttpWebResponse resppayment = (HttpWebResponse)ex.Response;
                tk.Status = "Submit Email Failed";
                goto A;
            }
            return SourceCode;
        }
        public string subship(string url, Main.taskset tk, CancellationToken ct, string info)
        {
        A: if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            string SourceCode = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
           request.Proxy = getproxy();
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.Headers.Add("Cookie", JSESSIONID + "; _abck=" + abck2 + "; bm_sz=" + bmsz2 + "; check=true; signal_on");
          request.Headers.Add("pragma", "no-cache");
            byte[] contentpaymentinfo = Encoding.UTF8.GetBytes(info);
            request.ContentLength = contentpaymentinfo.Length;
           request.Headers.Add("cache-control", "no-cache");
            request.Headers.Add("accept-encoding", "gzip, deflate, br");
            request.Headers.Add("accept-language", "en-US,en;q=0.9");
            request.Headers.Add("origin", "https://www.footlocker.com");
            request.Referer = "https://www.footlocker.com/checkout";
            request.Headers.Add("Sec-Fetch-Dest", "empty");
            request.Headers.Add("Sec-Fetch-Mode", "cors");
            request.Headers.Add("Sec-Fetch-Site", "same-origin");
            request.Headers.Add("x-csrf-token", csrftoken);
            request.Headers.Add("x-fl-request-id", Guid.NewGuid().ToString());
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.105 Safari/537.36";
            Stream paymentstream = request.GetRequestStream();
            paymentstream.Write(contentpaymentinfo, 0, contentpaymentinfo.Length);
            paymentstream.Close();
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                tk.Status = "Submit Shipping";
                //  Regex rex3 = new Regex(@"(?<=cart-guid=)([^;]+)");
                //  cartguid = "cart-guid=" + rex3.Match(response.Headers["Set-Cookie"].ToString());
                var wu = response.Headers["Set-Cookie"].ToString();
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;
                if (response.ContentEncoding == "gzip")
                {
                    readStream = new StreamReader(new GZipStream(receiveStream, CompressionMode.Decompress), Encoding.GetEncoding("utf-8"));
                }
                else { readStream = new StreamReader(receiveStream, Encoding.UTF8); }
                SourceCode = readStream.ReadToEnd();
                JObject jo = JObject.Parse(SourceCode);
                response.Close();
                readStream.Close();
            }
            catch (WebException ex)
            {
                HttpWebResponse resppayment = (HttpWebResponse)ex.Response;
                tk.Status = "Submit Shipping Failed";
                 Stream processtream = resppayment.GetResponseStream();
                 StreamReader readprocessstream = new StreamReader(processtream, Encoding.UTF8);
                string processcode = readprocessstream.ReadToEnd();  
                goto A;
            }
            return SourceCode;
        }
        public WebProxy getproxy()
        {

            WebProxy wp = new WebProxy();
            if (Mainwindow.proxypool.Count == 0)
            {
                wp = default;
            }
            else
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
            return wp;
        }
        public string getAdvproxy()
        {
            string proxyaddress = null;
            if (Mainwindow.proxypool.Count == 0)
            {
                proxyaddress = "";
            }
            else
            {
                int random = ran.Next(0, Mainwindow.proxypool.Count);
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
            return proxyaddress;
        }
    }
}

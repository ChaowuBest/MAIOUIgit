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
        string abck = "840774E6D404A74C033B0D58CB1B8FBA~-1~YAAQH0Z7aBvCuMV0AQAAdbiCEwRUunIIM/sXORAkvR6y7VmGVqWRa53Z+qbJA7oAJHYVcwEIBf1NlwCL1iCjKi7mtwRIW4nkQwOAejQkPW4rtz0DO6G1YlkN8vwbMQt147EvlD8cpAWQFJhQ/ad/tzg3H9r62SHg3sj192Y5pyJ7QaT0gYq0XoOEue4eULTDuTZYsjw3bxE0/PNcBbl1Di2+mRRMdr03H8A0RIqrdxZFilPWa0+nvakmVsBDPui58LGZ/dH2kQVwYkeleaknSGzsMeDAUnudBxpuyl0gssrxm4Jap05m1w0etpu7J0hbNe9xoInSf1TxIdtuRGhaAZi1cCu2KqiX~-1~-1~-1";
        string bmsz = "234074504A7E8821ED4C982F3D1839CD~YAAQH0Z7aInBuMV0AQAAaNaBEwmNzTfStecu/vumKERpPtvJ3WI/ucA7SsQLlePLwQFJGkUWGhk1rySAhaUHesGOvopJCwOHtgpJanZbGS5Lnk2yjffsHDV1ez5NYlz03MQY5Hd6u0RgKtcPDJqZxDsguHyaOMyDwA5oBvFQY81pe6qJ9JQ5NLOrGTERigGyH6uULg==";
        string abck3 = "";
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
            request.Proxy = getproxy();
            request.Method = "PUT";
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.Headers.Add("Cookie", JSESSIONID + "; _abck=" + abck + "; bm_sz=" + bmsz + "; check=true; signal_on;");
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
                else { readStream = new StreamReader(receiveStream, Encoding.UTF8); }
                SourceCode = readStream.ReadToEnd();
                JObject jo = JObject.Parse(SourceCode);
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
            request.Headers.Add("Cookie", JSESSIONID + "; _abck=" + abck + "; bm_sz=" + bmsz + "; check=true; signal_on");
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
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
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
                //  Stream processtream = resppayment.GetResponseStream();
                //  StreamReader readprocessstream = new StreamReader(processtream, Encoding.UTF8);
                // string processcode = readprocessstream.ReadToEnd();  
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

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
        string cartguid=null;
        string productid = null;
        string requestid = Guid.NewGuid().ToString();
        string abck = "86090819620FE9E2720218875EA1B98D~-1~YAAQNiMduKqIMvl0AQAAxGpbDAS0PYLF4EzbX9uwX/fKSH/Ua1Ewm2ej9TiQCNCcLSQOjPt83P1TUFlSDYZcJoxs0U9z3bqHeIuHAKsqpDg4R+NMG5IGMsn/RdkqzBgIB3yW3tN87AdW9SaooP3FtLPzOCwhjnLdUlK0AmSH1mKqsf9wtBLjfOs3SOSZy7n1lTy2OA2OgMhjmhKVDWNU4HHnuSD3MauHriGJaNOWRYmPzznZb9B51IofhvIKLgkPo4mrYTlXA9N3FJW2r3Adkg/AJFfrQ3nZ4yv8uPF5hnaNuIKnDXR4sO2l4N0joG5oTjcVpTa9MaPg8V+5OOPH6D24O/6+5lZe~-1~-1~-1";
        string bmsz = "616FA65CD6C6EC06D8BF3DED2D39F079~YAAQtfAPF5MAWwp1AQAAGzi+CwkEfINm+9W16LRpZ18Y9I9Jgt+b3GjG1fQAogAOzbaBbv/31HD6kkHMAnsfXOesbKBfkB56mc8N9c6graFs0FzP3r2B5kZimCEZ1fx6i7yi/ERjGJC1cwpa970kXcVBL+UCAgynD11gmk9rf9INrW9jU3inE2P38y7Qd+gfMQwDBg==";
        string abck3 = "A49B210D6590F78CD40BF1D44A3FF864~-1~YAAQnB/JFy+psLVzAQAAQKD4uAQbszxW+JZjR6gKBXO016iH5k8z5AXLZeugHP5hC9v7NQrUiOdC2yP+Npq7QflA/FJC472DftmnKCrOrb58OErTR6rayuatl5ITiT9iNOnaCkx3RLUvywoVEk79PCUwFGihqx37FBFd0se1mSSZqKgzQFS2xv8pihO1BmAfw2gesm/750Le+K1kp2+op2XJJHESSSPHgcYd2XsFsFMDJMafboPG1Mpu9Bst8iTgrM7ch3SY8u5cjL8MP32z1XI2Yar1OfKSRxelqrBAH6HmE+RpRExJefupXEo4rL0Eh2ySAtkNvN8WoJz2lkFZyioKLpGCUJ9/iJ+G52gWRRRcj+Ac~-1~-1~-1";
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
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var cookie=response.Headers["Set-Cookie"].ToString();
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
                }else {readStream = new StreamReader(receiveStream, Encoding.UTF8);}
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
        public string ATC(string url, Main.taskset tk, CancellationToken ct,string info)
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
            request.Headers.Add("Cookie", JSESSIONID+"; _abck="+abck+"; bm_sz="+bmsz+"; check=true; signal_on");
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
                Regex rex3 = new Regex(@"(?<=cart-guid=)([^;]+)");
                cartguid = "cart-guid=" + rex3.Match(response.Headers["Set-Cookie"].ToString());
                var wu=response.Headers["Set-Cookie"].ToString();
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
                    tk.Status = "Proxy Error";
                }
              //  Stream processtream = resppayment.GetResponseStream();
              //  StreamReader readprocessstream = new StreamReader(processtream, Encoding.UTF8);
               // string processcode = readprocessstream.ReadToEnd();  
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
    }
}

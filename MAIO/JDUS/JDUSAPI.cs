using PuppeteerSharp;
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
        string setATCcookie = null;
        string abck = "0A54B7B1F74BBCD5ABAEA202C1573753~-1~YAAQnB/JF06usLVzAQAA8On4uAQwzgyO+NKZwNb/KWe3DLARNH62taNNPxMiM+pQ7sYyLa4cCtxIT+NjR5a55hOW8qUxuecyN4rNWaO3KYguD6Tm0/b9pmT8V7Irr+nIgXkvekXmhhfv201/JDwJ/3PeE21t0LSna2BgmfjfiygIFTFAGiwmHcyv6B09nUxZWs+VArGOmr+trKqjSs1lYf8V4Sk8qn52LUbl+dhi/ChAqNgRFs1o9fsJnhlaYLLBIi75bVWsrFB5/PNcP7ouZyeKmCaU1OSEOknzb8OSIFUAlhPeP7uO+zfM59GGgrRW3H9rjtDK9GqDawtOG+BuPvyfuvYY0P1l+UOCWaIzD/eUGpu0~-1~-1~-1";
        string abck2 = "A49B210D6590F78CD40BF1D44A3FF864~-1~YAAQnB/JFy+psLVzAQAAQKD4uAQbszxW+JZjR6gKBXO016iH5k8z5AXLZeugHP5hC9v7NQrUiOdC2yP+Npq7QflA/FJC472DftmnKCrOrb58OErTR6rayuatl5ITiT9iNOnaCkx3RLUvywoVEk79PCUwFGihqx37FBFd0se1mSSZqKgzQFS2xv8pihO1BmAfw2gesm/750Le+K1kp2+op2XJJHESSSPHgcYd2XsFsFMDJMafboPG1Mpu9Bst8iTgrM7ch3SY8u5cjL8MP32z1XI2Yar1OfKSRxelqrBAH6HmE+RpRExJefupXEo4rL0Eh2ySAtkNvN8WoJz2lkFZyioKLpGCUJ9/iJ+G52gWRRRcj+Ac~-1~-1~-1";
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
            request.ContentType = "application/json; charset=UTF-8";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
            request.Headers.Add("accept-encoding", "gzip, deflate, br");
            request.Headers.Add("accept-language", "en-US,en;q=0.9");
            request.Headers.Add("origin", "https://www.jdsports.com/");
            request.Headers.Add("cache-control", "max-age=0");
          //  request.Headers.Add("Cookie",setATCcookie);
            request.Headers.Add("Sec-Fetch-Dest", "document");
            request.Headers.Add("Sec-Fetch-Mode", "navigate");
            request.Headers.Add("Sec-Fetch-Site", "none");
            request.Headers.Add("sec-fetch-user", "?1");
            request.Headers.Add("upgrade-insecure-requests", "1");
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (url.Contains("miniCart"))
                {
                    var cc = response.Headers["Set-Cookie"];
                    cookiename = new string[] { "JSESSIONID", "FNL_RISKIFIED_ID", "bm_mi", "akaalb_www-jdsports" };
                    for (int i = 0; i < cookiename.Length; i++)
                    {
                        Regex rex3 = new Regex(@"(?<=" + cookiename[i] + "=)([^;]+)");
                        cookiename[i] += "=" + rex3.Match(cc).ToString();
                        setATCcookie += cookiename[i] + "; ";
                    }
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
                response.Close();
                readStream.Close();
                tk.Status = "Get Size";
            }
            catch (WebException ex)
            {
                HttpWebResponse response = (HttpWebResponse)ex.Response;
             
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
            if (setATCcookie == null)
            {
                Thread.Sleep(2000);
                goto A;
            }
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
           request.Proxy = wp;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            byte[] contentpaymentinfo = Encoding.UTF8.GetBytes(info);
            request.Accept = "application/json, text/javascript, */*; q=0.01";
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("Accept-Language", "en-US,en;q=0.9");
            request.Headers.Add("Cookie", setATCcookie + "_abck=" + abck);
            // request.Headers.Add("Cookie", setATCcookie+ " _abck=0682092AA2E399B208B05419A712CFE4~-1~YAAQDmAZuKkDPLBzAQAA6Y9ZtAQZvAYDvOPCO/mEgmdgDmbNM42JPhR4kFY/gQOXjb2+Yg1z3UcH8SnaeCt3Y8XfUCd9IB8GDqSnz/27Cw9KtCpgBY2ab2PIKBJvGH8wPe/psK5VvMkCMtyw61Nyw+vSI/jsf+krxTEkjrA1actaSnHSaAnt+EDrdcqPPR2GdQvPw1bxXf9FsirzxWcOlQuIdAlihDmerDQ+Nxae1XBDijfAXiXJVHgsyIMnu6R/cOOR9o2M8vnMy0UW5PLXm80d0iIEgwuqMi7kg8SXEE64n+s3dSVAswxlEcVaSeNTaj/t7N0C8RLlt9yFdeTb7vcF8qmS5/KbJlZsvbekeYucCigJ~-1~-1~-1");
            request.ContentLength = contentpaymentinfo.Length;
            request.Headers.Add("Origin", "https://www.jdsports.com/");
            request.Headers.Add("Sec-Fetch-Dest", "empty");
            request.Headers.Add("Sec-Fetch-Mode", "cors");
            request.Headers.Add("Sec-Fetch-Site", "same-origin");
            request.Headers.Add("x-requested-with", "XMLHttpRequest");
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
            Stream paymentstream = request.GetRequestStream();
            paymentstream.Write(contentpaymentinfo, 0, contentpaymentinfo.Length);
            paymentstream.Close();
            try
            {
             HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                tk.Status = "ATC success";
                var cc = response.Headers["Set-Cookie"];
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
                SourceCode = readStream.ReadToEnd().Replace("\"","");
                Regex rex = new Regex(@"(?<=value=).\d{17}[^ ]");
                Regex rex2 = new Regex(@"(?<=value=)(-).\d{17}[^ ]");
                string _dynSessConf = "";
                if (rex2.IsMatch(SourceCode))
                {
                     _dynSessConf = rex2.Match(SourceCode).ToString();
                }
                else
                {
                    _dynSessConf = rex.Match(SourceCode).ToString();
                }
           
                return _dynSessConf;
            }
            catch (WebException)
            {
                goto A;
            }
        }
        public void SubmittingShipping(string url, Main.taskset tk, CancellationToken ct, string info)
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
            request.Accept = "*/*";
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("Accept-Language", "en-US,en;q=0.9");
            request.Headers.Add("cache-control", " max-age=0");
            request.Headers.Add("Cookie", setATCcookie+"_abck="+abck);
            request.ContentLength = contentpaymentinfo.Length;
            request.Headers.Add("Origin", "https://www.jdsports.com/");
            request.Headers.Add("Sec-Fetch-Dest", "document");
            request.Headers.Add("Sec-Fetch-Mode", "navigate");
            request.Headers.Add("Sec-Fetch-Site", "same-origin");
            request.Headers.Add("sec-fetch-user", "?1");
            request.Headers.Add("upgrade-insecure-requests", "1");
            request.Headers.Add("x-requested-with", "XMLHttpRequest");
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
            Stream paymentstream = request.GetRequestStream();
            paymentstream.Write(contentpaymentinfo, 0, contentpaymentinfo.Length);
            paymentstream.Close();
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                tk.Status = "Submitting Shipping";
                var cc = response.Headers["Set-Cookie"];
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
               
               // goto A;
            }
        }
        public void SubmittingShipping2(string url, Main.taskset tk, CancellationToken ct, string info)
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
            request.Accept = "*/*";
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("Accept-Language", "en-US,en;q=0.9");
            request.Headers.Add("cache-control", " max-age=0");
            request.Headers.Add("Cookie", setATCcookie + "_abck=" + abck2);
            request.ContentLength = contentpaymentinfo.Length;
            request.Headers.Add("Origin", "https://www.jdsports.com/");
            request.Headers.Add("Sec-Fetch-Dest", "document");
            request.Headers.Add("Sec-Fetch-Mode", "navigate");
            request.Headers.Add("Sec-Fetch-Site", "same-origin");
            request.Headers.Add("sec-fetch-user", "?1");
            request.Headers.Add("upgrade-insecure-requests", "1");
            request.Headers.Add("x-requested-with", "XMLHttpRequest");
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
            Stream paymentstream = request.GetRequestStream();
            paymentstream.Write(contentpaymentinfo, 0, contentpaymentinfo.Length);
            paymentstream.Close();
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                tk.Status = "Submitting Shipping";
                var cc = response.Headers["Set-Cookie"];
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

                goto A;
            }
        }
        public void SubmittingBilling(string url, Main.taskset tk, CancellationToken ct, string info)
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
            request.ContentType = "application/x-www-form-urlencoded";
            byte[] contentpaymentinfo = Encoding.UTF8.GetBytes(info);
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("Accept-Language", "en-US,en;q=0.9");
            request.Headers.Add("cache-control", " max-age=0");
            request.Headers.Add("Cookie", setATCcookie + "_abck=" + abck2);
            request.ContentLength = contentpaymentinfo.Length;
            request.Headers.Add("Origin", "https://www.jdsports.com/");
            request.Headers.Add("Sec-Fetch-Dest", "document");
            request.Headers.Add("Sec-Fetch-Mode", "navigate");
            request.Headers.Add("Sec-Fetch-Site", "same-origin");
            request.Headers.Add("sec-fetch-user", "?1");
            request.Headers.Add("upgrade-insecure-requests", "1");
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
            Stream paymentstream = request.GetRequestStream();
            request.Headers.Add("x-requested-with", "XMLHttpRequest");
            paymentstream.Write(contentpaymentinfo, 0, contentpaymentinfo.Length);
            paymentstream.Close();
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                tk.Status = "Submitting Billing";
                var cc = response.Headers["Set-Cookie"];
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
                if (response.StatusCode.ToString() == "Forbbiden")
                {
                    goto A;
                }
                var chao=response.Headers["Location"];
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
        }
        public void SubmittingOrder(string url, Main.taskset tk, CancellationToken ct, string info)
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
            request.Accept = "*/*";
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("Accept-Language", "en-US,en;q=0.9");
            request.Headers.Add("cache-control", " max-age=0");
            request.Headers.Add("Cookie", setATCcookie + "_abck=" + abck2);
            request.ContentLength = contentpaymentinfo.Length;
            request.Headers.Add("Origin", "https://www.jdsports.com/");
            request.Headers.Add("Sec-Fetch-Dest", "document");
            request.Headers.Add("Sec-Fetch-Mode", "navigate");
            request.Headers.Add("Sec-Fetch-Site", "same-origin");
            request.Headers.Add("sec-fetch-user", "?1");
            request.Headers.Add("upgrade-insecure-requests", "1");
            request.Headers.Add("x-requested-with", "XMLHttpRequest");
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
            Stream paymentstream = request.GetRequestStream();
            paymentstream.Write(contentpaymentinfo, 0, contentpaymentinfo.Length);
            paymentstream.Close();
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                tk.Status = "Submitting Order";
                var cc = response.Headers["Set-Cookie"];
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
                if (response.StatusCode.ToString() == "Forbbiden")
                {
                    goto A;
                }
            }
        }
    }
}

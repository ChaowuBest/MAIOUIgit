using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace MAIO
{
    class TheNorthFaceAPI
    {
        Random ran = new Random();
        string setATCcookie = "";
        string[] cookiename;
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
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Proxy = wp;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            byte[] contentpaymentinfo = Encoding.UTF8.GetBytes(info);
            request.Accept = "application/json, text/javascript, */*; q=0.01";
            // request.Headers.Add("Cookie", @"cn3=0; _abck=D15C8E4570B6B79D26453D785FFFF7EC~-1~YAAQt1sauM/0Uy1zAQAAeYkmQQTQIsBea70EtEpCbEYEsmVCZv6deQhiIjWql+MLlygndpZjFjKMZAa4bi88a/uu872LhxGXKbwtCLiNtSQqLBo2nM7apbkpovbkAT5gukF91Q/aNvUjaWkY4mUYAS86Kbw7sFj64aQZKSAxJUx0pZFsMrE0vatfeX9oR2JyQXoyrYn5wxtzTwy7Peyqc7e10Kr80kVIHT/7e6xcMRKiJOajrkkqVSFVB9VkzA+yvzZX9ouzDMOWhuR5r0bUpoP/SjX0WrcijZnmFktr8Vdgrie02wfhwIwya5nemjAi~-1~-1~-1;");
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
                cookiename = new string[] { "JSESSIONID", "akavpau_VP_Scheduled_Maintenance", "", "SHOPPINGCART7001", "", "WC_PERSISTENT", "WC_ACTIVEPOINTER", };
                Regex rex1 = new Regex(@"(WC_AUTHENTICATION_)\d{9}");
                Regex rex2 = new Regex(@"(WC_USERACTIVITY_)\d{9}");
                cookiename[4] = rex1.Match(cc).ToString();
                cookiename[2] = rex2.Match(cc).ToString();
                for (int i = 0; i < cookiename.Length; i++)
                {
                    Regex rex3 = new Regex(@"(?<=" + cookiename[i] + "=)([^;]+)");
                    TnfCheckoutcookie.cookielist[i].Name = cookiename[i];
                    cookiename[i] += "=" + rex3.Match(cc).ToString();
                    TnfCheckoutcookie.cookielist[i].Value= rex3.Match(cc).ToString();
                    TnfCheckoutcookie.cookielist[i].Domain = "www.thenorthface.com";
                    setATCcookie += cookiename[i] + "; ";                 
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
                string orderid = ja["orderId"][0].ToString();
                return orderid;
            }
            catch (Exception ex)
            {

                goto A;
            }
        }
        public string shipping(string url, Main.taskset tk, CancellationToken ct, string info)
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
            var vhao = setATCcookie + @"_abck=6B1F86726B56B280CE1E1B6B0F52E87F~0~YAAQt1sauAqYWS1zAQAASDPCRQRkXVaReL4shFhAGm31XmxlmD3vdbCdNdtwY3tI7OEvqbCCOoysmIiEyQPd/V6nKBB8GtCzbN42dZmWXpA01yhbcoF8yBKNTWFaHx/0V2d228vAFjisBfn0x0Mun8uuD5Vxg26TzPD4DcMI8Oe1P5jrmfBpk09/y59u/A/5Lf0sntUaJVhMDNqiQEnXPyqOscU4fNi/C5dqA6qIzrylfo6btxcGkKEjXP4pGdv1dNCIhoZCIVKC/+Uwk3FEMPSiJ9kKBZvCZt83Soe6e+j0iO1ZfpSGMCBzHL3Wf4e3GnIZ4z1Dwrzw5ARKWA==~-1~-1~-1; bm_sz=524251366C1A0862CDA936AB8C2469AE~YAAQfSZzaHrFdC5zAQAA7Hq8RQg3tdx66S40ALG3yb4MMWbVi4kbv1jP0GtlhCrX9NsgozpV67sh77NR+0reRUsX9Iax0worjBzKp2KRwID9eD4F+OVyHLPkwGMBUtOKcDQJ9P2dCSNhPWbfU/GTJfL1/lukeH8kLFcHesaIIkc0I/BQZCqmXpdrdB1OgSLL6r3Le384";
            request.Headers.Add("Cookie", vhao);
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
            request.ContentLength = contentpaymentinfo.Length;
            request.Host = "www.thenorthface.com";
            request.Headers.Add("Origin", "https://www.thenorthface.com");
            request.Headers.Add("Sec-Fetch-Dest", "empty");
            request.Headers.Add("Sec-Fetch-Mode", "cors");
            request.Headers.Add("Sec-Fetch-Site", "same-origin");
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36 OPR/68.0.3618.173";
            request.Headers.Add("X-Requested-With", "XMLHttpRequest");
            Stream paymentstream = request.GetRequestStream();
            paymentstream.Write(contentpaymentinfo, 0, contentpaymentinfo.Length);
            paymentstream.Close();
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                tk.Status = "SubmittingShipping";
                var cc = response.Headers["Set-Cookie"];
                Regex rex3 = new Regex(@"(?<=WC_PERSISTENT)([^;]+)");
                if (rex3.Match(cc).Success)
                {
                    cookiename[5] = "WC_PERSISTENT" + rex3.Match(cc).ToString();
                    setATCcookie = "";
                    for (int i = 0; i < cookiename.Length; i++)
                    {
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
                if (SourceCode.Contains("errorMessage"))
                {
                    tk.Status = "Shipping error";
                    goto A;
                }
                return SourceCode;
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
        public void preorder(string url, Main.taskset tk, CancellationToken ct, string info)
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
            var vhao = setATCcookie + @"_abck=6B1F86726B56B280CE1E1B6B0F52E87F~0~YAAQt1sauAqYWS1zAQAASDPCRQRkXVaReL4shFhAGm31XmxlmD3vdbCdNdtwY3tI7OEvqbCCOoysmIiEyQPd/V6nKBB8GtCzbN42dZmWXpA01yhbcoF8yBKNTWFaHx/0V2d228vAFjisBfn0x0Mun8uuD5Vxg26TzPD4DcMI8Oe1P5jrmfBpk09/y59u/A/5Lf0sntUaJVhMDNqiQEnXPyqOscU4fNi/C5dqA6qIzrylfo6btxcGkKEjXP4pGdv1dNCIhoZCIVKC/+Uwk3FEMPSiJ9kKBZvCZt83Soe6e+j0iO1ZfpSGMCBzHL3Wf4e3GnIZ4z1Dwrzw5ARKWA==~-1~-1~-1; bm_sz=524251366C1A0862CDA936AB8C2469AE~YAAQfSZzaHrFdC5zAQAA7Hq8RQg3tdx66S40ALG3yb4MMWbVi4kbv1jP0GtlhCrX9NsgozpV67sh77NR+0reRUsX9Iax0worjBzKp2KRwID9eD4F+OVyHLPkwGMBUtOKcDQJ9P2dCSNhPWbfU/GTJfL1/lukeH8kLFcHesaIIkc0I/BQZCqmXpdrdB1OgSLL6r3Le384";
            request.Headers.Add("Cookie", vhao);
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
            request.ContentLength = contentpaymentinfo.Length;
            request.Host = "www.thenorthface.com";
            request.Headers.Add("Origin", "https://www.thenorthface.com");
            request.Headers.Add("Sec-Fetch-Dest", "empty");
            request.Headers.Add("Sec-Fetch-Mode", "cors");
            request.Headers.Add("Sec-Fetch-Site", "same-origin");
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36 OPR/68.0.3618.173";
            request.Headers.Add("X-Requested-With", "XMLHttpRequest");
            Stream paymentstream = request.GetRequestStream();
            paymentstream.Write(contentpaymentinfo, 0, contentpaymentinfo.Length);
            paymentstream.Close();
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                tk.Status = "SubmittingBilling";
                var cc = response.Headers["Set-Cookie"];
                Regex rex3 = new Regex(@"(?<=WC_PERSISTENT)([^;]+)");
                if (rex3.Match(cc).Success)
                {
                    cookiename[5] = "WC_PERSISTENT" + rex3.Match(cc).ToString();
                    TnfCheckoutcookie.cookielist[5].Value = rex3.Match(cc).ToString();
                    setATCcookie = "";
                    for (int i = 0; i < cookiename.Length; i++)
                    {
                        setATCcookie += cookiename[i] + "; ";
                    }           
                }
                else
                {

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
        public void orderdetail(string url, Main.taskset tk, CancellationToken ct, string info)
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
            request.ContentLength = contentpaymentinfo.Length;
            Stream paymentstream = request.GetRequestStream();
            paymentstream.Write(contentpaymentinfo, 0, contentpaymentinfo.Length);
            paymentstream.Close();
            request.Accept = "application/json, text/javascript, */*; q=0.01";
            var vhao = setATCcookie + @"_abck=6B1F86726B56B280CE1E1B6B0F52E87F~0~YAAQt1sauAqYWS1zAQAASDPCRQRkXVaReL4shFhAGm31XmxlmD3vdbCdNdtwY3tI7OEvqbCCOoysmIiEyQPd/V6nKBB8GtCzbN42dZmWXpA01yhbcoF8yBKNTWFaHx/0V2d228vAFjisBfn0x0Mun8uuD5Vxg26TzPD4DcMI8Oe1P5jrmfBpk09/y59u/A/5Lf0sntUaJVhMDNqiQEnXPyqOscU4fNi/C5dqA6qIzrylfo6btxcGkKEjXP4pGdv1dNCIhoZCIVKC/+Uwk3FEMPSiJ9kKBZvCZt83Soe6e+j0iO1ZfpSGMCBzHL3Wf4e3GnIZ4z1Dwrzw5ARKWA==~-1~-1~-1; bm_sz=524251366C1A0862CDA936AB8C2469AE~YAAQfSZzaHrFdC5zAQAA7Hq8RQg3tdx66S40ALG3yb4MMWbVi4kbv1jP0GtlhCrX9NsgozpV67sh77NR+0reRUsX9Iax0worjBzKp2KRwID9eD4F+OVyHLPkwGMBUtOKcDQJ9P2dCSNhPWbfU/GTJfL1/lukeH8kLFcHesaIIkc0I/BQZCqmXpdrdB1OgSLL6r3Le384";
            request.Headers.Add("Cookie", vhao + "; WC_SESSION_ESTABLISHED=true");
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
            request.Host = "www.thenorthface.com";
            request.Headers.Add("Origin", "https://www.thenorthface.com");
            request.Headers.Add("Sec-Fetch-Dest", "empty");
            request.Headers.Add("Sec-Fetch-Mode", "cors");
            request.Headers.Add("Sec-Fetch-Site", "same-origin");
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36 OPR/68.0.3618.173";
            request.Headers.Add("X-Requested-With", "XMLHttpRequest");
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var cc = response.Headers["Set-Cookie"];
                Stream receiveStream = response.GetResponseStream();
                Regex rex3 = new Regex(@"(?<=WC_PERSISTENT)([^;]+)");
                if (rex3.Match(cc).Success)
                {
                    cookiename[5] = "WC_PERSISTENT" + rex3.Match(cc).ToString();
                    TnfCheckoutcookie.cookielist[5].Value = rex3.Match(cc).ToString();
                    setATCcookie = "";
                    for (int i = 0; i < cookiename.Length; i++)
                    {
                        setATCcookie += cookiename[i] + "; ";
                    }
                }
                else
                {
                }
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
        public string Checkout2(string url, Main.taskset tk, CancellationToken ct)
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
            string paypalurl = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(AlwaysGoodCertificate);
            request.Accept = "*/*";
            request.Proxy = wp;
            request.Headers.Add("Cookie", setATCcookie); //+ @"_abck=C2FCFCDE684A30D3A5A5C7F0F53373F5~0~YAAQt1sauFj6cC1zAQAAvu9LVQT+MZlZxHEfwuzP/Vxj8eU5COjTO+CUTKWU0rshGcvIO8ulIeqJyin2Q1sMeXkg4T6eASWhS9LjTY7RPHVmqQs8Bqhrqj5PMVqUG+SpAkMKqlV1eAwrUeFjfKm9Bsry2u2DdxRQJIPZOPGcT9w1T+QZ/PQfkrwPavpOczoIJ+HWOOmVcjO4MvzPBl0FZNqm1R6XfkMUhOEb4sc9A68i61LbQZI55Hjjr3SLmav2ooyvFxWW2VlZT0PojCFkjE/IkyONiqIQh9UEFzY1Cs+icuqcVtYlu5dUFeIZSErbW0rI0/uWRal0F9aUIQ==~-1~-1~-1; bm_sz=D3E84ED77D3408AE5ED7F61C8105E52C~YAAQt1sauLrNcC1zAQAAv90xVQgGtiyn7HdEpR98qimst+z2W1CGHsBAPjFc8OCL42LlilodC4lHinlUVaSykAfoGXh6mMJ7ofpNxDBrJVDA/huAWP2JiJu3hGJX0pLV88wQyR3ybYoWRNHl4UQxunOlgEDff5y/d9ZccHrkFnbhkWsNk3B5yljXzyhf2eiQ5wYVixpC");
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
            request.Host = "www.thenorthface.com";
            request.Headers.Add("Sec-Fetch-Dest", "document");
            request.Headers.Add("Sec-Fetch-Mode", "navigate");
            request.Headers.Add("Sec-Fetch-Site", "same-origin");
            request.Headers.Add("Sec-Fetch-User", "?1");
            request.Headers.Add("Upgrade-Insecure-Requests", "1");  
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36 OPR/68.0.3618.173";
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                tk.Status = "SubmittingPayment";
                paypalurl = response.ResponseUri.ToString();
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
                paypalurl=response.ResponseUri.ToString();
                if (response.ContentEncoding == "gzip")
                {
                    readStream = new StreamReader(new GZipStream(receiveStream, CompressionMode.Decompress), Encoding.GetEncoding("utf-8"));
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.UTF8);
                }
            }
            if (paypalurl == "")
            {
                goto A;
            }
            return paypalurl;
        }
        private static bool AlwaysGoodCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
        {
            return true;
        }
    }
}

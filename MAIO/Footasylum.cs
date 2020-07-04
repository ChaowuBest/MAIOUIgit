using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Markup;

namespace MAIO
{
    class Footasylum
    {
        public  bool randomsize = false;
        string basketid = "";
        public string link = "";
        public string profile = "";
        public string size = "";
        public Main.taskset tk = null;
        string coucustomer_id = "";
        FootasylumAPI fasyapi = new FootasylumAPI();
        public void StartTask(CancellationToken ct, CancellationTokenSource cts)
        {
            string skuid = "";
        A: JObject joprofile = JObject.Parse(profile);
            try
            {
                if (ct.IsCancellationRequested)
                {
                    tk.Status = "IDLE";
                    ct.ThrowIfCancellationRequested();

                }
                skuid=GetSKUID(ct);
            }
            catch (NullReferenceException ex)
            {
                goto A;
            }
            catch (OperationCanceledException)
            {
                return;
            }
        B: string payinfo = "";
            try
            {
                Checkout(joprofile.ToString(), skuid,ct);
            }
            catch (NullReferenceException)
            {
                goto B;
            }
            catch (OperationCanceledException)
            {
                return;
            }
        C: int i = 0;
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            try
            {
                Processorder(profile, ct);
            }
            catch (NullReferenceException)
            {
                goto C;
            }
            catch (OperationCanceledException)
            {
                return;
            }
             AutoCheckout(profile, ct, basketid);
        }
        public string GetSKUID(CancellationToken ct)
        {
            string url = link;
            var pid =link.Split("-");
            string pid2=pid[pid.Length-1].Replace("/","");
            var group =fasyapi.GetHtmlsource(link,tk,ct);
            string sourcecode = group[0];
             coucustomer_id = group[1];
            Regex regex = new Regex(@"{[^}]+}");
            MatchCollection match = regex.Matches(sourcecode);
            string skuid = "";
            foreach (var i in match)
            {
                if (i.ToString().Contains(pid2)&&i.ToString().Contains("Size "+size+""))
                {
                    Regex regexsku = new Regex(@"\d{7}");
                    MatchCollection matchsku = regexsku.Matches(i.ToString());
                    skuid =matchsku[0].ToString();
                }
            }
            return skuid;
        }
        public void Checkout(string profile, string skuid, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }

            string url = "https://www.footasylum.com/page/xt_orderform_additem/?target=ajx_basket.asp&sku=" + skuid + "&_=" + GetTimeStamp() + "";
            fasyapi.Checkout(url, tk, ct);
        }
        public void Processorder(string profile, CancellationToken ct)
        {
            string url = "https://www.footasylum.com/page/nw-api/initiatecheckout/";
            var session=fasyapi.PostCheckout(url, tk, ct);
            basketid = session[0];
            string paymenturl = "https://secure.footasylum.com/?checkoutSessionId=" + session[1]; 
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            if (Config.webhook == "")
            {
                tk.Status = paymenturl;
            }
            else
            {
                string pd2 = "{\"username\":\"MAIO\",\"avatar_url\":\"https://i.loli.net/2020/05/24/VfWKsEywcXZou1T.jpg\",\"embeds\":[{\"title\":\"You Just Chekcout!\",\"color\":3329330,\"footer\":{\"text\":\"" + "MAIO" + DateTime.Now.ToLocalTime().ToString() + "\",\"icon_url\":\"https://i.loli.net/2020/05/24/VfWKsEywcXZou1T.jpg\"},\"fields\":[{\"name\":\"Checkout out!!!\",\"value\":\"" + paymenturl + "\\t\\t\\t\\tSize:" + tk.Size + "\\t\\t\\t\\t\",\"inline\":false}]}]}";
                Http(Config.webhook, pd2);
                tk.Status = "Check Webhook";
            }
        }
        public void Http(string url, string postDataStr)
        {
        Retry: Random ra = new Random();
            int sleeptime = ra.Next(0, 3000);
            Thread.Sleep(sleeptime);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json; charset=utf-8";
            request.Method = "POST";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
            byte[] bytes = Encoding.UTF8.GetBytes(postDataStr);
            request.ContentLength = bytes.Length;
            Stream webstream = request.GetRequestStream();
            webstream.Write(bytes, 0, bytes.Length);
            webstream.Close();
            try
            {
                HttpWebResponse httpWebResponse = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                Thread.Sleep(5000000);
                tk.Status = ex.Message.ToString();
                goto Retry;
            }

        }
        public void AutoCheckout(string profile, CancellationToken ct, string basketid)
        {
            JObject jo = JObject.Parse(profile);
            string countryurl = "https://paymentgateway.checkout.footasylum.net/basket/shipping?medium=web&apiKey=lGJjE+ccd0SiBdu3I6yByRp3/yY8uVIRFa9afLx+2YSrSwkWDfxq0YKUsh96/tP84CZO4phvoR+0y9wtm9Dh5w==&checkout_client=secure";
            string postinfo = "{\"postcode\":\"08904\",\"country\":\"US\",\"basket\":{\"id\":110056757,\"basketItems\":[{\"id\":\"0930291\",\"qty\":1}]}}";
            fasyapi.Postcountry(countryurl,tk,ct,postinfo);

        }
        #region 时间戳
        public static string GetTimeStamp()
        {
            System.DateTime time = System.DateTime.Now;
            long ts = ConvertDateTimeToInt(time);
            return ts.ToString();
        }
        /// <summary>  
        /// 将c# DateTime时间格式转换为Unix时间戳格式  
        /// </summary>  
        /// <param name="time">时间</param>  
        /// <returns>long</returns>  
        private static long ConvertDateTimeToInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long t = (time.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位      
            return t;
        }
        #endregion
    }
}

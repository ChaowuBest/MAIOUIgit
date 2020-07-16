
using Newtonsoft.Json.Linq;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
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
        string checkoutsession = "";
        string paymenturl = "";
        FootasylumAPI fasyapi = new FootasylumAPI();
        string skuid = "";
        public void StartTask(CancellationToken ct, CancellationTokenSource cts)
        {
           
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
        B: //string payinfo = "";
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
        C: 
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
            checkoutsession = session[1];
             paymenturl = "https://secure.footasylum.com/?checkoutSessionId=" + session[1];
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
            string url = "https://paymentgateway.checkout.footasylum.net/basket/paraspar?basket_id="+checkoutsession+"&medium=web&apiKey=lGJjE+ccd0SiBdu3I6yByRp3/yY8uVIRFa9afLx+2YSrSwkWDfxq0YKUsh96/tP84CZO4phvoR+0y9wtm9Dh5w==&checkout_client=secure";
            string[] info = new string[2];

             info = fasyapi.get(url,tk,ct);
            string parasparId = info[1];
            string amount = info[0].Replace(".", "");

           string countryurl = "https://r9udv3ar7g.execute-api.eu-west-2.amazonaws.com/prod/basket?checkout_client=secure";
            string countryinfo = "{\"fascia_id\":1,\"channel_id\":2,\"currency_code\":\"USD\",\"customer\":{\"customer_id\":\""+ coucustomer_id + "\",\"sessionID\":null,\"hash\":\"xcx\"},\"basket_id\":"+ parasparId + ",\"shipping_country\":\"US\"}";
            fasyapi.Post(countryurl,tk,ct,countryinfo);
            string emailinfo = "";
            try
            {
                 emailinfo = "{\"fascia_id\":1,\"channel_id\":2,\"currency_code\":\"USD\",\"customer\":{\"email\":\"" + jo["EmailAddress"].ToString() + "\"}}";
            }
            catch
            {
                tk.Status = "Emailerror";
                Thread.Sleep(360000000);
            }
            string emailurl = "https://r9udv3ar7g.execute-api.eu-west-2.amazonaws.com/prod/customer/check?checkout_client=secure";
            fasyapi.Post(emailurl,tk,ct, emailinfo);

            string shippingurl = "https://fa93z8zyn6.execute-api.eu-west-2.amazonaws.com/prod/checkout/shipping?checkout_client=secure";
            string shippinginfo = "{\"postcode\":\""+jo["Zipcode"].ToString() +"\",\"country\":\""+jo["Country"].ToString()+"\",\"basket\":{\"id\":"+ parasparId + ",\"basketItems\":[{\"id\":\""+ skuid + "\",\"qty\":1}]}}";
            fasyapi.Post(shippingurl,tk,ct,shippinginfo);

            string checkouturl = "https://paymentgateway.checkout.footasylum.net/basket/shipping?medium=web&apiKey=lGJjE+ccd0SiBdu3I6yByRp3/yY8uVIRFa9afLx+2YSrSwkWDfxq0YKUsh96/tP84CZO4phvoR+0y9wtm9Dh5w==&checkout_client=secure";
            string checkoutinfo = "{\"basketId\":"+parasparId+",\"subs\":[],\"type\":\"subscription\"}";
            fasyapi.Putcheckot(checkouturl,tk,ct,checkoutinfo);

            string checkoutinfo2 = "{\"basketId\":"+parasparId+",\"type\":\"shipping\",\"shippingTotal\":0,\"shippingCode\":\"MP_USASD\",\"shippingCarrier\":\"Standard Delivery\"}";
            fasyapi.Putcheckot(checkouturl, tk, ct, checkoutinfo2);

            string checkouturl3 = "https://paymentgateway.checkout.footasylum.net/customer?medium=web&apiKey=lGJjE+ccd0SiBdu3I6yByRp3/yY8uVIRFa9afLx+2YSrSwkWDfxq0YKUsh96/tP84CZO4phvoR+0y9wtm9Dh5w==&checkout_client=secure";
            string shippingaddress = "{\"cartId\":"+parasparId+",\"customer\":{\"firstname\":\""+jo["FirstName"].ToString()+ "\",\"lastname\":\"" + jo["LastName"].ToString() + "\",\"email\":\"" + jo["EmailAddress"].ToString() + "\",\"mobile\":\"" + jo["Tel"].ToString() + "\",\"title\":\"Mr\",\"newsletter\":1,\"sessionId\":null,\"parasparId\":\""+coucustomer_id+"\"}," +
                "\"shippingAddress\":{\"company\":\"\",\"address1\":\""+ jo["Address1"].ToString() + "\",\"address2\":\""+ jo["Address2"].ToString() + "\",\"city\":\""+ jo["City"].ToString() + "\",\"country\":\""+jo["Country"].ToString()+"\",\"postcode\":\""+jo["Zipcode"].ToString()+ "\",\"shortCountry\":\"" + jo["Country"].ToString() + "\",\"delivery_instructions\":\"\"},\"billingAddress\":{\"company\":\"\",\"address1\":\"" + jo["Address1"].ToString() + "\",\"address2\":\"" + jo["Address2"].ToString() + "\"," +
                "\"city\":\"" + jo["City"].ToString() + "\",\"country\":\"" + jo["Country"].ToString() + "\",\"postcode\":\"" + jo["Zipcode"].ToString() + "\",\"shortCountry\":\"" + jo["Country"].ToString() + "\",\"delivery_instructions\":\"\"},\"authState\":\"CUSTOMER_INVALID\"}";
            fasyapi.Putcheckot(checkouturl3,tk,ct,shippingaddress);

            string stripeurl = "https://api.stripe.com/v1/sources";
            var expire=jo["MMYY"].ToString().Split("/");
            string guid = Guid.NewGuid().ToString();
            string muid = Guid.NewGuid().ToString();
            string sid = Guid.NewGuid().ToString();
            string stripeinfo = "type=card&currency=USD&amount="+ amount + "&owner[name]="+ jo["FirstName"].ToString() + " "+ jo["LastName"].ToString() + "&owner[email]="+ UrlEncode(jo["EmailAddress"].ToString())  + "&owner[address][line1]="+ jo["Address1"].ToString() + "&owner[address][city]="+ jo["City"].ToString() + "&owner[address][postal_code]="+ jo["Zipcode"].ToString() + "&owner[address][country]="+ jo["Country"].ToString()+ "&metadata[description]=New Checkout payment for FA products&redirect[return_url]=https%3A%2F%2Fsecure.footasylum.com%2Fredirect-result%3FcheckoutSessionId%3D" + checkoutsession+"%26disable_root_load%3Dtrue&card[number]="+jo["Cardnum"].ToString()+"&card[cvc]="+ jo["Cvv"].ToString() + "&card[exp_month]="+ expire[0]+ "&card[exp_year]="+ expire[1]+ "&guid="+ guid + "&muid="+muid+"&sid="+sid+ "&payment_user_agent=stripe.js%2Fe2b3eff8%3B+stripe-js-v3%2Fe2b3eff8&time_on_page=738208&referrer=https%3A%2F%2Fsecure.footasylum.com%2F%3FcheckoutSessionId%3D" + checkoutsession+"&key=pk_live_y7GywYfDSuh3fr8oraR8g66U";
            fasyapi.Post(stripeurl,tk,ct, stripeinfo.Replace(" ","+"));
            string stripeinfo2 = "type=three_d_secure&amount="+ amount + "&currency=USD&metadata[cart_id]=" + parasparId+"&metadata[customer_id]="+coucustomer_id+"&metadata[description]=New+Checkout+payment+for+FA+products&three_d_secure[card]="+stripetoken.striptoken+"&redirect[return_url]=https%3A%2F%2Fsecure.footasylum.com%2Fredirect-result%3FcheckoutSessionId%3D" + checkoutsession+"%26disable_root_load%3Dtrue&guid="+guid+"&muid="+muid+"&sid="+sid+ "&payment_user_agent=stripe.js%2Fe2b3eff8%3B+stripe-js-v3%2Fe2b3eff8&time_on_page=738208&referrer=https%3A%2F%2Fsecure.footasylum.com%2F%3FcheckoutSessionId%3D"+checkoutsession+"&key=pk_live_y7GywYfDSuh3fr8oraR8g66U";
            fasyapi.Post(stripeurl,tk,ct, stripeinfo2.Replace(" ","+"));

            string finalurl = "https://paymentgateway.checkout.footasylum.net/basket/trans-code?medium=web&apiKey=lGJjE+ccd0SiBdu3I6yByRp3/yY8uVIRFa9afLx+2YSrSwkWDfxq0YKUsh96/tP84CZO4phvoR+0y9wtm9Dh5w==&checkout_client=secure";
            string finalinfo = "{\"basketId\":"+parasparId+",\"stripeToken\":\""+stripetoken.striptoken+"\"}";
            fasyapi.Putfinal(finalurl,tk,ct,finalinfo);

            string processurl = "https://paymentgateway.checkout.footasylum.net/basket/trans-code?code=5557620&medium=web&apiKey=lGJjE+ccd0SiBdu3I6yByRp3/yY8uVIRFa9afLx+2YSrSwkWDfxq0YKUsh96/tP84CZO4phvoR+0y9wtm9Dh5w==&checkout_client=secure";
            var status = "";
            for (int i = 0; i < 10; i++)
            {
                 status = fasyapi.Getstatus(processurl, tk, ct);
                if (status.Contains("pending"))
                {

                }
                else
                {
                    tk.Status = status;
                    break;
                }    
            }
            tk.Status = status;
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
        public string UrlEncode(string str)
        {
            StringBuilder builder = new StringBuilder();
            foreach (char c in str)
            {
                if (HttpUtility.UrlEncode(c.ToString()).Length > 1)
                {
                    builder.Append(HttpUtility.UrlEncode(c.ToString()).ToUpper());
                }
                else
                {
                    builder.Append(c);
                }
            }
            return builder.ToString();
        }
    }
}

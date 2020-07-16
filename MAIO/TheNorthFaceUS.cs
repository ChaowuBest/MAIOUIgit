using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace MAIO
{
    class TheNorthFaceUS
    {
        public bool randomsize = false;
        public string link = "";
        public string profile = "";
        public string size = "";
        public string sizeid = "";
        public string categoryId = "";
        public string productid = "";
        public string orderid = "";
        public string addressid = "";
        public Main.taskset tk = null;
        TheNorthFaceAPI tnfAPI = new TheNorthFaceAPI();
        public void StartTask(CancellationToken ct, CancellationTokenSource cts)
        {
        A: JObject joprofile = JObject.Parse(profile);
            try
            {
                GetSkuid(ct);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (NullReferenceException)
            {
                goto A;
            }
            try
            {
                ATC(ct);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            try
            {
                SubmitShipping(ct, joprofile);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            try
            {
                payment(ct);
            }
            catch (OperationCanceledException)
            {
                return;
            }

        }
        public void GetSkuid(CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            string url = tk.Sku;
            bool isstocknull = true;
            JArray ja = (JArray)tnfAPI.GetHtmlsource(url, tk, ct);
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            categoryId = ja[0]["PDPProduct"]["categoryId"].ToString();
            productid = ja[0]["PDPProduct"]["parentId"].ToString();
            var sizegroup = ja[0]["PDPProduct"]["eids"].ToString();
            string eid = "";
            foreach (var i in sizegroup)
            {
                if (i.ToString().Contains(size))
                {
                    eid = i.ToString();
                    break;
                }
            }
        B: string monitorurl = "https://www.thenorthface.com/webapp/wcs/stores/servlet/VFAjaxProductAvailabilityView?langId=-1&storeId=7001&productId=" + productid + "&requestype=ajax&requesttype=ajax";
            JObject jo = (JObject)tnfAPI.GetHtmlsource(monitorurl, tk, ct);
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            foreach (var i in jo["partNumbers"].ToArray())
            {
                if (i.ToString().Contains(eid))
                {
                    Regex rex = new Regex(@"\d{6}");
                    sizeid = rex.Match(i.ToString()).ToString();
                    break;
                }
            }
            foreach (var i in jo["stock"].ToArray())
            {
                if (i.ToString().Contains(sizeid))
                {
                    var st = i.ToString().Replace(sizeid, "").Replace(",", "").Replace(" ", "").Replace("\"", "").Replace(":", "");
                    int stock = int.Parse(st);
                    if (stock > 0)
                    {
                        isstocknull = false;
                    }
                    break;
                }
            }
            if (isstocknull)
            {
                tk.Status = "Monitoring";
                goto B;
            }
        }
        public void ATC(CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            string url = "https://www.thenorthface.com/webapp/wcs/stores/servlet/VFAjaxOrderItemAdd";
            string info = "categoryId=" + categoryId + "&storeId=7001&langId=-1&catalogId=20001&catEntryId=" + sizeid + "&quantity=1&orderId=.&URL=%2F&requesttype=ajax";
            orderid = tnfAPI.ATC(url, tk, ct, info);
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
        }
        public void SubmitShipping(CancellationToken ct,JObject jo)
        {
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            string url = "https://www.thenorthface.com/shop/VFAjaxAVSAddressAdd";
            long time = (long)(DateTime.Now.ToUniversalTime() - timeStampStartTime).TotalMilliseconds;
            string info = "storeId=7001&country="+ jo["Country"].ToString() + "&country="+ jo["Country"].ToString() + "&toOrderId="+orderid+"&addressType=S&outAddressName=addressId&isVerificationRequired=yes&URL=%2F&qasVerificationPage=true&formatPhoneWhileTyping=true&countryCode=US&nickName="+time+"&personTitle=Mr.&firstName="+ jo["FirstName"].ToString() + "&lastName="+ jo["LastName"].ToString() + "&address1="+ jo["Address1"].ToString() + "&address2="+ jo["Address2"].ToString() + "&zipCode="+ jo["Zipcode"].ToString() + "&city="+ jo["City"].ToString() + "&state="+ jo["State"].ToString() + "&phone1="+jo["Tel"].ToString()+"&email1="+ System.Web.HttpUtility.UrlEncode(jo["EmailAddress"].ToString()) + "&qasOperation=verificationEngine&requesttype=ajax";
            tnfAPI.shipping(url,tk,ct,info.Replace(" ","+"));
            #region
            /* JObject jo2 = JObject.Parse(sourcecod);
             addressid=jo2["addressId"].ToString();
             if (ct.IsCancellationRequested)
             {
                 tk.Status = "IDLE";
                 ct.ThrowIfCancellationRequested();
             }

               string urlcheck1 = "https://www.thenorthface.com/shop/VFAjaxOrderItemUpdate";
               string checkaddress1 = "storeId=7001&orderId="+orderid+"&calculateOrder=1&calculationUsage=-1%2C-2%2C-5%2C-7&keepAutoAddedItems=true&showOrder=1&skipDOMInventoryCheck=Y&addressId="+addressid+"&requesttype=ajax";       
               string source=tnfAPI.shipping(urlcheck1,tk,ct,checkaddress1);
               JObject jo3 = JObject.Parse(source);
               string orderitemid= jo3["items"][0]["id"].ToString();
               string shipid = jo3["items"][0]["shipModeId"].ToString();
               string checkaddress2 = "storeId=7001&langId=-1&orderId=" + orderid + "&calculateOrder=1&calculationUsage=-1%2C-2%2C-5%2C-7&showOrder=0&keepAutoAddedItems=true&skipDOMInventoryCheck=Y&orderItemId_1="+ orderitemid + "&shipModeId="+ shipid + "&requesttype=ajax";
               tnfAPI.shipping(urlcheck1,tk,ct,checkaddress2);*/
            #endregion
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            string info2 = "storeId=7001&orderId="+orderid+"&URL=VFBillingAndPaymentView&errorURL=VFShippingAddressView%3ForderId%"+orderid+"&containsPayPal=&isSavedAddress=true&shipToStore=&removeGiftServices=";          
            string getpaymengurl = "https://www.thenorthface.com/shop/OrderPrepare";
            tnfAPI.preorder(getpaymengurl,tk,ct,info2);
        }
        public void payment(CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            string getorderviewurl = "https://www.thenorthface.com/shop/VFAjaxGetOrderView";
            string orderinfo = "storeId=7001&orderId="+orderid+"&requesttype=ajax";
            tnfAPI.orderdetail(getorderviewurl, tk, ct, orderinfo);

            string AjaxOrderCalculate = "orderId="+orderid+"&storeId=7001&langId=-1&updatePrices=1&calculationUsageId=-1&calculationUsageId=-2&calculationUsageId=-3&calculationUsageId=-4&calculationUsageId=-5&calculationUsageId=-6&calculationUsageId=-7&showOrder=1&promoCode=&altAction=OrderCalculate&requesttype=ajax";
            string ajaxorderurl = "https://www.thenorthface.com/webapp/wcs/stores/servlet/AjaxOrderCalculate";
            tnfAPI.orderdetail(ajaxorderurl,tk,ct,AjaxOrderCalculate);

            string url = "https://www.thenorthface.com/shop/SendToPaypal?storeId=7001&orderId="+orderid+"&callSource=Payment&useraction=commit&cancelURL=https%3a%2f%2fwww.thenorthface.com%2fshop%2fVFBillingAndPaymentView%3forderId%3d"+orderid+"%26storeId%3d7001&returnURL=https%3a%2f%2fwww.thenorthface.com%2fshop%2fZCReturnFromPaypal%3fcallSource%3dPayment%26orderId%3d"+orderid+"%26shippingAddressName%3dPayPal%2bShipping%26billingAddressName%3dPayPal%2bBilling%26storeId%3d7001%26URL%3dOrderOKView ";
            string paypallink=tnfAPI.Checkout2(url,tk,ct);
            string pd2 = "{\"username\":\"MAIO\",\"avatar_url\":\"https://i.loli.net/2020/05/24/VfWKsEywcXZou1T.jpg\",\"embeds\":[{\"title\":\"You Just Chekcout!\",\"color\":3329330,\"footer\":{\"text\":\"" + "MAIO" + DateTime.Now.ToLocalTime().ToString() + "\",\"icon_url\":\"https://i.loli.net/2020/05/24/VfWKsEywcXZou1T.jpg\"},\"fields\":[{\"name\":\"Checkout out!!!\",\"value\":\"" + paypallink + "\\t\\t\\t\\tSize:" + tk.Size + "\\t\\t\\t\\t\",\"inline\":false}]}]}";         
            Http("https://discordapp.com/api/webhooks/517871792677847050/qry12HP2IqJQb2sAfSNBmpUmFPOdPsVXUYY2_yhDgckgznpeVtRpNbwvO1Oma6nMGeK9", pd2);
            Http(Config.webhook, pd2);
            tk.Status = "Success";

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
            catch (WebException)
            {
                Thread.Sleep(500);
                goto Retry;
            }

        }
        private static DateTime timeStampStartTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    }
}

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MAIO.JDUS
{
    class JDUS
    {
        public bool randomsize = false;
        public string link = "";
        public string profile = "";
        public string size = "";
        public string sizeid = "";
        public Main.taskset tk = null;
        string _dynSessConf = null;
        public JDUSAPI jdusapi = new JDUSAPI();
        Dictionary<string, string> _diData = new Dictionary<string, string>();
        public void StartTask(CancellationToken ct, CancellationTokenSource cts)
        {
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            string sku = null;
            JObject joprofile = JObject.Parse(profile);
            try
            {
                Regex rexprod = new Regex(@"(prod)\d{5,}[^?]");
                Regex rexstyle = new Regex(@"(?<=styleId=)\w{2,}[^&]");
                Regex rexcolor = new Regex(@"(?<=colorId=)\w{2,}[^&]");
                string style = rexstyle.Match(tk.Sku).ToString();
                string prod = rexprod.Match(tk.Sku).ToString();
                string color = rexcolor.Match(tk.Sku).ToString();
                Task getsession = new Task(() => GetJession(ct));
            A: if (style != "" && prod != "" && color != "")
                {
                    try
                    {
                       getsession.Start();
                        sku = Getskuid(prod, style, color, ct);
                    }
                    catch (NullReferenceException)
                    {
                        goto A;
                    }
                B:
                    try
                    {
                      ATC(ct,prod,sku);
                    }
                    catch (NullReferenceException)
                    {
                        goto B;
                    }
                    C:
                    try
                    {
                        SubmittingShippig(ct, joprofile);
                    }
                    catch(NullReferenceException)
                    {
                        goto C;
                    }
                    D:
                    try
                    {
                        SubmitBilling(ct,joprofile);
                    }
                    catch(NullReferenceException)
                    {
                        goto D;
                    }
                    E:
                    try
                    {
                        submitorder(ct, joprofile);
                    }
                    catch (NullReferenceException)
                    {
                        goto E;
                    }
                }
                else
                {
                    tk.Status = "Error SKU";
                }
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }
        private string Getskuid(string prod, string style, string color, CancellationToken ct)
        {
        A: string url = "https://www.jdsports.com/store/browse/json/productSizesJson.jsp?productId=" + prod + "&styleId=" + style + "&colorId=" + color + "";
            string sourcecode = jdusapi.GetHtmlsource(url, tk, ct);
            JObject jo = JObject.Parse(sourcecode);
            JArray ja = JArray.Parse(jo["productStoreSizes"].ToString());
            string base64sku = "";
            var chao=ja.ToString();
            for (int i=0;i<ja.Count;i++)
            {
                if (ja[i]["sizeValue"].ToString().Replace(".0", "") == tk.Size)
                {
                    byte[] bytes = Convert.FromBase64String(ja[i]["stockLevel"].ToString());
                    string stock = Encoding.Default.GetString(bytes);
                    if (int.Parse(stock) == 0)
                    {
                        goto A;
                    }
                    else
                    {
                        base64sku = ja[i]["sku"].ToString();
                        break;
                    }
                }
            }
            byte[] byte64 = Convert.FromBase64String(base64sku);
            base64sku = Encoding.Default.GetString(byte64);
            return base64sku;
        }
        private void GetJession(CancellationToken ct)
        {
            string url = "https://www.jdsports.com/store/cart/gadgets/miniCart.jsp";
            jdusapi.GetHtmlsource(url,tk,ct);
        }
        private void ATC(CancellationToken ct,string prod,string skuid)
        {
            string url = "https://www.jdsports.com/store/browse/productDetail.jsp?productId="+ prod + "&_DARGS=/store/browse/productDetailDisplay.jsp.flAddToCart";
            string info = "_dyncharset=UTF-8&requiresSessionConfirmation=false&/atg/commerce/order/purchase/CartModifierFormHandler.addItemToOrderSuccessURL=/store/browse/productDetail.jsp?productId="+prod+"&_D:/atg/commerce/order/purchase/CartModifierFormHandler.addItemToOrderSuccessURL= &/atg/commerce/order/purchase/CartModifierFormHandler.addItemToOrderErrorURL=/store/browse/productDetail.jsp?productId="+prod+"&dontCachePDP=true&_D:/atg/commerce/order/purchase/CartModifierFormHandler.addItemToOrderErrorURL= &catalogRefIds="+skuid+"&_D:catalogRefIds= &productId="+prod+"&_D:productId= &items=&_D:items= &quantity=1&_D:quantity= &/atg/commerce/order/purchase/CartModifierFormHandler.dimensionId=&_D:/atg/commerce/order/purchase/CartModifierFormHandler.dimensionId= &Add To Cart=Add To Cart&_D:Add To Cart= &_DARGS=/store/browse/productDetailDisplay.jsp.flAddToCart";
            _dynSessConf = jdusapi.ATC(url,tk,ct,info);
        }
        private void SubmittingShippig(CancellationToken ct,JObject jo)
        {
            string url = "https://www.jdsports.com/store/checkout/shipping.jsp?_DARGS=/store/checkout/shipping.jsp";
            // string info = "_dyncharset=UTF-8&_dynSessConf="+_dynSessConf+"&firstName=wu&_D%3AfirstName=+&lastName=chao&_D%3AlastName=+&address1=225+Raritan+Ave&_D%3Aaddress1=+&address2=&_D%3Aaddress2=+&city=Highland+Park&_D%3Acity=+&_D%3Astate=+&state=NJ&zip=08904&_D%3Azip=+&phone=443-986-9527&_D%3Aphone=+&email=wu1553992118%40gmail.com&_D%3Aemail=+&%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.validateBadChar=true&_D%3A%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.validateBadChar=+&country=US&_D%3Acountry=+&shippingMethod=Economy&_D%3AshippingMethod=+&_D%3AshippingMethod=+&_D%3AshippingMethod=+&_D%3AshippingMethod=+&receiveEmail=false&_D%3AreceiveEmail=+&shipToAddressName=&_D%3AshipToAddressName=+&copyNewAddrToBilling=false&_D%3AcopyNewAddrToBilling=+&upsValidation=true&_D%3AupsValidation=+&fromContinueBillingButton=true&_D%3AfromContinueBillingButton=+&%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.sessionExpirationURL=%2Fstore%2Fcart%2Fcart.jsp%3Ftimeouterror%3Dtrue&_D%3A%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.sessionExpirationURL=+&%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.applyShippingGroupsSuccessURL=%2Fstore%2Fcheckout%2Fbilling.jsp&_D%3A%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.applyShippingGroupsSuccessURL=+&%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.applyShippingGroupsErrorURL=%2Fstore%2Fcheckout%2Fshipping.jsp&_D%3A%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.applyShippingGroupsErrorURL=+&%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.applyShippingGroups=moveToBilling&_D%3A%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.applyShippingGroups=+&_DARGS=%2Fstore%2Fcheckout%2Fshipping.jsp";
            string info = "_dyncharset=UTF-8&_dynSessConf="+ _dynSessConf + "&firstName="+jo["FirstName"].ToString() +"&_D%3AfirstName=+&lastName="+ jo["LastName"].ToString() + "&_D%3AlastName=+&address1="+ jo["Address1"].ToString() + "&_D%3Aaddress1=+&address2=&_D%3Aaddress2=+&city="+ jo["City"].ToString() + "&_D%3Acity=+&_D%3Astate=+&state="+ jo["State"].ToString() + "&zip="+ jo["Zipcode"].ToString() + "&_D%3Azip=+&phone="+ jo["Tel"].ToString() + "&_D%3Aphone=+&email=" + System.Web.HttpUtility.UrlEncode(jo["EmailAddress"].ToString())+ "&_D%3Aemail=+&%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.validateBadChar=true&_D%3A%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.validateBadChar=+&country=US&_D%3Acountry=+&shippingMethod=Economy&_D%3AshippingMethod=+&_D%3AshippingMethod=+&_D%3AshippingMethod=+&_D%3AshippingMethod=+&receiveEmail=false&_D%3AreceiveEmail=+&shipToAddressName=&_D%3AshipToAddressName=+&copyNewAddrToBilling=false&_D%3AcopyNewAddrToBilling=+&upsValidation=true&_D%3AupsValidation=+&fromContinueBillingButton=true&_D%3AfromContinueBillingButton=+&%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.sessionExpirationURL=%2Fstore%2Fcart%2Fcart.jsp%3Ftimeouterror%3Dtrue&_D%3A%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.sessionExpirationURL=+&%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.applyShippingGroupsSuccessURL=%2Fstore%2Fcheckout%2Fbilling.jsp&_D%3A%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.applyShippingGroupsSuccessURL=+&%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.applyShippingGroupsErrorURL=%2Fstore%2Fcheckout%2Fshipping.jsp&_D%3A%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.applyShippingGroupsErrorURL=+&%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.applyShippingGroups=moveToBilling&_D%3A%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.applyShippingGroups=+&_DARGS=%2Fstore%2Fcheckout%2Fshipping.jsp";
            string url2 = "https://www.jdsports.com/store/checkout/shipping.jsp?_DARGS=/store/checkout/modals/upsAddressValidation.jsp.overrideShippingForm";
            string info2 = "_dyncharset=UTF-8&_dynSessConf="+_dynSessConf+"&%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.address.firstName=wu&_D%3A%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.address.firstName=+&%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.address.lastName=chao&_D%3A%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.address.lastName=+&%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.usedUpsSuggestion=false&_D%3A%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.usedUpsSuggestion=+&%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.address.address1=225+Raritan+Ave&_D%3A%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.address.address1=+&%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.address.address2=&_D%3A%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.address.address2=+&%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.address.city=Highlandpark&_D%3A%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.address.city=+&%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.address.state=NJ&_D%3A%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.address.state=+&%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.address.postalCode=08904&_D%3A%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.address.postalCode=+&%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.address.phoneNumber=443-986-9527&_D%3A%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.address.phoneNumber=+&%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.address.email=wu1553992118%40gmail.com&_D%3A%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.address.email=+&%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.address.country=US&_D%3A%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.address.country=+&%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.address.email=wu1553992118%40gmail.com&_D%3A%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.address.email=+&%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.copyNewAddrToBilling=false&_D%3A%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.copyNewAddrToBilling=+&%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.receiveEmail=false&_D%3A%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.receiveEmail=+&%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.shippingMethod=Economy&_D%3A%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.shippingMethod=+&%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.overrideShippingAddress=true&_D%3A%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.overrideShippingAddress=+&%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.applyShippingGroupsSuccessURL=%2Fstore%2Fcheckout%2Fbilling.jsp&_D%3A%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.applyShippingGroupsSuccessURL=+&%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.applyShippingGroupsErrorURL=%2Fstore%2Fcheckout%2Fshipping.jsp&_D%3A%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.applyShippingGroupsErrorURL=+&%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.applyShippingGroups=Continue+with+Original&_D%3A%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FShippingGroupFormHandler.applyShippingGroups=+&_DARGS=%2Fstore%2Fcheckout%2Fmodals%2FupsAddressValidation.jsp.overrideShippingForm";
            jdusapi.SubmittingShipping(url, tk, ct, info.Replace(" ","+"));
            jdusapi.SubmittingShipping2(url2, tk, ct, info2.Replace(" ", "+"));
        }
        private void SubmitBilling(CancellationToken ct,JObject jo)
        {
            string url = "https://www.jdsports.com/store/checkout/billing.jsp?_DARGS=/store/checkout/billing.jsp";
           string info = "_dyncharset=UTF-8&creditCardNumberTemp=5250+3410+1025+4252&_D%3AcreditCardNumberTemp=+&_D%3AexpirationMonthTemp=+&expirationMonthTemp=01&_D%3AexpirationYearTemp=+&expirationYearTemp=2024&securityCodeTemp=052&_D%3AsecurityCodeTemp=+&copyAddressFromShipping=on&firstName="+jo["FirstName"].ToString() +"&_D%3AfirstName=+&lastName="+ jo["LastName"].ToString() + "&_D%3AlastName=+&address1="+ jo["Address1"].ToString() + "&_D%3Aaddress1=+&address2=&_D%3Aaddress2=+&city="+ jo["City"].ToString() + "&_D%3Acity=+&_D%3Astate=+&state="+ jo["State"].ToString() + "&zip="+ jo["Zipcode"].ToString() + "&_D%3Azip=+&phone="+ jo["Tel"].ToString() + "&_D%3Aphone=+&email="+ System.Web.HttpUtility.UrlEncode(jo["EmailAddress"].ToString()) + "&_D%3Aemail=+&country=US&_D%3Acountry=+&creditCardTypeTemp=MASTERCARD&_D%3AcreditCardTypeTemp=+&transactionType=CC&%2Fatg%2Fstore%2Forder%2Fpurchase%2FBillingFormHandler.creditCardNickname=fline&_D%3A%2Fatg%2Fstore%2Forder%2Fpurchase%2FBillingFormHandler.creditCardNickname=+&giftCardNumber=&giftCardPin=&wcCustomerId=&_D%3AcheckoutWithPayPal=+&%2Fatg%2Fstore%2Forder%2Fpurchase%2FBillingFormHandler.formGCStatishFied=false&_D%3A%2Fatg%2Fstore%2Forder%2Fpurchase%2FBillingFormHandler.formGCStatishFied=+&%2Fatg%2Fstore%2Forder%2Fpurchase%2FBillingFormHandler.overrideBillingAddress=true&_D%3A%2Fatg%2Fstore%2Forder%2Fpurchase%2FBillingFormHandler.overrideBillingAddress=+&%2Fatg%2Fstore%2Forder%2Fpurchase%2FBillingFormHandler.sessionExpirationURL=%2Fstore%2Fcart%2Fcart.jsp%3Ftimeouterror%3Dtrue&_D%3A%2Fatg%2Fstore%2Forder%2Fpurchase%2FBillingFormHandler.sessionExpirationURL=+&moveToConfirmSuccessURL=%2Fstore%2Fcheckout%2Freview.jsp&_D%3AmoveToConfirmSuccessURL=+&moveToConfirmErrorURL=%2Fstore%2Fcheckout%2Fbilling.jsp&_D%3AmoveToConfirmErrorURL=+&%2Fatg%2Fstore%2Forder%2Fpurchase%2FBillingFormHandler.mobileLogin=&_D%3A%2Fatg%2Fstore%2Forder%2Fpurchase%2FBillingFormHandler.mobileLogin=+&%2Fatg%2Fstore%2Forder%2Fpurchase%2FBillingFormHandler.billingWithNewAddressAndNewCard=submit&_D%3A%2Fatg%2Fstore%2Forder%2Fpurchase%2FBillingFormHandler.billingWithNewAddressAndNewCard=+&_DARGS=%2Fstore%2Fcheckout%2Fbilling.jsp";          
            #region
          /*  this._diData.Add("_dyncharset", "UTF-8");
            this._diData.Add("_dynSessConf", this._dynSessConf);
            this._diData.Add("creditCardNumberTemp", jo["Cardnum"].ToString());
            this._diData.Add("_D:creditCardNumberTemp", " ");
            this._diData.Add("_D:expirationMonthTemp", " ");
            this._diData.Add("_D:expirationYearTemp", " ");
            this._diData.Add("securityCodeTemp", jo["Cvv"].ToString());
            this._diData.Add("_D:securityCodeTemp", " ");
            this._diData.Add("firstName", jo["FirstName"].ToString());
            this._diData.Add("_D:firstName", " ");
            this._diData.Add("lastName", jo["LastName"].ToString());
            this._diData.Add("_D:lastName", " ");
            this._diData.Add("address1", jo["Address1"].ToString());
            this._diData.Add("_D:address1", " ");
            this._diData.Add("address2", jo["Address2"].ToString());
            this._diData.Add("_D:address2", " ");
            this._diData.Add("city", jo["City"].ToString());
            this._diData.Add("_D:city", " ");
            this._diData.Add("_D:state", " ");
            this._diData.Add("zip", jo["Zipcode"].ToString());
            this._diData.Add("_D:zip", " ");
            this._diData.Add("phone", jo["Tel"].ToString());
            this._diData.Add("_D:phone", " ");
            this._diData.Add("email", jo["EmailAddress"].ToString());
            this._diData.Add("_D:email", " ");
            this._diData.Add("country", "US");
            this._diData.Add("_D:country", " ");
            this._diData.Add("creditCardTypeTemp", "MASTERCARD");
            this._diData.Add("_D:creditCardTypeTemp", " ");
            this._diData.Add("transactionType", "CC");
            this._diData.Add("/atg/store/order/purchase/BillingFormHandler.creditCardNickname", "fline");
            this._diData.Add("_D:/atg/store/order/purchase/BillingFormHandler.creditCardNickname", " ");
            this._diData.Add("_D:checkoutWithPayPal", " ");
            this._diData.Add("wcCustomerId", "");
            this._diData.Add("/atg/store/order/purchase/BillingFormHandler.formGCStatishFied", "false");
            this._diData.Add("_D:/atg/store/order/purchase/BillingFormHandler.formGCStatishFied", " ");
            this._diData.Add("/atg/store/order/purchase/BillingFormHandler.overrideBillingAddress", "true");
            this._diData.Add("_D:/atg/store/order/purchase/BillingFormHandler.overrideBillingAddress", " ");
            this._diData.Add("/atg/store/order/purchase/BillingFormHandler.sessionExpirationURL", "/store/cart/cart.jsp?timeouterror=true");
            this._diData.Add("_D:/atg/store/order/purchase/BillingFormHandler.sessionExpirationURL", " ");
            this._diData.Add("moveToConfirmSuccessURL", "/store/checkout/review.jsp");
            this._diData.Add("_D:moveToConfirmSuccessURL", " ");
            this._diData.Add("moveToConfirmErrorURL", "/store/checkout/billing.jsp");
            this._diData.Add("_D:moveToConfirmErrorURL", " ");
            this._diData.Add("/atg/store/order/purchase/BillingFormHandler.billingWithNewAddressAndNewCard", "submit");
            this._diData.Add("_D:/atg/store/order/purchase/BillingFormHandler.billingWithNewAddressAndNewCard", " ");
            this._diData.Add("_DARGS", "/store/checkout/billing.jsp");
            this._diData.Add("expirationMonthTemp", "2026");
            this._diData.Add("expirationYearTemp", "06");
            this._diData.Add("state", jo["State"].ToString());
            this._diData.Add("creditCardType", "MASTERCARD");*/
            #endregion
            jdusapi.SubmittingBilling(url,tk,ct, info);
        }
        private void submitorder(CancellationToken ct, JObject jo)
        {
            string url = "https://www.jdsports.com/store/checkout/review.jsp?_DARGS=/store/checkout/review.jsp";
            string info = "_dyncharset=UTF-8&_dynSessConf="+_dynSessConf+"&%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FCommitOrderFormHandler.mobileOrder=false&_D%3A%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FCommitOrderFormHandler.mobileOrder=+&bb_field=&_D%3Abb_field=+&%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FCommitOrderFormHandler.sessionExpirationURL=%2Fstore%2Fcart%2Fcart.jsp%3Ftimeouterror%3Dtrue&_D%3A%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FCommitOrderFormHandler.sessionExpirationURL=+&%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FCommitOrderFormHandler.commitOrderSuccessURL=%2Fstore%2Fcheckout%2Fconfirm.jsp&_D%3A%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FCommitOrderFormHandler.commitOrderSuccessURL=+&%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FCommitOrderFormHandler.commitOrderErrorURL=%2Fstore%2Fcheckout%2Fbilling.jsp&_D%3A%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FCommitOrderFormHandler.commitOrderErrorURL=+&maxOrderDecline=maxOrderDecline&_D%3AmaxOrderDecline=+&%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FCommitOrderFormHandler.commitOrder=Process+Order&_D%3A%2Fatg%2Fcommerce%2Forder%2Fpurchase%2FCommitOrderFormHandler.commitOrder=+&_DARGS=%2Fstore%2Fcheckout%2Freview.jsp";
            jdusapi.SubmittingOrder(url,tk,ct,info);
        }
    }
}

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAIO
{
    static class Nike_Requestpaylod
    {
        public static string previewpaylod = null;
        public static string previewpayinfo = null;
        public static string priviewpayment = null;
        static  Nike_Requestpaylod()
        {
            JObject payLoad = new JObject(
new JProperty("request",
new JObject(
new JProperty("country", ""),
new JProperty("currency", ""),
new JProperty("email", ""),
new JProperty("locale", ""),
new JProperty("channel", "NIKECOM"),
//   new JProperty("clientInfo",
//  new JObject(new JProperty("deviceId", deviceid))),
new JProperty("promotionCodes",
new JArray("code")),
new JProperty("items",
new JArray(
  new JObject(
  new JProperty("id", Guid.NewGuid().ToString()),
  new JProperty("skuId", ""),
  new JProperty("shippingMethod", ""),
  new JProperty("quantity", ""),
   new JProperty("fulfillmentDetails",
     new JObject(
     new JProperty("type", "SHIP"),
     new JProperty("getBy",
     new JObject(
     new JProperty("maxDate",
     new JObject(
     new JProperty("dateTime", ""),
     new JProperty("timezone", "America/Indiana/Indianapolis"),
     new JProperty("precision", "DAY"))
         ))),
     new JProperty("location",
     new JObject(
     new JProperty("type", "address/shipping"),
     new JProperty("postalAddress",
     new JObject(
     new JProperty("country", ""),
     new JProperty("address1", ""),
     new JProperty("address2", ""),
     new JProperty("postalCode", ""),
     new JProperty("city", ""),
     new JProperty("state", ""))))
     ))),
  new JProperty("recipient",
     new JObject(
     new JProperty("firstName", ""),
     new JProperty("lastName", ""))),
  new JProperty("contactInfo",
     new JObject(
     new JProperty("phoneNumber", ""),
     new JProperty("email", ""))),
  new JProperty("shippingAddress",
     new JObject(
     new JProperty("address1", ""),
     new JProperty("address2", ""),
     new JProperty("city", ""),
     new JProperty("postalCode", ""),
     new JProperty("state", ""),
      new JProperty("preferred", "true"),
     new JProperty("country", ""))))
  )))));

          JObject  payinfo = new JObject(
new JProperty("checkoutId", ""),
new JProperty("total", ""),
new JProperty("currency", ""),
new JProperty("country", ""),
new JProperty("items",
new JArray(
new JObject(
new JProperty("productId", ""),
new JProperty("contactInfo",
new JObject(
new JProperty("phoneNumbers", ""),
new JProperty("email", "")
)),
new JProperty("recipient",
new JObject(
new JProperty("firstName", ""),
new JProperty("lastName", "")
)),
new JProperty("fulfillmentDetails",
new JObject(
new JProperty("type", "SHIP"),
new JProperty("getBy",
new JObject(
new JProperty("maxDate",
new JObject(
new JProperty("dateTime", ""),
new JProperty("timezone", "America/Indiana/Indianapolis"),
new JProperty("precision", "DAY"))))),
new JProperty("location",
new JObject(
new JProperty("type", "address/shipping"),
new JProperty("postalAddress",
new JObject(
new JProperty("country", ""),
new JProperty("address1", ""),
new JProperty("address2", ""),
new JProperty("postalCode", ""),
new JProperty("city", ""),
new JProperty("state", ""))))))),
new JProperty("shippingAddress",
new JObject(
new JProperty("address1", ""),
new JProperty("address2", ""),
new JProperty("city", ""),
new JProperty("postalCode", ""),
new JProperty("country", "")))))),
new JProperty("paymentInfo",
new JArray(
new JObject(
new JProperty("id", ""),
new JProperty("type", "CreditCard"),
new JProperty("creditCardInfoId", ""),
new JProperty("billingInfo",
new JObject(
new JProperty("name",
new JObject(
new JProperty("firstName", ""),
new JProperty("lastName", ""))),
new JProperty("address",
new JObject(
new JProperty("address1", ""),
new JProperty("address2", ""),
new JProperty("city", ""),
new JProperty("postalCode", ""),
new JProperty("state", ""),
new JProperty("country", ""))),
new JProperty("contactInfo",
new JObject(
new JProperty("phoneNumber", ""),
new JProperty("email", "")))
))
))));
          JObject  paymentoken = new JObject(
new JProperty("request",
new JObject(
new JProperty("country", ""),
new JProperty("currency", ""),
new JProperty("email", ""),
new JProperty("locale", ""),
new JProperty("paymentToken", ""),
new JProperty("channel", "NIKECOM"),
 new JProperty("promotionCodes",
 new JArray("code")),
new JProperty("totals",
new JObject(
new JProperty("total", ""),
new JProperty("fulfillment",
new JObject(
new JProperty("total",  ""),
new JProperty("details",
new JObject(
new JProperty("price", 0),
new JProperty("discount", 0))
)
)
))
),
new JProperty("items",
new JArray(
new JObject(
new JProperty("fulfillmentDetails",
new JObject(
new JProperty("type", "SHIP"),
new JProperty("getBy",
new JObject(
new JProperty("maxDate",
new JObject(
new JProperty("dateTime", ""),
new JProperty("timezone", "America/Indiana/Indianapolis"),
new JProperty("precision", "DAY"))))),
new JProperty("location",
new JObject(
new JProperty("type", "address/shipping"),
new JProperty("postalAddress",
new JObject(
new JProperty("country", ""),
new JProperty("address1", ""),
new JProperty("postalCode", ""),
new JProperty("city", ""),
new JProperty("state", ""))))))),
new JProperty("id", ""),
new JProperty("skuId", ""),
new JProperty("shippingMethod", ""),
new JProperty("quantity", ""),
new JProperty("recipient",
new JObject(
new JProperty("firstName", ""),
new JProperty("lastName", ""))),
new JProperty("contactInfo",
new JObject(
new JProperty("phoneNumber", ""),
new JProperty("email", ""))),
new JProperty("shippingAddress",
new JObject(
new JProperty("address1", ""),
new JProperty("address2", ""),
new JProperty("city", ""),
new JProperty("state", ""),
new JProperty("postalCode", ""),
new JProperty("country", ""))
))
)))));
            previewpaylod = payLoad.ToString();
            previewpayinfo = payinfo.ToString();
            priviewpayment=paymentoken.ToString();
        }
      
    }
}

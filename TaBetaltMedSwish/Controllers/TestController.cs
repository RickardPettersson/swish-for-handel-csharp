using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using TaBetaltMedSwish.Helpers;

namespace TaBetaltMedSwish.Controllers
{
    public class TestController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        // GET: Test
        public ActionResult Index()
        {
            Helpers.VisitorLogHelper.Log();

            return View();
        }

        [HttpPost]
        public ActionResult Swish(FormCollection f)
        {
            Helpers.VisitorLogHelper.Log();

            // Input data från forumlär
            string buyerNumber = f["inputBuyerNumber"];
            int amount = Convert.ToInt32(f["inputAmountr"]);
            string message = f["inputMessage"];

            // Objekt att hålla resultetat efter en swish betlanings förfrågan
            SwishApi.Models.PaymentRequestECommerceResponse result = null;

            // Kontrollera om web.config är satt att vara i test läge
            if (Config.TestMode)
            {
                // Hämta sökvägen till certifikatet
                string certificatePath = HostingEnvironment.MapPath(@"~/App_Data/" + Config.Test.CertificateFileName);

                // Create a SwishApi Client object
                SwishApi.Client client = new SwishApi.Client(certificatePath, Config.Test.CertificatePassword, Config.Test.CallbackURL);

                // Make the payment request
                result = client.MakePaymentRequest(buyerNumber, amount, message);
            }
            else
            {
                // Hämta sökvägen till certifikatet
                string certificatePath = HostingEnvironment.MapPath(@"~/App_Data/" + Config.Production.CertificateFileName);

                // Create a SwishApi Client object
                SwishApi.Client client = new SwishApi.Client(certificatePath, Config.Production.CertificatePassword, Config.Production.CallbackURL, Config.Production.PayeePaymentReference, Config.Production.PayeeAlias);

                // Make the payment request
                result = client.MakePaymentRequest(buyerNumber, amount, message);
            }

            ViewBag.result = result;

            return View();
        }

        [HttpPost]
        public JsonResult PaymentStatus(string statusUrl)
        {
            Helpers.VisitorLogHelper.Log();

            SwishApi.Models.CheckPaymentRequestStatusResponse result = null;

            // Kontrollera om web.config är satt att vara i test läge
            if (Config.TestMode)
            {
                // Hämta sökvägen till certifikatet
                string certificatePath = HostingEnvironment.MapPath(@"~/App_Data/" + Config.Test.CertificateFileName);

                // Create a SwishApi Client object
                SwishApi.Client client = new SwishApi.Client(certificatePath, Config.Test.CertificatePassword, Config.Test.CallbackURL);

                // Kontrollera betalnings status med hjälp av certifikatet som en byte array
                result = client.CheckPaymentStatus(statusUrl);
            }
            else
            {
                // Hämta sökvägen till certifikatet
                string certificatePath = HostingEnvironment.MapPath(@"~/App_Data/" + Config.Production.CertificateFileName);

                // Create a SwishApi Client object
                SwishApi.Client client = new SwishApi.Client(certificatePath, Config.Production.CertificatePassword, Config.Production.CallbackURL, Config.Production.PayeePaymentReference, Config.Production.PayeeAlias);

                // Kontrollera betalnings status med hjälp av certifikatet som en byte array
                result = client.CheckPaymentStatus(statusUrl);
            }

            // When someone like to use this live i should log this and maybe change the status of some order or somethign to be paid or what the status says.
            // To make a refund you need to save the value of paymentReference
            // var paymentReference = result.paymentReference;

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// Refund a payment with paymentReference from a payment status that says PAID
        /// </summary>
        /// <param name="id">paymentReference</param>
        /// <param name="a">Amount to refund</param>
        /// <param name="p">payeePaymentReference = Order ID eller annat som identifierar ordern som är betald och i detta fall återköpt</param>
        /// <returns></returns>
        public ActionResult Refund(string id, int a, string p)
        {
            Helpers.VisitorLogHelper.Log();

            SwishApi.Models.RefundResponse result;

            // Kontrollera om web.config är satt att vara i test läge
            if (Config.TestMode)
            {
                // Hämta sökvägen till certifikatet
                string certificatePath = HostingEnvironment.MapPath(@"~/App_Data/" + Config.Test.CertificateFileName);

                // Create a SwishApi Client object
                SwishApi.Client client = new SwishApi.Client(certificatePath, Config.Test.CertificatePassword, Config.Test.CallbackURL);

                // Kontrollera betalnings status med hjälp av certifikatet som en byte array
                // "Återköp" strängen är meddelandet användaren ser vid återbetalning i Swish appen, här bör man kankse i en produktionsmiljö skicka med mer detaljer
                result = client.Refund(id, (double)a, "Återköp", Config.Test.RefundCallbackURL);
            }
            else
            {
                // Hämta sökvägen till certifikatet
                string certificatePath = HostingEnvironment.MapPath(@"~/App_Data/" + Config.Production.CertificateFileName);

                // Create a SwishApi Client object
                SwishApi.Client client = new SwishApi.Client(certificatePath, Config.Production.CertificatePassword, Config.Production.CallbackURL, Config.Production.PayeePaymentReference, Config.Production.PayeeAlias);

                // Kontrollera betalnings status med hjälp av certifikatet som en byte array
                // "Återköp" strängen är meddelandet användaren ser vid återbetalning i Swish appen, här bör man kankse i en produktionsmiljö skicka med mer detaljer
                result = client.Refund(id, (double)a, "Återköp", Config.Test.RefundCallbackURL);
            }

            return View();
        }

        public string Callback()
        {
            Helpers.VisitorLogHelper.Log();

            Stream req = Request.InputStream;
            req.Seek(0, System.IO.SeekOrigin.Begin);
            string json = new StreamReader(req).ReadToEnd();

            logger.Debug("/Test/Callback > json: " + json);

            SwishApi.Models.CheckPaymentRequestStatusResponse resultObject = JsonConvert.DeserializeObject<SwishApi.Models.CheckPaymentRequestStatusResponse>(json);

            switch (resultObject.status)
            {
                case "CREATED":
                    // Borde kanske alldrig få CREATED här...
                    break;
                case "PAID":
                    // Betalningen är klar
                    break;
                case "DECLINED":
                    // Användaren avbröt betalningen
                    break;
                case "ERROR":
                    // Något gick fel, om betalningen inte sker inom 3 minuter skickas ERROR
                    break;
            }

            // When someone like to use this live i should log this and maybe change the status of some order or somethign to be paid or what the status says.
            // To make a refund you need to save the value of paymentReference
            // var paymentReference = resultObject.paymentReference;


            return "OK";
        }

        public string RefundCallback()
        {
            Helpers.VisitorLogHelper.Log();

            // Exempel Callback json sträng
            // 
            Stream req = Request.InputStream;
            req.Seek(0, System.IO.SeekOrigin.Begin);
            string json = new StreamReader(req).ReadToEnd();

            logger.Debug("/Test/RefundCallback > json: " + json);

            SwishApi.Models.CheckRefundStatusResponse resultObject = JsonConvert.DeserializeObject<SwishApi.Models.CheckRefundStatusResponse>(json);

            switch (resultObject.status)
            {
                case "DEBITED,":
                    // Återköpt
                    break;
                case "PAID":
                    // Betald
                    break;
                case "ERROR":
                    // Något gick fel
                    break;
            }

            // When someone like to use this live i should log this and maybe change the status of some order or something to be repaid or what the status says.
            // Use payerPaymentReference to get the order
            // var paymentref = resultObject.payerPaymentReference;

            return "OK";
        }
    }
}
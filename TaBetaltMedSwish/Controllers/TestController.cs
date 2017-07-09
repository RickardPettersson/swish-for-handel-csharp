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
            string payeePaymentReference = f["inputReference"];
            string buyerNumber = f["inputBuyerNumber"];
            int amount = Convert.ToInt32(f["inputAmountr"]);
            string message = f["inputMessage"];

            // Objekt att hålla resultetat efter en swish betlanings förfrågan
            SwishPaymentRequestResponse result = null;

            // Kontrollera om web.config är satt att vara i test läge
            if (Config.TestMode)
            {
                // Hämta sökvägen till certifikatet
                string certificatePath = HostingEnvironment.MapPath(@"~/App_Data/" + Config.Test.CertificateFileName);

                // Läs in certifikat filen som en byte array, detta är endast för att visa att funktionen nedan kan ta en byte array som skulle kunna hämtats från en databas
                byte[] certDataBytes = System.IO.File.ReadAllBytes(certificatePath);

                // Gör en Swish betalnings förfrågan med certifikatet som en byte array
                result = Helpers.SwishHelper.PaymentRequestWithByteArray(buyerNumber, amount, message, Config.Test.CallbackURL, certDataBytes, Config.Test.CertificatePassword, Config.Test.PaymentRequestURL, payeePaymentReference, Config.Test.PayeeAlias);
            }
            else
            {
                // Hämta sökvägen till certifikatet
                string certificatePath = HostingEnvironment.MapPath(@"~/App_Data/" + Config.Production.CertificateFileName);

                // Läs in certifikat filen som en byte array, detta är endast för att visa att funktionen nedan kan ta en byte array som skulle kunna hämtats från en databas
                byte[] certDataBytes = System.IO.File.ReadAllBytes(certificatePath);

                // Gör en Swish betalnings förfrågan med certifikatet som en byte array
                result = Helpers.SwishHelper.PaymentRequestWithByteArray(buyerNumber, amount, message, Config.Production.CallbackURL, certDataBytes, Config.Production.CertificatePassword, Config.Production.PaymentRequestURL, payeePaymentReference, Config.Production.PayeeAlias);
            }

            ViewBag.result = result;

            return View();
        }

        [HttpPost]
        public JsonResult PaymentStatus(string statusUrl)
        {
            Helpers.VisitorLogHelper.Log();

            SwishCheckPaymentRequestStatusResponse result = null;

            // Kontrollera om web.config är satt att vara i test läge
            if (Config.TestMode)
            {
                // Hämta sökvägen till certifikatet
                string certificatePath = HostingEnvironment.MapPath(@"~/App_Data/" + Config.Test.CertificateFileName);

                // Läs in certifikat filen som en byte array, detta är endast för att visa att funktionen nedan kan ta en byte array som skulle kunna hämtats från en databas
                byte[] certDataBytes = System.IO.File.ReadAllBytes(certificatePath);

                // Kontrollera betalnings status med hjälp av certifikatet som en byte array
                result = Helpers.SwishHelper.CheckPaymentRequestStatusWithByteArray(statusUrl, certDataBytes, Config.Test.CertificatePassword);
            }
            else
            {
                // Hämta sökvägen till certifikatet
                string certificatePath = HostingEnvironment.MapPath(@"~/App_Data/" + Config.Production.CertificateFileName);

                // Läs in certifikat filen som en byte array, detta är endast för att visa att funktionen nedan kan ta en byte array som skulle kunna hämtats från en databas
                byte[] certDataBytes = System.IO.File.ReadAllBytes(certificatePath);

                // Kontrollera betalnings status med hjälp av certifikatet som en byte array
                result = Helpers.SwishHelper.CheckPaymentRequestStatusWithByteArray(statusUrl, certDataBytes, Config.Production.CertificatePassword);
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

            string result = string.Empty;

            // Kontrollera om web.config är satt att vara i test läge
            if (Config.TestMode)
            {
                // Hämta sökvägen till certifikatet
                string certificatePath = HostingEnvironment.MapPath(@"~/App_Data/" + Config.Test.CertificateFileName);

                // Läs in certifikat filen som en byte array, detta är endast för att visa att funktionen nedan kan ta en byte array som skulle kunna hämtats från en databas
                byte[] certDataBytes = System.IO.File.ReadAllBytes(certificatePath);

                // Kontrollera betalnings status med hjälp av certifikatet som en byte array
                // "Återköp" strängen är meddelandet användaren ser vid återbetalning i Swish appen, här bör man kankse i en produktionsmiljö skicka med mer detaljer
                result = SwishHelper.PaymentRefundWithWithByteArray(p, id, a, "Återköp", Config.Test.RefundCallbackURL, certDataBytes, Config.Test.CertificatePassword, Config.Test.PaymentRefundURL, Config.Test.PayeeAlias);
            }
            else
            {
                // Hämta sökvägen till certifikatet
                string certificatePath = HostingEnvironment.MapPath(@"~/App_Data/" + Config.Production.CertificateFileName);

                // Läs in certifikat filen som en byte array, detta är endast för att visa att funktionen nedan kan ta en byte array som skulle kunna hämtats från en databas
                byte[] certDataBytes = System.IO.File.ReadAllBytes(certificatePath);

                // Kontrollera betalnings status med hjälp av certifikatet som en byte array
                // "Återköp" strängen är meddelandet användaren ser vid återbetalning i Swish appen, här bör man kankse i en produktionsmiljö skicka med mer detaljer
                result = SwishHelper.PaymentRefundWithWithByteArray(p, id, a, "Återköp", Config.Production.RefundCallbackURL, certDataBytes, Config.Production.CertificatePassword, Config.Production.PaymentRefundURL, Config.Production.PayeeAlias);
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

            SwishCheckPaymentRequestStatusResponse resultObject = JsonConvert.DeserializeObject<SwishCheckPaymentRequestStatusResponse>(json);

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

            SwishRefundSatusCheckResponse resultObject = JsonConvert.DeserializeObject<SwishRefundSatusCheckResponse>(json);

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
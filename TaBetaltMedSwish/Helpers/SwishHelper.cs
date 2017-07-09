using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Hosting;

namespace TaBetaltMedSwish.Helpers
{

    public class SwishHelper
    {
        // Logger to log to text files in /logs/
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Denna metod gör en betalnings förfrågan med hjälp av ett certifikat i en fil
        /// </summary>
        /// <param name="phonenumber"></param>
        /// <param name="amount"></param>
        /// <param name="message"></param>
        /// <param name="callbackUrl"></param>
        /// <param name="certificatePath"></param>
        /// <param name="certificatePassword"></param>
        /// <param name="URL"></param>
        /// <param name="payeePaymentReference"></param>
        /// <param name="payeeAlias"></param>
        /// <returns></returns>
        public static SwishPaymentRequestResponse PaymentRequestWithFile(string phonenumber, int amount, string message, string callbackUrl, string certificatePath, string certificatePassword, string URL = "https://mss.swicpc.bankgirot.se/swish-cpcapi/api/v1/paymentrequests/", string payeePaymentReference = "0123456789", string payeeAlias = "1231181189")
        {
            try
            {
                // Läs in certifikat filen som en byte array
                byte[] certDataBytes = System.IO.File.ReadAllBytes(certificatePath);

                // Använd byte arrayen och gör betalnings förfrågan
                return PaymentRequestWithByteArray(phonenumber, amount, message, callbackUrl, certDataBytes, certificatePassword, URL, payeePaymentReference, payeeAlias);
            }
            catch (Exception ex)
            {
                logger.Error("PaymentRequestWithFile > Exception > phonenumber: " + phonenumber + " - amount: " + amount + " - message: \"" + message + "\" - callbackUrl: \"" + callbackUrl + "\" - certificatePath: \"" + certificatePath + "\" - certificatePassword:: \"" + certificatePassword + "\" - URL: \"" + URL + "\" - payeePaymentReference: \"" + payeePaymentReference + "\" - payeeAlias: " + payeeAlias + " - Exception: " + ex.ToString());

                return new SwishPaymentRequestResponse()
                {
                    Location = "",
                    PaymentRequestToken = "",
                    Error = "Exception: " + ex.ToString()
                };
            }
        }

        /// <summary>
        /// Denna metod gör en betalnings förfrågan med hjälp av ett certifikat i en byte array
        /// </summary>
        /// <param name="phonenumber"></param>
        /// <param name="amount"></param>
        /// <param name="message"></param>
        /// <param name="callbackUrl"></param>
        /// <param name="certificateData"></param>
        /// <param name="certificatePassword"></param>
        /// <param name="URL"></param>
        /// <param name="payeePaymentReference"></param>
        /// <param name="payeeAlias"></param>
        /// <returns></returns>
        public static SwishPaymentRequestResponse PaymentRequestWithByteArray(string phonenumber, int amount, string message, string callbackUrl, byte[] certificateData, string certificatePassword, string URL = "https://mss.swicpc.bankgirot.se/swish-cpcapi/api/v1/paymentrequests/", string payeePaymentReference = "0123456789", string payeeAlias = "1231181189")
        {
            try
            {
                // Data should be sent as a JSON object
                string DataToPost = "{ \"payeePaymentReference\": \"" + payeePaymentReference + "\", " +
                                        "\"callbackUrl\": \"" + callbackUrl + "\", " +
                                         "\"payerAlias\": \"" + phonenumber + "\", " +
                                         "\"payeeAlias\": \"" + payeeAlias + "\", " +
                                         "\"amount\": \"" + amount + "\", " +
                                         "\"currency\": \"SEK\", " +
                                         "\"message\": \"" + message + "\" }";

                // If no phonenumber input then do a m-commerce request without payerAlias
                if (string.IsNullOrEmpty(phonenumber))
                {
                    DataToPost = "{ \"payeePaymentReference\": \"" + payeePaymentReference + "\", " +
                                        "\"callbackUrl\": \"" + callbackUrl + "\", " +
                                         "\"payeeAlias\": \"" + payeeAlias + "\", " +
                                         "\"amount\": \"" + amount + "\", " +
                                         "\"currency\": \"SEK\", " +
                                         "\"message\": \"" + message + "\" }";
                }


                // Create HttpWebRequest object with certificate
                HttpWebRequest req = CreateSwishRequest(URL, certificateData, certificatePassword);

                // Turn our request string into a byte stream
                byte[] postBytes = Encoding.UTF8.GetBytes(DataToPost);

                // Set the content length
                req.ContentLength = postBytes.Length;

                // Get the request stream
                Stream requestStream = req.GetRequestStream();

                // Now send the data
                requestStream.Write(postBytes, 0, postBytes.Length);

                // Close the stream
                requestStream.Close();

                try
                {
                    // Get the response object
                    WebResponse response = req.GetResponse();

                    // Convert the WebResponse objecvt to a HttpWebResponse object
                    HttpWebResponse httpResponse = (HttpWebResponse)response;

                    // Get the Location header where the url to check payment request status is
                    string location = response.Headers["Location"];

                    // Get Payment Request Token, but this only if doing a request for m-commerce not e-commerce
                    string paymentRequestToken = response.Headers["PaymentRequestToken"];

                    logger.Debug("PaymentRequestWithByteArray > Success > phonenumber: " + phonenumber + " - amount: " + amount + " - message: \"" + message + "\" - callbackUrl: \"" + callbackUrl + "\" - certificatePath: \"\" - certificatePassword:: \"" + certificatePassword + "\" - URL: \"" + URL + "\" - payeePaymentReference: \"" + payeePaymentReference + "\" - payeeAlias: " + payeeAlias + " - location: \"" + location + "\" - paymentRequestToken: \"" + paymentRequestToken + "\"");

                    // Return the result
                    return new SwishPaymentRequestResponse()
                    {
                        Location = location,
                        PaymentRequestToken = paymentRequestToken,
                        Error = ""
                    };
                }
                catch (WebException e)
                {
                    using (WebResponse response = e.Response)
                    {
                        HttpWebResponse httpResponse = (HttpWebResponse)response;

                        using (Stream data = response.GetResponseStream())
                        {
                            string text = new StreamReader(data).ReadToEnd();

                            logger.Error("PaymentRequestWithByteArray > Exception > phonenumber: " + phonenumber + " - amount: " + amount + " - message: \"" + message + "\" - callbackUrl: \"" + callbackUrl + "\" - certificateData: \"\" - certificatePassword:: \"" + certificatePassword + "\" - URL: \"" + URL + "\" - payeePaymentReference: \"" + payeePaymentReference + "\" - payeeAlias: " + payeeAlias + " - Error code: " + httpResponse.StatusCode + " - Content: " + text);

                            // TODO: Här borde man kanske hantera olika felmeddelanden, see GetSwish AB API dokumentation för felmeddelanden man kan få

                            return new SwishPaymentRequestResponse()
                            {
                                Location = "",
                                PaymentRequestToken = "",
                                Error = "Error code: " + httpResponse.StatusCode + " - Content: " + text
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("PaymentRequestWithByteArray > Exception > phonenumber: " + phonenumber + " - amount: " + amount + " - message: \"" + message + "\" - callbackUrl: \"" + callbackUrl + "\" - certificateData: \"\" - certificatePassword:: \"" + certificatePassword + "\" - URL: \"" + URL + "\" - payeePaymentReference: \"" + payeePaymentReference + "\" - payeeAlias: " + payeeAlias + " - Exception: " + ex.ToString());

                return new SwishPaymentRequestResponse()
                {
                    Location = "",
                    PaymentRequestToken = "",
                    Error = "Exception: " + ex.ToString()
                };
            }
        }

        /// <summary>
        /// Denna metod kontrollerar status för en Swish betalning med hjälp av ett certifikat i en fil
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="certificatePath"></param>
        /// <param name="certificatePassword"></param>
        /// <returns></returns>
        public static SwishCheckPaymentRequestStatusResponse CheckPaymentRequestStatusWithFile(string URL, string certificatePath, string certificatePassword)
        {
            try
            {
                // Läs in certifikat filen som en byte array
                byte[] certDataBytes = System.IO.File.ReadAllBytes(certificatePath);

                // Använd byte arrayen och gör anroppet för att kontrollera betalnings status
                return CheckPaymentRequestStatusWithByteArray(URL, certDataBytes, certificatePassword);
            }
            catch (Exception ex)
            {
                logger.Error("CheckPaymentRequestStatusWithFile > Exception > URL: \"" + URL + "\" + - certificatePath: \"" + certificatePath + "\" - certificatePassword: \"" + certificatePassword + "\" - Exception: " + ex.ToString());

                return new SwishCheckPaymentRequestStatusResponse()
                {
                    errorCode = "Exception",
                    errorMessage = ex.ToString()
                };
            }
        }

        /// <summary>
        /// Denna metod kontrollerar status för en Swish betalning med hjälp av ett certifikat i en byte array
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="certificateData"></param>
        /// <param name="certificatePassword"></param>
        /// <returns></returns>
        public static SwishCheckPaymentRequestStatusResponse CheckPaymentRequestStatusWithByteArray(string URL, byte[] certificateData, string certificatePassword)
        {
            try
            {
                ServicePointManager.Expect100Continue = true;

                // Swish för handel API need TLS 1.1 or higher
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11;

                // Make a Uri object of the URL
                Uri requestURI = new Uri(URL);

                // Create the Request Object
                HttpWebRequest req = CreateSwishRequest(URL, certificateData, certificatePassword);

                // Set the Request Object parameters
                req.Method = "GET";

                try
                {
                    // Get the response object
                    WebResponse response = req.GetResponse();

                    String result = "";

                    using (StreamReader rdr = new StreamReader(response.GetResponseStream()))
                    {
                        result = rdr.ReadToEnd();
                    }

                    logger.Debug("CheckPaymentRequestStatusWithByteArray > Success > URL: \"" + URL + "\" + - certificateData: \"\" - certificatePassword: \"" + certificatePassword + "\" - result: " + result);

                    SwishCheckPaymentRequestStatusResponse resultObject = JsonConvert.DeserializeObject<SwishCheckPaymentRequestStatusResponse>(result);

                    return resultObject;
                }
                catch (WebException e)
                {
                    logger.Error("CheckPaymentRequestStatusWithByteArray > WebException > URL: \"" + URL + "\" + - certificateData: \"\" - certificatePassword: \"" + certificatePassword + "\" - WebException: " + e.Message.ToString());

                    // TODO: Här borde man kanske hantera olika felmeddelanden, see GetSwish AB API dokumentation för felmeddelanden man kan få

                    return new SwishCheckPaymentRequestStatusResponse()
                    {
                        errorCode = "WebException",
                        errorMessage = e.Message.ToString()
                    };
                }
            }
            catch (Exception ex)
            {
                logger.Error("CheckPaymentRequestStatusWithByteArray > Exception > URL: \"" + URL + "\" + - certificateData: \"\" - certificatePassword: \"" + certificatePassword + "\" - Exception: " + ex.ToString());

                return new SwishCheckPaymentRequestStatusResponse()
                {
                    errorCode = "Exception",
                    errorMessage = ex.ToString()
                };
            }
        }

        /// <summary>
        /// Funktion för att göra återköp av en swish betalning med hjälp av ett certifikat i en fil
        /// </summary>
        /// <param name="payerPaymentReference"></param>
        /// <param name="originalPaymentReference"></param>
        /// <param name="amount"></param>
        /// <param name="message"></param>
        /// <param name="callbackUrl"></param>
        /// <param name="certificatePath"></param>
        /// <param name="certificatePassword"></param>
        /// <param name="URL"></param>
        /// <param name="payeePaymentReference"></param>
        /// <param name="payeeAlias"></param>
        /// <returns></returns>
        public static string PaymentRefundWithFile(string payerPaymentReference, string originalPaymentReference, int amount, string message, string callbackUrl, string certificatePath, string certificatePassword, string URL = "https://mss.swicpc.bankgirot.se/swish-cpcapi/api/v1/paymentrequests/", string payeeAlias = "1231181189")
        {
            try
            {
                // Läs in certifikat filen som en byte array
                byte[] certDataBytes = System.IO.File.ReadAllBytes(certificatePath);

                // Använd byte arrayen och gör återköps anropet
                return PaymentRefundWithWithByteArray(payerPaymentReference, originalPaymentReference, amount, message, callbackUrl, certDataBytes, certificatePassword, URL, payeeAlias);
            }
            catch (Exception ex)
            {
                logger.Error("PaymentRefundWithFile > Exception > payerPaymentReference: \"" + payerPaymentReference + "\" - originalPaymentReference: \"" + originalPaymentReference + " - amount: " + amount + " - message: \"" + message + "\" - callbackUrl: \"" + callbackUrl + "\" - certificatePath: \"" + certificatePath + "\" - certificatePassword:: \"" + certificatePassword + "\" - URL: \"" + URL + "\" - payeeAlias: " + payeeAlias + " - Exception: " + ex.ToString());

                return "Exception: " + ex.ToString();
            }
        }

        /// <summary>
        /// Funktion för att göra återköp av en swish betalning med hjälp av ett certifikat i en byte array
        /// </summary>
        /// <param name="payerPaymentReference"></param>
        /// <param name="originalPaymentReference"></param>
        /// <param name="amount"></param>
        /// <param name="message"></param>
        /// <param name="callbackUrl"></param>
        /// <param name="certificateData"></param>
        /// <param name="certificatePassword"></param>
        /// <param name="URL"></param>
        /// <param name="payeePaymentReference"></param>
        /// <param name="payerAlias"></param>
        /// <returns></returns>
        public static string PaymentRefundWithWithByteArray(string payerPaymentReference, string originalPaymentReference, int amount, string message, string callbackUrl, byte[] certificateData, string certificatePassword, string URL = "https://mss.swicpc.bankgirot.se/swish-cpcapi/api/v1/paymentrequests/", string payerAlias = "1231181189")
        {
            try
            {
                // Data should be sent as a JSON object
                string DataToPost = "{ \"payerPaymentReference\": \"" + payerPaymentReference + "\", " +
                                        "\"originalPaymentReference\": \"" + originalPaymentReference + "\", " +
                                        "\"callbackUrl\": \"" + callbackUrl + "\", " +
                                         "\"payerAlias\": \"" + payerAlias + "\", " +
                                         "\"amount\": \"" + amount + "\", " +
                                         "\"currency\": \"SEK\", " +
                                         "\"message\": \"" + message + "\" }";


                // Create HttpWebRequest object with certificate
                HttpWebRequest req = CreateSwishRequest(URL, certificateData, certificatePassword);

                // Turn our request string into a byte stream
                byte[] postBytes = Encoding.UTF8.GetBytes(DataToPost);

                // Set the content length
                req.ContentLength = postBytes.Length;

                // Get the request stream
                Stream requestStream = req.GetRequestStream();

                // Now send the data
                requestStream.Write(postBytes, 0, postBytes.Length);

                // Close the stream
                requestStream.Close();

                try
                {
                    // Get the response object
                    WebResponse response = req.GetResponse();

                    // Convert the WebResponse objecvt to a HttpWebResponse object
                    HttpWebResponse httpResponse = (HttpWebResponse)response;

                    // Get the Location header where the url to check payment request status is
                    string location = response.Headers["Location"];

                    logger.Debug("PaymentRefundWithWithByteArray > Success > payerPaymentReference: \"" + payerPaymentReference + "\" - originalPaymentReference: \"" + originalPaymentReference + "\" - amount: " + amount + " - message: \"" + message + "\" - callbackUrl: \"" + callbackUrl + "\" - certificateData: \"\" - certificatePassword:: \"" + certificatePassword + "\" - URL: \"" + URL + "\" - payerAlias: " + payerAlias + " - location: \"" + location + "\"");

                    // Return the result
                    return location;
                }
                catch (WebException e)
                {
                    using (WebResponse response = e.Response)
                    {
                        HttpWebResponse httpResponse = (HttpWebResponse)response;

                        using (Stream data = response.GetResponseStream())
                        {
                            string text = new StreamReader(data).ReadToEnd();

                            logger.Error("PaymentRefundWithWithByteArray > WebException > payerPaymentReference: \"" + payerPaymentReference + "\" - originalPaymentReference: \"" + originalPaymentReference + " - amount: " + amount + " - message: \"" + message + "\" - callbackUrl: \"" + callbackUrl + "\" - certificateData: \"\" - certificatePassword:: \"" + certificatePassword + "\" - URL: \"" + URL + "\" - payerAlias: " + payerAlias + " - Error code: " + httpResponse.StatusCode + " - Content: " + text);

                            // TODO: Här borde man kanske hantera olika felmeddelanden, see GetSwish AB API dokumentation för felmeddelanden man kan få

                            return "Error code: " + httpResponse.StatusCode + " - Content: " + text;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("PaymentRefundWithWithByteArray > Exception > payerPaymentReference: \"" + payerPaymentReference + "\" - originalPaymentReference: \"" + originalPaymentReference + " - amount: " + amount + " - message: \"" + message + "\" - callbackUrl: \"" + callbackUrl + "\" - certificateData: \"\" - certificatePassword:: \"" + certificatePassword + "\" - URL: \"" + URL + "\" - payerAlias: " + payerAlias + " - Exception: " + ex.ToString());

                return "Exception: " + ex.ToString();
            }
        }

        /// <summary>
        /// Denna metod kontrollerar status för en Swish återbetalning med hjälp av ett certifikat i en fil
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="certificatePath"></param>
        /// <param name="certificatePassword"></param>
        /// <returns></returns>
        public static SwishRefundSatusCheckResponse CheckRefundStatusWithFile(string URL, string certificatePath, string certificatePassword)
        {
            try
            {
                // Läs in certifikat filen som en byte array
                byte[] certDataBytes = System.IO.File.ReadAllBytes(certificatePath);

                // Använd byte arrayen och gör kontroll på återköpet
                return CheckRefundStatusWithByteArray(URL, certDataBytes, certificatePassword);
            }
            catch (Exception ex)
            {
                logger.Error("SwishRefundSatusCheckResponse > Exception > URL: \"" + URL + "\" + - certificatePath: \"" + certificatePath + "\" - certificatePassword: \"" + certificatePassword + "\" - Exception: " + ex.ToString());

                return new SwishRefundSatusCheckResponse()
                {
                    errorCode = "Exception",
                    errorMessage = ex.ToString()
                };
            }
        }

        /// <summary>
        /// Denna metod kontrollerar status för en Swish återbetalning med hjälp av ett certifikat i en byte array
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="certificateData"></param>
        /// <param name="certificatePassword"></param>
        /// <returns></returns>
        public static SwishRefundSatusCheckResponse CheckRefundStatusWithByteArray(string URL, byte[] certificateData, string certificatePassword)
        {
            try
            {
                ServicePointManager.Expect100Continue = true;

                // Swish för handel API need TLS 1.1 or higher
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11;

                // Make a Uri object of the URL
                Uri requestURI = new Uri(URL);

                // Create the Request Object
                HttpWebRequest req = CreateSwishRequest(URL, certificateData, certificatePassword);

                // Set the Request Object parameters
                req.Method = "GET";

                try
                {
                    // Get the response object
                    WebResponse response = req.GetResponse();

                    String result = "";

                    using (StreamReader rdr = new StreamReader(response.GetResponseStream()))
                    {
                        result = rdr.ReadToEnd();
                    }

                    logger.Debug("CheckRefundStatusWithByteArray > Success > URL: \"" + URL + "\" + - certificateData: \"\" - certificatePassword: \"" + certificatePassword + "\" - result: " + result);

                    SwishRefundSatusCheckResponse resultObject = JsonConvert.DeserializeObject<SwishRefundSatusCheckResponse>(result);

                    return resultObject;
                }
                catch (WebException e)
                {
                    logger.Error("CheckRefundStatusWithByteArray > WebException > URL: \"" + URL + "\" + - certificateData: \"\" - certificatePassword: \"" + certificatePassword + "\" - WebException: " + e.Message.ToString());

                    return new SwishRefundSatusCheckResponse()
                    {
                        errorCode = "WebException",
                        errorMessage = e.Message.ToString()
                    };
                }
            }
            catch (Exception ex)
            {
                logger.Error("CheckRefundStatusWithByteArray > Exception > URL: \"" + URL + "\" + - certificateData: \"\" - certificatePassword: \"" + certificatePassword + "\" - Exception: " + ex.ToString());

                return new SwishRefundSatusCheckResponse()
                {
                    errorCode = "Exception",
                    errorMessage = ex.ToString()
                };
            }
        }

        /// <summary>
        /// Denna funktion hjälper till att skapa ett HTTP anrop mot Swish med hjälp av ett certifikat i en byte array
        /// Bygger på funktionen ovan men modifierad att ta en byte array av certifikatet istället för filens sökväg.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="clientCertData"></param>
        /// <param name="clientCertPass"></param>
        /// <returns></returns>
        private static HttpWebRequest CreateSwishRequest(String url, byte[] clientCertData, String clientCertPass)
        {
            //Basic set up 
            ServicePointManager.CheckCertificateRevocationList = false;

            //Tls12 does not work 
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11;

            //Load client certificates 
            var clientCerts = new X509Certificate2Collection();

            clientCerts.Import(clientCertData, clientCertPass ?? "", X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);

            //Assert CA certs in cert store, and get root CA 
            X509Certificate2 rootCertificate = AssertCertsInStore(clientCerts);

            var req = HttpWebRequest.Create(url) as HttpWebRequest;

            req.ClientCertificates = clientCerts;
            req.Method = "POST";
            req.ContentType = "application/json; charset=UTF-8";
            req.AllowAutoRedirect = false;

            //Verify server root CA by comparing to client cert root CA 
            req.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
            {
                var chainRootCa = chain?.ChainElements?.OfType<X509ChainElement>().LastOrDefault()?.Certificate;

                if (rootCertificate == null || chainRootCa == null) return false;

                return rootCertificate.Equals(chainRootCa);
                //Same root CA as client cert 
            };

            return req;
        }

        /// <summary>
        /// Funktion som fixar till certifikatet så man slipper installera det på certifikat storen på servern
        /// Kopierat rakt av från en webbforum post på https://www.wn.se/showthread.php?p=20516204#post20516204
        /// Jack Jönsson från infringo.se skriver att det är en funktion från Digrad affärssystem som släppte i det forum inlägget
        /// </summary>
        /// <param name="certs"></param>
        /// <returns></returns>
        private static X509Certificate2 AssertCertsInStore(X509Certificate2Collection certs)
        {
            //Create typed array 
            var certArr = certs.OfType<X509Certificate2>().ToArray();

            //Build certificate chain 
            var chain = new X509Chain();

            chain.ChainPolicy.ExtraStore.AddRange(certArr.Where(o => !o.HasPrivateKey).ToArray());

            var privateCert = certArr.FirstOrDefault(o => o.HasPrivateKey);

            if (privateCert == null) return null;

            var result = chain.Build(privateCert);

            //Get CA certs 
            var caCerts = chain.ChainElements.OfType<X509ChainElement>().Where(o => !o.Certificate.HasPrivateKey).Select(o => o.Certificate).ToArray();

            if (caCerts == null || caCerts.Length == 0) return null;

            //Assert CA certs in intermediate CA store 
            var intermediateStore = new X509Store(StoreName.CertificateAuthority, StoreLocation.CurrentUser);

            intermediateStore.Open(OpenFlags.ReadWrite);

            foreach (var ca in caCerts)
            {
                if (!intermediateStore.Certificates.Contains(ca))
                    intermediateStore.Add(ca);
            }

            intermediateStore.Close();

            //Return last CA in chain (root CA) 
            return caCerts.LastOrDefault();
        }
    }

    /// <summary>
    /// Objekt som representerar resultatet från en swish betalnings förfrågan
    /// </summary>
    public class SwishPaymentRequestResponse
    {
        public string Error { get; set; }
        public string Location { get; set; }
        public string PaymentRequestToken { get; set; }
    }

    /// <summary>
    /// Objekt som representerar resultatet från en kontrollera swish betalning eller ett callback anrop
    /// </summary>
    public class SwishCheckPaymentRequestStatusResponse
    {
        public string errorCode { get; set; }
        public string errorMessage { get; set; }
        public string id { get; set; }
        public string payeePaymentReference { get; set; }
        public string paymentReference { get; set; }
        public string callbackUrl { get; set; }
        public string payerAlias { get; set; }
        public string payeeAlias { get; set; }
        public double amount { get; set; }
        public string currency { get; set; }
        public string message { get; set; }
        public string status { get; set; }
        public DateTime dateCreated { get; set; }
        public DateTime? datePaid { get; set; }
    }

    public class SwishRefundSatusCheckResponse
    {
        public string errorCode { get; set; }
        public string errorMessage { get; set; }
        public string id { get; set; }
        public string payerPaymentReference { get; set; }
        public string originalPaymentReference { get; set; }
        public string callbackUrl { get; set; }
        public string payerAlias { get; set; }
        public string payeeAlias { get; set; }
        public double amount { get; set; }
        public string currency { get; set; }
        public string message { get; set; }
        public string status { get; set; }
        public DateTime dateCreated { get; set; }
        public DateTime? datePaid { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaBetaltMedSwish.Helpers
{
    public class Config
    {
        public static bool TestMode
        {
            get
            {
                if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("test_mode"))
                {
                    return bool.Parse(System.Configuration.ConfigurationManager.AppSettings["test_mode"]);
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// This setting only use when its stuff to show or being used on tabetaltmedswish.se website
        /// </summary>
        public static bool PublicWebsiteMode
        {
            get
            {
                if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("public_website_mode"))
                {
                    return bool.Parse(System.Configuration.ConfigurationManager.AppSettings["public_website_mode"]);
                }
                else
                {
                    return false;
                }
            }
        }
        

        public class Test
        {
            public static string CertificateFileName
            {
                get
                {
                    if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("test_certificateFileName"))
                    {
                        return System.Configuration.ConfigurationManager.AppSettings["test_certificateFileName"];
                    }
                    else
                    {
                        return "";
                    }
                }
            }
            public static string PaymentRequestURL
            {
                get
                {
                    if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("test_paymentRequestURL"))
                    {
                        return System.Configuration.ConfigurationManager.AppSettings["test_paymentRequestURL"];
                    }
                    else
                    {
                        return "";
                    }
                }
            }
            public static string PaymentRefundURL
            {
                get
                {
                    if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("test_paymentRefundURL"))
                    {
                        return System.Configuration.ConfigurationManager.AppSettings["test_paymentRefundURL"];
                    }
                    else
                    {
                        return "";
                    }
                }
            }
            public static string CertificatePassword
            {
                get
                {
                    if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("test_certificatePassword"))
                    {
                        return System.Configuration.ConfigurationManager.AppSettings["test_certificatePassword"];
                    }
                    else
                    {
                        return "";
                    }
                }
            }
            public static string PayeeAlias
            {
                get
                {
                    if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("test_payeeAlias"))
                    {
                        return System.Configuration.ConfigurationManager.AppSettings["test_payeeAlias"];
                    }
                    else
                    {
                        return "";
                    }
                }
            }
            public static string CallbackURL
            {
                get
                {
                    if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("test_callbackURL"))
                    {
                        return System.Configuration.ConfigurationManager.AppSettings["test_callbackURL"];
                    }
                    else
                    {
                        return "";
                    }
                }
            }
            public static string RefundCallbackURL
            {
                get
                {
                    if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("test_refundCallbackURL"))
                    {
                        return System.Configuration.ConfigurationManager.AppSettings["test_refundCallbackURL"];
                    }
                    else
                    {
                        return "";
                    }
                }
            }
        }

        public class Production
        {
            public static string CertificateFileName
            {
                get
                {
                    if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("production_certificateFileName"))
                    {
                        return System.Configuration.ConfigurationManager.AppSettings["production_certificateFileName"];
                    }
                    else
                    {
                        return "";
                    }
                }
            }
            public static string PaymentRequestURL
            {
                get
                {
                    if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("production_paymentRequestURL"))
                    {
                        return System.Configuration.ConfigurationManager.AppSettings["production_paymentRequestURL"];
                    }
                    else
                    {
                        return "";
                    }
                }
            }
            public static string PaymentRefundURL
            {
                get
                {
                    if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("production_paymentRefundURL"))
                    {
                        return System.Configuration.ConfigurationManager.AppSettings["production_paymentRefundURL"];
                    }
                    else
                    {
                        return "";
                    }
                }
            }
            public static string CertificatePassword
            {
                get
                {
                    if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("production_certificatePassword"))
                    {
                        return System.Configuration.ConfigurationManager.AppSettings["production_certificatePassword"];
                    }
                    else
                    {
                        return "";
                    }
                }
            }
            public static string PayeeAlias
            {
                get
                {
                    if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("production_payeeAlias"))
                    {
                        return System.Configuration.ConfigurationManager.AppSettings["production_payeeAlias"];
                    }
                    else
                    {
                        return "";
                    }
                }
            }
            public static string CallbackURL
            {
                get
                {
                    if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("production_callbackURL"))
                    {
                        return System.Configuration.ConfigurationManager.AppSettings["production_callbackURL"];
                    }
                    else
                    {
                        return "";
                    }
                }
            }
            public static string RefundCallbackURL
            {
                get
                {
                    if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("production_refundCallbackURL"))
                    {
                        return System.Configuration.ConfigurationManager.AppSettings["production_refundCallbackURL"];
                    }
                    else
                    {
                        return "";
                    }
                }
            }
        }
    }
}
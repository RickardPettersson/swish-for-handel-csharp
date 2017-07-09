using NLog;
using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaBetaltMedSwish.Helpers
{
    public class VisitorLogHelper
    {
        // Logger to log to text files in /logs/
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Funktion som loggar till databasen alla anrop som görs för att ha lite statisitk på antal personer som går in på tabetaltmedswish.se
        /// </summary>
        public static void Log()
        {
            try
            {
                // Denna funktion ska endast köras om projektet är satt i publik website läget = körs på tabetaltmedswish.se
                if (Config.PublicWebsiteMode)
                {
                    if (HttpContext.Current != null)
                    {
                        string UrlReferrer = string.Empty;

                        if (HttpContext.Current.Request.UrlReferrer != null)
                        {
                            UrlReferrer = String.IsNullOrWhiteSpace(HttpContext.Current.Request.UrlReferrer.OriginalString) ? "" : HttpContext.Current.Request.UrlReferrer.OriginalString;
                        }

                        string EnterUrl = string.Empty;

                        EnterUrl = String.IsNullOrWhiteSpace(HttpContext.Current.Request.Url.OriginalString) ? "" : HttpContext.Current.Request.Url.OriginalString;

                        using (var db = new Database("db"))
                        {
                            var newLog = new Models.DBVisitorLog()
                            {
                                Url = EnterUrl,
                                UrlReferrer = UrlReferrer,
                                UserAgent = HttpContext.Current.Request.UserAgent,
                                UserHost = HttpContext.Current.Request.UserHostAddress,
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now
                            };

                            db.Insert(newLog);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Helpers/VisitorLogHelper.cs > Log > Exception: " + ex.ToString());
            }
        }
    }
}
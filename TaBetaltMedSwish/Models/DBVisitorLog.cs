using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaBetaltMedSwish.Models
{
    [TableName("visitor_log")]
    [PrimaryKey("visitor_log_id")]
    public class DBVisitorLog
    {
        [Column("visitor_log_id")]
        public int Id { get; set; }

        [Column("url")]
        public string Url { get; set; }

        [Column("url_referrer")]
        public string UrlReferrer { get; set; }

        // HttpContext.Current.Request.UserHostAddress
        [Column("user_host")]
        public string UserHost { get; set; }

        // HttpContext.Current.Request.UserAgent
        [Column("user_agent")]
        public string UserAgent { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebClientApplication.Models
{
    public class AccountSetting
    {
        private const byte CRefreshTimeout = 10;
        //[Key, ForeignKey("ApplicationUser")]
        public int Id { get; set; }
        public virtual ICollection<StockTickerName> StockTickerNames { get; set; }

        public byte RefreshTimeout { get; set; } = CRefreshTimeout;
        public string ApplicationUserId { get; set; }

        //public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
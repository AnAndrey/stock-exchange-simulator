using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebClinetApplication.Models
{
    public class StockTickerName
    {
        [Key]
        public int  Id { get; set; }

        public string Name { get; set; }

        public int AccountSettingId { get; set; }

        public virtual AccountSetting AccountSetting { get; set; }
    }
}

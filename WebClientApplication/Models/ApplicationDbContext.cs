using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace WebClientApplication.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public virtual IDbSet<StockTickerName> StockTickerNames { get; set; }
        public virtual IDbSet<AccountSetting> AccountSettings { get; set; }

    }
}
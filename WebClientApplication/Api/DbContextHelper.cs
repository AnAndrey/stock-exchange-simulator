using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WebClientApplication.Api
{
    public interface IContextDbHelper
    {
        void BulkDelete<T>(DbContext dbContext, IEnumerable<T> entities) where T: class;
        void BulkInsert<T>(DbContext dbContext, IEnumerable<T> entities) where T: class;
    }
    public class DbContextHelper: IContextDbHelper
    {
        public void BulkDelete<T>(DbContext dbContext, IEnumerable<T> entities) where T : class
        {
            dbContext.BulkDelete(entities);
        }

        public void BulkInsert<T>(DbContext dbContext, IEnumerable<T> entities) where T : class
        {
            dbContext.BulkInsert(entities);
        }
    }
}
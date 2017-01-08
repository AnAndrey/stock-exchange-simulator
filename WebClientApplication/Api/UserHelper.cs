using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using Microsoft.AspNet.Identity;

namespace WebClientApplication.Api
{
    public interface IUserHelper
    {
        string GetUserId(IPrincipal identity);
    }
    public class UserHelper: IUserHelper
    {
        public string GetUserId(IPrincipal identity)
        {
            return identity.Identity.GetUserId();
        }
    }
}
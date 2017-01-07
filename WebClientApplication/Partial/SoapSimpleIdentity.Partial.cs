using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonSecurity;

namespace WebClientApplication.StockServiceReference
{
    public partial class SoapSimpleIdentity
    {
        public static SoapSimpleIdentity TheSimplestIdentityEver { get; } = new SoapSimpleIdentity()
        {
            Token = TheSimplestIdentityValidator.CreateTrustFullIdentity().Token

        };
    }
}
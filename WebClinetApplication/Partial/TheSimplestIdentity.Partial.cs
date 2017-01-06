using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonSecurity;

namespace WebClinetApplication.StockServiceReference
{
    public partial class TheSimplestIdentity
    {
        public static TheSimplestIdentity TheSimplestIdentityEver { get; } = new TheSimplestIdentity()
        {
            Token = TheSimplestIdentityValidator.CreateTrustFullIdentity().Token

        };
    }
}
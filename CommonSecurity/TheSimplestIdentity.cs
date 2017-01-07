using System.Web.Services.Protocols;
using CommonSecurityApi;

namespace CommonSecurity
{

    public class TheSimplestIdentity : SoapHeader, IAuthIdentity
    {
        public string Token { get; set; }
    }
}
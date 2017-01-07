using System.Web.Services.Protocols;
using CommonSecurityApi;

namespace WebService
{
    public class SoapSimpleIdentity : SoapHeader, IAuthIdentity
    {
        public SoapSimpleIdentity()
        {
        }

        public SoapSimpleIdentity(IAuthIdentity identity)
        {
            Token = identity.Token;
        }

        public string Token { get; set; }
    }
}
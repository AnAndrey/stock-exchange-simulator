using System.Web.Services.Protocols;

namespace CommonSecurity
{
    public class TheSimplestIdentity : SoapHeader
    {
        public string Token { get; set; }
    }
}
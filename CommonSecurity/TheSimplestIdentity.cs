using System.Web.Services.Protocols;

namespace CommonSecurity
{
    /// <summary>
    /// Container for security token
    /// </summary>
    public class TheSimplestIdentity : SoapHeader
    {
        /// <summary>
        /// Security token
        /// </summary>
        public string Token { get; set; }
    }
}
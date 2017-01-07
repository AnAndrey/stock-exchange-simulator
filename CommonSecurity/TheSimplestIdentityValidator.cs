using System;
using CommonSecurityApi;

namespace CommonSecurity
{

    public class TheSimplestIdentityValidator : IIdentityValidator
    {
        private const string CSecurityIdentificator = "2BFF2891-A984-4273-99E7-03177642793D";

        /// <summary>
        /// Creatres trust full identity
        /// </summary>
        public static TheSimplestIdentity CreateTrustFullIdentity()
        {
            return new TheSimplestIdentity()
            {
                Token = CSecurityIdentificator
            };
        }

        public virtual bool CanWeTrustTo(IAuthIdentity identity)
        {
            return CSecurityIdentificator.Equals(identity.Token, StringComparison.Ordinal);
        }
    }
}
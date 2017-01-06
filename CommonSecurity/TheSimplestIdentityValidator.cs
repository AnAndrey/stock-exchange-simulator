using System;

namespace CommonSecurity
{
    public partial class TheSimplestIdentityValidator
    {
        private const string CSecurityIdentificator = "2BFF2891-A984-4273-99E7-03177642793D";

        public static TheSimplestIdentity CreateTrustFullIdentity()
        {
            return new TheSimplestIdentity()
            {
                Token = CSecurityIdentificator
            };
        }

        public virtual bool CanWeTrustTo(TheSimplestIdentity identity)
        {
            return CSecurityIdentificator.Equals(identity.Token, StringComparison.Ordinal);
        }
    }
}
using System;

namespace CommonSecurity
{
    /// <summary>
    /// Provides security checking
    /// </summary>
    public partial class TheSimplestIdentityValidator
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

        /// <summary>
        /// Validates identity
        /// </summary>
        public virtual bool CanWeTrustTo(TheSimplestIdentity identity)
        {
            return CSecurityIdentificator.Equals(identity.Token, StringComparison.Ordinal);
        }
    }
}
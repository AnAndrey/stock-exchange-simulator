namespace CommonSecurityApi
{

    /// <summary>
    /// Provides security checking
    /// </summary>
    public interface IIdentityValidator
    {
        /// <summary>
        /// Validates identity
        /// </summary>
        bool CanWeTrustTo(IAuthIdentity identity);
    }
}
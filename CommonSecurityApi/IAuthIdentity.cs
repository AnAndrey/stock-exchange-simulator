namespace CommonSecurityApi
{
    /// <summary>
    /// Container for security token
    /// </summary>
    public interface IAuthIdentity
    {
        /// <summary>
        /// Security token
        /// </summary>
        string Token { get; set; }
    }
}
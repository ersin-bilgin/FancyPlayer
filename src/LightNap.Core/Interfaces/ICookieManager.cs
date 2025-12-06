namespace LightNap.Core.Interfaces
{
    /// <summary>  
    /// Provides methods to manage cookies.  
    /// </summary>  
    public interface ICookieManager
    {
        void SetCookie(string key, string value, bool isPersistent, DateTime expires);
        string? GetCookie(string key);
        void RemoveCookie(string key);
    }

}
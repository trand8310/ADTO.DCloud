namespace ADTOSharp.Web.Security.AntiForgery
{
    /// <summary>
    /// This interface is internally used by ADTO framework and normally should not be used by applications.
    /// If it's needed, use 
    /// <see cref="IADTOSharpAntiForgeryManager"/> and cast to 
    /// <see cref="IADTOSharpAntiForgeryValidator"/> to use 
    /// <see cref="IsValid"/> method.
    /// </summary>
    public interface IADTOSharpAntiForgeryValidator
    {
        bool IsValid(string cookieValue, string tokenValue);
    }
}
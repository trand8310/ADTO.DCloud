namespace ADTOSharp.Web.Security.AntiForgery
{
    public interface IADTOSharpAntiForgeryManager
    {
        IADTOSharpAntiForgeryConfiguration Configuration { get; }

        string GenerateToken();
    }
}

namespace ADTOSharp.AspNetCore;

public class ADTOSharpApplicationBuilderOptions
{
    /// <summary>
    /// Default: true.
    /// </summary>
    public bool UseCastleLoggerFactory { get; set; }

    /// <summary>
    /// Default: true.
    /// </summary>
    public bool UseADTOSharpRequestLocalization { get; set; }

    /// <summary>
    /// Default: true.
    /// </summary>
    public bool UseSecurityHeaders { get; set; }

    public ADTOSharpApplicationBuilderOptions()
    {
        UseCastleLoggerFactory = true;
        UseADTOSharpRequestLocalization = true;
        UseSecurityHeaders = true;
    }
}
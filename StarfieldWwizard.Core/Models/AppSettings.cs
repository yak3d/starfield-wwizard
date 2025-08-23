namespace StarfieldWwizard.Core.Models;

public class AppSettings
{
    public string? StarfieldDataDirectory
    {
        get; set;
    }

    public string? AppBackgroundRequestedTheme
    {
        get; set;
    }

    public bool EnableVgmStreamDownload { get; set; } = true;

    public bool EnableFfmpegDownload { get; set; } = true;
}
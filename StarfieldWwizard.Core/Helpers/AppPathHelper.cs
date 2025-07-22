namespace StarfieldWwizard.Helpers;

public class AppPathHelper
{
    public static readonly string DefaultApplicationDataFolder = "StarfieldWwizard/ApplicationData";
    public static readonly string DefaultLogsDataFolder = "StarfieldWwizard/Logs";
    private static readonly string _localAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

    public static readonly string ApplicationDataFolder =
        Path.Combine(_localAppDataFolder, DefaultApplicationDataFolder);
    public static readonly string LogsDataFolder = Path.Combine(_localAppDataFolder, DefaultLogsDataFolder);
}
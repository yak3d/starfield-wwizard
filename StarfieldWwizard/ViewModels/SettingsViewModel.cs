using System.Reflection;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Serilog;
using StarfieldWwizard.Contracts.Services;
using StarfieldWwizard.Helpers;
using Windows.ApplicationModel;

namespace StarfieldWwizard.ViewModels;

public partial class SettingsViewModel : ObservableRecipient
{
    private readonly IThemeSelectorService _themeSelectorService;
    private readonly IStarfieldDataDirectoryService _starfieldDataDirectoryService;

    [ObservableProperty]
    public partial ElementTheme ElementTheme
    {
        get;
        set;
    }

    [ObservableProperty]
    public partial string VersionDescription
    {
        get;
        set;
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SetStarfieldDataDirectoryCommand))]
    public partial string StarfieldDataDirectory
    {
        get;
        set;
    }

    [ObservableProperty]
    public partial bool StarfieldDataDirectoryErrorIsOpen
    {
        get;
        set;
    }

    [ObservableProperty]
    public partial string StarfieldDataDirectoryErrorType
    {
        get;
        set;
    }

    [ObservableProperty]
    public partial Brush StarfieldDataDirectoryTextboxColor
    {
        get;
        set;
    }

    [ObservableProperty]
    public partial string StarfieldDataDirectoryErrorMessage
    {
        get;
        set;
    }

    partial void OnStarfieldDataDirectoryChanged(string value)
    {
        SetStarfieldDataDirectoryCommand.Execute(value);
    }

    partial void OnStarfieldDataDirectoryChanging(string value)
    {
        Log.Information(value);
    }

    partial void OnElementThemeChanged(ElementTheme value)
    {
        Log.Information(value.ToString());
    }


    public ICommand SwitchThemeCommand
    {
        get;
    }

    [RelayCommand]
    public async Task SetStarfieldDataDirectory()
    {
        StarfieldDataDirectoryErrorIsOpen = false;
        if (!string.IsNullOrEmpty(StarfieldDataDirectory))
        {
            try
            {
                await _starfieldDataDirectoryService.SetDataDirectoryAsync(StarfieldDataDirectory);
            }
            catch (FileNotFoundException ex)
            {
                Log.Error("{0}", ex);
                StarfieldDataDirectoryErrorIsOpen = true;
                StarfieldDataDirectoryErrorType = "Error Setting Data Directory";
                StarfieldDataDirectoryErrorMessage = "The directory below does not exist.";
            }
        }
    }

    public SettingsViewModel(
        IThemeSelectorService themeSelectorService,
        IStarfieldDataDirectoryService starfieldDataDirectoryService)
    {
        _themeSelectorService = themeSelectorService;
        _starfieldDataDirectoryService = starfieldDataDirectoryService;
        ElementTheme = _themeSelectorService.Theme;
        StarfieldDataDirectory = _starfieldDataDirectoryService.StarfieldDataDirectory;
        StarfieldDataDirectoryErrorIsOpen = false;
        StarfieldDataDirectoryErrorType = string.Empty;
        StarfieldDataDirectoryErrorMessage = string.Empty;
        VersionDescription = GetVersionDescription();

        SwitchThemeCommand = new RelayCommand<ElementTheme>(
            async (param) =>
            {
                if (ElementTheme != param)
                {
                    ElementTheme = param;
                    await _themeSelectorService.SetThemeAsync(param);
                }
            });

        // SetStarfieldDataDirectoryCommand = new TextChangedEventHandler(
        //     async (param) =>
        //     {
        //     });
    }



    private static string GetVersionDescription()
    {
        Version version;

        if (RuntimeHelper.IsMSIX)
        {
            var packageVersion = Package.Current.Id.Version;

            version = new(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
        }
        else
        {
            version = Assembly.GetExecutingAssembly().GetName().Version!;
        }

        return $"{"AppDisplayName".GetLocalized()} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    }
}

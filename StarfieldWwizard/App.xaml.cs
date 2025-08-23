using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Serilog;
using StarfieldWwizard.Activation;
using StarfieldWwizard.Contracts.Services;
using StarfieldWwizard.Core.Contracts.Services;
using StarfieldWwizard.Core.Services;
using StarfieldWwizard.Helpers;
using StarfieldWwizard.Models;
using StarfieldWwizard.Services;
using StarfieldWwizard.ViewModels;
using StarfieldWwizard.Views;

namespace StarfieldWwizard;

// To learn more about WinUI 3, see https://docs.microsoft.com/windows/apps/winui/winui3/.
public partial class App : Application
{
    // The .NET Generic Host provides dependency injection, configuration, logging, and other services.
    // https://docs.microsoft.com/dotnet/core/extensions/generic-host
    // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
    // https://docs.microsoft.com/dotnet/core/extensions/configuration
    // https://docs.microsoft.com/dotnet/core/extensions/logging
    public IHost Host
    {
        get;
    }

    public static T GetService<T>()
        where T : class
    {
        if ((App.Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }

        return service;
    }

    public static WindowEx MainWindow { get; } = new MainWindow();

    public static UIElement? AppTitlebar
    {
        get; set;
    }

    public App()
    {
        InitializeComponent();

        SetupSerilog();

        Host = Microsoft.Extensions.Hosting.Host.
        CreateDefaultBuilder().
        UseContentRoot(AppContext.BaseDirectory).
        ConfigureServices((context, services) =>
        {
            // Default Activation Handler
            services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

            // Other Activation Handlers

            // Services
            services.AddSingleton<ILocalSettingsService, LocalSettingsService>();
            services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
            services.AddSingleton<IStarfieldDataDirectoryService, StarfieldDataDirectoryService>();
            services.AddTransient<INavigationViewService, NavigationViewService>();

            services.AddSingleton<IActivationService, ActivationService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<INavigationService, NavigationService>();

            // Core Services
            services.AddSingleton<ISampleDataService, SampleDataService>();
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<IArchiveService, BSArchiveService>();
            services.AddSingleton<IWwiseSoundbankService, WwiseSoundbankService>();
            services.AddSingleton<IFfmpegDependencyService, FfMpegDependencyService>();
            services.AddSingleton<IVgmStreamService, VgmStreamService>();

            // Views and ViewModels
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<SettingsPage>();
            services.AddTransient<ListDetailsViewModel>();
            services.AddTransient<ListDetailsPage>();
            services.AddTransient<ShellPage>();
            services.AddTransient<ShellViewModel>();

            // Configuration
            services.Configure<LocalSettingsOptions>(context.Configuration.GetSection(nameof(LocalSettingsOptions)));
        }).
        Build();

        UnhandledException += App_UnhandledException;
    }

    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        Log.Fatal(e.Exception, "Unhandled exception occurred");

        // Try to show error in console/debug output since UI might not be available
        System.Diagnostics.Debug.WriteLine($"Unhandled Exception: {e.Exception}");
        Console.WriteLine($"Unhandled Exception: {e.Exception}");
    }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);

        try
        {
            // await App.GetService<IFfmpegDependencyService>().DownloadFfMpegIfNotExists();
            await App.GetService<IArchiveService>().InitializeAsync();
            await App.GetService<IActivationService>().ActivateAsync(args);

            await App.GetService<IVgmStreamService>().DownloadVgmStreamIfNotExists();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Failed to launch application");

            try
            {
                await App.GetService<IActivationService>().ActivateAsync(args);
            }
            catch
            {
                // Last resort: just activate the main window
                MainWindow.Activate();
            }
        }
    }

    private void SetupSerilog()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(
                Path.Combine(AppPathHelper.DefaultLogsDataFolder,
                    $"starfieldwwizard.log"
                ),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 5,
                rollOnFileSizeLimit: true,
                fileSizeLimitBytes: 524288000
            )
            .CreateLogger();
    }
}

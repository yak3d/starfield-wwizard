using System.Linq.Expressions;
using Microsoft.Extensions.Options;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Starfield;
using StarfieldWwizard.Contracts.Services;
using StarfieldWwizard.Core.Contracts.Services;
using StarfieldWwizard.Core.Helpers;
using StarfieldWwizard.Core.Models;
using StarfieldWwizard.Helpers;
using StarfieldWwizard.Models;
using Windows.ApplicationModel;
using Windows.Storage;

namespace StarfieldWwizard.Services;

public class LocalSettingsService : ILocalSettingsService
{
    private const string _defaultLocalSettingsFile = "LocalSettings.json";

    private readonly IFileService _fileService;
    private readonly LocalSettingsOptions _options;

    private readonly string _localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    public readonly string ApplicationDataFolder;
    private readonly string _localsettingsFile;

    private AppSettings _settings = new();

    private bool _isInitialized;

    public LocalSettingsService(IFileService fileService, IOptions<LocalSettingsOptions> options)
    {
        _fileService = fileService;
        _options = options.Value;

        ApplicationDataFolder = AppPathHelper.ApplicationDataFolder;
        _localsettingsFile = _options.LocalSettingsFile ?? _defaultLocalSettingsFile;
    }

    private async Task InitializeAsync()
    {
        if (!_isInitialized)
        {
            _settings = await Task.Run(() => _fileService.Read<AppSettings>(ApplicationDataFolder, _localsettingsFile));

            if (_settings == null)
            {
                _settings = new AppSettings();
                await CreateDefaultSettingsAsync();
            }

            _isInitialized = true;
        }
    }

    private async Task CreateDefaultSettingsAsync()
    {
        using (var env = GameEnvironment.Typical.Starfield(StarfieldRelease.Starfield))
        {
            _settings.StarfieldDataDirectory = env.DataFolderPath;
        }

        await Task.Run(() => _fileService.Save(ApplicationDataFolder, _localsettingsFile, _settings));
    }


    public async Task<AppSettings> GetSettingsAsync()
    {
        await InitializeAsync();
        return _settings;
    }

    public async Task SaveSettingsAsync(AppSettings settings)
    {
        await InitializeAsync();
        _settings = settings;
        await Task.Run(() => _fileService.Save(ApplicationDataFolder, _localsettingsFile, _settings));
    }

    public async Task<T?> GetSettingAsync<T>(Expression<Func<AppSettings, T>> selector)
    {
        await InitializeAsync();
        var compiled = selector.Compile();
        return compiled(_settings);
    }

    public async Task UpdateSettingAsync<T>(Expression<Func<AppSettings, T>> selector, T value)
    {
        await InitializeAsync();

        if (selector.Body is MemberExpression memberExpression)
        {
            var property = memberExpression.Member as System.Reflection.PropertyInfo;
            if (property != null && property.CanWrite)
            {
                property.SetValue(_settings, value);
                await Task.Run(() => _fileService.Save(ApplicationDataFolder, _localsettingsFile, _settings));
            }
        }
    }
}

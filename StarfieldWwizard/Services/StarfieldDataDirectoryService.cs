﻿using StarfieldWwizard.Contracts.Services;
using StarfieldWwizard.Core.Contracts.Services;

namespace StarfieldWwizard.Services;

public class StarfieldDataDirectoryService : IStarfieldDataDirectoryService
{
    private const string SettingsKey = "StarfieldDataDirectory";

    public string StarfieldDataDirectory
    {
        get;
        set;
    } = "E:\\SteamLibrary\\steamapps\\common\\Starfield\\Data";
    private readonly ILocalSettingsService _localSettingsService;

    public StarfieldDataDirectoryService(ILocalSettingsService localSettingsService, IArchiveService archiveService)
    {
        _localSettingsService = localSettingsService;
        archiveService.ToString();
    }

    public async Task InitializeAsync()
    {
        StarfieldDataDirectory = await LoadFromSettingsAsync();
        await Task.CompletedTask;
    }

    public async Task SetDataDirectoryAsync(string path)
    {
        StarfieldDataDirectory = path;

        await SaveStarfieldDataDirectoryAsync(StarfieldDataDirectory);
    }

    private async Task SaveStarfieldDataDirectoryAsync(string starfieldDataDirectory)
    {
        var dirInfo = new DirectoryInfo(starfieldDataDirectory);
        if (dirInfo.Exists)
        {
            await _localSettingsService.SaveSettingAsync(SettingsKey, starfieldDataDirectory);
        }
        else
        {
            throw new FileNotFoundException($"The directory specified by {starfieldDataDirectory} does not exist.");
        }
    }

    private async Task<string> LoadFromSettingsAsync()
    {
        var path = await _localSettingsService.ReadSettingAsync<string>(SettingsKey);

        if (Path.Exists(path))
        {
            return path;
        }
        
        throw new FileNotFoundException($"The directory specified by {path} does not exist.");
    }
}
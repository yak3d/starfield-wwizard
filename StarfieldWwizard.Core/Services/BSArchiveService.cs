using System.Collections;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Archives;
using Noggog;
using Serilog;
using StarfieldWwizard.Contracts.Services;
using StarfieldWwizard.Core.Contracts.Services;
using StarfieldWwizard.Core.Models;

namespace StarfieldWwizard.Core.Services;

public class BSArchiveService(ILocalSettingsService settingsService) : IArchiveService
{
    private readonly GameRelease _gameRelease = GameRelease.Starfield;
    private string GameDataDir
    {
        get; set;
    }
    private IEnumerable<FilePath> ArchivePaths
    {
        get;
        set;
    }

    private IEnumerable<FilePath> GetAllArchives() => Archive.GetApplicableArchivePaths(GameRelease.Starfield, GameDataDir);

    public IEnumerable<DataFile> GetContentByFilename(string filename) => ArchivePaths
        .Where(archive => archive.NameWithoutExtension.Contains("WwiseSounds", StringComparison.InvariantCultureIgnoreCase))
        .SelectMany(archive => Archive.CreateReader(_gameRelease, archive).Files.
            Where(file => Path.GetFileName(file.Path).Equals(filename, StringComparison.InvariantCultureIgnoreCase)))
        .Select(archiveFile => new DataFile
        {
            Filename = filename,
            Path = archiveFile.Path,
            Data = archiveFile.GetBytes()
        });

    public IEnumerable<DataFile> GetContentByFileInDirectory(
        string filename,
        string path
    ) => ArchivePaths.Where(archivePath => archivePath.Path.Contains(path))
        // .Where(archiveFile => Path.GetFileName(archiveFile.Path)
        //     .Equals(filename, StringComparison.InvariantCultureIgnoreCase))
        .SelectMany(filePath => Archive.CreateReader(_gameRelease, filePath).Files)
        .Where(file => Path.GetFileName(file.Path).Equals(filename, StringComparison.InvariantCultureIgnoreCase))
        .Select(archiveFile => new DataFile
        {
            Filename = Path.GetFileName(archiveFile.Path),
            Path = archiveFile.Path,
            Data = archiveFile.GetBytes()
        });

    public async Task InitializeAsync()
    {
        GameDataDir = await settingsService.GetSettingAsync(s => s.StarfieldDataDirectory) ?? string.Empty;
        ArchivePaths = GetAllArchives();
        Archive.GetApplicableArchivePaths(_gameRelease, GameDataDir).ForEach(files =>
        {
            Log.Information(files);
        });
    }
}
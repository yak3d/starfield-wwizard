using Noggog;
using Serilog;
using StarfieldWwizard.Core.Contracts.Services;
using StarfieldWwizard.Core.Helpers;
using StarfieldWwizard.Core.Models;

namespace StarfieldWwizard.Core.Services;

public class WwiseSoundbankService(IArchiveService archiveService) : IWwiseSoundbankService
{
    public async Task<IEnumerable<WwiseStreamedFile>> GetWwiseStreamedFilesAsync()
    {
        var files = archiveService.GetContentByFileInDirectory("soundbanksinfo.json",
            "Starfield\\Data\\Starfield - WwiseSoundsPatch.ba2");

        var soundBanksInfoWrapper = await Json.BytesToObject<SoundBanksInfoJsonWrapper>(files.First().Data);
        return soundBanksInfoWrapper.SoundBanksInfo.StreamedFiles;
    }

    public Task<string> GetSfxFromArchivesAsWav(int soundId)
    {
        var filename = $"{soundId}.wem";
        var soundFromArchive = archiveService.GetContentByFilename(filename);

        var fromArchive = soundFromArchive.ToList();
        if (fromArchive.Count() > 1)
        {
            Log.Warning(
                "Looked in archives for sound file {0} but found {1} versions of the same file. Will grab first record to play",
                filename,
                fromArchive.Count()
            );
        } else if (!fromArchive.Any())
        {
            throw new FileNotFoundException($"Unable to find sound file with name {filename}");
        }

        var wem = fromArchive.First();
        return Task.FromResult(WemConverter.Wem2Wav(wem.Data));
    }
}
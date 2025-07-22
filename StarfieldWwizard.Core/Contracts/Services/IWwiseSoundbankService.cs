using StarfieldWwizard.Core.Models;

namespace StarfieldWwizard.Core.Contracts.Services;

public interface IWwiseSoundbankService
{
    Task<IEnumerable<WwiseStreamedFile>> GetWwiseStreamedFilesAsync();
    Task<string> GetSfxFromArchivesAsWav(int soundId);
}
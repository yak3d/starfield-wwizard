using StarfieldWwizard.Core.Models;

namespace StarfieldWwizard.Core.Contracts.Services;

public interface IArchiveService
{
    IEnumerable<DataFile> GetContentByFilename(string filename);
    IEnumerable<DataFile> GetContentByFileInDirectory(string filename, string path);

    Task InitializeAsync();
}
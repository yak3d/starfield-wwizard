namespace StarfieldWwizard.Core.Contracts.Services;

public interface IVgmStreamService
{
    public bool VgmStreamOnPath();
    public bool VgmStreamInBaseDirectory();
    public Task DownloadVgmStreamIfNotExists();
}
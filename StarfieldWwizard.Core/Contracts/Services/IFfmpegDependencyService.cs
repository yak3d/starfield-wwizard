namespace StarfieldWwizard.Core.Contracts.Services;

public interface IFfmpegDependencyService
{
    public bool FfmpegOnPath();
    public bool FfmpegInBaseDirectory();
    public Task DownloadFfMpegIfNotExists();
}
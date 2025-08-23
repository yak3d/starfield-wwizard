namespace StarfieldWwizard.Core.Models;

public class SoundBanksInfoJsonWrapper
{
    public SoundBanksInfo SoundBanksInfo
    {
        get; init;
    }
}
public class SoundBanksInfo
{
    public IEnumerable<WwiseStreamedFile> StreamedFiles
    {
        get;
        init;
    }
}
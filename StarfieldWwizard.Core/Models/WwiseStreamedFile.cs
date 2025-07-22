using StarfieldWwizard.Core.Helpers;

namespace StarfieldWwizard.Core.Models;

public class WwiseStreamedFile
{
    public int Id { get; init; }
    public string Language { get; init; }
    public string ShortName { get;
        init;
    }

    public string SfxName => Path.GetFileNameWithoutExtension(this.ShortName);
    public string SfxType => SfxName.CaptureUntil('_');

    public override string ToString() => $"{Id} | {Language} | {SfxName}";
}
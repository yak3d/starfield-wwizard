using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using StarfieldWwizard.Core.Models;

namespace StarfieldWwizard.Models;

public partial class SoundEffect : ObservableObject
{

    public int Id
    {
        get;
        init;
    }

    public string Language
    {
        get;
        init;
    }

    public string ShortName
    {
        get;
        init;
    }

    // public partial object _buttonContent = new FontIcon { Glyph = "\uF5b0" };

    [ObservableProperty]
    public partial object? ButtonContent
    {
        get;
        set;
    // }
    } = new FontIcon() { Glyph = "\uF5B0" };

    public string SfxName => Path.GetFileNameWithoutExtension(this.ShortName);

    public override string ToString() => $"{Id} | {Language} | {SfxName}";
}

public static class SoundEffectExtensions
{
    public static SoundEffect FromWwiseStreamedFile(this WwiseStreamedFile file) => new SoundEffect
    {
        Id = file.Id,
        Language = file.Language,
        ShortName = file.ShortName,
    };
}
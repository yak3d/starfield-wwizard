using System.Collections.ObjectModel;
using System.Media;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Mutagen.Bethesda;
using Noggog;
using Serilog;
using StarfieldWwizard.Contracts.ViewModels;
using StarfieldWwizard.Core.Contracts.Services;
using StarfieldWwizard.Core.Helpers;
using StarfieldWwizard.Core.Models;
using StarfieldWwizard.Models;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;

namespace StarfieldWwizard.ViewModels;

public partial class ListDetailsViewModel : ObservableRecipient, INavigationAware
{
    private readonly IWwiseSoundbankService _wwiseSoundbankService;

    [ObservableProperty]
    public partial SampleOrder? Selected
    {
        get;
        set;
    }

    [ObservableProperty]
    public partial string SearchText
    {
        get;
        set;
    }

    [ObservableProperty]
    public partial IMediaPlaybackSource? PlaybackSource { get; set; } = null;

    [ObservableProperty]
    public partial MediaPlayer Player { get; set; } = new();
    public ICommand PlaySfxCommand
    {
        get;
    }

    public ObservableCollection<SampleOrder> SampleItems { get; private set; } = new();
    public List<SoundEffect> SfxFiles { get; private set; } = new();
    public ObservableCollection<SoundEffect> VisibleSfxFiles { get; private set; } = new();

    public ListDetailsViewModel(IWwiseSoundbankService wwiseSoundbankService)
    {
        _wwiseSoundbankService = wwiseSoundbankService;

        PlaySfxCommand = new RelayCommand<SoundEffect>(
            async void (param) =>
            {
                try
                {
                    // do sound
                    if (param == null)
                    {
                        return;
                    }

                    param.ButtonContent = new ProgressRing
                    {
                        IsActive = true
                    };
                    var pathToWav = await wwiseSoundbankService.GetSfxFromArchivesAsWav(param.Id);
                    var wavFile = await StorageFile.GetFileFromPathAsync(pathToWav);
                    param.ButtonContent = new FontIcon()
                    {
                        Glyph = "\uE71A",
                    };

                    var wavSource = MediaSource.CreateFromStorageFile(wavFile);
                    var wavMediaItem = new MediaPlaybackItem(wavSource);
                    var displayProps = wavMediaItem.GetDisplayProperties();
                    displayProps.Type = MediaPlaybackType.Music;
                    displayProps.MusicProperties.Title = param.SfxName;
                    Player.Source = wavMediaItem;
                    Player.Play();
                    //var soundPlayer = new SoundPlayer(pathToWav);
                    //soundPlayer.Play();

                    param.ButtonContent = new FontIcon()
                    {
                        Glyph = "\uF5B0",
                    };
                }
                catch (Exception e)
                {
                    Log.Error(e, "Failed to play sound");
                }
            });
    }

    partial void OnSearchTextChanged(string value)
    {
        VisibleSfxFiles.Clear();
        VisibleSfxFiles.AddRange(
            SfxFiles.Where(
                sfx => sfx.SfxName.Contains(value, StringComparison.InvariantCultureIgnoreCase)
                )
            );
    }

    public async void OnNavigatedTo(object parameter)
    {
        var data2 = await _wwiseSoundbankService.GetWwiseStreamedFilesAsync();

        foreach (var item in data2)
        {
            var soundEffect = item.FromWwiseStreamedFile();
            SfxFiles.Add(soundEffect);
            VisibleSfxFiles.Add(soundEffect);
        }
    }

    public void OnNavigatedFrom()
    {
    }

    public void EnsureItemSelected()
    {
        // Selected ??= SampleItems.First();
    }
}

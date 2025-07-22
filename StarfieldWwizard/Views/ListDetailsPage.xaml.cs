
using Microsoft.UI.Xaml.Controls;

using StarfieldWwizard.ViewModels;

namespace StarfieldWwizard.Views;

public sealed partial class ListDetailsPage : Page
{
    public ListDetailsViewModel ViewModel
    {
        get;
    }

    public ListDetailsPage()
    {
        ViewModel = App.GetService<ListDetailsViewModel>();
        InitializeComponent();
        SfxMpe.SetMediaPlayer(ViewModel.Player);
    }
    
    



    // private void OnViewStateChanged(object sender, ListDetailsViewState e)
    // {
    //     if (e == ListDetailsViewState.Both)
    //     {
    //         ViewModel.EnsureItemSelected();
    //     }
    // }
}

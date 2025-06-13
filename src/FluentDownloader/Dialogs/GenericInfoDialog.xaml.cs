using Microsoft.UI.Xaml.Controls;
using System.Windows.Input;

namespace FluentDownloader.Dialogs
{
    /// <summary>
    /// Generic dialog to show info about ffmpeg or ytdlp. Uses <see cref="GenericInfoDialogViewModel"/> as data context
    /// </summary>
    public sealed partial class GenericInfoDialog : Page
    {
        public GenericInfoDialog()
        {
            this.InitializeComponent();
        }
    }

    /// <summary>
    /// Helper view model class to provide <see cref="GenericInfoDialog"/> data context
    /// </summary>
    public class GenericInfoDialogViewModel
    {
        public string VersionText { get; set; }
        public string PathText { get; set; }
        public string ButtonText { get; set; }
        public string GithubLink { get; set; }
        public string GithubLinkText { get; set; }
        public ICommand ButtonCommand { get; set; }
        public bool IsButtonVisible { get; set; }

        public GenericInfoDialogViewModel(
            string versionText,
            string descriptionText,
            string buttonText,
            ICommand buttonCommand,
            bool isButtonVisible,
            string githubLink,
            string githubLinkText
            )
        {
            VersionText = versionText;
            PathText = descriptionText;
            ButtonText = buttonText;
            ButtonCommand = buttonCommand;
            IsButtonVisible = isButtonVisible;
            GithubLink = githubLink;
            GithubLinkText = githubLinkText;
        }
    }

}
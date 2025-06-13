using FluentDownloader.Models;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.ApplicationModel.Resources;
using System.Threading.Tasks;

namespace FluentDownloader.Dialogs
{
    public interface IDialogService
    {
        Task<bool> ShowInstallDialogAsync(string? title, object content);

        Task ShowNotificationDialogAsync(string? title, string content);

        Task ShowNotificationDialogAsync(string? title, object content);

        void AddPopUpNotification(string title, string message, InfoBarSeverity severity);

        Task<bool> PromptUserAsync(string? title, string content, string acceptOption, string rejectOption);
    }
}
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Text;

namespace FluentDownloader.Pages
{
    public sealed partial class UnhandledExceptionPage : Page
    {
        public UnhandledExceptionPage()
        {
            this.InitializeComponent();
        }

        public UnhandledExceptionPage(Exception ex)
        {
            this.InitializeComponent();
            FormatException(ex);
        }

        private string FormatException(Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            FormatExceptionRecursive(ex, sb, 0);
            ContentTextBlock.Text = sb.ToString();
            return sb.ToString();
        }

        private void FormatExceptionRecursive(Exception ex, StringBuilder sb, int level)
        {
            try
            {
                string indent = new string(' ', level * 4);
                sb.AppendLine($"{indent}Тип исключения: {ex.GetType()}");
                sb.AppendLine($"{indent}Сообщение: {ex.Message}");
                sb.AppendLine($"{indent}Источник: {ex.Source}");
                if (!string.IsNullOrEmpty(ex.HelpLink))
                {
                    sb.AppendLine($"{indent}HelpLink: {ex.HelpLink}");
                }
                if (ex.Data != null && ex.Data.Count > 0)
                {
                    sb.AppendLine($"{indent}Данные:");
                    foreach (var key in ex.Data.Keys)
                    {
                        sb.AppendLine($"{indent}  {key}: {ex.Data[key]}");
                    }
                }
                sb.AppendLine($"{indent}StackTrace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    sb.AppendLine($"{indent}Вложенное исключение:");
                    FormatExceptionRecursive(ex.InnerException, sb, level + 1);
                }
            }
            catch
            {
                return;
            }

        }
    }
}

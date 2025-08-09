using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentDownloader.Models
{
    public partial class QueueItem : ObservableObject
    {
        public string Title { get; set; } = null!;
        public string Size { get; set; } = null!;
        private VideoInQueueStatus _status;
        public VideoInQueueStatus Status
        {
            get => _status;
            set
            {
                SetProperty(ref _status, value);
                OnPropertyChanged(StatusDisplay);
            }
        }
        public string StatusDisplay => Status.GetLocalizedDisplayName();
        public string Resolution { get; set; } = null!;
    }
}

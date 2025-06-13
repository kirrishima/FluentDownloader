using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentDownloader
{
    public interface IProgressBar
    {
        /// <summary>
        /// Used to update progress bar
        /// </summary>
        /// <param name="progress"></param>
        void UpdateInstallProgress(int progress);

        void SetProgressBarPaused(bool paused);
        void SetProgressBarError(bool error);
        void SetProgressBarRunning(bool running);
    }
}

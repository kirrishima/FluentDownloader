using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentDownloader.Helpers
{
    /// <summary>
    /// Tracks the time elapsed since the last property update and logs the duration.
    /// </summary>
    /// <typeparam name="T">The type of the property being updated.</typeparam>
    public class TimedPropertyUpdater<T>
    {
        private DateTime _lastUpdateTime;

        /// <summary>
        /// Initializes a new instance of <see cref="TimedPropertyUpdater{T}"/> with an optional initial value.
        /// </summary>
        /// <param name="initialValue">The initial value of the property (default is null or default for value types).</param>
        public TimedPropertyUpdater(T? initialValue = default)
        {
            _lastUpdateTime = DateTime.Now;
            Debug.WriteLine($"[{initialValue} created at {_lastUpdateTime:HH}]");
        }

        /// <summary>
        /// Updates the property and logs the elapsed time since the last update.
        /// </summary>
        /// <param name="newValue">The new value of the property.</param>
        /// <returns>The time elapsed since the last update.</returns>
        public TimeSpan LogStep(T newValue)
        {
            var elapsed = DateTime.Now - _lastUpdateTime;

            Debug.WriteLine($"[{newValue}] Time since last update: {elapsed.TotalMilliseconds} ms");

            _lastUpdateTime = DateTime.Now;

            return elapsed;
        }
    }
}

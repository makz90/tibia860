﻿using System;

namespace OpenTibia.Threading
{
    public class SchedulerEvent : DispatcherEvent, IComparable<SchedulerEvent>
    {
        public SchedulerEvent(int executeInMilliseconds, Action execute) : base(execute)
        {
            this.timeout = DateTime.UtcNow.AddMilliseconds( Math.Max(50, executeInMilliseconds) );
        }

        private DateTime timeout;

        public TimeSpan Timeout
        {
            get
            {
                DateTime now = DateTime.UtcNow;

                if (timeout > now)
                {
                    return timeout.Subtract(now);
                }

                return TimeSpan.Zero;
            }
        }

        public int CompareTo(SchedulerEvent scheduledEvent)
        {
            return timeout.CompareTo(scheduledEvent.timeout);
        }
    }
}
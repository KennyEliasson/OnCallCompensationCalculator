using System;

namespace Woot
{
    public class OnCallSession
    {
        public OnCallSession(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }

        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public OnCallCalculation CalculateOnCall()
        {
            var timespan = End - Start;
            var oncall = new OnCallCalculation();
            for (var i = 0; i < timespan.TotalHours; i++)
            {
                var current = Start.AddHours(i);
                oncall.DetermineOnCallType(current);
            }

            return oncall;
        }
        
        internal double TotalHours()
        {
            return (End - Start).TotalHours;
        }
    }
}

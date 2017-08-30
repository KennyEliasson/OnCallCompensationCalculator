using System.Collections.Generic;
using Ical.Net.Interfaces.DataTypes;
using System.Linq;

namespace Woot
{
    public class Employee
    {
        public Employee(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public List<OnCallSession> Sessions { get; set; } = new List<OnCallSession>();

        public void AddOnCall(IDateTime evStart, IDateTime evEnd)
        {
            Sessions.Add(new OnCallSession(evStart.AsSystemLocal, evEnd.AsSystemLocal));
        }

        public double TotalHours()
        {
            return Sessions.Sum(x => x.TotalHours());
        }

        public OnCallCalculation CalculateOnCall()
        {
            var totalOnCall = new OnCallCalculation();
            foreach (var session in Sessions)
            {
                totalOnCall.Add(session.CalculateOnCall());
            }

            return totalOnCall;
        }
    }
}

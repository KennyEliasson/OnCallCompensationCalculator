using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Ical.Net;

namespace Woot
{
    public class OpsGenieCalendarParser
    {
        public Dictionary<string, Employee> Parse(string fileName)
        {
            var calendarCollection = Calendar.LoadFromFile(fileName);
            var firstCalendar = calendarCollection.First();

            var regex = new Regex(@"([A-zé ]*) \(user\) is on call");
            var employees = new Dictionary<string, Employee>();

            var firstEvent = firstCalendar.Events.Min(x => x.Start);
            var lastEvent = firstCalendar.Events.Max(x => x.Start);

            Console.WriteLine($"Has events from {firstEvent.AsSystemLocal.ToShortDateString()} to {lastEvent.AsSystemLocal.ToShortDateString()}");

            foreach (var ev in firstCalendar.Events)
            {
                var match = regex.Match(ev.Summary);
                if (match.Success)
                {
                    var name = match.Groups[1].ToString();
                    if (!employees.ContainsKey(name))
                    {
                        employees.Add(name, new Employee(name));
                    }

                    employees[name].AddOnCall(ev.Start, ev.End);
                }
            }

            return employees;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Ical.Net;
using Ical.Net.Interfaces.DataTypes;

namespace Woot
{
    class Program
    {
        static void Main(string[] args)
        {
            var calendarCollection = Calendar.LoadFromFile(@"C:\temp\Woot\OnCallCompensationCalculator\OnCallCompensationCalculator\Instore_Europe_schedule.ics");
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

            // Per month
            foreach (var employeeItem in employees.Where(x => x.Key == "Kenny Eliasson"))
            {
                var employee = employeeItem.Value;
                var totalOnCall = employee.CalculateOnCall();

                Console.WriteLine("=================");
                Console.WriteLine(employee.Name);
                Console.WriteLine("On call sessions: " + employee.OnCallSessions.Count);
                Console.WriteLine($"Total hours: {employee.TotalHours()} (Qualified: {totalOnCall.Qualified}, Regular: {totalOnCall.Regular}, Work: {totalOnCall.Work})");
                Console.WriteLine("Compensation: " + totalOnCall.Compensation());
             
            }

            Console.ReadLine();
        }
    }

    public class Employee
    {
        public Employee(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public List<DateRange> OnCallSessions { get; set; } = new List<DateRange>();

        public void AddOnCall(IDateTime evStart, IDateTime evEnd)
        {
            OnCallSessions.Add(new DateRange(evStart.AsSystemLocal, evEnd.AsSystemLocal));
        }

        public double TotalHours()
        {
            return OnCallSessions.Sum(x => x.TotalHours());
        }

        public OnCall CalculateOnCall()
        {
            var totalOnCall = new OnCall();
            foreach (var session in OnCallSessions)
            {
                totalOnCall.Add(session.Calculate());
            }

            return totalOnCall;
        }
    }

    public class OnCall
    {
        public void Add(OnCall onCall)
        {
            Qualified += onCall.Qualified;
            Regular += onCall.Regular;
            Work += onCall.Work;
        }

        public int Qualified { get; set; }
        public int Regular { get; set; }
        public int Work { get; set; }

        public void DetermineOnCallType(DateTime current)
        {
            // Lägg till stöd för röda dagar
            if (current.DayOfWeek == DayOfWeek.Saturday || current.DayOfWeek == DayOfWeek.Sunday) {
                Qualified++;
            } else if (current.Hour >= 8 && current.Hour < 17) { //Arbetstid då det vi vet att det inte är lördag eller söndag
                Work++;
            } else if (current.Hour >= 17 && current.DayOfWeek == DayOfWeek.Friday) { // Över 17 på en fredag
                Qualified++;
            } else if (current.Hour <= 7 && current.DayOfWeek == DayOfWeek.Monday) { // Innan 7 på måndag
                Qualified++;
            } else if (current.Hour >= 17 || current.Hour < 8) { // Mellan 17 och 08 på en vardag
                Regular++;
            } else {
                throw new Exception();
            }
        }

        public decimal Compensation()
        {
            return (Qualified * (decimal)94.80) + (Regular * (decimal)47.40);
        }
    }

    public interface IHour
    {
        bool Match(DateTime current);
    }

    public class QualifedHour : IHour
    {
        public bool Match(DateTime current)
        {
            if (current.DayOfWeek == DayOfWeek.Saturday || current.DayOfWeek == DayOfWeek.Sunday)
            {
                return true;
            }
    }

    public class WorkHour
    {

    }

    public class RegularHour
    {

    }

    public class DateRange
    {
        public DateRange(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }

        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public OnCall Calculate(DateRange calculationRange = null)
        {
            var timespan = End - Start;
            var oncall = new OnCall();
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

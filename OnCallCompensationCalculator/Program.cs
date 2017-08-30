using System;

namespace Woot
{
    class Program
    {
        static void Main(string[] args)
        {
            var employees = new OpsGenieCalendarParser().Parse(@"C:\temp\Woot\OnCallCompensationCalculator\OnCallCompensationCalculator\Instore_Europe_schedule.ics");

            foreach (var employeeItem in employees)
            {
                var employee = employeeItem.Value;
                var totalOnCall = employee.CalculateOnCall();

                Console.WriteLine("=================");
                Console.WriteLine(employee.Name);
                Console.WriteLine("On call sessions: " + employee.Sessions.Count);
                Console.WriteLine($"Total hours: {employee.TotalHours()} (Qualified: {totalOnCall.Qualified}, Regular: {totalOnCall.Regular}, Work: {totalOnCall.Work})");
                Console.WriteLine("Compensation: " + totalOnCall.Compensation().ToString("C"));
             
            }

            Console.ReadLine();
        }
    }
}

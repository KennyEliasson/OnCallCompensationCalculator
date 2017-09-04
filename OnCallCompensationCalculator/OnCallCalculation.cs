using SvenskaHögtider;
using System;

namespace Woot
{
    public class OnCallCalculation
    {
        public void Add(OnCallCalculation onCall)
        {
            Qualified += onCall.Qualified;
            Regular += onCall.Regular;
            Work += onCall.Work;
        }

        public int Qualified { get; set; }
        public int Regular { get; set; }
        public int Work { get; set; }


        public static DateTime RoundToHours(DateTime input)
        {
            DateTime dt = new DateTime(input.Year, input.Month, input.Day, input.Hour, 0, 0);

            if (input.Minute > 29)
                return dt.AddHours(1);
            else
                return dt;
        }

        public void DetermineOnCallType(DateTime current)
        {
            // Avrunda till timme (ongoing timmar splittas i två delar, en som är "Past" och en future)
            current = RoundToHours(current);

            if (current.DayOfWeek == DayOfWeek.Saturday || current.DayOfWeek == DayOfWeek.Sunday || current.IsHoliday()) {
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
}

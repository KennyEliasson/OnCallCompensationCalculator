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
}

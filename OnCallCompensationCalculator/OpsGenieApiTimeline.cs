using Flurl;
using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Woot
{
    internal class OpsGenieApiTimeline
    {
        public OpsGenieApiTimeline()
        {
        }

        internal async Task<Dictionary<string, Employee>> Request()
        {
            var response = await "https://api.opsgenie.com/v2/schedules/Instore Europe_schedule/timeline?identifierType=name&intervalUnit=months&interval=12&date=2017-11-01T08:00:00%2B02:00"
                .WithHeader("Authorization", "GenieKey <ADD KEY>")
                .GetJsonAsync<TimelineResponse>();

            var employees = new Dictionary<string, Employee>();
            
            var timeline = response.Data.FinalTimeline;
            foreach(var rotation in timeline.Rotations)
            {
                foreach(var period in rotation.Periods)
                {
                    var employeeName = period.Recipient.Name;
                    if (!employees.ContainsKey(employeeName))
                    {
                        employees.Add(employeeName, new Employee(employeeName));
                    }

                    employees[employeeName].AddOnCall(period.StartDate, period.EndDate);
                }
            }




            return employees;
        }
    }

    public class TimelineResponse
    {
        public TimelineResponseData Data { get; set; }

        public class TimelineResponseData
        {
            public TimelineResponseTimeline FinalTimeline { get; set; }
        }

        public class TimelineResponseTimeline
        {
            public class TimelineResponseRotation
            {
                public List<TimelineResponsePeriod> Periods { get; set; }

                public class TimelineResponsePeriod
                {
                    public DateTime StartDate { get; set; }
                    public DateTime EndDate { get; set; }
                    public string Type { get; set; }

                    public TimelineResponseRecipient Recipient { get; set; }

                    public class TimelineResponseRecipient
                    {
                        public string Name { get; set; }
                    }
                }
            }

            public List<TimelineResponseRotation> Rotations { get; set; }
        }
    }
}
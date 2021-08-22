using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSurvey.UseCases.Ports.In
{
    public class AvailablePollList
    {
        public class AvailablePoll
        {
            public string Id { get; set; }
            public string Name {  get; set; }
        }

        public DateTime UpdatedAt { get; set; }
        public List<AvailablePoll> Polls { get; private set; } = new List<AvailablePoll>();
    }
}

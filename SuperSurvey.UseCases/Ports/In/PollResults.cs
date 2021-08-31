using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSurvey.UseCases.Ports.In
{
    public class PollResults
    {
        public class OptionResult
        {
            public string Description { get; set; }
            public int VotesCount { get; set; }
            public double VotesPercentage { get; set; }
        }
        public int PollId { get; set; }

        public DateTime UpdatedAt { get; set; }
        public OptionResult[] Results { get; set; }

    }
}

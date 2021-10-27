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
            public OptionResult()
            {
                Description = string.Empty;
                PictureUrl = string.Empty;
            }
            public string Description { get; set; }
            public string PictureUrl { get; set; }
            public int VotesCount { get; set; }
            public double VotesPercentage { get; set; }
        }
        public PollResults()
        {
            Name = string.Empty;
            Results = Array.Empty<OptionResult>();
        }
        public int PollId { get; set; }
        public string Name { get; set; }

        public DateTime UpdatedAt { get; set; }
        public OptionResult[] Results { get; set; }

    }
}

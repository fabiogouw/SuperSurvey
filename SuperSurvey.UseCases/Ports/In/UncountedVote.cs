using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSurvey.UseCases.Ports.In
{
    public class UncountedVote
    {
        public string Option { get; set; }
        public DateTime CastedAt { get; set; }
    }
}

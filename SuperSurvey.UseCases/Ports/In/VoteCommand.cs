using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSurvey.UseCases.Ports.In
{
    public class VoteCommand
    {
        public string UserId { get; set; }
        public int PollId { get; set; }
        public string SelectedOption { get; set; }
    }
}

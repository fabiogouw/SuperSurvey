using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSurvey.UseCases.Ports.In
{
    public class VoteCommand
    {
        public int UserId { get; set; }
        public int PollId { get; set; }
        public int SelectedOption { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

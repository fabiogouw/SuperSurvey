using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSurvey.Domain
{
    public class Vote
    {
        public string Id {  get; set; }
        public string ChosenOption { get; set; }
        public int PollId { get; private set; }
    }
}

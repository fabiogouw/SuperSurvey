using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSurvey.Domain
{
    public class Voter
    {
        public Vote CastVote(Poll poll, Option chosenOption)
        {
            var vote = new Vote.Builder()
                .WithId(1)
                .WithPollId(poll.Id)
                .WithChosenOption(chosenOption.Id)
                .Build();
            return vote;
        }
    }
}

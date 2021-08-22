using SuperSurvey.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSurvey.UseCases.Ports.Out
{
    public interface VoteRepository
    {
        Task<Vote[]> GetUncountedVotes();
        Task Save(Vote vote);
    }
}

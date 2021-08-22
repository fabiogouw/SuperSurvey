using SuperSurvey.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSurvey.UseCases.Ports.Out
{
    public interface VoterRepository
    {
        Task<Voter> GetById(string id);
    }
}

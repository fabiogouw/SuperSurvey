using SuperSurvey.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSurvey.UseCases.Ports.Out
{
    public interface PollRepository
    {
        Task<Poll> Save(Poll poll);
        Task<List<Poll>> GetAllActive(DateTime currentDateTime);
        Task<Poll> GetById(int id);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSurvey.UseCases.Ports.In
{
    public interface ManagePollsUseCase
    {
        Task<AvailablePollList> ListPolls();
        Task<PollDTO> GetPollById(int id);
    }
}

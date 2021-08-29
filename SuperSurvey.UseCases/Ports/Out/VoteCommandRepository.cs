using SuperSurvey.Domain;
using SuperSurvey.UseCases.Ports.In;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSurvey.UseCases.Ports.Out
{
    public interface VoteCommandRepository
    {
        Task Save(VoteCommand command);
    }
}

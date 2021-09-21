using SuperSurvey.Domain;
using SuperSurvey.UseCases.Ports.In;
using SuperSurvey.UseCases.Ports.Out;
using System.Threading.Tasks;

namespace SuperSurvey.UseCases
{
    public class CastVoteUseCaseImpl : CastVoteUseCase
    {
        private readonly VoteCommandRepository _voteRepository;

        public CastVoteUseCaseImpl(VoteCommandRepository voteRepository)
        {
            _voteRepository = voteRepository;
        }
        public async Task CastVote(VoteCommand command)
        {
            await _voteRepository.Save(command);
        }
    }
}

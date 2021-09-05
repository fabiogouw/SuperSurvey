using SuperSurvey.Domain;
using SuperSurvey.UseCases.Ports.In;
using SuperSurvey.UseCases.Ports.Out;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSurvey.UseCases
{
    public class CastVoteUseCaseImpl : CastVoteUseCase
    {
        private readonly PollRepository _pollRepository;
        private readonly VoteCommandRepository _voteRepository;

        public CastVoteUseCaseImpl(PollRepository pollRepository,
            VoteCommandRepository voteRepository)
        {
            _pollRepository = pollRepository;
            _voteRepository = voteRepository;
        }
        public async Task CastVote(VoteCommand command)
        {
            Poll poll = await _pollRepository.GetById(command.PollId);
            Option option = poll.GetOption(command.SelectedOption);
            await _voteRepository.Save(command);
        }
    }
}

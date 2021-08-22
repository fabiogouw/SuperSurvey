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
        private readonly VoterRepository _voterRepository;
        private readonly VoteRepository _voteRepository;

        public CastVoteUseCaseImpl(PollRepository pollRepository,
            VoterRepository voterRepository,
            VoteRepository voteRepository)
        {
            _pollRepository = pollRepository;
            _voterRepository = voterRepository;
            _voteRepository = voteRepository;
        }
        public async Task CastVote(VoteCommand command)
        {
            Voter voter = await _voterRepository.GetById(command.UserId);
            Poll poll = await _pollRepository.GetById(command.PollId);
            Option option = poll.GetOption(command.SelectedOption);
            Vote vote = voter.CastVote(option);
            await _voteRepository.Save(vote);
        }
    }
}

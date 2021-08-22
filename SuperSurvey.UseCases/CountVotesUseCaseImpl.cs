using SuperSurvey.UseCases.Ports.In;
using SuperSurvey.UseCases.Ports.Out;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSurvey.UseCases
{
    public class CountVotesUseCaseImpl : CountVotesUseCase
    {
        private readonly PollRepository _pollRepository;
        private readonly VoteRepository _voteRepository;

        public CountVotesUseCaseImpl(PollRepository pollRepository,
            VoteRepository voteRepository)
        {
            _pollRepository = pollRepository;
            _voteRepository = voteRepository;
        }

        public async Task CountVotes(IEnumerable<UncountedVote> votes)
        {
            var allVotes = await _voteRepository.GetUncountedVotes();
            var groupedVotes = allVotes.GroupBy(v => v.PollId);
            foreach(var votesByPoll in groupedVotes)
            {
                var poll = await _pollRepository.GetById(votesByPoll.Key);
                foreach(var vote in votesByPoll)
                {
                    poll.AddVote(vote);
                }
                await _pollRepository.Save(poll);
            }
        }
    }
}

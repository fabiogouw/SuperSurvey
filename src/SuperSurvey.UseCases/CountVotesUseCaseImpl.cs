using SuperSurvey.Domain;
using SuperSurvey.UseCases.Ports.In;
using SuperSurvey.UseCases.Ports.Out;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuperSurvey.UseCases
{
    public class CountVotesUseCaseImpl : CountVotesUseCase
    {
        private readonly PollRepository _pollRepository;

        public CountVotesUseCaseImpl(PollRepository pollRepository)
        {
            _pollRepository = pollRepository;
        }

        public async Task CountVotes(IEnumerable<VoteCommand> voteCommands)
        {
            var groupedVoteCommands = voteCommands.GroupBy(v => v.PollId);
            foreach(var voteCommandsByPoll in groupedVoteCommands)
            {
                var voter = new AnonymousVoter();
                var poll = await _pollRepository.GetById(voteCommandsByPoll.Key);
                foreach(var voteCommand in voteCommandsByPoll)
                {
                    var selectedOption = poll.GetOption(voteCommand.SelectedOption);
                    var vote = voter.CastVote(poll, selectedOption);
                    poll.AddVote(vote);
                }
                await _pollRepository.Save(poll);
            }
        }
    }
}

using SuperSurvey.Domain;
using SuperSurvey.UseCases.Ports.In;
using SuperSurvey.UseCases.Ports.Out;

namespace SuperSurvey.UseCases
{
    public class ViewResultsUseCaseImpl : ViewResultsUseCase
    {
        private readonly PollRepository _pollRepository;

        public ViewResultsUseCaseImpl(PollRepository pollRepository)
        {
            _pollRepository = pollRepository;
        }
        public async Task<PollResults> getResults(int pollId)
        {
            Poll poll = await _pollRepository.GetById(pollId);
            double totalVotes = poll.Options.Sum(option => option.VoteCount);
            var pollResults = new PollResults()
            {
                PollId = poll.Id,
                Results = poll.Options.Select(option => new PollResults.OptionResult()
                {
                    Description = option.Description,
                    VotesCount = option.VoteCount,
                    VotesPercentage = totalVotes > 0 ? option.VoteCount / totalVotes * 100 : 0
                }).ToArray()
            };
            return pollResults;
        }
    }
}

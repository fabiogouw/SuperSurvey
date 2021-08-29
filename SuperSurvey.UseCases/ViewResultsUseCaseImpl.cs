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
            return new PollResults();
        }
    }
}

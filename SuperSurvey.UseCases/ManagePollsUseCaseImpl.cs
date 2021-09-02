using SuperSurvey.UseCases.Ports.In;
using SuperSurvey.UseCases.Ports.Out;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSurvey.UseCases
{
    public class ManagePollsUseCaseImpl : ManagePollsUseCase
    {
        private readonly PollRepository _pollRepository;

        public ManagePollsUseCaseImpl(PollRepository pollRepository)
        {
            _pollRepository = pollRepository;
        }

        public async Task<AvailablePollList> ListPolls()
        {
            var polls = await _pollRepository.GetAllActive(DateTime.Now);
            var resuls = new AvailablePollList();
            resuls.Polls.AddRange(polls.Select(poll => new AvailablePollList.AvailablePoll()
            {
                Id = poll.Id,
                Name = poll.Name
            }));
            return resuls;
        }

        public async Task<PollDTO> GetPollById(int id)
        {
            var poll = await _pollRepository.GetById(id);
            return new PollDTO()
            {
                Id = poll.Id,
                Name = poll.Name,
                Options = poll.Options.Select(option => new PollDTO.OptionDTO()
                {
                    Id = option.Id,
                    Description = option.Description,
                    PictureUrl = option.PictureUrl
                }).ToList()
            };
        }
    }
}

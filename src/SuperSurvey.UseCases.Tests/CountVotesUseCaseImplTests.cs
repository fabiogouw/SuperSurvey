using Xunit;
using Moq;
using SuperSurvey.Domain;
using SuperSurvey.UseCases.Ports.In;
using SuperSurvey.UseCases.Ports.Out;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SuperSurvey.UseCases.Tests
{
    public class CountVotesUseCaseImplTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task Should_NotUseRepository_When_EmptyArrayOfVoteCommandsIsIssued()
        {
            var mock = new Mock<PollRepository>();
            var sut = new CountVotesUseCaseImpl(mock.Object);
            var votes = new List<VoteCommand>();

            await sut.CountVotes(votes);

            mock.Verify(m => m.Save(It.IsAny<Poll>()), Times.Never());
        }
    }
}
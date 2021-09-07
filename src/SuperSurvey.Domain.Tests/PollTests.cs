using FluentAssertions;
using Xunit;

namespace SuperSurvey.Domain.Tests;
public class PollTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public void Should_CountVote_When_NewVoteIsAdded()
    {
        var sut = new Poll.Builder()
            .WithOption(new Option.Builder()
                .WithId(999)
                .Build())
            .Build();

        sut.AddVote(new Vote.Builder()
            .WithChosenOption(999)
            .Build());

        sut.Options.Count.Should().Be(1);
        sut.Options[0].VoteCount.Should().Be(1);
        sut.Options[0].Id.Should().Be(999);
    }
}
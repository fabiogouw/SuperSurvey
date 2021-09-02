using Microsoft.AspNetCore.Mvc;
using SuperSurvey.UseCases.Ports.In;

namespace SuperSurvey.WebApp.Controllers;
public class PollController : Controller
{
    private readonly ManagePollsUseCase _managePollsUseCase;
    private readonly ViewResultsUseCase _viewResultsUseCase;
    private readonly CastVoteUseCase _castVoteUseCase;

    public PollController(ManagePollsUseCase managePollsUseCase,
        CastVoteUseCase castVoteUseCase,
        ViewResultsUseCase viewResultsUseCase)
    {
        _managePollsUseCase = managePollsUseCase;
        _castVoteUseCase = castVoteUseCase;
        _viewResultsUseCase = viewResultsUseCase;
    }

    // GET: PollController
    public async Task<ActionResult> Index()
    {
        var polls = await _managePollsUseCase.ListPolls();
        return View(polls);
    }

    // GET: PollController/Detail/5
    public async Task<ActionResult> Detail(int id)
    {
        var poll = await _managePollsUseCase.GetPollById(id);
        return View(poll);
    }

    // GET: PollController/Vote/5
    [HttpPost]
    //[ValidateAntiForgeryToken]
    public async Task<ActionResult> Vote(int id, IFormCollection collection)
    {
        int voteId = int.Parse(collection["VoteId"]);
        await _castVoteUseCase.CastVote(new VoteCommand()
        {
            CreatedAt = DateTime.Now,
            PollId = id,
            SelectedOption = voteId,
            UserId = 0
        });
        return RedirectToAction("Results", new { Id = id });
    }

    // GET: PollController/Results/5
    public async Task<ActionResult> Results(int id)
    {
        var results = await _viewResultsUseCase.getResults(id);
        return View(results);
    }
}

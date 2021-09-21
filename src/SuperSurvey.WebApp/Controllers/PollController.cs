using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SuperSurvey.UseCases.Ports.In;
using System;
using System.Threading.Tasks;

namespace SuperSurvey.WebApp.Controllers;
public class PollController : Controller
{
    private readonly ManagePollsUseCase _managePollsUseCase;
    private readonly ViewResultsUseCase _viewResultsUseCase;
    private readonly CastVoteUseCase _castVoteUseCase;
    private readonly ILogger<PollController> _logger;

    public PollController(ManagePollsUseCase managePollsUseCase,
        CastVoteUseCase castVoteUseCase,
        ViewResultsUseCase viewResultsUseCase,
        ILogger<PollController> logger)
    {
        _managePollsUseCase = managePollsUseCase;
        _castVoteUseCase = castVoteUseCase;
        _viewResultsUseCase = viewResultsUseCase;
        _logger = logger;
    }

    // GET: PollController
    public async Task<ActionResult> Index()
    {
        try
        {
            var polls = await _managePollsUseCase.ListPolls();
            return View(polls);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            throw;
        }
    }

    // GET: PollController/Detail/5
    public async Task<ActionResult> Detail(int id)
    {
        try
        {
            var poll = await _managePollsUseCase.GetPollById(id);
            return View(poll);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            throw;
        }
    }

    // GET: PollController/Vote/5
    [HttpPost]
    public async Task<ActionResult> Vote(int id, IFormCollection collection)
    {
        try
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
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            throw;
        }
    }

    // GET: PollController/Results/5
    public async Task<ActionResult> Results(int id)
    {
        try
        {
            var results = await _viewResultsUseCase.getResults(id);
            return View(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            throw;
        }
    }
}

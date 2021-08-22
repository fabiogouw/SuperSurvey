using Microsoft.AspNetCore.Mvc;
using SuperSurvey.UseCases.Ports.In;

namespace SuperSurvey.WebApp.Controllers;
public class PollController : Controller
{
    private readonly ManagePollsUseCase _managePollsUseCase;
    private readonly ViewResultsUseCase _viewResultsUseCase;

    public PollController(ManagePollsUseCase managePollsUseCase)
    {
        _managePollsUseCase = managePollsUseCase;
    }

    // GET: PollController
    public async Task<ActionResult> Index()
    {
        var polls = await _managePollsUseCase.ListPolls();
        return View(polls);
    }

    // GET: PollController/Details/5
    public ActionResult Details(int id)
    {
        return View();
    }

    // GET: PollController/Results/5
    public async Task<ActionResult> Results(int id)
    {
        var results = await _viewResultsUseCase.getResults(id);
        return View(results);
    }

    // POST: PollController/Vote
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Vote(IFormCollection collection)
    {
        try
        {
            return RedirectToAction(nameof(Results));
        }
        catch
        {
            return View();
        }
    }
}

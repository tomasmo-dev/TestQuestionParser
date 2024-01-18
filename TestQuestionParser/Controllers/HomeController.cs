using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TestQuestionParser.Models;
using TestQuestionParser.Services;

namespace TestQuestionParser.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly FileService _fileService;

    public HomeController(ILogger<HomeController> logger, FileService fileService)
    {
        _logger = logger;
        _fileService = fileService;
    }

    public IActionResult Index()
    {
        if (TempData["Error"] != null)
            ViewData["Error"] = TempData["Error"]!.ToString()!;
        if (TempData["Success"] != null)
            ViewData["Success"] = TempData["Success"]!.ToString()!;
        
        return View();
    }
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpPost]
    public async Task<IActionResult> PostData(HomeIndexUploadModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Invalid model state";
            return RedirectToAction("Index");
        }

        var success = await _fileService.ParseExcelFile(model);
        
        if (success)
        {
            TempData["Success"] = "Successfully parsed excel file";
            return RedirectToAction("Index");
        }
        else
        {
            TempData["Error"] = "Failed to parse excel file";
            return RedirectToAction("Index");
        }
        
    }
}
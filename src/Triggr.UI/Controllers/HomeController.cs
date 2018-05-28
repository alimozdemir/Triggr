using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Triggr.Data;
using Triggr.Providers;
using Triggr.Services;
using Triggr.UI.Models;

namespace Triggr.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILanguageService _languageService;

        public HomeController(ILanguageService languageService)
        {
            _languageService = languageService;
        }

        public IActionResult Index()
        {
            var languages = _languageService.Languages;
            return View(languages);
        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

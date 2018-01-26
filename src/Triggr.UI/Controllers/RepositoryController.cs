using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Triggr.Data;
using Triggr.Providers;

namespace Triggr.UI.Controllers
{
    public class RepositoryController : Controller
    {
        private readonly TriggrContext _context;
        private readonly IProviderFactory _providerFactory;
        public RepositoryController(TriggrContext context, IProviderFactory providerFactory)
        {
            _context = context;
            _providerFactory = providerFactory;

        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetRepositories()
        {
            var lists = await _context.Repositories.ToListAsync();
            return Json(lists);
        }

        public IActionResult GetProvider(string url)
        {
            return Json(_providerFactory.GetProviderType(url));
        }

        [HttpPost]
        public async Task<IActionResult> AddRepository([FromBody]Models.AddRepositoryViewModel model)
        {
            bool result = false;
            if (ModelState.IsValid)
            {
                var provider = _providerFactory.GetProviderType(model.Url);

                Repository repository = new Repository();
                repository.Provider = provider;
                repository.UpdatedTime = DateTimeOffset.Now;
                repository.Url = model.Url;

                _context.Add(repository);
                var affected = await _context.SaveChangesAsync();
                result = affected > 0;
            }

            return Json(result);
        }

    }
}
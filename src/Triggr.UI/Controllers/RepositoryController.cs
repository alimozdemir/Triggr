using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Triggr.Data;
using Triggr.Providers;
using Triggr.Services;

namespace Triggr.UI.Controllers
{
    public class RepositoryController : Controller
    {
        private readonly TriggrContext _context;
        private readonly IProviderFactory _providerFactory;
        private readonly IWebhookFactory _webhookFactory;

        public RepositoryController(TriggrContext context, IProviderFactory providerFactory, IWebhookFactory webhookFactory)
        {
            _context = context;
            _providerFactory = providerFactory;
            _webhookFactory = webhookFactory;
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

        public IActionResult GetValidation(string url)
        {
            return Json(new
            {
                Valid = _providerFactory.GetProviderType(url),
                Webhook = _webhookFactory.IsSupported(url)
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddRepository([FromBody]Models.AddRepositoryViewModel model)
        {
            bool result = false;
            var providerType = _providerFactory.GetProviderType(model.Url);
            if (ModelState.IsValid && !string.IsNullOrEmpty(providerType))
            {
                var service = _webhookFactory.GetService(model.Url);
                if (service != null)
                {
                    var dbRecord = _context.Repositories.FirstOrDefaultAsync(i => i.Name.Equals(model.Name) 
                        && i.OwnerName.Equals(model.Owner));

                    if (dbRecord == null)
                    {
                        Repository repository = new Repository();
                        repository.Provider = providerType;
                        repository.UpdatedTime = DateTimeOffset.Now;
                        repository.Url = model.Url;
                        repository.Name = model.Name;
                        repository.OwnerName = model.Owner;
                        repository.Token = model.Token;

                        if (await service.AddHookAsync(repository))
                        {
                            _context.Add(repository);
                            var affected = await _context.SaveChangesAsync();
                            result = affected > 0;
                        }
                    }
                    else
                    {
                        //return already exist
                    }
                }
            }

            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveRepository([FromBody]Models.IdFormViewModel model)
        {
            bool result = false;

            if (ModelState.IsValid)
            {
                var repo = await _context.Repositories.FindAsync(model.Id);
                if (repo != null)
                {
                    _context.Remove(repo);
                    var affected = await _context.SaveChangesAsync();
                    result = affected > 0;
                }
            }

            return Json(result);
        }

    }
}
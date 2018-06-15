using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Triggr.Data;
using Triggr.Providers;
using Triggr.Services;
using Triggr.UI.Models;

namespace Triggr.UI.Controllers
{
    public class RepositoryController : Controller
    {
        private readonly TriggrContext _context;
        private readonly IProviderFactory _providerFactory;
        private readonly IWebhookFactory _webhookFactory;
        private readonly IBackgroundJobClient _jobClient;
        private readonly IContainerService _containerService;

        public RepositoryController(TriggrContext context, IProviderFactory providerFactory, IWebhookFactory webhookFactory, IBackgroundJobClient jobClient, IContainerService containerService)
        {
            _context = context;
            _providerFactory = providerFactory;
            _webhookFactory = webhookFactory;
            _jobClient = jobClient;
            _containerService = containerService;
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
                    var dbRecord = await _context.Repositories.FirstOrDefaultAsync(i => i.Name.Equals(model.Name)
                        && i.OwnerName.Equals(model.Owner));

                    if (dbRecord == null)
                    {
                        Data.Repository repository = new Data.Repository();
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
        public async Task<IActionResult> RemoveRepository([FromBody]IdStringFormViewModel model) //[FromBody]Models.IdFormViewModel
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
        public IActionResult Container(string Id)
        {
            if (string.IsNullOrEmpty(Id))
                return RedirectToAction("Index");

            var container = _containerService.GetContainer(Id);

            return View(container);
        }

        public IActionResult ProbeActivation(string repoId, string probeId)
        {
            var id = _jobClient.Enqueue<ProbeControl>(i => i.Execute(null, probeId, repoId));
            return Json(id);
        }
        public IActionResult ProbeRawJson(string repoId, string probeId)
        {
            var container = _containerService.GetContainer(repoId);
            if (container != null)
            {
                var probes = container.CheckForProbes();
                var probe = probes.FirstOrDefault(i => i.Id.Equals(probeId));

                if (probe != null)
                {
                    var json = JsonConvert.SerializeObject(probe, Formatting.Indented, new Newtonsoft.Json.Converters.StringEnumConverter());
                    return Json(json);
                }
            }

            return Json(-1);

        }
    }
}
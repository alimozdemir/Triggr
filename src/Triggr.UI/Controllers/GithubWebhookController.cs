using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebHooks;
using Newtonsoft.Json.Linq;
using Triggr.UI.Models;
using Microsoft.Extensions.Primitives;
using System.Linq;

namespace Triggr.UI.Controllers
{
    public class GithubWebhookController : Controller
    {
        /*[GitHubWebHook]
        public IActionResult HandlerForPush(JObject data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok();
        }

        [GeneralWebHook]
        public IActionResult HandlerForPush(string receiverName, string id, string eventName, JObject data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok();
        }*/

        [HttpPost]
        public IActionResult HandlerForPush([FromBody]GithubPushModel model)
        {
            if (ModelState.IsValid)
            {
                if (Request.Headers.TryGetValue("X-GitHub-Event", out StringValues value))
                {
                    if (value.Count == 1)
                    {
                        var eventName = value[0];

                        if (eventName.Equals("push"))
                        {
                            var changedFiles = model
                                            .Commits
                                            .SelectMany(c => c.Modified).Distinct();
                                            
                            var owner = model.Repository.Owner.Name;
                            var repoName = model.Repository.Name;

                            return Ok();
                        }
                    }

                }
            }

            return BadRequest(ModelState);
        }
    }
}
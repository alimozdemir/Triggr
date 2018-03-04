using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebHooks;
using Newtonsoft.Json.Linq;

namespace Triggr.UI.Controllers
{
    public class GithubWebhookController : Controller
    {
        [GitHubWebHook(EventName = "push")]
        public IActionResult HandlerForPush(string id, JObject data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            return Ok();
        }
    }
}
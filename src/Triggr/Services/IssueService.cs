using Octokit;
using Triggr.Wrappers;

namespace Triggr.Services
{
    public class IssueService : IMessageService
    {
        private readonly GithubWrapper _client;

        public IssueService(GithubWrapper client)
        {
            _client = client;
        }
        public ActuatorType MessageType => ActuatorType.GitHubIssue;

        public void Send(Data.Repository repository, Actuator act, string message)
        {
            
            _client.CreateIssue(repository, "Triggr Issue", message, act.Assign);
        }
    }
}
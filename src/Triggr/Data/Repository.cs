using System;

namespace Triggr.Data
{
    public class Repository
    {
        public string Id { get; set; }
        public string Provider { get; set; }
        public string Reference { get; set; }
        public string Url { get; set; }
        public string Token { get; set; }
        public string OwnerName { get; set; }
        public string Name { get; set; }
        public bool WebHook { get; set; }
        public string WebHookId { get; set; }
        public DateTimeOffset UpdatedTime { get; set; }
    }
}
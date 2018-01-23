using System;

namespace Triggr
{
    public sealed class Container
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Languages { get; set; }
        public DateTimeOffset UpdatedTime { get; set; }
    }
}
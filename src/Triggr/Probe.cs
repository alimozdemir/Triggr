using System;
using System.Collections.Generic;

namespace Triggr
{
    public class Probe
    {
        public ProbeType ProbeType { get; set; }
        public string ObjectPath { get; set; }
        public string ObjectType { get; set; }
        public string ObjectName { get; set; }
        public string Modifiers { get; set; }
        public DateTimeOffset CreationDate { get; set; }

    }

    public enum ProbeType
    {
        CodeChanges,
        StaticAnalysis
    }
}
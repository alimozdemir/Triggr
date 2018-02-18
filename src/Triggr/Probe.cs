using System;
using System.Collections.Generic;

namespace Triggr
{
    public class Probe
    {
        public string Id { get; set; }
        public ProbeType ProbeType { get; set; }
        public ObjectInformation Object { get; set; }
        public string Modifiers { get; set; }
        public DateTimeOffset CreationDate { get; set; }
        public Metrics Metrics { get; set; }
        public List<Actuator> Actuators { get; set; }
    }

    public class ObjectInformation
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Type { get; set; }
    }

    public class Metrics
    {
        public ReportType Strategy { get; set; }
        public List<string> Arguments { get; set; }
    }

    public class Actuator
    {
        public ActuatorType Type { get; set; }
        public List<string> Emails { get; set; }
        public string Assign { get; set; }
    }

    public enum ActuatorType
    {
        GitHubIssue,
        Email
    }

    public enum ReportType
    {
        Always,
        Diff
    }
    public enum ProbeType
    {
        CodeChanges,
        StaticAnalysis
    }
}
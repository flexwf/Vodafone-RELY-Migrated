using System;

namespace RELY_ScheduledTasks
{
    public class S15ExtractsViewModel
    {
        public string Extracts { get; set; }
        public string FileName { get; set; }
        public string ExtractFileType { get; set; }

       // public string ExtractType { get; set; }

    }
    public partial class GKeyValueViewModel
    {
        public string Value { get; set; }
    }
    public class GErrorLogViewModel
    {
        public DateTime ErrorDateTime { get; set; }
        public string SourceProject { get; set; }
        public string Controller { get; set; }
        public string Method { get; set; }
        public string StackTrace { get; set; }
        public string UserName { get; set; }
        public string ErrorType { get; set; }
        public string ErrorDescription { get; set; }
        public string Resolution { get; set; }
        public string ErrorOwner { get; set; }
        public string FieldName { get; set; }
        public string Status { get; set; }
    }


}

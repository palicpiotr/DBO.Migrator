using System.Collections.Generic;

namespace DBO.DataTransport.HelpersStandard.EFCore
{
    public class DataToExecute<T>
    {
        public IEnumerable<T> Data { get; set; }
        public string ParamName { get; set; }
        public string TypeName { get; set; }
        public string[] ExcludedColumns { get; set; }
    }
}

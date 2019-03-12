using System.Collections.Generic;

namespace Datalect.Sql
{
    public class Query : IQuery
    {
        public IReadOnlyDictionary<string, object> BindVariables { get; set; }
        public string Text { get; set; }
    }
}

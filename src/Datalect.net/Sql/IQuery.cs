using System.Collections.Generic;

namespace Datalect.Sql
{
    public interface IQuery
    {
        IReadOnlyDictionary<string, object> BindVariables { get; }
        string Text { get; }
    }
}
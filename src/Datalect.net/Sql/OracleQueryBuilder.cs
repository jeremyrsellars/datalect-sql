using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Datalect.Util;

namespace Datalect.Sql
{
    public class OracleQueryBuilder
    {
        public static object Apply(Func<object[],object> fn, IEnumerable<object> tokens) => 
            fn(tokens.ToArray());

        public static object Expression(params object[] tokens) => tokens;

        public static object Or(params object[] expressions)
        {
            expressions = expressions.RemoveNulls().ToArray();
            switch (expressions.Length)
            {
                case 0:
                    return "0=0";
                case 1:
                    return expressions[0];
                default:
                    return ImmutableList<object>.Empty
                       .Add("(")
                       .AddRange(expressions.Interpose("OR"))
                       .Add(")");
            }
        }

        public static object And(params object[] expressions)
        {
            expressions = expressions.RemoveNulls().ToArray();
            switch (expressions.Length)
            {
                case 0:
                    return "0=0";
                case 1:
                    return expressions[0];
                default:
                    return ImmutableList<object>.Empty
                       .Add("(")
                       .AddRange(expressions.Interpose("AND"))
                       .Add(")");
            }
        }

        public static IQuery Build(params object[] sqlOrBindVariables)
        {
            string NormalizeDescription(string bindVariablePart) =>
                bindVariablePart == null ? null : Regex.Replace(bindVariablePart, @"\W+", "_");
            string IndexToBindVariable(int index, string optionalDescription) => ":" + (NormalizeDescription(optionalDescription) ?? "b") + index;

            var bindVariables = new List<BoundValue>();
            var text = new StringBuilder();

            foreach(var sqlOrBindVariable in FlattenAndValidate(sqlOrBindVariables))
            {
                string rep;
                if (sqlOrBindVariable is string sql)
                {
                    rep = sql;
                }
                else if (sqlOrBindVariable is BoundValue bv)
                {
                    var index = bindVariables.IndexOf(bv);
                    if (index < 0)
                    {
                        index = bindVariables.Count;
                        bindVariables.Add(bv);
                    }
                    rep = IndexToBindVariable(index, bv.Description);
                }
                else
                {
                    throw new ArgumentException($"Only literal SQL strings and BoundValue objects are supported. Type: {sqlOrBindVariable?.GetType()?.FullName ?? "null"}");
                }
                text.Append(" ").Append(rep);
            }

            return new Query
            {
                BindVariables =
                    bindVariables
                        .Select((value, i) => (Key: IndexToBindVariable(i, value.Description), Value: value.Value))
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                Text = text.ToString().Trim(),
            };
        }

        private static IEnumerable<object> FlattenAndValidate(IEnumerable items)
        {
            foreach (var item in items)
            {
                if (item is null)
                {
                    // No need to yield null.
                }
                else if (item is string)
                {
                    yield return item;
                }
                else if (item is BoundValue)
                {
                    yield return item;
                }
                else if (item is IEnumerable list)
                {
                    foreach (var x in FlattenAndValidate(list))
                        yield return x;
                }
                else
                {
                    throw new ArgumentException(
                        $"Only literal SQL strings, BoundValue objects, and IEnumerable of these are supported. Type: {item?.GetType()?.FullName ?? "null"}");
                }
            }
        }
    }
}

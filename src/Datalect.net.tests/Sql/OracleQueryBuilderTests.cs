using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Datalect.Util;

namespace Datalect.Sql
{
    using System.Runtime.CompilerServices;
    using static OracleQueryBuilder;

    [TestFixture]
    public class OracleQueryBuilderTests
    {
        [TestCaseSource(nameof(SqlExamples))]
        public void SqlIsCorrect((int LineNumber, string ExpectedSql, object SqlParts) test)
        {
            var actual = Build(test.SqlParts).Text;
            AssertCanonicalSql(test.ExpectedSql, actual,
                $"Example defined at line {test.LineNumber}");
        }

        public static IEnumerable<(int LineNumber, string ExpectedSql, object SqlParts)> SqlExamples = new[]
        {
            Example("1=1",                     And("1=1")),
            Example("( 1=1 AND 2=2 )",         And("1=1", "2=2")),
            Example("( 1=1 AND 2=2 AND 3=3 )", And("1=1", "2=2", "3=3")),
            Example("( 1=1 AND 2=2 )",         Apply(And, "1=1,2=2".Split(','))),

            Example("1=1",                     Or("1=1")),
            Example("( 1=1 OR 2=2 )",          Or("1=1", "2=2")),
            Example("( 1=1 OR 2=2 OR 3=3 )",   Or("1=1", "2=2", "3=3")),

            Example("a + b",       Apply(x => x.Interpose("+"), "a,b".Split(','))),
            Example(":b0 a b",     Apply(x => x.Prepend(BoundValue.Create(null)), "a,b".Split(','))),

            Example("",            Expression()),
            Example("a",           Expression("a")),
            Example("a b",         Expression("a", "b")),

            Example("SELECT * FROM dual /*no-CTE*/",
                            Expression(WithTableExpressions(),
                                       "SELECT * FROM dual /*no-CTE*/")),

            Example($"WITH booleans AS ( {Booleans} ) SELECT booleans.* FROM booleans",
                            Expression(WithTableExpressions(("booleans", Booleans)),
                                       "SELECT booleans.* FROM booleans")),

            Example($"WITH booleans AS ( {Booleans} ) , zeros AS ( {Zeros} ) SELECT * FROM booleans, zeros",
                            Expression(WithTableExpressions(("booleans", Booleans), ("zeros", Zeros)),
                                       "SELECT * FROM booleans, zeros")),
        };

        private static (int LineNumber, string ExpectedSql, object SqlParts) Example(
            string expectedSql, object sqlParts, [CallerLineNumber] int lineNumber = 0) =>
            (lineNumber, expectedSql, sqlParts);

        private static void AssertCanonicalSql(string expectedSql, string actual, string message) =>
            Assert.AreEqual(
                SqlUtil.CanonicalWhitespace(expectedSql),
                SqlUtil.CanonicalWhitespace(actual),
                message + Environment.NewLine + actual);

        private const string Booleans =
            "SELECT 'false' FROM dual UNION ALL SELECT 'true' FROM dual";

        private const string Zeros = "SELECT 0 FROM dual";
    }
}

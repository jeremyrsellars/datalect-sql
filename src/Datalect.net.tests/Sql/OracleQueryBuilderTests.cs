using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace Datalect.Sql
{
    using static OracleQueryBuilder;

    [TestFixture]
    public class OracleQueryBuilderTests
    {
        [TestCaseSource(nameof(SqlExamples))]
        public void SqlIsCorrect((string ExpectedSql, object SqlParts) test)
        {
            var actual = Build(test.SqlParts).Text;
            AssertCanonicalSql(test.ExpectedSql, actual);
        }

        public static IEnumerable<(string ExpectedSql, object SqlParts)> SqlExamples = new[]
        {
            ("1=1",                     And("1=1")),
            ("( 1=1 AND 2=2 )",         And("1=1", "2=2")),
            ("( 1=1 AND 2=2 AND 3=3 )", And("1=1", "2=2", "3=3")),
            ("( 1=1 AND 2=2 )",         Apply(And, "1=1,2=2".Split(','))),

            ("1=1",                     Or("1=1")),
            ("( 1=1 OR 2=2 )",          Or("1=1", "2=2")),
            ("( 1=1 OR 2=2 OR 3=3 )",   Or("1=1", "2=2", "3=3")),

            ("a b",         Apply(x => x, "a,b".Split(','))),

            ("a b",         Expression("a", "b")),
        };

        private static void AssertCanonicalSql(string expectedSql, string actual)
        {
            Assert.AreEqual(
                Util.SqlUtil.CanonicalWhitespace(expectedSql),
                Util.SqlUtil.CanonicalWhitespace(actual),
                actual);
        }
    }
}

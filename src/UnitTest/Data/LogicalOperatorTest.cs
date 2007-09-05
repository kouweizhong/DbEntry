
#region usings

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using org.hanzify.llf.util;
using org.hanzify.llf.Data;
using org.hanzify.llf.Data.Dialect;
using org.hanzify.llf.Data.SqlEntry;

#endregion

namespace org.hanzify.llf.UnitTest.Data
{
    [TestFixture]
    public class LogicalOperatorTest
    {
        [Test]
        public void TestAnd()
        {
            WhereCondition c = CK.K["Id"] > 5 && CK.K["Id"] < 9;
            DataParamterCollection dpc = new DataParamterCollection();
            Assert.AreEqual("([Id] > @Id_0) And ([Id] < @Id_1)", c.ToSqlText(ref dpc, new SqlServer2000()));
        }

        [Test]
        public void TestOr()
        {
            WhereCondition c = CK.K["Id"] > 5 || CK.K["Id"] < 9;
            DataParamterCollection dpc = new DataParamterCollection();
            Assert.AreEqual("([Id] > @Id_0) Or ([Id] < @Id_1)", c.ToSqlText(ref dpc, new SqlServer2000()));
        }

        [Test]
        public void TestTwoWay()
        {
            WhereCondition c = CK.K["Id"] > 5 || CK.K["Id"] < 9;
            WhereCondition c1 = CK.K["Id"] > 5 | CK.K["Id"] < 9;
            DataParamterCollection dpc = new DataParamterCollection();
            DataParamterCollection dpc1 = new DataParamterCollection();
            Assert.AreEqual(c.ToSqlText(ref dpc, new SqlServer2000()), c1.ToSqlText(ref dpc1, new SqlServer2000()));
        }
    }
}
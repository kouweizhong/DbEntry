using System;
using System.Text.RegularExpressions;
using Leafing.Data;
using Leafing.Data.Common;
using Leafing.Data.Definition;
using Leafing.Data.Model;
using Leafing.Data.SqlEntry;
using Leafing.UnitTest.Data.Objects;
using Leafing.Core;
using NUnit.Framework;

namespace Leafing.UnitTest.Data.Inner
{
    #region objects

    class NotPublic : DbObject
    {
        public string Name;
    }

    [Serializable]
    public class TableA : DbObjectModel<TableA>
    {
        public string Name { get; set; }
		public HasOne<TableB> tableB { get; private set; }
    }

    [Serializable]
    public class TableB : DbObjectModel<TableB>
    {
        [Index(UNIQUE = true, IndexName = "Url_TableAId", ASC = false)]
        public string Url { get; set; }

        [Index(UNIQUE = true, IndexName = "Url_TableAId", ASC = false)]
        [DbColumn("TableAId")]
		public BelongsTo<TableA, long> TB { get; private set; }
    }

	public class ClassSite : DbObjectModel<ClassSite> {}

	public class indexSample : DbObjectModel<indexSample>
    {
        [Index(UNIQUE = true, IndexName = "indexname1", ASC = true)]
        [Index(UNIQUE = false, IndexName = "indexname2", ASC = true)]
        public string Name { get; set; }

        [Index(UNIQUE = true, IndexName = "indexname1", ASC = true)]
        [Index(UNIQUE = false, IndexName = "indexname2", ASC = true)]
        [DbColumn("SiteId")]
		public BelongsTo<ClassSite, long> Site { get; private set; }

        [Index(UNIQUE = true, IndexName = "qid", ASC = true)]
        [DbColumn("SiteId"), Index(IndexName = "xxx"), Index(IndexName = "ccc")]
		public BelongsTo<ClassSite, long> Site2 { get; private set; }

		public indexSample ()
		{
			Site = new BelongsTo<ClassSite, long> (this, "SiteId");
			Site2 = new BelongsTo<ClassSite, long> (this, "SiteId");
		}
    }

    #endregion

    [TestFixture]
    public class CommonTest
    {
        [Test]
        public void Test1()
        {
            Assert.IsTrue(Regex.IsMatch("https://localhost", CommonRegular.UrlRegular));
            Assert.IsTrue(Regex.IsMatch("http://a.b.c", CommonRegular.UrlRegular));
            Assert.IsTrue(Regex.IsMatch("https://a.b.c/", CommonRegular.UrlRegular));
            Assert.IsTrue(Regex.IsMatch("http://a.b.c/a.html?a=bcd&e=12.3", CommonRegular.UrlRegular));
            Assert.IsFalse(Regex.IsMatch("httpss://a.b.c", CommonRegular.UrlRegular));
            Assert.IsFalse(Regex.IsMatch("a.http://a.b.c", CommonRegular.UrlRegular));
            Assert.IsFalse(Regex.IsMatch("http://a.b.c/a.html?a=bcd&e=aaa()", CommonRegular.UrlRegular));
        }

        [Test]
        public void TestOrderByParse()
        {
            Assert.IsNull(OrderBy.Parse(""));
            Assert.IsNull(OrderBy.Parse(null));

            const string s = "Id desc, Name";
            var exp = new OrderBy((DESC)"Id", (ASC)"Name");
            var dst = OrderBy.Parse(s);
            var ds = new DataParameterCollection();
            string expStr = exp.ToSqlText(ds, DbEntry.Provider.Dialect);
            string dstStr = dst.ToSqlText(ds, DbEntry.Provider.Dialect);
            Assert.AreEqual(expStr, dstStr);
        }

        [Test]
        public void TestCloneObject()
        {
			var p = new People {Id = 10, Name = "abc"}; p.pc.Value = new PCs { Name = "uuu" };

            var p1 = (People)ModelContext.CloneObject(p);
            Assert.AreEqual(10, p1.Id);
            Assert.AreEqual("abc", p1.Name);
            // Assert.IsNull(p1.pc);
        }

        [Test]
        public void TestBaseType()
        {
            var oi = ModelContext.GetInstance(typeof(People)).Info;
            Assert.AreEqual("People", oi.HandleType.Name);
        }

        [Test]
        public void TestBaseType2()
        {
            Type t = new People().GetType();
            ObjectInfo oi = ModelContext.GetInstance(t).Info;
            Assert.AreEqual("People", oi.HandleType.Name);
        }

        [Test]
        public void TestIndexes()
        {
            var t = typeof (indexSample);
            var f = t.GetProperty("Name");
            var os = ClassHelper.GetAttributes<IndexAttribute>(f, false);
            Assert.AreEqual(2, os.Length);
        }

        [Test]
        public void TestIndexes2()
        {
            var t = typeof(indexSample);
            var f = t.GetProperty("Site");
            var os = ClassHelper.GetAttributes<IndexAttribute>(f, false);
            Assert.AreEqual(2, os.Length);

            f = t.GetProperty("Site2");
            os = ClassHelper.GetAttributes<IndexAttribute>(f, false);
            Assert.AreEqual(3, os.Length);
        }

        [Test]
        public void TestX()
        {
            DbEntry.DropAndCreate(typeof(TableA));
            DbEntry.DropAndCreate(typeof(TableB));

            var t1 = new TableA {Name = "TestName1"};
            t1.Save();

            var t2 = TableA.FindById(1);
            var t3 = new TableB {Url = "TestUrl1"};
			t3.TB.Value = t2;
            t3.Validate();
            t3.Save();
        }

        [Test]
        public void TestNotPublicClass()
        {
            try
            {
                var obj = new NotPublic {Name = "tom"};
                DbEntry.Insert(obj);
                Assert.IsTrue(false);
            }
            catch (DataException ex)
            {
                Assert.AreEqual("[Leafing.UnitTest.Data.Inner.NotPublic]The model class should be public.", ex.Message);
            }
        }
    }
}

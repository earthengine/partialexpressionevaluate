using NUnit.Framework;
using PartialExpressionEvaluate;
using System;

namespace TestPartialEvaluate
{
    [TestFixture]
    public class UnitTest1
    {
        [Test]
        public void TestSimpleMethod()
        {
            var r = PartialEvaluator.PartialEvaluate<int,int,int>((i1, i2) => i1 + i2, 10);
            Assert.That(r.Compile()(20), Is.EqualTo(30));
        }

        [Test]
        public void TestFunctionCall()
        {
            var r = PartialEvaluator.PartialEvaluate<int, int, string>((i1, i2) => string.Format("{0},{1}",i1, i2), 10);
            Assert.That(r.Compile()(20), Is.EqualTo("10,20"));
        }

        [Test]
        public void TestUnary()
        {
            var r = PartialEvaluator.PartialEvaluate<int, int, int>((i1, i2) => i1, 10);
            Assert.That(r.Compile()(20), Is.EqualTo(10));
        }
        [Test]
        public void TestLambda()
        {
            var r = PartialEvaluator.PartialEvaluate<int, int, int>((i1, i2) => i1 + ((Func<int,int>)((x)=>x*i1))(i2), 10);
            Assert.That(r.Compile()(20), Is.EqualTo(210));
        }
    }
}

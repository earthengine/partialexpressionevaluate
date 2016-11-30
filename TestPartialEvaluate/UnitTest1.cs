using NUnit.Framework;
using PartialExpressionEvaluate;
using System;
using System.Linq.Expressions;

namespace TestPartialEvaluate
{
    [TestFixture]
    public class UnitTest1
    {
        [Test]
        public void TestSimpleMethod()
        {
            Expression<Func<int, int, int>> v = (i1, i2) => i1 + i2;
            var s = v.ToString();

            var r = PartialEvaluator.PartialEvaluate<int,int,int>((i1, i2) => i1 + i2, 10);
            Assert.That(r.Compile()(20), Is.EqualTo(30));
        }

        [Test]
        public void TestFunctionCall()
        {
            var g = Expression.Lambda(Expression.IsTrue(Expression.Constant(true))) as Expression<Func<bool>>;
            var r = PartialEvaluator.PartialEvaluate<int, int, string>((i1, i2) => string.Format("{0},{1}",i1+5+(-i1), -i2), 10);
            Assert.That(r.Compile()(20), Is.EqualTo("5,-20"));
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
        [Test]
        public void InterpreterToExecutable()
        {
            var r = PartialEvaluator.PartialEvaluate<Expression<Func<int,int>>, int, int>((f,i) => f.Compile()(i), i => i*100);
            Assert.That(r.Compile()(20), Is.EqualTo(2000));
        }
        [Test]
        public void InterpreterToCompiler()
        {
            var r = PartialEvaluator.PartialEvaluate<Expression<Func<int, int>>, int, int>((f, i) => f.Compile()(i), i => i * 100);
            Assert.That(r.Compile()(20), Is.EqualTo(2000));
        }

        //private delegate T Loop<T>(Loop<T> l, T t);

        //private Func<Func<T,T>,T> Y<T>()
        //{
        //    Func<Func<T, T>, Loop<T>> v = f => (Loop<T>)((l, t) => f(l(l, t)));
            
        //}

        //private Expression<Func<string, int, Func<char,string,int,int>, int>> CheckDigit()
        //{
        //    return (s, start, c) =>            
        //        s.Length < start ? -1 : (
        //            s.Length == start ? 0 : (                        
        //                s[start] >= '0' && s[start] <= '9' ?
        //                    c(s[start], s, start + 1) : -1));
        //}

        //private Expression<Func<string, int, int>> SimpleInterpreter()
        //{
        //    var e1 = CheckDigit();
        //    Expression<Loop<Tuple<char, string, int, int>>> e2 = (l2, t) => l2(c - '0', ss, st);
        //    var s = Expression.Variable(typeof(string));
        //    var start = Expression.Variable(typeof(int));
        //    return Expression.Lambda(Expression.Invoke(e1, s, start, e2), s, start) as Expression<Func<string, int, int>>;

        //}

        private int SimpleInterpreter(int v, string s, int start)
        {
            if (s.Length < start) return -1;
            if (s.Length == start) return v;
            if (s[start] >= '0' && s[start] <= 9)
            {
                return SimpleInterpreter(v, s, start + 1);
            } else if (s[start] == '+')
            {
                var r = SimpleInterpreter(s, start + 1);
                if (r < 0) return -1;
                return v + r;
            } else return -1;
        }

        [Test]
        public void Examine()
        {
            var r = PartialEvaluator.PartialEvaluate<int, int, int>((k, u) => k * (k * (k + 1) + u + 1) + u * u, 2);
            Assert.That(r.Compile()(20), Is.EqualTo(2 * (7 + 20) + 20 * 20));
        }
    }
}

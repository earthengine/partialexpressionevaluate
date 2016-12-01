using NUnit.Framework;
using PartialExpressionEvaluate;
using PartialExpressionEvaluate.SimpleExpression;
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

        [Test]
        public void Examine()
        {
            var r = PartialEvaluator.PartialEvaluate<int, int, int>((k, u) => k * (k * (k + 1) + u + 1) + u * u, 2);
            Assert.That(r.Compile()(20), Is.EqualTo(2 * (7 + 20) + 20 * 20));
            var ex = PartialEvaluator.PartialEvaluateExp<int, int, int>();
        }

        delegate Func<T,T> Loop<T>(Loop<T> l);

        public Expression<Func<Func<Func<T, T>, Func<T, T>>, Func<T, T>>> Y<T>()
        {
            return h => ((Loop<T>)(x => h(t => x(x)(t))))(x => h(x(x)));
        }

        [Test]
        public void TestSimpleExpression()
        {
            Func<string, int> env = 
                s => { if (s == "x") return 5; else if (s == "y") return 7; else throw new Exception(""); };

            var exp = SimpleExpressions.OfAdd(SimpleExpressions.OfVar("x"), SimpleExpressions.OfMul(SimpleExpressions.OfNum(2),
                SimpleExpressions.OfVar("y")));
            var r = exp.Eval(env);

            Assert.That(r, Is.EqualTo(19));
        }
    }
}

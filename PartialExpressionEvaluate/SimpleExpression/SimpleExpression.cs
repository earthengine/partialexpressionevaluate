using System;

namespace PartialExpressionEvaluate.SimpleExpression
{
    public interface ISimpleExpression
    {
        int Eval(Func<string, int> env);
    }

    internal class NumType : ISimpleExpression
    {
        private readonly int value;

        internal NumType(int v)
        {
            value = v;
        }

        public int Eval(Func<string, int> env)
        {
            return value;
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }

    internal class VarExpression : ISimpleExpression
    {
        private readonly string name;

        internal VarExpression(string name)
        {
            this.name = name;
        }

        public int Eval(Func<string, int> env)
        {
            return env(name);
        }

        public override string ToString()
        {
            return name;
        }
    }

    internal class AddExpression : ISimpleExpression
    {
        private readonly ISimpleExpression left;
        private readonly ISimpleExpression right;

        internal AddExpression(ISimpleExpression left, ISimpleExpression right)
        {
            this.left = left;
            this.right = right;
        }

        public int Eval(Func<string, int> env)
        {
            return left.Eval(env) + right.Eval(env);
        }

        public override string ToString()
        {
            return $"({left}+{right})";
        }
    }

    internal class MulExpression : ISimpleExpression
    {
        private readonly ISimpleExpression left;
        private readonly ISimpleExpression right;

        internal MulExpression(ISimpleExpression left, ISimpleExpression right)
        {
            this.left = left;
            this.right = right;
        }

        public int Eval(Func<string, int> env)
        {
            return left.Eval(env) * right.Eval(env);
        }

        public override string ToString()
        {
            return $"({left}*{right})";
        }
    }

    public static class SimpleExpressions
    {
        public static ISimpleExpression OfNum(int i)
        {
            return new NumType(i);
        }
        public static ISimpleExpression OfVar(string name)
        {
            return new VarExpression(name);
        }
        public static ISimpleExpression OfAdd(ISimpleExpression e1, ISimpleExpression e2)
        {
            return new AddExpression(e1, e2);
        }
        public static ISimpleExpression OfMul(ISimpleExpression e1, ISimpleExpression e2)
        {
            return new MulExpression(e1, e2);
        }
    }
}

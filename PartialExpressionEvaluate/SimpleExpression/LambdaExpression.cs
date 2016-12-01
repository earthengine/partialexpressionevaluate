using System;
using System.Collections.Generic;

namespace PartialExpressionEvaluate.SimpleExpression.Lambda
{
    public interface ILambdaValue
    {
        bool IsNum();
        int GetNumValue();
    }

    public interface ILambdaExpression
    {
        ILambdaValue Eval(Dictionary<string, ILambdaValue> env);
    }
    internal class IntExpression : ILambdaExpression
    {
        private readonly int value;

        internal IntExpression(int value)
        {
            this.value = value;
        }

        public ILambdaValue Eval(Dictionary<string, ILambdaValue> env)
        {
            return new NumberValue(value);
        }
    }

    internal class NumberValue : ILambdaValue
    {
        private int value;

        internal NumberValue(int value)
        {
            this.value = value;
        }

        public int GetNumValue()
        {
            return value;
        }

        public bool IsNum()
        {
            return true;
        }
    }

    internal class VarExpression : ILambdaExpression
    {
        private readonly string name;

        internal VarExpression(string name)
        {
            this.name = name;
        }

        public ILambdaValue Eval(Dictionary<string, ILambdaValue> env)
        {
            return env[name];
        }
    }

    internal class AbsExpression : ILambdaExpression
    {
        private readonly string argName;
        private readonly ILambdaExpression body;

        internal AbsExpression(string argName, ILambdaExpression body)
        {
            this.argName = argName;
            this.body = body;
        }

        public ILambdaValue Eval(Dictionary<string, ILambdaValue> env)
        {
            return new ClosureValue(argName, body, env);
        }
    }

    internal class ClosureValue : ILambdaValue
    {
        private string argName;
        private ILambdaExpression body;
        private Dictionary<string, ILambdaValue> env;

        internal ClosureValue(string argName, ILambdaExpression body, Dictionary<string, ILambdaValue> env)
        {
            this.argName = argName;
            this.body = body;
            this.env = env;
        }

        public int GetNumValue()
        {
            throw new InvalidOperationException("Closure is not number");
        }

        public bool IsNum()
        {
            return false;
        }

        internal ILambdaValue Eval(ILambdaValue arg, Dictionary<string, ILambdaValue> env)
        {
            var ev1 = new Dictionary<string, ILambdaValue>(env);
            ev1.Add(argName, arg);
            return body.Eval(env);
        }
    }

    internal class ApplyExpression : ILambdaExpression
    {
        private readonly ILambdaExpression f;
        private readonly ILambdaExpression arg;

        public ApplyExpression(ILambdaExpression f, ILambdaExpression arg)
        {
            this.f = f;
            this.arg = arg;
        }

        public ILambdaValue Eval(Dictionary<string, ILambdaValue> env)
        {
            var arg1 = arg.Eval(env);
            var c = f.Eval(env) as ClosureValue;
            if (c != null)
            {
                return c.Eval(arg1, env);
            }
            else throw new InvalidOperationException("Apply a non-function");

        }
    }

    internal class AddExpression : ILambdaExpression
    {
        private readonly ILambdaExpression left;
        private readonly ILambdaExpression right;

        internal AddExpression(ILambdaExpression left, ILambdaExpression right)
        {
            this.left = left;
            this.right = right;
        }

        public ILambdaValue Eval(Dictionary<string, ILambdaValue> env)
        {
            var v1 = left.Eval(env);
            var v2 = right.Eval(env);
            if (v1 is NumberValue && v2 is NumberValue)
            {
                return new NumberValue((v1 as NumberValue).GetNumValue() + (v2 as NumberValue).GetNumValue());
            }
            else throw new InvalidOperationException("Adding non-numbers");
        }
    }

    internal class IfExpression : ILambdaExpression
    {
        private readonly ILambdaExpression condition;
        private readonly ILambdaExpression trueValue;
        private readonly ILambdaExpression falseValue;

        public IfExpression(ILambdaExpression cond, ILambdaExpression t, ILambdaExpression f)
        {
            condition = cond;
            trueValue = t;
            falseValue = f;
        }

        public ILambdaValue Eval(Dictionary<string, ILambdaValue> env)
        {
            var b = condition.Eval(env);
            if (b is NumberValue)
            {
                if ((b as NumberValue).GetNumValue() == 1)
                    return trueValue.Eval(env);
                else return falseValue.Eval(env);
            }
            else throw new InvalidOperationException("Condition is not number");
        }
    }

    public static class LambdaExpressions {
        public static ILambdaExpression IntExp(int i)
        {
            return new IntExpression(i);
        }
        public static ILambdaExpression VarExp(string name)
        {
            return new VarExpression(name);
        }
        public static ILambdaExpression AbsExp(string argName, ILambdaExpression body)
        {
            return new AbsExpression(argName, body);
        }
        public static ILambdaExpression ApplyExp(ILambdaExpression f, ILambdaExpression arg)
        {
            return new ApplyExpression(f, arg);
        }
        public static ILambdaExpression AddExp(ILambdaExpression left, ILambdaExpression right)
        {
            return new AddExpression(left, right);
        }
        public static ILambdaExpression IfExp(ILambdaExpression cond, ILambdaExpression t, ILambdaExpression f)
        {
            return new IfExpression(cond, t, f);
        }

        public static ILambdaValue Interpret(ILambdaExpression e)
        {
            return e.Eval(new Dictionary<string, ILambdaValue>());
        }
    }
}

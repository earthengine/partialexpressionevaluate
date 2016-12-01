using System;
using System.Collections.Generic;
using System.Linq;

namespace PartialExpressionEvaluate.SimpleExpression.FlowChart
{
    public interface IExpr
    {
        int Eval(Dictionary<string, int> env);
    }

    public interface ICommand
    {
        int Run(int label, Dictionary<string,int> env, Program p);
    }
    public class Program
    {
        private readonly List<ICommand> cmds;
        private readonly List<string> vars;

        internal Program(IEnumerable<string> vars, IEnumerable<ICommand> cmds){
            this.vars = vars.ToList();
            this.cmds = cmds.ToList();
        }
        internal ICommand GetCommand(int label)
        {
            return cmds[label];
        }

        internal Dictionary<string, int> GetEnv(IEnumerable<int> args)
        {
            return vars.Zip(args, (s, i) => new { s, i })
                       .ToDictionary(x => x.s, x => x.i);
        }
    }
    internal class GotoCommand : ICommand
    {
        private readonly int label;

        internal GotoCommand(int label)
        {
            this.label = label;
        }

        public int Run(int label, Dictionary<string, int> env, Program p)
        {
            return p.GetCommand(label).Run(label, env, p);
        }
    }
    internal class AssignCommand : ICommand
    {
        private readonly string var;
        private readonly IExpr exp;
        internal AssignCommand(string var, IExpr exp)
        {
            this.var = var;
            this.exp = exp;
        }

        public int Run(int label, Dictionary<string, int> env, Program p)
        {
            var ne = new Dictionary<string,int>(env);
            ne[var]=exp.Eval(env);
            return p.GetCommand(label + 1).Run(label + 1, ne, p);

        }
    }
    internal class IfCommand : ICommand
    {
        private readonly IExpr cond;
        private readonly int trueLabel;
        private readonly int falseLabel;
        internal IfCommand(IExpr cond, int trueLabel, int falseLabel)
        {
            this.cond = cond;
            this.trueLabel = trueLabel;
            this.falseLabel = falseLabel;
        }

        public int Run(int label, Dictionary<string, int> env, Program p)
        {
            var v = cond.Eval(env);
            if (v == 0)
                return p.GetCommand(falseLabel).Run(falseLabel, env, p);
            else
                return p.GetCommand(trueLabel).Run(trueLabel, env, p);
        }
    }
    internal class ReturnCommand : ICommand
    {
        private readonly IExpr value;
        internal ReturnCommand(IExpr value)
        {
            this.value = value;
        }

        public int Run(int label, Dictionary<string, int> env, Program p)
        {
            return value.Eval(env);
        }
    }
    internal class IntExpr : IExpr
    {
        private readonly int value;
        internal IntExpr(int value)
        {
            this.value = value;            
        }

        public int Eval(Dictionary<string, int> env)
        {
            return value;
        }
    }
    internal class VarExpr : IExpr
    {
        private readonly string name;
        internal VarExpr(string name)
        {
            this.name = name;
        }

        public int Eval(Dictionary<string, int> env)
        {
            return env[name];
        }
    }
    internal class UnaryExpr : IExpr
    {
        private Func<int, int> op;
        private IExpr exp;

        public int Eval(Dictionary<string, int> env)
        {
            return op(exp.Eval(env));
        }
    }
    internal class BinaryExpr : IExpr
    {
        private Func<int, int, int> op;
        private IExpr left;
        private IExpr right;

        public BinaryExpr(IExpr left, IExpr right, Func<int, int, int> op)
        {
            this.left = left;
            this.right = right;
            this.op = op;
        }

        public int Eval(Dictionary<string, int> env)
        {
            return op(left.Eval(env), right.Eval(env));
        }
    }


    public static class FlowChart
    {
        public static IExpr IntExp(int i)
        {
            return new IntExpr(i);
        }
        public static IExpr VarExp(string name)
        {
            return new VarExpr(name);
        }
        public static ICommand GotoCmd(int label)
        {
            return new GotoCommand(label);
        }
        public static ICommand AssignCmd(string var, IExpr exp)
        {
            return new AssignCommand(var, exp);
        }
        public static ICommand IfCmd(IExpr exp, int ltrue, int lfalse)
        {
            return new IfCommand(exp, ltrue, lfalse);
        }
        public static ICommand ReturnCmd(IExpr v)
        {
            return new ReturnCommand(v);
        }
        public static Program Prog(IEnumerable<string> vars, IEnumerable<ICommand> cmds)
        {
            return new Program(vars, cmds);
        }

        public static int Interpret(Program pgm, IEnumerable<int> args)
        {
            return pgm.GetCommand(0).Run(1, pgm.GetEnv(args), pgm);
        }
        public static IExpr AddExp(IExpr left, IExpr right)
        {
            return new BinaryExpr(left, right, (x, y) => x + y);
        }
    }   
}

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace PartialExpressionEvaluate
{
    public class ExpressionEvaluator : ExpressionVisitor
    {
        public override Expression Visit(Expression node)
        {
            if (node != null)
            {
                switch (node.NodeType)
                {                   
                    //UnaryExpression, eval to self
                    case ExpressionType.Convert:
                    case ExpressionType.ConvertChecked:
                        break;

                    //UnaryExpression
                    case ExpressionType.Negate:
                    case ExpressionType.NegateChecked:
                    case ExpressionType.Not:
                    case ExpressionType.ArrayLength:
                    case ExpressionType.Quote:
                    case ExpressionType.TypeAs:
                    case ExpressionType.Decrement:
                    case ExpressionType.Increment:
                    case ExpressionType.IsFalse:
                    case ExpressionType.IsTrue:
                    case ExpressionType.OnesComplement:
                    case ExpressionType.Throw:
                    case ExpressionType.UnaryPlus:
                    case ExpressionType.Unbox:
                        break;

                    //BinaryExpression
                    case ExpressionType.Add:
                    case ExpressionType.AddChecked:
                    case ExpressionType.AndAlso:
                    case ExpressionType.And:
                    case ExpressionType.Equal:
                    case ExpressionType.ExclusiveOr:
                    case ExpressionType.Divide:
                    case ExpressionType.GreaterThan:
                    case ExpressionType.GreaterThanOrEqual:
                    case ExpressionType.LeftShift:
                    case ExpressionType.Modulo:
                    case ExpressionType.LessThan:
                    case ExpressionType.LessThanOrEqual:
                    case ExpressionType.Coalesce:
                    case ExpressionType.Multiply:
                    case ExpressionType.MultiplyChecked:
                    case ExpressionType.NotEqual:
                    case ExpressionType.Or:
                    case ExpressionType.OrElse:
                    case ExpressionType.Power:
                    case ExpressionType.RightShift:
                    case ExpressionType.Subtract:
                    case ExpressionType.SubtractChecked:
                        break;
                    
                    //BinaryExpression, assign
                    case ExpressionType.AddAssign:
                    case ExpressionType.AddAssignChecked:
                    case ExpressionType.AndAssign:
                    case ExpressionType.Assign:
                    case ExpressionType.ExclusiveOrAssign:
                    case ExpressionType.LeftShiftAssign:
                    case ExpressionType.ModuloAssign:
                    case ExpressionType.MultiplyAssign:
                    case ExpressionType.MultiplyAssignChecked:
                    case ExpressionType.OrAssign:
                    case ExpressionType.PostDecrementAssign:
                    case ExpressionType.PostIncrementAssign:
                    case ExpressionType.PowerAssign:
                    case ExpressionType.PreDecrementAssign:
                    case ExpressionType.PreIncrementAssign:
                    case ExpressionType.RightShiftAssign:
                    case ExpressionType.SubtractAssign:
                    case ExpressionType.SubtractAssignChecked:
                    case ExpressionType.DivideAssign:
                        break;

                    //ConstantExpression
                    case ExpressionType.Constant:
                        break;

                    //DefaultExpression
                    case ExpressionType.Default:
                        break;

                    //BinaryExpression or MethodCallExpression
                    case ExpressionType.ArrayIndex:
                        break;
                    
                    //BlockExpression
                    case ExpressionType.Block:
                        break;
                    
                    //MethodCallExpression
                    case ExpressionType.Call:
                        break;
                    
                    //ConditionalExpression
                    case ExpressionType.Conditional:
                        break;
                    
                    //DebugInfoExpression
                    case ExpressionType.DebugInfo:
                        break;

                    //DynamicExpression
                    case ExpressionType.Dynamic:
                        break;

                    //??
                    case ExpressionType.Extension:
                        break;
                    
                    //GotoExpression
                    case ExpressionType.Goto:
                        break;

                    //IndexExpression
                    case ExpressionType.Index:
                        break;
                    
                    //InvocationExpression
                    case ExpressionType.Invoke:
                        break;

                    //LabelExpression
                    case ExpressionType.Label:
                        break;
                    
                    //LambdaExpression
                    case ExpressionType.Lambda:
                        break;
                    
                    //ListInitExpression
                    case ExpressionType.ListInit:
                        break;
                    case ExpressionType.Loop:
                        break;
                    
                    //MemberExpression
                    case ExpressionType.MemberAccess:
                        break;
                    
                    //MemberInitExpression
                    case ExpressionType.MemberInit:
                        break;
                    
                    //NewExpression
                    case ExpressionType.New:
                        break;
                    
                    //NewArrayExpression
                    case ExpressionType.NewArrayBounds:
                        break;
                    
                    //NewArrayExpression
                    case ExpressionType.NewArrayInit:
                        break;
                    
                    //ParameterExpression
                    case ExpressionType.Parameter:
                        break;
                    
                    //RunTimeVariablesExpression
                    case ExpressionType.RuntimeVariables:
                        break;

                    //SwitchExpression
                    case ExpressionType.Switch:
                        break;
                    
                    //TryExpression
                    case ExpressionType.Try:
                        break;
                    
                    //TypeBinaryExpression
                    case ExpressionType.TypeEqual:
                    case ExpressionType.TypeIs:
                        break;
                }
            }
            return base.Visit(node);
        }
    }

    public static class ExpressionHelper
    {
        public static Expression<Func<T>> Add<T>(Expression<Func<T>> p1, Expression<Func<T>> p2)
        {
            return Expression.Lambda(Expression.Add(p1.Body, p2.Body)) as Expression<Func<T>>;
        }
    }

    public class DelegateEvaluationVisitor : ExpressionVisitor
    {
        private readonly Func<Expression, Expression> visit;

        public DelegateEvaluationVisitor(Func<Expression, Expression> visit)
        {
            this.visit = visit;
        }
        public override Expression Visit(Expression node)
        {
            var r = base.Visit(node);
            return visit(r);
        }
    }

    public class PartialEvaluationVisitor<T> : ExpressionVisitor
    {
        private readonly ParameterExpression p;
        private T t1;
        public PartialEvaluationVisitor(T t1, ParameterExpression p)
        {
            this.t1 = t1;
            this.p = p;
        }

        public override Expression Visit(Expression exp)
        {
            if (exp == null) return null;
            switch (exp.NodeType)
            {
                case ExpressionType.ConvertChecked:
                case ExpressionType.Convert:
                    return VisitUnary((UnaryExpression)exp);
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    {
                        var r = VisitUnary((UnaryExpression)exp) as UnaryExpression;
                        if (r.Operand is ConstantExpression)
                        {
                            return Expression.Constant(Expression.Lambda(r).Compile().DynamicInvoke());
                        }
                        return r;
                    }
                case ExpressionType.Parameter:                    
                    return exp == p ?
                        Expression.Constant(t1) :
                        VisitParameter((ParameterExpression)exp);
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.ExclusiveOr:
                    {
                        var r = VisitBinary((BinaryExpression)exp) as BinaryExpression;
                        if (r.Left is ConstantExpression && r.Right is ConstantExpression)
                        {
                            return Expression.Constant(Expression.Lambda(r).Compile().DynamicInvoke());
                        }
                        return r;
                    }
                case ExpressionType.TypeIs:
                    return VisitTypeBinary((TypeBinaryExpression)exp);
                case ExpressionType.Conditional:
                    return VisitConditional((ConditionalExpression)exp);
                case ExpressionType.Constant:
                    return VisitConstant((ConstantExpression)exp);
                case ExpressionType.MemberAccess:
                    return VisitMember((MemberExpression)exp);
                case ExpressionType.Call:
                    return VisitMethodCall((MethodCallExpression)exp);
                case ExpressionType.Lambda:
                    var mi = typeof(ExpressionVisitor).GetRuntimeMethods().First(
                        x => x.Name=="VisitLambda");
                    mi = mi.MakeGenericMethod(exp.Type);
                    return (LambdaExpression)mi.Invoke(this, new object[] { exp });
                case ExpressionType.New:
                    return VisitNew((NewExpression)exp);
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    return VisitNewArray((NewArrayExpression)exp);
                case ExpressionType.Invoke:
                    return VisitInvocation((InvocationExpression)exp);
                case ExpressionType.MemberInit:
                    return VisitMemberInit((MemberInitExpression)exp);
                case ExpressionType.ListInit:
                    return VisitListInit((ListInitExpression)exp);
                default:
                    throw new Exception(string.Format("Unhandled expression type: '{0}'", exp.NodeType));
            }
        }
    }

    public class PartialEvaluator
    {
        public static Expression<Func<T2, R>> PartialEvaluate<T1,T2,R>(Expression<Func<T1,T2,R>> expr, T1 t1)
        {
            var vis = new DelegateEvaluationVisitor(exp =>
            {
                return
                    exp == null ? null : (
                    exp.NodeType == ExpressionType.Parameter && exp == expr.Parameters[0] ?
                                (exp == expr.Parameters[0] ? Expression.Constant(t1) : exp) : (
                    new[] { ExpressionType.Negate, ExpressionType.NegateChecked, ExpressionType.Not,
                            ExpressionType.ArrayLength, ExpressionType.Quote,
                            ExpressionType.TypeAs}.Contains(exp.NodeType) ?
                                ((exp as UnaryExpression).Operand is ConstantExpression ?
                                    Expression.Constant(Expression.Lambda(exp).Compile().DynamicInvoke()) : exp) : (
                    new[] { ExpressionType.Add,
                            ExpressionType.AddChecked, ExpressionType.Subtract,
                            ExpressionType.SubtractChecked, ExpressionType.Multiply,
                            ExpressionType.MultiplyChecked, ExpressionType.Divide,
                            ExpressionType.Modulo, ExpressionType.And, ExpressionType.AndAlso,
                            ExpressionType.Or, ExpressionType.OrElse, ExpressionType.LessThan,
                            ExpressionType.LessThanOrEqual, ExpressionType.GreaterThan,
                            ExpressionType.GreaterThanOrEqual, ExpressionType.Equal,
                            ExpressionType.NotEqual, ExpressionType.Coalesce,
                            ExpressionType.ArrayIndex, ExpressionType.RightShift,
                            ExpressionType.LeftShift, ExpressionType.ExclusiveOr }.Contains(exp.NodeType) ?
                                        ((exp as BinaryExpression).Left is ConstantExpression
                                            && (exp as BinaryExpression).Right is ConstantExpression ?
                                                Expression.Constant(Expression.Lambda(exp).Compile().DynamicInvoke()) : exp) : exp)));
            });
            return Expression.Lambda(vis.Visit(expr.Body), expr.Parameters[1]) as Expression<Func<T2, R>>;
        }

        public static Expression<Func<Expression<Func<T1, T2, R>>, T1, Expression<Func<T2,R>>>> PartialEvaluateExp<T1,T2,R>(){
            return (expr, t1) =>
                Expression.Lambda(new DelegateEvaluationVisitor(exp =>
                        exp == null ? null : (
                        exp.NodeType == ExpressionType.Parameter && exp == expr.Parameters[0] ?
                                    (exp == expr.Parameters[0] ? Expression.Constant(t1) : exp) : (
                        new[] { ExpressionType.Negate, ExpressionType.NegateChecked, ExpressionType.Not,
                            ExpressionType.ArrayLength, ExpressionType.Quote,
                            ExpressionType.TypeAs}.Contains(exp.NodeType) ?
                                    ((exp as UnaryExpression).Operand is ConstantExpression ?
                                        Expression.Constant(Expression.Lambda(exp).Compile().DynamicInvoke()) : exp) : (
                        new[] { ExpressionType.Add,
                            ExpressionType.AddChecked, ExpressionType.Subtract,
                            ExpressionType.SubtractChecked, ExpressionType.Multiply,
                            ExpressionType.MultiplyChecked, ExpressionType.Divide,
                            ExpressionType.Modulo, ExpressionType.And, ExpressionType.AndAlso,
                            ExpressionType.Or, ExpressionType.OrElse, ExpressionType.LessThan,
                            ExpressionType.LessThanOrEqual, ExpressionType.GreaterThan,
                            ExpressionType.GreaterThanOrEqual, ExpressionType.Equal,
                            ExpressionType.NotEqual, ExpressionType.Coalesce,
                            ExpressionType.ArrayIndex, ExpressionType.RightShift,
                            ExpressionType.LeftShift, ExpressionType.ExclusiveOr }.Contains(exp.NodeType) ?
                                            ((exp as BinaryExpression).Left is ConstantExpression
                                                && (exp as BinaryExpression).Right is ConstantExpression ?
                                                    Expression.Constant(Expression.Lambda(exp).Compile().DynamicInvoke()) : exp) : exp)))
                ).Visit(expr.Body), expr.Parameters[1]) as Expression<Func<T2, R>>;
        }

    }
}

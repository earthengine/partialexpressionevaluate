using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace PartialExpressionEvaluate
{
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
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    return VisitUnary((UnaryExpression)exp);
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
                    return VisitBinary((BinaryExpression)exp);
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
            var vis = new PartialEvaluationVisitor<T1>(t1, expr.Parameters[0]);
            return Expression.Lambda(vis.Visit(expr.Body), expr.Parameters[1]) as Expression<Func<T2, R>>;
        }
    }
}

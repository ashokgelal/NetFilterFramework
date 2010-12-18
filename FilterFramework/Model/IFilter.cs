#region Using Directives

using System;
using System.Linq.Expressions;

#endregion

namespace FilterFramework.Model
{
    public interface IFilter<T>
    {
        Expression<Func<T, bool>> MyExpressionFunc { get; }
        string LeftExpression { get; }
        string RightExpression { get; }
        string Operator { get; }
        bool IsEnabled { get; set; }
        bool TryChangingOperator(string op);
        bool TryChangingRightExpression(string right);
    }
}
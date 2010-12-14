#region Using Directives

using System;
using System.Linq.Expressions;

#endregion

namespace FilterFramework.Model
{
    public interface IFilter<T>
    {
        Expression<Func<T, bool>> MyExpressionFunc { get; }
    }
}
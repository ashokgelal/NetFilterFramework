#region Using Directives

using System;
using System.Linq.Expressions;

#endregion

namespace FilterFramework.Model
{
    internal class BinaryExpressionFilter<T> : IFilter<T>
    {
        private ParameterExpression MyParameterExpression { get; set; }
        private MemberExpression MyMemberExpression { get; set; }
        private ConstantExpression MyConstantExpression { get; set; }
        private BinaryExpression MyBinaryExpression { get; set; }

        public BinaryExpressionFilter(string left, BinaryOperators op, string right)
        {
            MyParameterExpression = Expression.Parameter(typeof (T), "T");
            MyMemberExpression = Expression.Property(MyParameterExpression, left);
            MyConstantExpression = Expression.Constant(right);
            MyBinaryExpression = BinaryOperatorFactory.CreateBinaryExpression(MyMemberExpression, op,
                                                                              MyConstantExpression);

            MyExpressionFunc = Expression.Lambda<Func<T, bool>>(MyBinaryExpression, MyParameterExpression);
        }

        public Expression<Func<T, bool>> MyExpressionFunc { get; private set; }
    }

    internal enum BinaryOperators
    {
        Equals = 0,
        LessThanOrEquals,
    }

    internal class BinaryOperatorFactory
    {
        internal static BinaryExpression CreateBinaryExpression(Expression left, BinaryOperators op,
                                                                ConstantExpression right)
        {
            switch (op)
            {
                case BinaryOperators.Equals:
                    return Expression.Equal(left, right);
            }
            throw new InvalidOperationException("Invalid Operator for Filter");
        }
    }
}
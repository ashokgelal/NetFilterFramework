#region Using Directives

using System;
using System.Linq.Expressions;

#endregion

namespace FilterFramework.Model
{
    internal class BinaryExpressionFilter<T> : IFilter<T>
    {
        private static readonly ParameterExpression MyParameterExpression = Expression.Parameter(typeof (T), "T");
        private MemberExpression MyLeftExpression { get; set; }
        private ConstantExpression MyRightExpression { get; set; }
        private BinaryExpression MyBinaryExpression { get; set; }
        public Expression<Func<T, bool>> MyExpressionFunc { get; private set; }

        public string LeftExpression
        {
            get { return MyLeftExpression.Member.Name; }
        }

        public string RightExpression
        {
            get { return MyRightExpression.Value.ToString(); }
        }

        public string Operator { get; private set; }

        public bool IsEnabled { get; set; }

        public BinaryExpressionFilter(string left, string op, string right)
        {
            MyLeftExpression = Expression.Property(MyParameterExpression, left);
            MyRightExpression = Expression.Constant(right);
            MyBinaryExpression = BinaryOperatorFactory.CreateBinaryExpression(MyLeftExpression, op,
                                                                              MyRightExpression);
            Operator = op;
            MyExpressionFunc = Expression.Lambda<Func<T, bool>>(MyBinaryExpression, MyParameterExpression);
            MyExpressionFunc.Compile();
        }
    }

    internal class BinaryOperatorFactory
    {
        internal static BinaryExpression CreateBinaryExpression(Expression left, string op,
                                                                ConstantExpression right)
        {
            switch (op)
            {
                case "=":
                    return Expression.Equal(left, right);
            }
            throw new InvalidOperationException("Invalid Operator for Filter");
        }
    }
}
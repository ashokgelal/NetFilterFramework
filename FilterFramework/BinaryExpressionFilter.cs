////////////////////////////////////////////////////////////////
//
// Copyright (c) 2011, Ashok Gelal
//
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at 
//
//	http://www.apache.org/licenses/LICENSE-2.0 
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License. 
//
////////////////////////////////////////////////////////////////

//
// Author: Ashok Gelal (http://ashokgelal.com)
// On github: https://github.com/ashokgelal/NetFilterFramework
//


﻿#region Using Directives

using System;
using System.Linq.Expressions;

#endregion

namespace FilterFramework
{
    internal class BinaryExpressionFilter<T> : IFilter<T>
    {
        #region Members and Properties

        private static readonly ParameterExpression MyParameterExpression = Expression.Parameter(typeof(T), "T");
        private MemberExpression MyLeftExpression { get; set; }
        private ConstantExpression MyRightExpression { get; set; }
        private BinaryExpression MyBinaryExpression { get; set; }
        public Expression<Func<T, bool>> MyExpressionFunc { get; private set; }

        public string LeftExpression { get; private set; }

        public string RightExpression { get; private set; }

        public string Operator { get; private set; }

        public bool IsEnabled { get; set; }

        #endregion

        #region Constructor, Initialization, and Disposal

        public BinaryExpressionFilter(string left, string op, string right)
        {
            LeftExpression = left;
            RightExpression = right;
            MyLeftExpression = Expression.Property(MyParameterExpression, left);
            MyRightExpression = BinaryOperatorFactory.CreateConstantExpression(MyLeftExpression, right);
            TryChangingOperator(op, MyRightExpression);
        }

        #endregion

        #region Methods

        public bool TryChangingRightExpression(string right)
        {
            try
            {
                var exp = BinaryOperatorFactory.CreateConstantExpression(MyLeftExpression, right);
                if (TryChangingOperator(Operator, exp))
                {
                    MyRightExpression = exp;
                    RightExpression = right;
                    // have to reset the expressionfunc
                    SetExpressionFunc();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool TryChangingOperator(string op)
        {
            return TryChangingOperator(op, MyRightExpression);
        }

        private bool TryChangingOperator(string op, ConstantExpression rightExp)
        {
            try
            {
                var exp = BinaryOperatorFactory.CreateBinaryExpression(MyLeftExpression, op,
                                                                       rightExp);
                if (exp == null)
                    return false;

                MyBinaryExpression = exp;
                Operator = op;
                SetExpressionFunc();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void SetExpressionFunc()
        {
            MyExpressionFunc = Expression.Lambda<Func<T, bool>>(MyBinaryExpression, MyParameterExpression);
            MyExpressionFunc.Compile();
        }

        public override string ToString()
        {
            return String.Format("{0} {1} {2}", LeftExpression, Operator, RightExpression);
        }

        #endregion
    }

    internal static class BinaryOperatorFactory
    {
        internal static BinaryExpression CreateBinaryExpression(Expression left, string op,
                                                                ConstantExpression right)
        {
            switch (op)
            {
                case "=":
                    return Expression.Equal(left, right);
                case ">=":
                    return Expression.GreaterThanOrEqual(left, right);
                case "<=":
                    return Expression.LessThanOrEqual(left, right);
                case ">":
                    return Expression.GreaterThan(left, right);
                case "<":
                    return Expression.LessThan(left, right);
                case "!=":
                    return Expression.NotEqual(left, right);
            }
            return null;
        }

        internal static ConstantExpression CreateConstantExpression(MemberExpression left, string right)
        {
            var type = left.Type;

            // TODO: address culture
            if (type == typeof(int))
                return Expression.Constant(Int32.Parse(right));
            if (type == typeof(double))
                return Expression.Constant(Double.Parse(right));
            if (type.BaseType == typeof(Enum))
                return Expression.Constant(Enum.Parse(type, right));
            if (type == typeof(Boolean))
                return Expression.Constant(Boolean.Parse(right));
            return Expression.Constant(right);
        }
    }
}
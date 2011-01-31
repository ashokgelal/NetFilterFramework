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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

#endregion

namespace FilterFramework
{
    public class FilterCollection<T> : List<IFilter<T>>
    {
        #region Members and Properties

        private Expression<Func<T, bool>> MyPredicate { get; set; }
        private readonly Expression<Func<T, bool>> _myDefaultPredicate;
        private Expression<Func<T, bool>> MyDefaultPredicate { get { return _myDefaultPredicate; }}

        #endregion

        #region Constructor, Initialization, and Disposal

        public FilterCollection()
        {
            _myDefaultPredicate = PredicateBuilderExtension.True<T>();
            MyPredicate = MyDefaultPredicate;
        }

        #endregion

        #region Methods

        public new void Add(IFilter<T> f)
        {
            if(f.IsEnabled)
                MyPredicate = MyPredicate.And(f.MyExpressionFunc);
            base.Add(f);
        }

        public IEnumerable<T> ApplyFilter(IEnumerable<T> coll)
        {
            IQueryable<T> query = coll.AsQueryable();
            var v = query.Where(MyPredicate);
            return v;
        }

        public new void Clear()
        {
            MyPredicate = MyDefaultPredicate;
            base.Clear();
        }

        #endregion

    }
}
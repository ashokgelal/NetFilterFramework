#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

#endregion

namespace FilterFramework.Model
{
    public class FilterCollection<T> : List<IFilter<T>>
    {
        private Expression<Func<T, bool>> MyPredicate { get; set; }

        public FilterCollection()
        {
            MyPredicate = PredicateBuilderExtension.False<T>();
        }

        public new void Add(IFilter<T> f)
        {
            MyPredicate = MyPredicate.Or(f.MyExpressionFunc);
            base.Add(f);
        }

        public IEnumerable<T> ApplyFilter(IEnumerable<T> coll)
        {
            IQueryable<T> query = coll.AsQueryable();
            return query.Where(MyPredicate);
        }
    }
}
#region Using Directives

using System.Collections.Generic;
using FilterFramework.Model;

#endregion

namespace FilterFramework.Controller
{
    public class FilterHandler<T>
    {
        private FilterCollection<T> MyFilterCollection { get; set; }

        public FilterHandler()
        {
            MyFilterCollection = new FilterCollection<T>();
        }

        public IEnumerable<T> ApplyFilter(IEnumerable<T> coll)
        {
            return MyFilterCollection.ApplyFilter(coll);
        }

        public void HandleNewFilterRequest(string expression)
        {
            BinaryExpressionFilter<T> filter = new BinaryExpressionFilter<T>("Ssid", BinaryOperators.Equals,
                                                                             "Rockstarz2");
            MyFilterCollection.Add(filter);
        }
    }
}
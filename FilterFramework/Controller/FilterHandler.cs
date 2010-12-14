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

        public IFilter<T> HandleNewFilterRequest(string expression)
        {
            IFilter<T> filter = new BinaryExpressionFilter<T>("Ssid", "=", "Rockstarz2") {IsEnabled = true};
            MyFilterCollection.Add(filter);
            return filter;
        }
    }
}
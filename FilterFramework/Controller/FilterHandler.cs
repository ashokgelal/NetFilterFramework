#region Using Directives

using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            string[] words = expression.Split(' ');

            if (words.Length == 3)
                return HandleNewFilterRequest(words[0], words[1], words[2]);
            return null;
        }

        public IFilter<T> HandleNewFilterRequest(string left, string op, string right)
        {
                // binary expression
            IFilter<T> filter = new BinaryExpressionFilter<T>(left, op, right) {IsEnabled = true};
            MyFilterCollection.Add(filter);
            return filter;
        }

        public ReadOnlyCollection<IFilter<T>> GetAllFilters()
        {
            return MyFilterCollection.AsReadOnly();
        }

        public void Refresh()
        {
            var filters = MyFilterCollection.ToArray();
            MyFilterCollection.Clear();
            foreach (var v in filters)
            {
                MyFilterCollection.Add(v);
            }
        }

        public void ClearFilters()
        {
            MyFilterCollection.Clear();
            Refresh();
        }

        public void ClearFilter(IFilter<T> filter)
        {
            MyFilterCollection.Remove(filter);
            Refresh();
        }
    }
}
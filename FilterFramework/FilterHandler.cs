#region Using Directives

using System.Collections.Generic;
using System.Collections.ObjectModel;

#endregion

namespace FilterFramework
{
    public class FilterHandler<T>
    {
        #region Members and Properties

        private FilterCollection<T> MyFilterCollection { get; set; }

        #endregion

        #region Constructor, Initialization, and Disposal

        public FilterHandler()
        {
            MyFilterCollection = new FilterCollection<T>();
        }

        #endregion

        #region Methods

        public IEnumerable<T> ApplyFilter(IEnumerable<T> coll)
        {
            return MyFilterCollection.ApplyFilter(coll);
        }

        public IFilter<T> CreateBinaryFilter(string expression)
        {
            string[] words = expression.Split(' ');

            if (words.Length == 3)
                return CreateBinaryFilter(words[0], words[1], words[2]);
            return null;
        }

        public IFilter<T> CreateBinaryFilter(string left, string op, string right)
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

        #endregion
    }
}
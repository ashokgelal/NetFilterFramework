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
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;
using System.Xml.Linq;


#endregion

namespace FilterFramework
{
    public class FilterHandler<T>
    {
        #region Members and Properties

        public FilterCollection<T> MyFilterCollection { get; set; }

        private const string XmlVersion = "0.1";

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
            return CreateBinaryFilter(expression, true);
        }

        public IFilter<T> CreateBinaryFilter(string expression, bool doAnd)
        {
            IFilter<T> retVal = null;
            try
            {
                var w1 = expression.Substring(0, expression.IndexOf(" "));
                expression = expression.Remove(0, w1.Length + 1);
                var w2 = expression.Substring(0, expression.IndexOf(" "));
                expression = expression.Remove(0, w2.Length + 1);
                var w3 = expression;

                if (!String.IsNullOrEmpty(w3))
                    retVal = CreateBinaryFilter(w1, w2, w3, doAnd);
            }
            catch (Exception)
            {
                retVal = null;
            }

            return retVal;
        }

        private IFilter<T> CreateBinaryFilter(string left, string op, string right, bool doAnd)
        {
            // binary expression
            IFilter<T> filter = new BinaryExpressionFilter<T>(left, op, right)
                                    {
                                        IsEnabled = true, DoAnd = doAnd
                                    };
            return filter;
        }

        public void AndFiltersWith(FilterCollection<T> obj2)
        {
            MyFilterCollection.AndWith(obj2);
        }

        public IFilter<T> CreateBinaryFilter(string left, string op, string right)
        {
            return CreateBinaryFilter(left, op, right, true);
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

        public bool HasFilters()
        {
            return MyFilterCollection.Count > 0;
        }

        public string ExportFiltersToXml(string filename)
        {
            if (MyFilterCollection == null)
                return "No filters available for export";

            try
            {
                XmlWriterSettings settings = new XmlWriterSettings{Indent = true};
                using (XmlWriter writer = XmlWriter.Create(filename, settings))
                {
                    writer.WriteStartElement("filters");
                    writer.WriteElementString("version", XmlVersion);
                    writer.WriteElementString("type", typeof(T).ToString());
                    WriteXml(writer);
                    writer.WriteEndElement(); // closes "Filters"
                }
                return String.Empty;
            }
            catch (Exception e) 
            {
                return e.Message;
            }
        }

        public string CreateFiltersFromXml(string filename, bool restoreOnError)
        {
            ClearFilters();
            return AppendFiltersFromXml(filename, restoreOnError);
        }

        public string AppendFiltersFromXml(string filename, bool restoreOnError)
        {
            // save to temp for restoring later on
            var temp = new FilterCollection<T>();
            temp.AddRange(MyFilterCollection);

            if(MyFilterCollection==null)
                MyFilterCollection = new FilterCollection<T>();
            try
            {
                XmlReaderSettings settings = new XmlReaderSettings { IgnoreComments = true, IgnoreWhitespace = true, IgnoreProcessingInstructions = true };
                using (XmlReader reader = XmlReader.Create(filename, settings))
                {
                    ReadXml(reader);
                }
                return String.Empty;
            }
            catch (Exception e)
            {
                if(restoreOnError)
                    MyFilterCollection = temp;
                Refresh();
                return e.Message;
            }
        }

        private void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement("filters");
            string version = reader.ReadElementContentAsString("version","");
            string type = reader.ReadElementContentAsString("type", "");

            if(String.IsNullOrEmpty(version) || !version.Equals(XmlVersion) || String.IsNullOrEmpty(type) || !type.Equals(typeof(T).ToString()))
                throw new FormatException("Filter format is not supported");

            while (reader.Name.Equals("filter"))
            {
                XElement blockNode = (XElement) XNode.ReadFrom(reader);
                var left = (string) blockNode.Attribute("leftExp");
                var op = (string) blockNode.Attribute("op");
                var right = (string) blockNode.Attribute("rightExp");
                var enabled = (bool) blockNode.Attribute("enabled");

                var filter = CreateBinaryFilter(left, op, right); // also adds to collection
                filter.IsEnabled = enabled;
            }
        }

        private void WriteXml(XmlWriter writer)
        {
            foreach (var filter in MyFilterCollection)
            {
                writer.WriteStartElement("filter");
                writer.WriteAttributeString("leftExp", filter.LeftExpression);
                writer.WriteAttributeString("op", filter.Operator);
                writer.WriteAttributeString("rightExp", filter.RightExpression);
                writer.WriteAttributeString("enabled", filter.IsEnabled.ToString());
                writer.WriteEndElement(); // end "filter"
            }
        }

        #endregion

        public void AddAll(Dictionary<int,IFilter<T>>.ValueCollection values)
        {
            throw new NotImplementedException();
        }
    }
}
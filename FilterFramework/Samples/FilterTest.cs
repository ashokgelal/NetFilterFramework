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

#endregion

namespace FilterFramework.Samples
{
    public class FilterTest
    {
        private IList<Student> Students { get; set; }
        private FilterHandler<Student> MyFilterHandler { get; set; }

        public FilterTest()
        {
            Initialize();
        }

        private IFilter<Student> FilterByFirstName()
        {
            // notice the space between operator and the operands
            var filter = MyFilterHandler.CreateBinaryFilter("FirstName = Jesse", true);
            MyFilterHandler.MyFilterCollection.Add(filter);
            var filteredStudents = MyFilterHandler.ApplyFilter(new ReadOnlyCollection<Student>(Students));
            Console.WriteLine("\n\nStudents where First Name = Jesse\n");
            PrintList(Students, filteredStudents);
            return filter;
        }

        private IFilter<Student> FilterByLastName()
        {
            // notice the space between operator and the operands
            var filter = MyFilterHandler.CreateBinaryFilter("LastName = Jackson", true);
            MyFilterHandler.MyFilterCollection.Add(filter);
            var filteredStudents = MyFilterHandler.ApplyFilter(new ReadOnlyCollection<Student>(Students));
            Console.WriteLine("\n\nStudents where Last Name = Jackson\n");
            PrintList(Students, filteredStudents);
            return filter;
        }

        private IFilter<Student> FilterFirstThreeStudents()
        {
            // notice the space between operator and the operands
            var filter = MyFilterHandler.CreateBinaryFilter("Id <= 3", true);
            MyFilterHandler.MyFilterCollection.Add(filter);
            var filteredStudents = MyFilterHandler.ApplyFilter(new ReadOnlyCollection<Student>(Students));
            Console.WriteLine("\n\nFirst three students (by Id)\n");
            PrintList(Students, filteredStudents);
            return filter;
        }

        private IFilter<Student> FilterInternationalStudents()
        {
            // notice the space between operator and the operands
            var filter = MyFilterHandler.CreateBinaryFilter("IsInternational = True", true);
            MyFilterHandler.MyFilterCollection.Add(filter);
            var filteredStudents = MyFilterHandler.ApplyFilter(new ReadOnlyCollection<Student>(Students));
            Console.WriteLine("\n\nInternational students\n");
            PrintList(Students, filteredStudents);
            return filter;
        }

        private IFilter<Student> FilterByGender(Gender g)
        {
            // notice the space between operator and the operands
            var filter = MyFilterHandler.CreateBinaryFilter(String.Format("Gender = {0}", g), true);
            MyFilterHandler.MyFilterCollection.Add(filter);
            var filteredStudents = MyFilterHandler.ApplyFilter(new ReadOnlyCollection<Student>(Students));
            Console.WriteLine(String.Format("\n\n{0} students\n", g));
            PrintList(Students, filteredStudents);
            return filter;
        }

        private static void PrintList(IList<Student> orgList, IEnumerable<Student> filteredList)
        {
            foreach (var student in filteredList)
                Console.WriteLine(student);
        }

        private void Initialize()
        {
            Students = new List<Student>
                           {
                               new Student("Jesse", "Fredericks") {IsInternational = false, Gender = Gender.Male},
                               new Student("Elenor", "Ruel") {IsInternational = true, Gender = Gender.Female},
                               new Student("Hàn Ngọc", "Trai") {IsInternational = true, Gender = Gender.Female},
                               new Student("Catherine", "Jackson") {IsInternational = false, Gender = Gender.Female},
                               new Student("Mahjub Khalid", "Daher") {IsInternational = true, Gender = Gender.Male},
                               new Student("Ashwaq Jawahir", "Shalhoub")
                                   {IsInternational = true, Gender = Gender.Female},
                               new Student("Douglas", "Rego") {IsInternational = false, Gender = Gender.Male},
                               new Student("Jose", "Kitterman") {IsInternational = false, Gender = Gender.Female},
                               new Student("Nancy", "Jackson") {IsInternational = false, Gender = Gender.Female},
                               new Student("Jesse", "Roberts") {IsInternational = false, Gender = Gender.Male},
                               new Student("Bob", "Jackson") {IsInternational = false, Gender = Gender.Male},
                               new Student("Bobby", "Rackson") {IsInternational = false, Gender = Gender.Female},
                               new Student("Boba", "Bob Jackson") {IsInternational = false, Gender = Gender.Female},
                           };

            MyFilterHandler = new FilterHandler<Student>();
        }

        private void FilterByMultipleProperties()
        {
            var filter = MyFilterHandler.CreateBinaryFilter("FirstName contains bob", true);
            MyFilterHandler.MyFilterCollection.Add(filter);
            filter = MyFilterHandler.CreateBinaryFilter("Gender = Female", true);
            MyFilterHandler.MyFilterCollection.Add(filter);
            filter = MyFilterHandler.CreateBinaryFilter("LastName !contains rack", true);
            MyFilterHandler.MyFilterCollection.Add(filter);
            var filteredStudents = MyFilterHandler.ApplyFilter(new ReadOnlyCollection<Student>(Students));
            Console.WriteLine("\n\nFemales with LastNames containing 'son' but not 'rack' \n");
            PrintList(Students, filteredStudents);
        }

        public void RunTests()
        {
            Console.WriteLine("Original Count: {0}", Students.Count);
            FilterByMultipleProperties();
            MyFilterHandler.ClearFilters();
            FilterByFirstName();
            MyFilterHandler.ClearFilters();
            FilterByLastName();

            MyFilterHandler.ClearFilters();
            FilterFirstThreeStudents();

            MyFilterHandler.ClearFilters();
            FilterInternationalStudents();

            MyFilterHandler.ClearFilters();
            FilterByGender(Gender.Female);

            // add international students filter to Female Students
            var filter = FilterInternationalStudents();
            // remove international students filter
            MyFilterHandler.ClearFilter(filter);
            FilterWithCurrentFilters();

            // changing filters
            MyFilterHandler.ClearFilters();
            ChangeInternationalStatus();
        }

        private void ChangeInternationalStatus()
        {
            Console.WriteLine("\n\n\nFilter edit demo");
            var filter = FilterInternationalStudents();
            Console.WriteLine("\n\n(Above) Before Changing Filter: {0}", filter);
            if (filter.TryChangingRightExpression("False"))
            {
                MyFilterHandler.Refresh();
                var filteredStudents = MyFilterHandler.ApplyFilter(new ReadOnlyCollection<Student>(Students));
                Console.WriteLine("\n\nAfter Changing Filter");
                PrintList(Students, filteredStudents);
            }
        }

        private void FilterWithCurrentFilters()
        {
            PrintFilters();
            var filteredStudents = MyFilterHandler.ApplyFilter(new ReadOnlyCollection<Student>(Students));
            Console.WriteLine("\nApplying current filters (Filters count: {0})\n", MyFilterHandler.GetAllFilters().Count);
            PrintList(Students, filteredStudents);
        }

        private void PrintFilters()
        {
            Console.WriteLine("\n\n** Current Filters **");
            foreach (var filter in MyFilterHandler.GetAllFilters())
                Console.WriteLine(filter);
        }
    }
}
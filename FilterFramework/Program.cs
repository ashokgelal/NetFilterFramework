using System;
using FilterFramework.Samples;

namespace FilterFramework
{
    internal class Program
    {
        public static void Main()
        {
            FilterTest students = new FilterTest();
            students.RunTests();
            Console.ReadKey();
        }
    }
}
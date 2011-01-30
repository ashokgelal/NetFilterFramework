namespace FilterFramework.Samples
{
    public class Student
    {
        private static int _idCount;
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public int Id { get; private set; }
        public bool IsInternational { get; set; }
        public Gender Gender { get; set; }

        public Student(string fname, string lname)
        {
            Id = ++_idCount;
            FirstName = fname;
            LastName = lname;
        }

        public override string ToString()
        {
            return string.Format("{0}: \t{4} {1} {2} ({3})", Id, FirstName, LastName, Gender, IsInternational?"*":" ");
        }
    }

    public enum Gender
    {
        Female,
        Male = 1
    }
}
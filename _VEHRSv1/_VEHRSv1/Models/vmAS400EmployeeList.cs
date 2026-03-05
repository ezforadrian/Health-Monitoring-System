namespace _VEHRSv1.Models
{
    public class vmAS400EmployeeList
    {
        public string IdNumber { get; set; }
        public string IdEnc { get; set; }
        public string FullName { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Position { get; set; }
        public DateOnly BirthDate { get; set; }
        public DateOnly DateHired { get; set; }

    }
}

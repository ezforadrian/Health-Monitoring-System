namespace _VEHRSv1.Models
{
    public class vmEmployeeHealthInfo
    {
        public vmAS400EmployeeList EmployeeDetailAS400 { get; set; }
        public vmPemeEmployeeRecord? PEMEEmployeeRecord { get; set; }
        public Ameheader? AMEEmployeeRecord { get; set; }
        
    }
}

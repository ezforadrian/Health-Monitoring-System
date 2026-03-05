namespace _VEHRSv1.Models
{
    public class vmSearchPeme
    {
        public string PemeidString { get; set; }
        public int Pemeid { get; set; }
        public string LastName { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string? MiddleName { get; set; }
        public DateOnly? Birthdate { get; set; }
        public string PositionRef { get; set; } = null!;
        public string PositionDescription { get; set; }
        public int StatusId { get; set; }
        public string StatusDescription { get; set; }
        public string? Idnumber { get; set; }
    }
}

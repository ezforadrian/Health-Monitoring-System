namespace _VEHRSv1.Models
{
    public class vmTestDetails
    {
        public int TestId { get; set; }
        public string TestIdEnc { get; set; }

        public string TestName { get; set; } = null!;

        public string TestCategory { get; set; } = null!;

        public bool IsActive { get; set; }


    }
}

namespace _VEHRSv1.Models
{
    public class vmTestResult
    {
        public int TestId { get; set; }
        public string TestIdEnc { get; set; }
        public string TestName { get; set; }
        public bool Result { get; set; }
        public int EcuHeaderId { get; set; }
        public string EcuHeaderIdEnc { get; set; }
    }
}

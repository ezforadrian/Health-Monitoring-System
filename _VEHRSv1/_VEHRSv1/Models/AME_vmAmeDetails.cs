namespace _VEHRSv1.Models
{
    public class AME_vmAmeDetails
    {
        public int AmedetailId { get; set; }
        public string AmedetailIdEnc { get; set; }
        public int AmeheaderId { get; set; }
        public int TestId { get; set; }
        public string TestIdEnc { get; set; }
        public bool Result { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedDateTime { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
        public string TestName { get; set; } = null!;
    }
}

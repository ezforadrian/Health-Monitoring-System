namespace _VEHRSv1.Models
{
    public class ECU_vmEcuDetails
    {
        public int EcudetailId { get; set; }
        public string EcudetailIdEnc { get; set; }
        public int EcuheaderId { get; set; }
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

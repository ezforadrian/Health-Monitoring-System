namespace _VEHRSv1.Models
{
    public class vmAddRecordPeme
    {
        //public List<vmDepartment> Departments { get; set; }
        public List<vmPosition> Positions { get; set; }
        public List<vmPemeStatus> PEMEStatus { get; set; }
        public List<vmMedicalEvaluator> MedicalEvaluator { get; set; }
        public vmSavePemeRecord vmSavePemeRecord { get; set; }
    }
}

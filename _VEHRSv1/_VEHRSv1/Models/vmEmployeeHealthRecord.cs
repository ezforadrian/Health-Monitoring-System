namespace _VEHRSv1.Models
{
    public class vmEmployeeHealthRecord
    {
        public vmEmployeeHealthInfo vmEmployeeHealthInfo { get; set; }
        public string EmpIdNumberEnc { get; set; }

        public List<vmEcuHeader>? VmEcuHeaders { get; set; }
        public List<vmAmeHeader>? VmAmeHeaders { get; set; }
        public List<vmMWRRecordPerEmployee>? VmMwrHeaders { get; set; }
        public List<vmTestDetails>? VmTestDetails { get; set; }
        
        

    }
}

using _VEHRSv1.Models;

namespace _VEHRSv1.Interface
{
    public interface IECURepository
    {
        List<vmEcuHeader> GetEcuByEmployeeIdNumber(string idNumber);
        List<vmTestDetails> GetEcuActiveTest();
        Task AddEcuHeader(ECUHeader ecuheader);
        bool AddEcuDetail(ECUDetail ecudetail);
        vmEmployeeHealthRecord GetDetailListByEmployeeIdNumber(string idNumber);
        ECUDetailEdit GetEcuDetailByEmployeeIdNumber(string idnumber, int ecuHeader);
        bool UpdateEcuRecord(ECUHeader eCUHeader, List<ECUDetail> eCUDetail);
        bool EcuRecordExist(string empIdNumber, DateOnly ecuDate);
        bool EcuRecordExist(string empIdNumber, DateOnly ecuDate, int ecuHeaderId);
    }
}

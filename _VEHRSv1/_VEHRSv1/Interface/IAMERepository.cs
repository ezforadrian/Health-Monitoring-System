using _VEHRSv1.Models;

namespace _VEHRSv1.Interface
{
    public interface IAMERepository
    {
        Task<List<AmeExcelRecord>> GetAllAmeRecordsAsync();
        List<vmAmeHeader> GetAmeByEmployeeIdNumber(string idNumber);
        vmEmployeeHealthRecord GetDetailListByEmployeeIdNumber(string employeeId);
        List<TestDetail> GetAllAmeActiveTest();
        List<vmTestDetails> GetAmeActiveTest();
        Task AddAmeExcelRecordsAsync(IEnumerable<AmeExcelRecord> records);
        Task<IEnumerable<AmeExcelRecord>> GetAmeRecordsByEmployeeIdAndAmeDateAsync(IEnumerable<string> employeeIds, IEnumerable<string> ameDates);
        
        
        //AmeHeaderTable
        Task AddAmeHeaderRecordsAsync(IEnumerable<Ameheader> records);
        Task<IEnumerable<Ameheader>> GetAmeHeaderRecordsByEmployeeIdAndAmeDateAsync(IEnumerable<string> employeeIds, IEnumerable<DateOnly> ameDates);

        
        //AmeDetails
        Task AddAmeDetailsRecordsAsync(IEnumerable<Amedetail> records);
        Task<IEnumerable<Amedetail>> GetAmeDetailRecordsByHeaderIdAndTestIdAsync(IEnumerable<int> headerId, IEnumerable<int> testId);

        List<vmExcelColumnHeader> AMEColumnHeaders();

        bool UpdateRemarks(int id, string remarks);
    }
}

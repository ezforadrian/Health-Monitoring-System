using _VEHRSv1.Interface;
using _VEHRSv1.Models;
using _VEHRSv1.Services;
using Microsoft.EntityFrameworkCore;

namespace _VEHRSv1.Repository
{
    public class AMERepository : IAMERepository
    {
        private readonly HswmsContext _db;
        private readonly IAS400PlantillaRepository _aS400PlantillaRepository;
        private readonly EncryptionService _encryptionService;
        private const string _key = "thequickbrownfox";

        public AMERepository(HswmsContext hswmsContext, IAS400PlantillaRepository aS400PlantillaRepository, EncryptionService encryptionService)
        {
            _db = hswmsContext;
            _encryptionService = encryptionService;
            _aS400PlantillaRepository = aS400PlantillaRepository;
        }
        public async Task AddAmeDetailsRecordsAsync(IEnumerable<Amedetail> records)
        {
            await _db.Amedetails.AddRangeAsync(records);
            await _db.SaveChangesAsync();
        }

        public async Task AddAmeExcelRecordsAsync(IEnumerable<AmeExcelRecord> records)
        {
            await _db.AmeExcelRecords.AddRangeAsync(records);
            await _db.SaveChangesAsync();
        }

        public async Task AddAmeHeaderRecordsAsync(IEnumerable<Ameheader> records)
        {
            await _db.Ameheaders.AddRangeAsync(records);
            await _db.SaveChangesAsync();
        }

        public List<vmExcelColumnHeader> AMEColumnHeaders()
        {
            throw new NotImplementedException();
        }

        public List<TestDetail> GetAllAmeActiveTest()
        {
            return _db.TestDetails.Where(x => x.TestCategory == "AME" && x.IsActive == true).ToList();
        }

        public Task<List<AmeExcelRecord>> GetAllAmeRecordsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Amedetail>> GetAmeDetailRecordsByHeaderIdAndTestIdAsync(IEnumerable<int> headerId, IEnumerable<int> testId)
        {
            return await _db.Amedetails
                                 .Where(r => headerId.Contains(r.AmeheaderId) && testId.Contains(r.TestId))
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Ameheader>> GetAmeHeaderRecordsByEmployeeIdAndAmeDateAsync(IEnumerable<string> employeeIds, IEnumerable<DateOnly> ameDates)
        {
            return await _db.Ameheaders
                                 .Where(r => employeeIds.Contains(r.Idnumber) && ameDates.Contains(r.Amedate))
                                 .ToListAsync();
        }

        public vmEmployeeHealthRecord GetDetailListByEmployeeIdNumber(string employeeId)
        {

            try
            {
                var employeeInfo = _aS400PlantillaRepository.GetEmployeeInfoUsingIdNumber(employeeId);
                var idNumberEnc = _encryptionService.Encrypt(_key, employeeId);
                var ameDetails = GetAmeByEmployeeIdNumber(employeeId);

                var returnRes = new vmEmployeeHealthRecord
                {
                    vmEmployeeHealthInfo = employeeInfo,
                    EmpIdNumberEnc = idNumberEnc,
                    VmAmeHeaders = ameDetails,
                    VmTestDetails = GetAmeActiveTest()
                };

                return returnRes;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<AmeExcelRecord>> GetAmeRecordsByEmployeeIdAndAmeDateAsync(IEnumerable<string> employeeIds, IEnumerable<string> ameDates)
        {
            return await _db.AmeExcelRecords
                                 .Where(r => employeeIds.Contains(r.EmployeeId) && ameDates.Contains(r.AmeDate))
                                 .ToListAsync();
        }

        public List<vmAmeHeader> GetAmeByEmployeeIdNumber(string idNumber)
        {
            try
            {
                var ecu = _db.Ameheaders
                .Where(e => e.Idnumber == idNumber)
                .Select(e => new vmAmeHeader
                {
                    EmployeeIdNumberEnc = _encryptionService.Encrypt(_key, e.Idnumber),
                    AmeHeaderIdEnc = _encryptionService.Encrypt(_key, e.AmeheaderId.ToString()),
                    AmeheaderId = e.AmeheaderId,
                    Amedate = e.Amedate,
                    RunDate = e.RunDate,
                    Branch = e.Branch,
                    Remarks = e.Remarks,
                    Amedetails = e.Amedetails.Select(d => new AME_vmAmeDetails
                    {
                        AmedetailId = d.AmedetailId,
                        AmedetailIdEnc = _encryptionService.Encrypt(_key, d.AmedetailId.ToString()),
                        AmeheaderId = d.AmeheaderId,
                        TestId = d.TestId,
                        TestIdEnc = _encryptionService.EncryptConstant(_key, d.TestId.ToString()),
                        Result = d.Result,
                        CreatedBy = d.CreatedBy,
                        CreatedDateTime = d.CreatedDateTime,
                        ModifiedBy = d.ModifiedBy,
                        ModifiedDateTime = d.ModifiedDateTime,
                        TestName = d.Test.TestName
                    }).ToList()
                })
                .ToList();

                return ecu;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public List<vmTestDetails> GetAmeActiveTest()
        {
            return _db.TestDetails
                    .Where(t => t.IsActive == true && t.TestCategory == "AME")
                    .Select(t => new vmTestDetails
                    {
                        TestId = t.TestId,
                        TestIdEnc = _encryptionService.EncryptConstant(_key, t.TestId.ToString()),
                        TestName = t.TestName,
                        TestCategory = t.TestCategory,
                        IsActive = t.IsActive,
                    }).ToList();
        }

        public bool UpdateRemarks(int id, string remarks)
        {
            try
            {
                //checked first if ame header exist
                var ameHeader = _db.Ameheaders.Where(x => x.AmeheaderId == id).FirstOrDefault();

                if (ameHeader == null)
                {
                    return false;
                }

                ameHeader.Remarks = remarks;

                return _db.SaveChanges() > 0;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}

using _VEHRSv1.Interface;
using _VEHRSv1.Models;
using _VEHRSv1.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace _VEHRSv1.Repository
{
    public class ECURepository : IECURepository
    {
        private readonly HswmsContext _hswmsContext;
        private readonly EncryptionService _encryptionService;
        private readonly IAS400PlantillaRepository _aS400PlantillaRepository;
        private const string _key = "thequickbrownfox";

        public ECURepository(HswmsContext hswmsContext, EncryptionService encryptionService, IAS400PlantillaRepository aS400PlantillaRepository)
        {
            _hswmsContext = hswmsContext;
            _encryptionService = encryptionService;
            _aS400PlantillaRepository = aS400PlantillaRepository;
        }

        public List<vmEcuHeader> GetEcuByEmployeeIdNumber(string idNumber)
        {
            try
            {
                var ecu = _hswmsContext.Ecuheaders
                .Where(e => e.Idnumber == idNumber)
                .Select(e => new vmEcuHeader
                {
                    EmployeeIdNumberEnc = _encryptionService.Encrypt(_key, e.Idnumber),
                    EcuHeaderIdEnc = _encryptionService.Encrypt(_key, e.EcuheaderId.ToString()),
                    EcuheaderId = e.EcuheaderId,
                    Ecudate = e.Ecudate,
                    RunDate = e.RunDate,
                    Branch = e.Branch,
                    Remarks = e.Remarks,
                    Ecudetails = e.Ecudetails.Select(d => new ECU_vmEcuDetails
                    {
                        EcudetailId = d.EcudetailId,
                        EcudetailIdEnc = _encryptionService.Encrypt(_key, d.EcudetailId.ToString()),
                        EcuheaderId = d.EcuheaderId,
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

        public List<vmTestDetails> GetEcuActiveTest()
        {
            return _hswmsContext.TestDetails
                    .Where(t => t.IsActive == true && t.TestCategory == "ECU")
                    .Select(t => new vmTestDetails { 
                        TestId  = t.TestId,
                        TestIdEnc = _encryptionService.EncryptConstant(_key, t.TestId.ToString()),
                        TestName = t.TestName,
                        TestCategory = t.TestCategory,
                        IsActive = t.IsActive,
                    }).ToList();
        }

        public async Task AddEcuHeader(ECUHeader ecuheader)
        {
            await _hswmsContext.Ecuheaders.AddAsync(ecuheader);
            await _hswmsContext.SaveChangesAsync();
        }

        public bool AddEcuDetail(ECUDetail ecudetail)
        {
            _hswmsContext.Ecudetails.Add(ecudetail);
            return _hswmsContext.SaveChanges() > 0;
        }

        public vmEmployeeHealthRecord GetDetailListByEmployeeIdNumber(string idNumber)
        {
            try
            {
                var employeeInfo = _aS400PlantillaRepository.GetEmployeeInfoUsingIdNumber(idNumber);
                var idNumberEnc = _encryptionService.Encrypt(_key, idNumber);
                var ecuDetails = GetEcuByEmployeeIdNumber(idNumber);

                var returnRes = new vmEmployeeHealthRecord
                {
                    vmEmployeeHealthInfo = employeeInfo,
                    EmpIdNumberEnc = idNumberEnc,
                    VmEcuHeaders = ecuDetails,
                    VmTestDetails = GetEcuActiveTest()
                };

                return returnRes;
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        public ECUDetailEdit GetEcuDetailByEmployeeIdNumber(string idnumber, int ecuHeader)
        {
            try
            {
                // Fetching ecuRecord along with its Ecudetails
                var ecuRecord = _hswmsContext.Ecuheaders
                    .Where(e => e.Idnumber == idnumber && e.EcuheaderId == ecuHeader)
                    .Select(e => new
                    {
                        e.Idnumber,
                        e.EcuheaderId,
                        e.Ecudate,
                        e.Branch,
                        e.Remarks,
                        Ecudetails = e.Ecudetails
                            .Where(a => a.EcuheaderId == ecuHeader)
                            .Select(a => new
                            {
                                a.TestId,
                                a.Result,
                                a.EcuheaderId
                            }).ToList()
                    }).FirstOrDefault();

                if (ecuRecord == null)
                    return null;

                // Fetch allEcuTest separately
                var allEcuTest = _hswmsContext.TestDetails
                    .Where(x => x.TestCategory == "ECU" && x.IsActive)
                    .Select(x => new
                    {
                        x.TestId,
                        x.TestName
                    }).ToList();

                // Combine results and apply encryption
                var testResults = allEcuTest.Select(x => new vmTestResult
                {
                    TestId = x.TestId,
                    TestName = x.TestName,
                    TestIdEnc = _encryptionService.EncryptConstant(_key, x.TestId.ToString()),
                    Result = ecuRecord.Ecudetails.FirstOrDefault(r => r.TestId == x.TestId).Result,
                    EcuHeaderId = ecuRecord.EcuheaderId,
                    EcuHeaderIdEnc = _encryptionService.Encrypt(_key, ecuRecord.EcuheaderId.ToString())
                }).ToList();

                var returnRes = new ECUDetailEdit
                {
                    IdEnc = _encryptionService.Encrypt(_key, ecuRecord.Idnumber),
                    EcuHeaderIdEnc = _encryptionService.Encrypt(_key, ecuRecord.EcuheaderId.ToString()),
                    EcuHeaderId = ecuRecord.EcuheaderId,
                    EcuDate = ecuRecord.Ecudate,
                    BranchCodeEnc = _encryptionService.EncryptConstant(_key, ecuRecord.Branch),
                    Remarks = ecuRecord.Remarks,
                    TestResults = testResults
                };

                return returnRes;
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                throw new ApplicationException("An error occurred while fetching ECU details.", ex);
            }
        }

        public bool UpdateEcuRecord(ECUHeader eCUHeader, List<ECUDetail> eCUDetail)
        {
            try
            {
                var ecuSelHeader = _hswmsContext.Ecuheaders
                    .FirstOrDefault(x => x.Idnumber == eCUHeader.Idnumber && x.EcuheaderId == eCUHeader.EcuheaderId);

                if (ecuSelHeader == null)
                {
                    return false;
                }

                // Update header details
                ecuSelHeader.Ecudate = eCUHeader.Ecudate;
                ecuSelHeader.Branch = eCUHeader.Branch;
                ecuSelHeader.Remarks = eCUHeader.Remarks;
                ecuSelHeader.ModifiedBy = eCUHeader.ModifiedBy;
                ecuSelHeader.ModifiedDateTime = eCUHeader.ModifiedDateTime;

                foreach (var item in eCUDetail)
                {
                    var ecuSelDetail = _hswmsContext.Ecudetails
                        .FirstOrDefault(x => x.EcuheaderId == item.EcuheaderId && x.TestId == item.TestId);

                    if (ecuSelDetail != null)
                    {
                        ecuSelDetail.Result = item.Result;
                    }
                    else
                    {
                        // If the detail record is not found, consider whether you want to:
                        // - Skip updating this detail
                        // - Add a new detail record
                        // - Continue and save the found records
                        // - Return false or handle it another way
                        // For now, we return false if any detail is not found
                        return false;
                    }
                }

                // Save all changes in one transaction
                return _hswmsContext.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                // Log the exception here if needed
                throw new ApplicationException("An error occurred while updating the ECU record.", ex);
            }
        }

        public bool EcuRecordExist(string empIdNumber, DateOnly ecuDate)
        {
            return _hswmsContext.Ecuheaders.Where(x => x.Idnumber == empIdNumber && x.Ecudate == ecuDate).Any(); 
        }

        public bool EcuRecordExist(string empIdNumber, DateOnly ecuDate, int ecuHeaderId)
        {
            return _hswmsContext.Ecuheaders.Where(x => x.Idnumber == empIdNumber && x.Ecudate == ecuDate && x.EcuheaderId != ecuHeaderId).Any();
        }
    }
}

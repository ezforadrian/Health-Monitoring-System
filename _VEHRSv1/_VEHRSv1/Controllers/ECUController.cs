using _VEHRSv1.Interface;
using _VEHRSv1.Models;
using _VEHRSv1.Repository;
using _VEHRSv1.Services;
using Microsoft.AspNetCore.Mvc;

namespace _VEHRSv1.Controllers
{
    public class ECUController : Controller
    {
        private readonly EncryptionService _encryptionService;
        private readonly IAS400PlantillaRepository _aS400PlantillaRepository;
        private readonly IECURepository _eCURepository;
        private readonly HswmsContext _hswmsContext;
        private readonly IPEMERepository _pEMERepository;
        private const string _key = "thequickbrownfox";
        public ECUController(EncryptionService encryptionService,
            IAS400PlantillaRepository aS400PlantillaRepository,
            IPEMERepository pEMERepository, IECURepository eCURepository, HswmsContext hswmsContext)
        {
            _encryptionService = encryptionService;
            _aS400PlantillaRepository = aS400PlantillaRepository;
            _eCURepository = eCURepository;
            _hswmsContext = hswmsContext;
            _pEMERepository = pEMERepository;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEcuRecord([FromBody] ECUAddRecord model)
        {
            try
            {
                TempData.Remove("SuccessMessage");

                var errors = new List<string>();

                if (!ModelState.IsValid)
                {
                    var errorsModel = ModelState.ToDictionary(
                        k => k.Key,
                        v => v.Value.Errors.Select(e => e.ErrorMessage).ToList()
                    );

                    foreach (var item in errorsModel)
                    {
                        errors.Add(item.Value.FirstOrDefault());
                    }

                    return Json(new { success = false, errors = errors });
                }
                else
                {
                    try
                    {
                        // Decrypt values
                        string empIdDec = _encryptionService.Decrypt(_key, model.IdEnc);
                        string branchCodeDec = _encryptionService.DecryptConstant(_key, model.BranchCodeEnc);

                        var testDetails = new List<(int TestId, bool Result)>();
                        var testIdList = model.TestIdEnc.Select(DecryptModelValueInt).ToList();

                        var allEcuTest = _eCURepository.GetEcuActiveTest();

                        // Add to testDetails with Result based on whether the TestId is in TestIdList
                        foreach (var item in allEcuTest)
                        {
                            testDetails.Add((item.TestId, testIdList.Contains(item.TestId)));
                        }

                        using (var transaction = await _hswmsContext.Database.BeginTransactionAsync())
                        {
                            try
                            {
                                //checking of duplicates EcuDate and IdNumber
                                if (_eCURepository.EcuRecordExist(empIdDec, model.EcuDate))
                                {
                                    errors.Add("Duplicate record exist.");
                                    return Json(new { success = false, errors = errors });
                                }
                                // Mapping of EcuHeader
                                var ecuHeader = new ECUHeader
                                {
                                    Ecudate = model.EcuDate,
                                    Idnumber = empIdDec,
                                    Branch = branchCodeDec,
                                    Remarks = model.Remarks,
                                    CreatedBy = "dbo",
                                    CreatedDateTime = DateTime.Now
                                };

                                await _eCURepository.AddEcuHeader(ecuHeader);

                                // Mapping of details
                                foreach (var testItem in testDetails)
                                {
                                    var ecuDetails = new ECUDetail
                                    {
                                        EcuheaderId = ecuHeader.EcuheaderId,
                                        TestId = testItem.TestId,
                                        Result = testItem.Result,
                                        CreatedBy = "dbo",
                                        CreatedDateTime = DateTime.Now
                                    };

                                    if (!_eCURepository.AddEcuDetail(ecuDetails))
                                    {
                                        throw new Exception("An error occurred while saving the ECU detail.");
                                    }
                                }

                                await transaction.CommitAsync();
                                //TempData["SuccessMessage"] = "Record saved successfully!";
                                return Json(new { success = true, message = "Record saved successfully!" });
                            }
                            catch (Exception ex)
                            {
                                await transaction.RollbackAsync();
                                errors.Add("An error occurred while saving the record. Please try again.");
                                return Json(new { success = false });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return Json(new { success = false, errors = new List<string> { "An error occurred while saving." } });
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }




        }

        [HttpGet]
        public IActionResult GetEcuDetails(string idEnc)
        {
            try
            {
                var idDec = _encryptionService.Decrypt(_key, idEnc);


                var details = _eCURepository.GetDetailListByEmployeeIdNumber(idDec);
                if (details == null)
                {
                    return Json(new { success = false, message = "Details not found" });
                }

                return Json(new { success = true, details });
            }
            catch (Exception ex)
            {
                // Log the exception
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult OpenEcuEditModal([FromBody] vmGenericDataModel model)
        {
            try
            {
                string idDec = _encryptionService.Decrypt(_key, model.EmpIdEnc);
                int ecuHeaderDec = int.Parse(_encryptionService.Decrypt(_key, model.EcuHeaderIdEnc));



                var details = _eCURepository.GetEcuDetailByEmployeeIdNumber(idDec, ecuHeaderDec);
                if (details == null)
                {
                    return Json(new { success = false, message = "Details not found" });
                }

                return Json(new { success = true, details });
            }
            catch (Exception ex)
            {
                // Log the exception
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateEcuRecord([FromBody] ECUEditRecord model)
        {
            try
            {
                TempData.Remove("SuccessMessage");

                var errors = new List<string>();

                if (!ModelState.IsValid)
                {
                    var errorsModel = ModelState.ToDictionary(
                        k => k.Key,
                        v => v.Value.Errors.Select(e => e.ErrorMessage).ToList()
                    );

                    foreach (var item in errorsModel)
                    {
                        errors.Add(item.Value.FirstOrDefault());
                    }

                    return Json(new { success = false, errors = errors });
                }
                else
                {
                    try
                    {
                        // Decrypt values
                        string empIdDec = _encryptionService.Decrypt(_key, model.IdEnc);
                        int ecuHeaderIdDec = int.Parse(_encryptionService.Decrypt(_key, model.EcuHeaderIdEnc));
                        string branchCodeDec = _encryptionService.DecryptConstant(_key, model.BranchCodeEnc);

                        var testDetails = new List<(int TestId, bool Result)>();
                        var testIdList = model.TestIdEnc.Select(DecryptModelValueInt).ToList();

                        var allEcuTest = _eCURepository.GetEcuActiveTest();

                        // Add to testDetails with Result based on whether the TestId is in TestIdList
                        foreach (var item in allEcuTest)
                        {
                            testDetails.Add((item.TestId, testIdList.Contains(item.TestId)));
                        }

                        using (var transaction = await _hswmsContext.Database.BeginTransactionAsync())
                        {
                            try
                            {
                                if (_eCURepository.EcuRecordExist(empIdDec, model.EcuDate, ecuHeaderIdDec))
                                {
                                    errors.Add("Duplicate record exist.");
                                    return Json(new { success = false, errors = errors });
                                }
                                // Mapping of EcuHeader
                                var ecuHeader = new ECUHeader
                                {
                                    EcuheaderId = ecuHeaderIdDec,
                                    Ecudate = model.EcuDate,
                                    Idnumber = empIdDec,
                                    Branch = branchCodeDec,
                                    Remarks = model.Remarks,
                                    CreatedBy = "",
                                    CreatedDateTime = DateTime.Now,
                                    ModifiedBy = "dbo",
                                    ModifiedDateTime = DateTime.Now
                                };

                                // Mapping of details
                                List<ECUDetail> eCUDetails = new List<ECUDetail>();
                                foreach (var testItem in testDetails)
                                {
                                    var ecuDetails = new ECUDetail
                                    {
                                        EcuheaderId = ecuHeader.EcuheaderId,
                                        TestId = testItem.TestId,
                                        Result = testItem.Result,
                                        CreatedBy = "dbo",
                                        CreatedDateTime = DateTime.Now
                                    };

                                    eCUDetails.Add(ecuDetails);
                                }

                                if (_eCURepository.UpdateEcuRecord(ecuHeader, eCUDetails))
                                {
                                    await transaction.CommitAsync();
                                    return Json(new { success = true, message = "Record saved successfully!" });
                                }
                                else
                                {
                                    await transaction.RollbackAsync();
                                    errors.Add("An error occurred while saving the record. Please try again.");
                                    return Json(new { success = false });
                                }
                                
                                
                            }
                            catch (Exception ex)
                            {
                                await transaction.RollbackAsync();
                                errors.Add("An error occurred while saving the record. Please try again.");
                                return Json(new { success = false });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return Json(new { success = false, errors = new List<string> { "An error occurred while saving." } });
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }




        }



        private int DecryptModelValueInt(string modelValue)
        {
            try
            {
                // Decrypt the value
                var decValue = _encryptionService.DecryptConstant(_key, modelValue);

                // Check if the decrypted value is null
                if (decValue == null)
                {
                    throw new Exception("Decryption failed: Decrypted value is null.");
                }

                // Try to parse the decrypted value to an integer
                if (!int.TryParse(decValue, out int intDecValue))
                {
                    throw new FormatException("Decryption failed: Value is not in a valid integer format.");
                }

                return intDecValue;
            }
            catch (FormatException ex)
            {
                // Rethrow the FormatException with a custom message
                throw new FormatException("Status is not in a valid integer format.", ex);
            }
            catch (Exception ex)
            {
                // Log the exception as needed
                throw new Exception("An error occurred while decrypting the status.", ex);
            }
        }
    }
}

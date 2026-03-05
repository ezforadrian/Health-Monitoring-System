using _VEHRSv1.Interface;
using _VEHRSv1.Models;
using _VEHRSv1.Repository;
using _VEHRSv1.Services;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using static _VEHRSv1.Models.vmAMEExcelUpload;

namespace _VEHRSv1.Controllers
{
    public class AMEController : Controller
    {
        private readonly IFileValidationService _fileValidationService;
        private readonly HswmsContext _db;
        private readonly IAMERepository _aMERepository;
        private readonly ILogger<AMEController> _logger;
        private readonly EncryptionService _encryptionService;
        private const string _key = "thequickbrownfox";

        public AMEController(IFileValidationService fileValidationService, HswmsContext hswmsContext, IAMERepository aMERepository, ILogger<AMEController> logger, EncryptionService encryptionService)
        {
            _fileValidationService = fileValidationService;
            _db = hswmsContext;
            _aMERepository = aMERepository;
            _logger = logger;
            _encryptionService = encryptionService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult UploadAmeExcel()
        {
            var model = new ViewModelExcelUploadInformation
            {
                Message = TempData["Message"]?.ToString(),
                UploadedRecordsCount = TempData["UploadedRecords"] != null ? (int)TempData["UploadedRecords"] : 0,
                DuplicateRecordsCount = TempData["DuplicateRecords"] != null ? (int)TempData["DuplicateRecords"] : 0,
                FailedRecordsCount = TempData["FailedRecords"] != null ? (int)TempData["FailedRecords"] : 0,
                FailedRecordsDetails = TempData["FailedRecordsDetails"] != null
                    ? System.Text.Json.JsonSerializer.Deserialize<List<(int RowNumber, string ErrorMessage)>>((string)TempData["FailedRecordsDetails"])
                    : new List<(int RowNumber, string ErrorMessage)>()
            };

            return View(model);
        }

        //--- 02 Uploading Action -- after clicking submit in the view
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadAmeExcelPost(IFormFile excelFile)
        {
            // Define expected column headers
            var expectedHeaders = new List<string>
            {
                "ID_NO", "BRANCH", "PATIENT_INFO", "BIRTH_MONTH", "AME_DATE",
                "AME_MONTH", "AME_QUARTER", "REQUEST", "SOA", "NAME", "POSITION",
                "AGE", "AGE_GROUP", "SEX", "HPN", "DM II", "DYSLIPIDEMIA",
                "HYPERURICEMIA", "OBESE", "NORMAL_FINDINGS", "TOTAL"
            };


            var model = new ViewModelExcelUploadInformation();

            var (isValid, errorMessage) = await _fileValidationService.ValidateFileAsync(excelFile);
            if (!isValid)
            {
                model.Message = errorMessage;
                return View("UploadAmeExcel", model);
            }

            var ameExcelRecord = new List<AmeExcelRecord>();
            var ameHeaderRecord = new List<Ameheader>();
            var ameHeaderRecordExist = new List<Ameheader>();
            var ameDetails = new List<Amedetail>();

            var failedRecords = new List<(int RowNumber, string ErrorMessage)>();

            try
            {
                using (var stream = new MemoryStream())
                {
                    await excelFile.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        foreach (var worksheet in package.Workbook.Worksheets)
                        {
                            var rowCount = worksheet.Dimension.Rows;
                            // Read and validate headers

                            var headers = new List<string>();
                            for (int col = 1; col <= worksheet.Dimension.Columns; col++)
                            {
                                headers.Add(worksheet.Cells[1, col].Text.Trim());
                            }

                            // Check if headers match expected headers
                            var headersMatch = expectedHeaders.SequenceEqual(headers);
                            if (!headersMatch)
                            {
                                TempData["ErrorMessage"] = "The column headers in the uploaded file are incorrect.";
                                return View("UploadAmeExcel", model);
                            }


                            for (int row = 2; row <= rowCount; row++)
                            {
                                try
                                {
                                    var ameExcelRecord_ = new AmeExcelRecord
                                    {
                                        EmployeeId = worksheet.Cells[row, 1].Text,
                                        Branch = worksheet.Cells[row, 2].Text,
                                        PatientInfo = worksheet.Cells[row, 3].Text,
                                        BirthMonth = worksheet.Cells[row, 4].Text,
                                        AmeDate = _fileValidationService.ConvertToDateOnly(worksheet.Cells[row, 5].Text).ToString(),
                                        AmeMonth = worksheet.Cells[row, 6].Text,
                                        AmeQuarter = worksheet.Cells[row, 7].Text,
                                        Request = worksheet.Cells[row, 8].Text,
                                        Soa = worksheet.Cells[row, 9].Text,
                                        Name = worksheet.Cells[row, 10].Text,
                                        Position = worksheet.Cells[row, 11].Text,
                                        Age = worksheet.Cells[row, 12].Text,
                                        AgeGroup = worksheet.Cells[row, 13].Text,
                                        Sex = worksheet.Cells[row, 14].Text,
                                        Hpn = _fileValidationService.CovertToNullableBool(worksheet.Cells[row, 15].Text),
                                        DmIi = _fileValidationService.CovertToNullableBool(worksheet.Cells[row, 16].Text),
                                        Dyslipidemia = _fileValidationService.CovertToNullableBool(worksheet.Cells[row, 17].Text),
                                        Hyperuricemia = _fileValidationService.CovertToNullableBool(worksheet.Cells[row, 18].Text),
                                        Obese = _fileValidationService.CovertToNullableBool(worksheet.Cells[row, 19].Text),
                                        NormalFinding = _fileValidationService.CovertToNullableBool(worksheet.Cells[row, 20].Text),
                                        TotalFinding = byte.Parse(worksheet.Cells[row, 21].Text)
                                    };

                                    ameExcelRecord.Add(ameExcelRecord_);

                                    var ameHeaderRecord_ = new Ameheader
                                    {
                                        Amedate = _fileValidationService.ConvertToDateOnly(worksheet.Cells[row, 5].Text),
                                        RunDate = DateOnly.FromDateTime(DateTime.Now),
                                        Idnumber = worksheet.Cells[row, 1].Text,
                                        Name = worksheet.Cells[row, 10].Text,
                                        Position = worksheet.Cells[row, 11].Text,
                                        Branch = worksheet.Cells[row, 2].Text,
                                        BirthMonth = _fileValidationService.ConvertToInt(worksheet.Cells[row, 4].Text),
                                        Amemonth = _fileValidationService.ConvertToInt(worksheet.Cells[row, 6].Text),
                                        Amequarter = _fileValidationService.ConvertToInt(worksheet.Cells[row, 7].Text),
                                        Request = worksheet.Cells[row, 8].Text,
                                        Soa = worksheet.Cells[row, 9].Text,
                                        CreatedBy = "dbo",
                                        CreatedDateTime = DateTime.Now,

                                    };

                                    ameHeaderRecord.Add(ameHeaderRecord_);
                                }
                                catch (Exception rowEx)
                                {
                                    failedRecords.Add((row, rowEx.Message));
                                }
                            }
                        }
                    }
                }

                using (var transaction = _db.Database.BeginTransaction())
                {
                    try
                    {
                        // Fetch existing records that match EmployeeId and AmeDate
                        var existingRecords = await _aMERepository.GetAmeHeaderRecordsByEmployeeIdAndAmeDateAsync(
                            ameHeaderRecord.Select(r => r.Idnumber).Distinct().ToList(),
                            ameHeaderRecord.Select(r => r.Amedate).Distinct().ToList()
                        );

                        // Identify duplicates
                        var duplicateRecords = ameHeaderRecord
                            .Where(r => existingRecords.Any(e => e.Idnumber == r.Idnumber && e.Amedate == r.Amedate))
                            .ToList();

                        // Exclude duplicates from the records to be saved
                        var newRecords = ameHeaderRecord.Except(duplicateRecords).ToList();

                        if (newRecords.Any())
                        {
                            await _aMERepository.AddAmeHeaderRecordsAsync(newRecords);
                        }


                        //with AmeHeaderId
                        var existingRecords_ = await _aMERepository.GetAmeHeaderRecordsByEmployeeIdAndAmeDateAsync(
                                newRecords.Select(r => r.Idnumber).ToList(),
                                newRecords.Select(r => r.Amedate).ToList()
                        );

                        var ameTest = _aMERepository.GetAllAmeActiveTest();

                        // Define a dictionary mapping test names to column indices with correct type
                        var testColumnMapping = new Dictionary<string, Func<AmeExcelRecord, bool>>
                        {
                            { "HPN", record => record.Hpn.GetValueOrDefault() },
                            { "DM II", record => record.DmIi.GetValueOrDefault() },
                            { "DYSLIPIDEMIA", record => record.Dyslipidemia.GetValueOrDefault() },
                            { "HYPERURICEMIA", record => record.Hyperuricemia.GetValueOrDefault() },
                            { "OBESE", record => record.Obese.GetValueOrDefault() },
                            { "NORMAL_FINDINGS", record => record.NormalFinding.GetValueOrDefault() },
                            { "TOTAL", record => record.TotalFinding > 0 } // Assuming TotalFinding is byte and you want a boolean
                        };

                        foreach (var item in ameExcelRecord)
                        {
                            //var a = item.EmployeeId;
                            //var b = _fileValidationService.ConvertToDateOnly(item.AmeDate);
                            //var ab = existingRecords_.FirstOrDefault(xyz => xyz.Idnumber == a && xyz.Amedate == b);
                            var correspondingRecord = existingRecords_.FirstOrDefault(a => a.Idnumber == item.EmployeeId && a.Amedate == _fileValidationService.ConvertToDateOnly(item.AmeDate));

                            if (correspondingRecord != null)
                            {
                                foreach (var test in ameTest)
                                {
                                    var testName = test.TestName; // Assuming TestName is the property for test names

                                    // Retrieve the result using the dictionary and cast to boolean
                                    var result = testColumnMapping.ContainsKey(testName)
                                        ? testColumnMapping[testName](item)
                                        : false; // Handle cases where the testName is not in the dictionary


                                    ameDetails.Add(new Amedetail
                                    {
                                        AmeheaderId = correspondingRecord.AmeheaderId,
                                        TestId = test.TestId,
                                        Result = result,
                                        CreatedBy = "dbo",
                                        CreatedDateTime = DateTime.Now,
                                    });
                                }
                            }
                        }

                        //saving to AMEDetails Table
                        // Fetch existing records that match EmployeeId and AmeDate
                        var existingRecordsDetails = await _aMERepository.GetAmeDetailRecordsByHeaderIdAndTestIdAsync(
                            ameDetails.Select(r => r.AmeheaderId).Distinct().ToList(),
                            ameDetails.Select(r => r.TestId).Distinct().ToList()
                        );



                        // Identify duplicates
                        var duplicateRecordsDetails = ameDetails
                            .Where(r => existingRecordsDetails.Any(e => e.AmeheaderId == r.AmeheaderId && e.TestId == r.TestId))
                            .ToList();

                        // Exclude duplicates from the records to be saved
                        var newRecordsDetails = ameDetails.Except(duplicateRecordsDetails).ToList();

                        if (newRecordsDetails.Any())
                        {
                            await _aMERepository.AddAmeDetailsRecordsAsync(newRecordsDetails);
                        }

                        transaction.Commit();

                        TempData["UploadedRecords"] = newRecords.Count;
                        TempData["DuplicateRecords"] = duplicateRecords.Count;
                        TempData["FailedRecords"] = failedRecords.Count;
                        TempData["FailedRecordsDetails"] = System.Text.Json.JsonSerializer.Serialize(failedRecords);
                        TempData["Message"] = "File uploaded successfully.";
                        return RedirectToAction("UploadAmeExcel");


                    }
                    catch (Exception ex)
                    {

                        transaction.Rollback();
                        TempData["ErrorMessage"] = $"An error occurred while uploading the file: {ex.Message}";
                        return RedirectToAction("UploadAmeExcel");
                    }
                }





            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while uploading the file.");

                // Set the error message in TempData
                TempData["ErrorMessage"] = $"An error occurred while uploading the file: {ex.InnerException}";
                return RedirectToAction("UploadAmeExcel");
            }
        }


        [HttpGet]
        public IActionResult GetAmeDetails(string idEnc)
        {
            try
            {
                var idDec = _encryptionService.Decrypt(_key, idEnc);


                var details = _aMERepository.GetDetailListByEmployeeIdNumber(idDec);
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

        public IActionResult SaveRemarks(string id, string remarks)
        {
            try
            {
                var idDec = int.Parse(_encryptionService.Decrypt(_key, id));

                if (!_aMERepository.UpdateRemarks(idDec, remarks))
                {
                    return Json(new { success = false, message = "Update encountered an error." });
                }

                return Json(new { success = true, message = "Remarks updated successfully." });
            }
            catch (Exception ex)
            {
                // Log the exception
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}

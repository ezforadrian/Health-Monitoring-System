using _VEHRSv1.Interface;
using _VEHRSv1.Models;
using _VEHRSv1.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;

namespace _VEHRSv1.Controllers
{
    public class ReportController : Controller
    {
        private readonly IPEMERepository _pEMERepository;
        private readonly IReportRepository _reportRepository;
        private readonly EncryptionService _encryptionService;
        private readonly IAMERepository _aMERepository;
        private readonly ConfigurationAppInfo _appInfo;
        private const string _key = "thequickbrownfox";

        public ReportController(
                                    IPEMERepository pEMERepository,
                                    IReportRepository reportRepository,
                                    EncryptionService encryptionService,
                                    IOptions<ConfigurationAppInfo> appInfo,
                                    IAMERepository aMERepository
                                )
        {
            _pEMERepository = pEMERepository;
            _reportRepository = reportRepository;
            _encryptionService = encryptionService;
            _aMERepository = aMERepository;
            _appInfo = appInfo.Value;
        }

        public IActionResult Index() => View();

        public async Task<IActionResult> PemeReports()
        {
            var pemeStatus = await _pEMERepository.GetAllActivePemeStatus();
            pemeStatus.Insert(0, new vmPemeStatus
            {
                IdEncrypt = _encryptionService.EncryptConstant(_key, "-100"),
                StatusId = -100,
                Description = "All"
            });

            ViewBag.Status = JsonConvert.SerializeObject(pemeStatus.Select(p => new { id = p.IdEncrypt, text = p.Description }));
            var reportExport = await _reportRepository.GetAllActiveExportTypes();
            ViewBag.ExportType = JsonConvert.SerializeObject(reportExport.Select(p => new { id = p.RefIdEnc, text = p.ReferenceDescription }));

            return View(new PemeReportViewModel { ReportParamG = new ReportsParameterGeneric() });
        }

        public async Task<IActionResult> AmeReports()
        {
            var ameTest = _reportRepository.GetAllActiveTest().Where(x => x.TestCategory == "AME").ToList();
            ameTest.Insert(0, new vmTestDetails
            {
                TestIdEnc = _encryptionService.EncryptConstant(_key, "-100"),
                TestId = -100,
                TestName = "All",
                TestCategory = "Both",
                IsActive = true
            });


            ViewBag.TestDetails = JsonConvert.SerializeObject(ameTest.Select(p => new { id = p.TestIdEnc, text = p.TestName }));

            return View(new AmeReportViewModel { AmeReportParameters = new AmeReportParameters() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GeneratePemeReportPreview(ReportsParameterGeneric model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });

            try
            {
                int statusId = int.Parse(_encryptionService.DecryptConstant(_key, model.Status));

                if (model.EncodedStartDate.HasValue && model.EncodedEndDate.HasValue)
                {
                    var pemeResult = _reportRepository.GetPemeReportResultByDateRangeAndStatusId(
                        model.EncodedStartDate.Value,
                        model.EncodedEndDate.Value,
                        statusId
                    );

                    return Json(new { success = true, returnToViewAsJson = new PemeReportViewModel { ReportParamG = model, PemeReportResults = pemeResult } });
                }

                return Json(new { success = false, errors = new List<string> { "Start date and end date are required." } });
            }
            catch
            {
                return Json(new { success = false, errors = new List<string> { "An error occurred while processing the request." } });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ValidatePemeParam(ReportsParameterGeneric model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });

            if (model.EncodedStartDate.HasValue && model.EncodedEndDate.HasValue)
            {
                TempData["PemeReportParameters"] = System.Text.Json.JsonSerializer.Serialize(model);
                var baseUrl = $"{Request.Scheme}://{Request.Host.Value}";
                var link = $"{baseUrl}/Report/GeneratePemeReportPDF";
                return Json(new { success = true, link });
            }

            return Json(new { success = false, errors = new List<string> { "Start date and end date are required." } });
        }

        [HttpGet]
        public async Task<IActionResult> GeneratePemeReportPDF(ReportsParameterGeneric model)
        {
            try
            {
                if (TempData["PemeReportParameters"] != null)
                {
                    model = System.Text.Json.JsonSerializer.Deserialize<ReportsParameterGeneric>(TempData["PemeReportParameters"].ToString());

                    if (model.EncodedStartDate.HasValue && model.EncodedEndDate.HasValue)
                    {
                        int statusId = int.Parse(_encryptionService.DecryptConstant(_key, model.Status));
                        string reportLink = $"https://descendo.pagcor.ph/ReportServer/Pages/ReportViewer.aspx?%2fVEHRS%2fDEVT%2fPemeReportByDateRangeAndStatus&rs:Command=Render&EncodedStartDate={model.EncodedStartDate}&EncodedEndDate={model.EncodedEndDate}&rs:Format=PDF&Status={statusId}";

                        WebRequest webRequest = WebRequest.Create(reportLink);
                        webRequest.Credentials = CredentialCache.DefaultCredentials;
                        WebResponse webResponse = webRequest.GetResponse();
                        return new FileStreamResult(webResponse.GetResponseStream(), "application/pdf");
                    }
                }

                return BadRequest();
            }
            catch
            {
                return Json(new { success = false, errors = new List<string> { "An error occurred while generating the report." } });
            }
        }



        //------------------AME
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateAmeReportPreview(AmeReportParameters model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });

            try
            {
                int testId = int.Parse(_encryptionService.DecryptConstant(_key, model.Findings));

                if (model.StartDate.HasValue && model.EndDate.HasValue)
                {
                    var ameResult = _reportRepository.GetAmeReportResultByDateRangeAndTestId(
                        model.StartDate.Value,
                        model.EndDate.Value,
                        testId
                    );

                    return Json(new { success = true, returnToViewAsJson = new AmeReportViewModel { AmeReportParameters = model, AmeReportResults = ameResult } });
                }

                return Json(new { success = false, errors = new List<string> { "Start date and end date are required." } });
            }
            catch
            {
                return Json(new { success = false, errors = new List<string> { "An error occurred while processing the request." } });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ValidateAmeParam(AmeReportParameters model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });

            if (model.StartDate.HasValue && model.EndDate.HasValue)
            {
                TempData["AmeReportParameters"] = System.Text.Json.JsonSerializer.Serialize(model);
                var baseUrl = $"{Request.Scheme}://{Request.Host.Value}";
                var link = $"{baseUrl}/Report/GenerateAmeReportPDF";
                return Json(new { success = true, link });
            }

            return Json(new { success = false, errors = new List<string> { "Start date and end date are required." } });
        }

        [HttpGet]
        public async Task<IActionResult> GenerateAmeReportPDF(AmeReportParameters model)
        {
            try
            {
                if (TempData["AmeReportParameters"] != null)
                {
                    model = System.Text.Json.JsonSerializer.Deserialize<AmeReportParameters>(TempData["AmeReportParameters"].ToString());

                    if (model.StartDate.HasValue && model.EndDate.HasValue)
                    {
                        int testId = int.Parse(_encryptionService.DecryptConstant(_key, model.Findings));
                        string reportLink = $"https://descendo.pagcor.ph/ReportServer/Pages/ReportViewer.aspx?%2fVEHRS%2fDEVT%2fAmeReportByDateRangeAndFindings&rs:Command=Render&StartDate={model.StartDate}&EndDate={model.EndDate}&rs:Format=PDF&Findings={testId}";

                        WebRequest webRequest = WebRequest.Create(reportLink);
                        webRequest.Credentials = CredentialCache.DefaultCredentials;
                        WebResponse webResponse = webRequest.GetResponse();
                        return new FileStreamResult(webResponse.GetResponseStream(), "application/pdf");
                    }
                }

                return BadRequest();
            }
            catch
            {
                return Json(new { success = false, errors = new List<string> { "An error occurred while generating the report." } });
            }
        }
    }
}

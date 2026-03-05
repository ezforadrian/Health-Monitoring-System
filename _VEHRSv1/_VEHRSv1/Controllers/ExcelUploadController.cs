using _VEHRSv1.Interface;
using _VEHRSv1.Models;
using Microsoft.AspNetCore.Mvc;
using _VEHRSv1.Services;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace _VEHRSv1.Controllers
{
    public class ExcelUploadController : Controller
    {
        private readonly ILogger<ExcelUploadController> _logger;
        private readonly HswmsContext _db; //commit and rollback
        private readonly IAppReferenceRepository _appReferenceRepository;


        public ExcelUploadController(
                                        IAppReferenceRepository appReferenceRepository
            )
        {
            _appReferenceRepository = appReferenceRepository;
        }

        public IActionResult Index()
        {
            try
            {
                var excelUploadList = _appReferenceRepository.GetAllUploadExcelRecords();

                var model = new vmExcelUploadInformation
                {
                    Message = TempData["Message"]?.ToString(),
                    UploadedRecordsCount = TempData["UploadedRecords"] != null ? (int)TempData["UploadedRecords"] : 0,
                    DuplicateRecordsCount = TempData["DuplicateRecords"] != null ? (int)TempData["DuplicateRecords"] : 0,
                    FailedRecordsCount = TempData["FailedRecords"] != null ? (int)TempData["FailedRecords"] : 0,
                    FailedRecordsDetails = TempData["FailedRecordsDetails"] != null
                        ? System.Text.Json.JsonSerializer.Deserialize<List<(int RowNumber, string ErrorMessage)>>((string)TempData["FailedRecordsDetails"])
                        : new List<(int RowNumber, string ErrorMessage)>()
                };

                var viewModel = new vmExcelUploadDetails
                {
                    ExcelUploadInformation = model,
                    AppReferenceExcelRecord = excelUploadList

                };

                return View(viewModel);
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}

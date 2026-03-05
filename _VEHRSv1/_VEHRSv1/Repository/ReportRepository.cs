using _VEHRSv1.Interface;
using _VEHRSv1.Models;
using _VEHRSv1.Services;
using Microsoft.EntityFrameworkCore;

namespace _VEHRSv1.Repository
{
    public class ReportRepository : IReportRepository
    {
        private readonly HswmsContext _hswmsContext;
        private readonly EncryptionService _encryptionService;
        private const string _key = "thequickbrownfox";

        public ReportRepository(HswmsContext hswmsContext, EncryptionService encryptionService)
        {
            _hswmsContext = hswmsContext;
            _encryptionService = encryptionService;
        }

        public async Task<List<ExportType>> GetAllActiveExportTypes()
        {
            return await _hswmsContext.AppReferences.Where(x => x.ReferenceGroup == 10)
                                .Select(x => new ExportType
                                {
                                    ReferenceId = x.ReferenceId,
                                    ReferenceGroupId = x.ReferenceGroup,
                                    ReferenceName = x.ReferenceName,
                                    ReferenceCode = x.ReferenceCode,
                                    Sort = x.Sort,
                                    ReferenceDescription = x.ReferenceDescription,
                                    RefIdEnc = _encryptionService.Encrypt(_key, x.ReferenceId.ToString())
                                })
                                .ToListAsync();
        }

        public List<PemeReportResult> GetPemeReportResultByDateRangeAndStatusId(DateOnly startdate, DateOnly enddate, int status)
        {
            try
            {
                var query = from a in _hswmsContext.Pemeheaders
                            join b in _hswmsContext.Pemedetails on a.Pemeid equals b.Pemeid into joinAB
                            from ab in joinAB.DefaultIfEmpty()
                            join c in _hswmsContext.Pemestatuses on a.StatusId equals c.StatusId into statusJoin
                            from c in statusJoin.DefaultIfEmpty()
                            join d in _hswmsContext.AppReferences on ab.MedicalEvaluatorId equals d.ReferenceId into reference
                            from d in reference.DefaultIfEmpty()
                            where (a.CreatedDateTime >= startdate.ToDateTime(TimeOnly.MinValue)) &&
                                  (a.CreatedDateTime <= enddate.ToDateTime(TimeOnly.MinValue)) && d.ReferenceGroup == 4 //medical eval
                            orderby a.CreatedDateTime ascending, a.LastName, a.FirstName, a.MiddleName
                            select new PemeReportResult
                            {
                                EmployeeId = a.Idnumber ?? "Not Available",
                                FullName = a.LastName + ", " + a.FirstName + " " + a.MiddleName,
                                BirthDate = a.Birthdate,
                                ExamDate = DateOnly.Parse(ab.ExamDate.ToString("MM/dd/yyyy")),
                                StatusDescription = c.Description,
                                Status = c.StatusId,
                                MedicalEvaluator = d.ReferenceName
                            };
                if (status != -100)
                {
                    query = query.Where(a => a.Status == status);
                }

                var result = query.ToList();

                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<vmTestDetails> GetAllActiveTest()
        {
            try
            {
                return _hswmsContext.TestDetails
                    .Select(x => new vmTestDetails
                    {
                        TestId = x.TestId,
                        TestIdEnc = _encryptionService.EncryptConstant(_key, x.TestId.ToString()),
                        TestName = x.TestName,
                        TestCategory = x.TestCategory,
                        IsActive = x.IsActive,
                    })
                    .ToList();
            }
            catch (Exception)
            {

                throw;
            }

        }


        //public List<AmeReportResult> GetAmeReportResultByDateRangeAndTestId(DateOnly startdate, DateOnly enddate, int testId)
        //{
        //    try
        //    {
        //        var query = from a in _hswmsContext.Ameheaders
        //                    join b in _hswmsContext.Amedetails on a.AmeheaderId equals b.AmeheaderId into joinAB
        //                    from ab in joinAB.DefaultIfEmpty()
        //                    join c in _hswmsContext.TestDetails on ab.TestId equals c.TestId into statusJoin
        //                    from c in statusJoin.DefaultIfEmpty()
        //                    where (a.Amedate >= startdate) &&
        //                          (a.Amedate <= enddate) && ab.Result == true
        //                    select new AmeReportResult
        //                    {
        //                        AmeDate = a.Amedate,
        //                        Branch = a.Branch,
        //                        IDNumber = a.Idnumber,
        //                        Name = a.Name,
        //                        Position = a.Position,
        //                        BirthMonth = a.BirthMonth,
        //                        CombinedFindings = "",
        //                        TestId = ab.TestId,
        //                        TestName = c.TestName

        //                    };
        //        if (testId != -100)
        //        {
        //            query = query.Where(a => a.TestId == testId);
        //        }

        //        var result = query.ToList().OrderBy(a => a.IDNumber);


        //        var testDetails = _hswmsContext.TestDetails
        //            .Where(t => t.IsActive == true && t.TestCategory == "AME")
        //            .Select(t => new vmTestDetails
        //            {
        //                TestId = t.TestId,
        //                TestIdEnc = _encryptionService.EncryptConstant(_key, t.TestId.ToString()),
        //                TestName = t.TestName,
        //                TestCategory = t.TestCategory,
        //                IsActive = t.IsActive,
        //            }).ToList();

        //        var finalResult = new List<AmeReportResult>();
        //        var currentId = "";
        //        var combinedFindings = "";
        //        foreach (var item in result)
        //        {
        //            if (currentId == "") //first record in loop
        //            {
        //                foreach (var test in testDetails)
        //                {
        //                    if (item.TestId == test.TestId)
        //                    {
        //                        combinedFindings = combinedFindings + " " + test.TestName;
        //                    }
        //                }

        //                currentId = item.IDNumber;
        //            }
        //            else
        //            {
        //                if (currentId == item.IDNumber)
        //                {

        //                }
        //                else
        //                { 

        //                }
        //                //not the first record in loop
        //                foreach (var test in testDetails)
        //                {
        //                    if (item.TestId == test.TestId)
        //                    {
        //                        combinedFindings = combinedFindings + " " + test.TestName;
        //                    }
        //                }

        //                currentId = item.IDNumber;
        //            }



        //        }



        //        return result;
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}


        public List<AmeReportResult> GetAmeReportResultByDateRangeAndTestId(DateOnly startdate, DateOnly enddate, int testId)
        {
            try
            {
                var query = from a in _hswmsContext.Ameheaders
                            join b in _hswmsContext.Amedetails on a.AmeheaderId equals b.AmeheaderId into joinAB
                            from ab in joinAB.DefaultIfEmpty()
                            join c in _hswmsContext.TestDetails on ab.TestId equals c.TestId into statusJoin
                            from c in statusJoin.DefaultIfEmpty()
                            where (a.Amedate >= startdate) &&
                                  (a.Amedate <= enddate) && ab.Result == true
                            select new
                            {
                                AmeDate = a.Amedate,
                                Branch = a.Branch,
                                IDNumber = a.Idnumber,
                                Name = a.Name,
                                Position = a.Position,
                                BirthMonth = a.BirthMonth,
                                TestId = ab.TestId,
                                TestName = c.TestName
                            };

                if (testId != -100)
                {
                    query = query.Where(a => a.TestId == testId);
                }

                var result = query.ToList()
                                  .GroupBy(x => new { x.IDNumber, x.AmeDate })
                                  .Select(g => new AmeReportResult
                                  {
                                      AmeDate = g.Key.AmeDate,
                                      Branch = g.First().Branch,
                                      IDNumber = g.Key.IDNumber,
                                      Name = g.First().Name,
                                      Position = g.First().Position,
                                      BirthMonth = g.First().BirthMonth,
                                      CombinedFindings = string.Join(", ", g.Select(x => x.TestName)),
                                      //TestId = g.First().TestId,
                                      //TestName = g.First().TestName // You might want to revise this as it can be misleading
                                  })
                                  .OrderBy(a => a.IDNumber)
                                  .ToList();

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}

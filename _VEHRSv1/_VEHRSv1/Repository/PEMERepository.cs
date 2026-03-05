using _VEHRSv1.Interface;
using _VEHRSv1.Models;
using _VEHRSv1.Services;
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;

namespace _VEHRSv1.Repository
{
    public class PEMERepository : IPEMERepository
    {
        private readonly HswmsContext _hswmsContext;
        private readonly EncryptionService _encryptionService;
        private readonly IAS400PlantillaRepository _aS400PlantillaRepository;
        private const string _key = "thequickbrownfox";
        public PEMERepository(HswmsContext hswmsContext, EncryptionService encryptionService, IAS400PlantillaRepository aS400PlantillaRepository)
        {
            _hswmsContext = hswmsContext;
            _encryptionService = encryptionService;
            _aS400PlantillaRepository = aS400PlantillaRepository;
        }
        public async Task<List<vmPemeStatus>> GetAllActivePemeStatus()
        {
            return await _hswmsContext.Pemestatuses
                                      .Where(x => x.IsActive == true)
                                      .Select(x => new vmPemeStatus { 
                                        IdEncrypt = _encryptionService.EncryptConstant(_key, x.StatusId.ToString()),
                                        StatusId = x.StatusId,
                                        Description = x.Description
                                      })
                                      .ToListAsync();
        }

        public async Task<List<vmMedicalEvaluator>> GetAllActiveMedicalEvaluator()
        {
            //get the reference code for the medical evaluator
            int groupCode = _hswmsContext.AppReferenceGroups.Where(x => x.GroupName == "Medical Evaluator").Select(x => x.ReferenceGroupId).FirstOrDefault();

            return await _hswmsContext.AppReferences
                                       .Where(x => x.IsActive == true && x.ReferenceGroup == groupCode)
                                       .Select(x => new vmMedicalEvaluator { 
                                            IdEncrypt = _encryptionService.EncryptConstant(_key, x.ReferenceId.ToString()),
                                            AppReferenceId = x.ReferenceId,
                                            ReferenceName = x.ReferenceName
                                       })
                                       .ToListAsync();
        }



        //check if peme header record already exist
        public bool CheckRecordExist(string lastname, string firstname, string middlename, DateOnly birthdate)
        {
            // Trim and convert names to uppercase
            lastname = lastname.Trim().ToUpper();
            firstname = firstname.Trim().ToUpper();
            middlename = middlename.Trim().ToUpper();

            // Check if the record exists
            return _hswmsContext.Pemeheaders.Any(x =>
                x.LastName.Trim().ToUpper() == lastname &&
                x.FirstName.Trim().ToUpper() == firstname &&
                x.MiddleName.Trim().ToUpper() == middlename &&
                x.Birthdate.HasValue && x.Birthdate == birthdate
            );
        }

        public async Task AddPemeheader(Pemeheader pemeheader)
        {
            await _hswmsContext.Pemeheaders.AddAsync(pemeheader);
            await _hswmsContext.SaveChangesAsync();
        }

        public bool AddPemedetail(Pemedetail pemedetail)
        {
            _hswmsContext.Pemedetails.Add(pemedetail);
            return _hswmsContext.SaveChanges() > 0;
        }

        public async Task<PagedResult<vmSearchPeme>> SearchPEMEAsync(string query, int page, int pageSize)
        {
            var records = _hswmsContext.Pemeheaders
                .Where(r => EF.Functions.Like(r.LastName, $"%{query}%") ||
                            EF.Functions.Like(r.FirstName, $"%{query}%") ||
                            EF.Functions.Like(r.MiddleName, $"%{query}%") ||
                            EF.Functions.Like((r.FirstName + ' ' + r.MiddleName + ' ' + r.LastName), $"%{query}%") ||
                            EF.Functions.Like(r.Idnumber, $"%{query}%") ||
                            EF.Functions.Like(r.PositionRef, $"%{query}%"))
                .Select(x => new vmSearchPeme
                {
                    PemeidString = _encryptionService.Encrypt(_key, x.Pemeid.ToString()),
                    Pemeid = x.Pemeid,
                    LastName = x.LastName,
                    FirstName = x.FirstName,
                    MiddleName = x.MiddleName,
                    Birthdate = x.Birthdate,
                    PositionRef = _encryptionService.EncryptConstant(_key, x.PositionRef),
                    PositionDescription = _aS400PlantillaRepository.GetPositionDescriptionByCode(x.PositionRef),
                    StatusId = x.StatusId,
                    StatusDescription = _hswmsContext.Pemestatuses.Where(s => s.StatusId == x.StatusId).Select(s => s.Description).FirstOrDefault(),
                    Idnumber = string.IsNullOrEmpty(x.Idnumber) ? "Not Available" : x.Idnumber,
                })
                .OrderBy(r => r.LastName);

            var totalRecords = await records.CountAsync();
            var results = await records.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedResult<vmSearchPeme>
            {
                TotalRecords = totalRecords,
                PageSize = pageSize,
                CurrentPage = page, // Set the current page correctly
                Results = results
            };
        }




        public string GetStatusDescriptionUsingStatusId(int statusId)
        {
            
            return _hswmsContext. Pemestatuses.Where(x => x.StatusId == statusId).Select(x => x.Description).FirstOrDefault();
        }

        public vmPemeEmployeeRecord GetEmployeePemeDetails(int id)
        {
            var pemeHeader = _hswmsContext.Pemeheaders.Include(p => p.Pemedetails).FirstOrDefault(p => p.Pemeid == id);

            if (pemeHeader == null)
            {
                return null;
            }

            var viewModel = new vmPemeEmployeeRecord
            {
                Pemeid = pemeHeader.Pemeid,
                PemeidString = _encryptionService.Encrypt(_key, pemeHeader.Pemeid.ToString()),
                LastName = pemeHeader.LastName,
                FirstName = pemeHeader.FirstName,
                MiddleName = pemeHeader.MiddleName == null ? "" : pemeHeader.MiddleName,
                Birthdate = pemeHeader.Birthdate,
                Position = _encryptionService.EncryptConstant(_key, pemeHeader.PositionRef),
                Status = _encryptionService.EncryptConstant(_key, pemeHeader.StatusId.ToString()),
                Idnumber = pemeHeader.Idnumber == null ? "Not Available" : pemeHeader.Idnumber,
                PemeDetails = pemeHeader.Pemedetails.Select(d => new vmPemeHeaderDetails
                {
                    DetailId = d.DetailId,
                    DetailIdString = _encryptionService.Encrypt(_key, d.DetailId.ToString()),
                    Pemeid = d.Pemeid,
                    PemeIdString = _encryptionService.Encrypt(_key, pemeHeader.Pemeid.ToString()),
                    ExamDate = d.ExamDate,
                    Remarks = d.Remarks,
                    MedicalEvaluatorId = d.MedicalEvaluatorId,
                    MedicalEvaluatorIdString = _encryptionService.EncryptConstant(_key, d.MedicalEvaluatorId.ToString()),
                    MedicalEvaluatorName = _hswmsContext.AppReferences.Where(x => x.ReferenceId == d.MedicalEvaluatorId).Select(x => x.ReferenceName).FirstOrDefault(),
                }).ToList()
            };

            return viewModel;
        }

        public bool CheckRecordExistUsingPemeId(int id)
        {
            return _hswmsContext.Pemeheaders.Where(p => p.Pemeid == id).Any();
        }

        public bool UpdatePemeheader(vmPemeEmployeeRecord model)
        {

            var pemeHeader = _hswmsContext.Pemeheaders.Where(x => x.Pemeid == model.Pemeid).FirstOrDefault();
            if (pemeHeader == null) 
            {
                return false;
            }

            pemeHeader.LastName = model.LastName.Trim();
            pemeHeader.FirstName = model.FirstName.Trim();
            pemeHeader.MiddleName = model.MiddleName.Trim();
            pemeHeader.Birthdate = model.Birthdate;
            pemeHeader.PositionRef = model.Position;
            pemeHeader.StatusId = int.Parse(model.Status);
            pemeHeader.Idnumber = model.Idnumber == "Not Available" ? null : model.Idnumber;
            pemeHeader.ModifiedBy = "dbo"; //update to current user
            pemeHeader.ModifiedDateTime = DateTime.Now;

            return _hswmsContext.SaveChanges() > 0;


        }

        public bool CheckPemeDetailsExistUsingPemeIdAndPemeExamDate(int pemeid, DateTime examdate)
        {
            return _hswmsContext.Pemedetails.Where(x => x.Pemeid == pemeid && x.ExamDate == examdate).Any();
        }

        public bool CheckPemeDetailsExistUsingPemeIdAndPemeExamDate(int pemeid, DateTime examdate, int pemedetailID)
        {
            return _hswmsContext.Pemedetails.Where(x => x.Pemeid == pemeid && x.ExamDate == examdate && x.DetailId != pemedetailID).Any();
        }

        public bool UpdatePemedetail(Pemedetail model)
        {
            var pemedetail = _hswmsContext.Pemedetails.Where(x => x.Pemeid == model.Pemeid && x.DetailId == model.DetailId ).FirstOrDefault();
            if (pemedetail == null)
            {
                return false;
            }

            pemedetail.ExamDate = model.ExamDate;
            pemedetail.Remarks = model.Remarks;
            pemedetail.MedicalEvaluatorId = model.MedicalEvaluatorId;

            return _hswmsContext.SaveChanges() > 0;
        }

        public vmPemeEmployeeRecord GetEmployeePemeDetails(string idNumber)
        {
            throw new NotImplementedException();
        }
    }
}

using _VEHRSv1.Helper;
using _VEHRSv1.Interface;
using _VEHRSv1.Models;
using _VEHRSv1.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Novell.Directory.Ldap;
using System.Data;
using System.DirectoryServices;
using System.Security.Cryptography.Xml;

namespace _VEHRSv1.Repository
{
    public class MwrRepository : IMwrRepository
    {
        private readonly HswmsContext _db;
        private readonly EncryptionService _encryptionService;
        private const string _key = "thequickbrownfox";

        public MwrRepository( HswmsContext hswmsContext, EncryptionService encryptionService)
        {
            _db = hswmsContext;
            _encryptionService = encryptionService;
        }


       

        public List<Mwrlist> Mwrlist()
        {

            var MwrList = _db.Mwrlists
                 .Select(mwr => new Mwrlist
                 {
                     ActivityName = mwr.ActivityName,
                     ActivityType = mwr.ActivityType,
                     MwrlistId = mwr.MwrlistId
                 })
                .ToList();


            return MwrList;

        }

        public List<Mwrdate> MwrlistDate()
        {

            var MwrListDate = _db.Mwrdates
                 .Select(mwrdate => new Mwrdate
                 {
                     MwractDate = mwrdate.MwractDate,
                     CreatedDate = mwrdate.CreatedDate,
                     CreatedBy = mwrdate.CreatedBy,
                     MwrlistId = mwrdate.MwrlistId
                 })
                .ToList();


            return MwrListDate;

        }

        public async Task<List<Mwrdate>> GetMwrDate(int MwrId)
        {

            var mwrListDate = await (from MwrlistDate in _db.Mwrdates
                                     where MwrlistDate.MwrlistId == MwrId
                                     select new Mwrdate
                                 {
                                     MwrdateId = MwrlistDate.MwrdateId,
                                     MwrlistId = MwrlistDate.MwrlistId,
                                     MwractDate = MwrlistDate.MwractDate,
                                     CreatedBy = MwrlistDate.CreatedBy,
                                     CreatedDate = MwrlistDate.CreatedDate
                                 })
                           .ToListAsync();
            
            return mwrListDate;

        }

        public async Task<List<vm_Mwrlist>> GetMwrlist()
        {
            var mwrList = await (from mwrlist in _db.Mwrlists
                                 join reference in _db.AppReferences 
                                 on mwrlist.ActivityType equals reference.ReferenceCode.ToString()
                                 where reference.ReferenceGroup == 7
                                 select new vm_Mwrlist
                                 {
                                     MwrlistId = mwrlist.MwrlistId,
                                     ActivityName = mwrlist.ActivityName,
                                     ActivityType = reference.ReferenceName,
                                     ReferenceCode = reference.ReferenceCode,
                                     CreatedBy = mwrlist.CreatedBy,
                                     CreatedDateTime = mwrlist.CreatedDateTime
                                 })
                           .ToListAsync(); 

            return mwrList;

        }

        public List<Mwractivity> mwrParticipantList()
        {


            var mwrListParticipant =  _db.Mwractivities
                .Select(mwrParticipant => new Mwractivity
                {
                MwractId = mwrParticipant.MwractId,
                ActDate = mwrParticipant.ActDate,
                Idnumber = mwrParticipant.Idnumber,
                MwrlistId = mwrParticipant.MwrlistId,
                CreatedBy = mwrParticipant.CreatedBy,
                CreatedDateTime = mwrParticipant.CreatedDateTime,
                })
                .ToList();

            return mwrListParticipant;

        }


        public async Task<List<Mwractivity>> GetParticipant()
        {
            var mwrList = await (from mwrPart in _db.Mwractivities
                                 select new Mwractivity
                                 {
                                     MwrlistId = mwrPart.MwrlistId,
                                     ActDate = mwrPart.ActDate,
                                     
                                     CreatedDateTime = mwrPart.CreatedDateTime
                                 })
                           .ToListAsync();

            return mwrList;

        }



        public Task<List<AppReference>> GetActivity()
        {

            var appReference = _db.AppReferences
                 .Select(Ap => new AppReference
                 {
                     ReferenceGroup = Ap.ReferenceGroup,
                     ReferenceCode = Ap.ReferenceCode,
                     ReferenceName = Ap.ReferenceName,
                     IsActive = Ap.IsActive,
                 })
                 .Where(x => x.ReferenceGroup == 7 && x.IsActive == true)
                 .OrderBy(x => x.ReferenceName)
                 .ToListAsync();


            return appReference;

        }

        public async Task<List<Mwrdate>> GetActivityDate(int MwrId)
        {

            var dateReference = await _db.Mwrdates
                 .Select(dr => new Mwrdate
                 {
                     MwractDate = dr.MwractDate,
                     MwrlistId = dr.MwrlistId,

                 })
                 .Where(x => x.MwrlistId == MwrId)
                 .OrderBy(x => x.MwractDate)
                 .ToListAsync();


            return dateReference;

        }



        public async Task<List<Mwrdate>> getDateSelectedMwr(int MwrId)
        {
            var mwrListDate = await (from MwrlistDate in _db.Mwrdates
                                    where MwrlistDate.MwrlistId == MwrId
                                    select new Mwrdate
                                    {
                                        MwrdateId = MwrlistDate.MwrdateId,
                                        MwrlistId = MwrlistDate.MwrlistId,
                                        MwractDate = MwrlistDate.MwractDate,
                                        CreatedBy = MwrlistDate.CreatedBy,
                                        CreatedDate = MwrlistDate.CreatedDate
                                    })
                           .ToListAsync();

            return mwrListDate;
        }


        public bool DeleteMwrAct(int MwrAct)
        {

            bool enrolledEmployeeAct = _db.Mwractivities.Any(x => x.MwrlistId == MwrAct);
            
            if (enrolledEmployeeAct == false)
            {

                var Selectedmwr = _db.Mwrlists.Where(a => a.MwrlistId == MwrAct).FirstOrDefault();
                var Selectedmwr2 = _db.Mwrdates.Where(a => a.MwrlistId == MwrAct).ToList();

                _db.Remove(Selectedmwr);
                _db.RemoveRange(Selectedmwr2);
                return Save();
            }
            else
            {
                return false;
            }
        }


        public bool DeleteMwrActDate(int MwrAct, DateTime Dmwrdate)
        {

            bool enrolledEmployeeAct = _db.Mwractivities.Any(x => x.MwrlistId == MwrAct && x.ActDate == Dmwrdate);

            if (enrolledEmployeeAct == false)
            {

                var SelectedmwrDate = _db.Mwrdates.Where(x => x.MwrlistId == MwrAct && x.MwractDate == Dmwrdate).FirstOrDefault();
                _db.Remove(SelectedmwrDate);
                return Save();
            }
            else
            {
                return false;
            }
        }

        public bool UpdateMwrAct(string CUser,Mwrlist UpdateMwr)
        {

            var SelectedMwr = _db.Mwrlists.Where(a => a.ActivityName == UpdateMwr.ActivityName).FirstOrDefault();
            var SelectedMwrType = _db.Mwrlists.Where(a => a.MwrlistId== UpdateMwr.MwrlistId).Select(x =>x.ActivityType).FirstOrDefault();
           
            bool isExist = _db.Mwrlists.Any(x => x.ActivityName == UpdateMwr.ActivityName);
            bool isExist2 = _db.Mwrlists.Any(x => x.ActivityName == UpdateMwr.ActivityName && x.ActivityType == UpdateMwr.ActivityType);
            
            
            var edit_mwr = (from mwr in _db.Mwrlists
                            where mwr.MwrlistId == UpdateMwr.MwrlistId
                            select mwr).Single();

            if (isExist)
            {
                if (isExist2)
                {
                    return false;
                }
                else
                {
                    if(isExist && SelectedMwrType != UpdateMwr.ActivityType)
                    {
                            edit_mwr.ActivityName = UpdateMwr.ActivityName.ToUpper();
                            edit_mwr.ActivityType = UpdateMwr.ActivityType;
                            edit_mwr.ModifiedBy = CUser;
                            edit_mwr.ModifiedDateTime = DateTime.Now;   
                    }
                    else
                    {
                        return false;
                    }
                }
                //return true;
            }

            edit_mwr.ActivityName = UpdateMwr.ActivityName.ToUpper();
            edit_mwr.ActivityType = UpdateMwr.ActivityType;
            edit_mwr.ModifiedBy = CUser;
            edit_mwr.ModifiedDateTime = DateTime.Now;
           
                
            return _db.SaveChanges() > 0;
        }
       


        public bool AddMwrAct(Mwrlist NewMwr)
        {
            _db.Mwrlists.Add(NewMwr);
            return Save();
        }

        public bool AddMwrParticipant(Mwractivity NewMwrParticipant)
        {
            _db.Mwractivities.Add(NewMwrParticipant);
            return Save();
        }

        public bool AddMwrActDate(Mwrdate NewMwrDate)
        {
            _db.Mwrdates.Add(NewMwrDate);
            return Save();
        }
        public bool Save()
        {
            return _db.SaveChanges() > 0;
        }

        public List<vmMWRRecordPerEmployee> GetAll(string idNumber)
        {
            // Retrieve data with minimal transformations
            var data = (from a in _db.Mwractivities
                        join b in _db.Mwrlists on a.MwrlistId equals b.MwrlistId into ab
                        from b in ab.DefaultIfEmpty()
                        join c in _db.Mwrdates on a.MwrlistId equals c.MwrlistId into ac
                        from c in ac.DefaultIfEmpty()
                        where a.Idnumber == idNumber

                        select new
                        {
                            a.Idnumber,
                            a.ActDate,
                            b.ActivityName,
                            b.ActivityType
                        }).ToList();

            // Convert data to the desired output format with type conversion
            var empMwr = data
                .Select(d => new vmMWRRecordPerEmployee
                {
                    EmpIdEnc = _encryptionService.Encrypt(_key, d.Idnumber),
                    IdNumber = d.Idnumber,
                    ActivityName = d.ActivityName,
                    ActDate = DateOnly.FromDateTime(d.ActDate),
                    ActivityType = GetReferenceName(int.Parse(d.ActivityType))
                }).ToList();

            return empMwr;
        }

        // Helper method to get the reference name
        private string GetReferenceName(int referenceCode)
        {
            return _db.AppReferences
                   .Where(r => r.ReferenceCode == referenceCode && r.ReferenceGroup == 7)
                   .Select(r => r.ReferenceName)
                   .FirstOrDefault();
        }
    }
}

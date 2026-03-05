using _VEHRSv1.Interface;
using _VEHRSv1.Models;
using _VEHRSv1.Services;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;

namespace _VEHRSv1.Repository
{
    public class AppReferenceRepository : IAppReferenceRepository
    {
        private readonly HswmsContext _db;
        private readonly EncryptionService _encryptionService;
        private const string _key = "thequickbrownfox";

        public AppReferenceRepository(
                                        HswmsContext hswmsContext,
                                        EncryptionService encryptionService
                                    )
        {
            _db = hswmsContext;
            _encryptionService = encryptionService;
        }


        public async Task<IEnumerable<vmAppReference>> GetAllUploadExcelRecords(int referenceGroup = 1)
        {
            return await _db.AppReferences
                            .Where(
                                    a =>
                                    a.ReferenceGroup == referenceGroup
                                    && a.IsActive == true)
                            .Select(a => new vmAppReference
                            {
                                ReferenceId = a.ReferenceId,
                                ReferenceIdString = _encryptionService.Encrypt(_key, a.ReferenceId.ToString()),
                                ReferenceGroup = a.ReferenceGroup,
                                ReferenceCode = a.ReferenceCode,
                                Sort = a.Sort,
                                ReferenceName = a.ReferenceName,
                                ReferenceDescription = a.ReferenceDescription
                            })
                            .OrderBy(a => a.Sort)
                            .ToListAsync();
        }

        public async Task<List<AppReference>> GetAllActiveRecords()
        {
            return await _db.AppReferences.Where(x => x.IsActive == true).ToListAsync();
        }
    }
}

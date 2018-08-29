using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.Extensions.Configuration;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Common;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Repositories.Interface;
using Skoruba.IdentityServer4.Admin.EntityFramework.Entities;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Repositories.Dapper
{
    public class PersistedGrantDapperRepository : IPersistedGrantRepository
    {
        private readonly IConfiguration _configuration;

        public bool AutoSaveChanges { get; set; } = true;

        public PersistedGrantDapperRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<PagedList<PersistedGrantDataView>> GetPersitedGrantsByUsers(string search, int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<PersistedGrantDataView>();

            using (var dbContext = ConnectionDatabase.Get(_configuration))
            {
                var persistedGrantsData = await dbContext.QueryAsync<PersistedGrantDataView>($@"select distinct PersistedGrants.SubjectId, Users.UserName as SubjectName
                                                                                                from PersistedGrants
                                                                                                inner join Users on Users.Id = CAST(PersistedGrants.SubjectId AS UNIQUEIDENTIFIER)
                                                                                                Where PersistedGrants.SubjectId like @search or Users.UserName like @search
                                                                                                OFFSET {page} ROWS FETCH NEXT {pageSize} ROWS ONLY;", new { search = $"%{search}%" });

                pagedList.Data.AddRange(persistedGrantsData);
                pagedList.TotalCount = persistedGrantsData.Count();
                pagedList.PageSize = pageSize;
            }

            return pagedList;
        }

        public async Task<PagedList<PersistedGrant>> GetPersitedGrantsByUser(string subjectId, int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<PersistedGrant>();

            using (var dbContext = ConnectionDatabase.Get(_configuration))
            {
                var persistedGrantsData = await dbContext.QueryAsync<PersistedGrant>($@"select * from PersistedGrants where SubjectId=@subjectId order by SubjectId OFFSET {page} ROWS FETCH NEXT {pageSize} ROWS ONLY;", new { subjectId });

                pagedList.Data.AddRange(persistedGrantsData);
                pagedList.TotalCount = persistedGrantsData.Count();
                pagedList.PageSize = pageSize;
            }

            return pagedList;
        }

        public Task<PersistedGrant> GetPersitedGrantAsync(string key)
        {
            using (var dbContext = ConnectionDatabase.Get(_configuration))
            {
                return dbContext.QueryFirstAsync<PersistedGrant>("select * from PersistedGrants where Key=@key", new { key });

            }
        }

        public async Task<int> DeletePersistedGrantAsync(string key)
        {
            using (var dbContext = ConnectionDatabase.Get(_configuration))
            {
                return await dbContext.ExecuteAsync("delete from PersistedGrants where Key=@key", new { key });
            }
        }

        public Task<bool> ExistsPersistedGrantsAsync(string subjectId)
        {
            using (var dbContext = ConnectionDatabase.Get(_configuration))
            {
                var persistedGrant = dbContext.QueryFirstOrDefaultAsync<PersistedGrant>("select * from PersistedGrants where SubjectId=@subjectId", new { subjectId });

                return Task.Run(() => persistedGrant != null);
            }
        }

        public async Task<int> DeletePersistedGrantsAsync(int userId)
        {
            using (var dbContext = ConnectionDatabase.Get(_configuration))
            {
                return await dbContext.ExecuteAsync("delete from PersistedGrants where SubjectId=@userId", new { userId });
            }
        }

        public async Task<int> SaveAllChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
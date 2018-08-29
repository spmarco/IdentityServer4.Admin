using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Common;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Repositories.Interface;
using Skoruba.IdentityServer4.Admin.EntityFramework.Entities;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Repositories.Dapper
{
    public class LogDapperRepository : ILogRepository
    {
        private readonly IConfiguration _configuration;

        public LogDapperRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<PagedList<Log>> GetLogsAsync(string search, int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<Log>();
            
            using (var dbContext = ConnectionDatabase.Get(_configuration))
            {
                var logs = await dbContext.QueryAsync<Log>($@"select * from Log where LogEvent like @search or Message like @search or Exception like @search order by Id OFFSET {page} ROWS FETCH NEXT {pageSize} ROWS ONLY;", new { search = $"%{search}%" });

                pagedList.Data.AddRange(logs);
                pagedList.TotalCount = logs.Count();
                pagedList.PageSize = pageSize;
            }

            return pagedList;
        }
    }
}
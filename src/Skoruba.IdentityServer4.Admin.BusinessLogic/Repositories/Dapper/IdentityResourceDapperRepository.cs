using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.Extensions.Configuration;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Common;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Repositories.Interface;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Repositories.Dapper
{
    public class IdentityResourceDapperRepository : IIdentityResourceRepository
    {
        private readonly IConfiguration _configuration;

        public IdentityResourceDapperRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<PagedList<IdentityResource>> GetIdentityResourcesAsync(string search, int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<IdentityResource>();

            using (var dbContext = ConnectionDatabase.Get(_configuration))
            {
                var apiResources = await dbContext.QueryAsync<IdentityResource>($@"select * from IdentityResources where Name like @search order by Name OFFSET {page} ROWS FETCH NEXT {pageSize} ROWS ONLY;", new { search = $"%{search}%" });

                pagedList.Data.AddRange(apiResources);
                pagedList.TotalCount = apiResources.Count();
                pagedList.PageSize = pageSize;
            }

            return pagedList;
        }

        public Task<IdentityResource> GetIdentityResourceAsync(int identityResourceId)
        {
            return Task.Run(() =>
            {
                using (var dbContext = ConnectionDatabase.Get(_configuration))
                {
                    IdentityResource identityResource;
                    using (var result = dbContext.QueryMultiple(@"select * from IdentityResources where Id = @identityResourceId;
                                                                  select * from IdentityClaims where IdentityResourceId = @identityResourceId", new { identityResourceId }))
                    {
                        identityResource = result.ReadFirst<IdentityResource>();
                        identityResource.UserClaims = result.Read<IdentityClaim>().ToList();
                    }

                    return identityResource;
                }
            });
        }

        public async Task<int> AddIdentityResourceAsync(IdentityResource identityResource)
        {
            using (var dbContext = ConnectionDatabase.Get(_configuration))
            {
                using (var transaction = dbContext.BeginTransaction())
                {
                    var affectedRows = 0;
                    var id = await dbContext.QueryFirstOrDefaultAsync<int>("insert into IdentityResources values (@Enabled,@Name,@DisplayName,@Description,@Required,@Emphasize,@ShowInDiscoveryDocument); select SCOPE_IDENTITY();", identityResource, transaction);

                    if (id == 0)
                        return id;

                    affectedRows = 1;

                    if (identityResource.UserClaims.Any())
                    {
                        affectedRows = await dbContext.ExecuteAsync($"insert into IdentityClaims values (@Type,@IdentityResourceId);", identityResource.UserClaims.Select(x => new
                        {
                            x.Type,
                            IdentityResourceId = id
                        }), transaction);
                    }

                    if (affectedRows == 0)
                        transaction.Rollback();
                    else
                        transaction.Commit();

                    return id;
                }
            }
        }

        public async Task<bool> CanInsertIdentityResourceAsync(IdentityResource identityResource)
        {
            using (var dbContext = ConnectionDatabase.Get(_configuration))
            {
                if (identityResource.Id == 0)
                {
                    var existsWithClientName = await dbContext.QueryFirstAsync<Client>("select * from IdentityResources where Name=@Name", new { identityResource.Name });
                    return existsWithClientName == null;
                }
                else
                {
                    var existsWithClientName = await dbContext.QueryFirstAsync<Client>("select * from IdentityResources where Name=@Nam Id<>@Id", new { identityResource.Name, identityResource.Id });
                    return existsWithClientName == null;
                }
            }
        }

        private async Task RemoveIdentityResourceClaimsAsync(IdentityResource identityResource)
        {
            using (var dbContext = ConnectionDatabase.Get(_configuration))
            {
                await dbContext.ExecuteAsync("delete from IdentityClaims where IdentityResourceId=@Id", new { identityResource.Id });
            }
        }

        public async Task<int> DeleteIdentityResourceAsync(IdentityResource identityResource)
        {
            using (var dbContext = ConnectionDatabase.Get(_configuration))
            {
                return await dbContext.ExecuteAsync("delete from IdentityResources where Id=@Id", new { identityResource.Id });
            }
        }

        public async Task<int> UpdateIdentityResourceAsync(IdentityResource identityResource)
        {
            //Remove old relations
            await RemoveIdentityResourceClaimsAsync(identityResource);

            var query = @"update IdentityResources 
                          set Enabled = @Enabled,
                              Name = @Name,
                              DisplayName = @DisplayName,
                              Description = @Description,
                              Required = @Required,
                              Emphasize = @Emphasize,
                              ShowInDiscoveryDocument = @ShowInDiscoveryDocument
                           where
                               Id=@Id";

            using (var dbContext = ConnectionDatabase.Get(_configuration))
            {
                using (var transaction = dbContext.BeginTransaction())
                {
                    var affectedRows = await dbContext.ExecuteAsync(query, identityResource, transaction);

                    if (affectedRows == 0) return affectedRows;
                    if (identityResource.UserClaims.Any())
                    {
                        affectedRows = await dbContext.ExecuteAsync($"insert into IdentityClaims values (@Type,@IdentityResourceId);", identityResource.UserClaims.Select(x => new
                        {
                            x.Type,
                            IdentityResourceId = identityResource.Id
                        }), transaction);
                    }

                    if (affectedRows == 0)
                        transaction.Rollback();
                    else
                        transaction.Commit();

                    return affectedRows;
                }
            }
        }

        public async Task<int> SaveAllChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
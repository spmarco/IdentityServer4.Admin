using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.Extensions.Configuration;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Common;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Repositories.Interface;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Repositories.Dapper
{
    public class ApiResourceDapperRepository : IApiResourceRepository
    {
        private readonly IConfiguration _configuration;
        public bool AutoSaveChanges { get; set; } = true;

        public ApiResourceDapperRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<PagedList<ApiResource>> GetApiResourcesAsync(string search, int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<ApiResource>();

            using (var dbContext = ConnectionDatabase.Get(_configuration))
            {
                var apiResources = await dbContext.QueryAsync<ApiResource>($@"select * from ApiResources where Name like @name OFFSET {page} ROWS FETCH NEXT {pageSize} ROWS ONLY;", new { name = $"%{search}%" });

                pagedList.Data.AddRange(apiResources);
                pagedList.TotalCount = apiResources.Count();
                pagedList.PageSize = pageSize;
            }

            return pagedList;
        }

        public Task<ApiResource> GetApiResourceAsync(int apiResourceId)
        {
            return Task.Run(() =>
            {
                using (var dbContext = ConnectionDatabase.Get(_configuration))
                {
                    ApiResource apiResource;
                    var query = @"select * from ApiResources where Id = @apiResourceId; 
                                  select * from ApiClaims where ApiResourceId = @apiResourceId;";

                    using (var result = dbContext.QueryMultiple(query, new { apiResourceId }))
                    {
                        apiResource = result.ReadFirstOrDefault<ApiResource>();

                        if (apiResource != null)
                        {
                            apiResource.UserClaims = result.Read<ApiResourceClaim>().ToList();
                            apiResource.Secrets = new List<ApiSecret>();
                            apiResource.Scopes = new List<ApiScope>();
                        }
                    }

                    return apiResource;

                }
            });
        }

        public async Task<bool> CanInsertApiResourceAsync(ApiResource apiResource)
        {
            using (var dbContext = ConnectionDatabase.Get(_configuration))
            {
                if (apiResource.Id == 0)
                {
                    var existsWithSameName = await dbContext.QueryFirstOrDefaultAsync<ApiResource>(@"select * from ApiResources where Name = @Name;", new { apiResource.Name });
                    return existsWithSameName == null;
                }
                else
                {
                    var existsWithSameName = await dbContext.QueryFirstOrDefaultAsync<ApiResource>(@"select * from ApiResources where Id = @Id and Name = @Name;", new { apiResource.Id, apiResource.Name });
                    return existsWithSameName == null;
                }
            }
        }

        public async Task<bool> CanInsertApiScopeAsync(ApiScope apiScope)
        {
            using (var dbContext = ConnectionDatabase.Get(_configuration))
            {
                if (apiScope.Id == 0)
                {
                    var existsWithSameName = await dbContext.QueryFirstOrDefaultAsync<ApiResource>(@"select * from ApiResources where Name = @Name;", new { apiScope.Name });
                    return existsWithSameName == null;
                }
                else
                {
                    var existsWithSameName = await dbContext.QueryFirstOrDefaultAsync<ApiResource>(@"select * from ApiResources where Id = @Id and Name = @Name;", new { apiScope.Id, apiScope.Name });
                    return existsWithSameName == null;
                }
            }
        }

        public async Task<int> AddApiResourceAsync(ApiResource apiResource)
        {
            using (var dbContext = ConnectionDatabase.Get(_configuration))
            {
                using (var transaction = dbContext.BeginTransaction())
                {
                    var affectedRows = 0;
                    var id = await dbContext.QueryFirstAsync<int>(@"insert into ApiResources values(@Enabled, @Name, @DisplayName, @Description); select SCOPE_IDENTITY()", apiResource, transaction);

                    if (id == 0)
                        return id;

                    affectedRows = 1;

                    if (apiResource.UserClaims.Any())
                    {
                        affectedRows = await dbContext.ExecuteAsync("insert into ApiClaims values (@Type,@ApiResourceId);", apiResource.UserClaims.Select(x => new
                        {
                            x.Type,
                            ApiResourceId = id
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

        private async Task RemoveApiResourceClaimsAsync(ApiResource identityResource)
        {
            //Remove old identity claims
            using (var dbContext = ConnectionDatabase.Get(_configuration))
            {
                await dbContext.ExecuteAsync("delete from ApiClaims where ApiResourceId = @Id;", new { identityResource.Id });
            }
        }

        public async Task<int> UpdateApiResourceAsync(ApiResource apiResource)
        {
            //Remove old relations
            await RemoveApiResourceClaimsAsync(apiResource);

            //Update with new data
            using (var dbContext = ConnectionDatabase.Get(_configuration))
            {
                using (var transaction = dbContext.BeginTransaction())
                {
                    var affectedRows = await dbContext.ExecuteAsync("update ApiResources set Enabled=@Enabled, Name=@Name, DisplayName=@DisplayName, Description=@Description  where Id=@Id;", apiResource, transaction);

                    if (affectedRows == 0) return affectedRows;

                    if (apiResource.UserClaims.Any())
                    {
                        affectedRows = await dbContext.ExecuteAsync("insert into ApiClaims values (@Type,@ApiResourceId);", apiResource.UserClaims.Select(x => new
                        {
                            x.Type,
                            ApiResourceId = apiResource.Id
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

        public async Task<int> DeleteApiResourceAsync(ApiResource apiResource)
        {
            await RemoveApiResourceClaimsAsync(apiResource);

            using (var dbContext = ConnectionDatabase.Get(_configuration))
            {
                var result = await dbContext.ExecuteAsync("delete from ApiResources where Id = @Id ", new { apiResource.Id });
                return Convert.ToInt16(result);
            }
        }

        public async Task<PagedList<ApiScope>> GetApiScopesAsync(int apiResourceId, int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<ApiScope>();

            using (var dbContext = ConnectionDatabase.Get(_configuration))
            {
                var query = $@"select * from ApiScopes 
                              inner join ApiResource on ApiResource.Id = ApiScopes.ApiResourceId  
                              where ApiResourceId = @apiResourceId
                              OFFSET {page} ROWS FETCH NEXT {pageSize} ROWS ONLY";


                var apiScopes = await dbContext.QueryAsync<ApiScope, ApiResource, ApiScope>(
                    query,
                    (apiScope, apiResource) =>
                    {
                        apiScope.ApiResource = apiResource;
                        return apiScope;
                    },
                    splitOn: "ApiResourceId");

                pagedList.Data.AddRange(apiScopes);
                pagedList.TotalCount = apiScopes.Count();
                pagedList.PageSize = pageSize;

                return pagedList;
            }
        }

        public Task<ApiScope> GetApiScopeAsync(int apiResourceId, int apiScopeId)
        {
            return Task.Run(() =>
            {
                using (var dbContext = ConnectionDatabase.Get(_configuration))
                {
                    var query = @"select * from ApiScopes where Id = @apiScopeId;
                                  select * from ApiScopeClaims where ApiScopeId = @apiScopeId;
                                  select ApiResources.* from ApiResources where id = @apiResourceId";

                    ApiScope apiScopes;
                    using (var result = dbContext.QueryMultiple(query, new { apiScopeId, apiResourceId }))
                    {
                        apiScopes = result.ReadFirstOrDefault<ApiScope>();
                        if (apiScopes != null)
                        {
                            apiScopes.UserClaims = result.Read<ApiScopeClaim>().ToList();
                            apiScopes.ApiResource = result.ReadFirstOrDefault<ApiResource>();
                        }
                    }

                    return apiScopes;
                }
            });
        }

        public async Task<int> AddApiScopeAsync(int apiResourceId, ApiScope apiScope)
        {
            var script = "insert into ApiScopes values (@Name,@DisplayName,@Description,@Required,@Emphasize,@ShowInDiscoveryDocument,@apiResourceId); select SCOPE_IDENTITY();";

            using (var dbContext = ConnectionDatabase.Get(_configuration))
            {
                using (var transaction = dbContext.BeginTransaction())
                {
                    int affectedRows;
                    var id = await dbContext.QueryFirstOrDefaultAsync<int>(script, new
                    {
                        apiScope.Name,
                        apiScope.DisplayName,
                        apiScope.Description,
                        apiScope.Required,
                        apiScope.Emphasize,
                        apiScope.ShowInDiscoveryDocument,
                        apiResourceId
                    }, transaction);

                    if (id == 0)
                        return id;
                    else
                        affectedRows = 1;

                    if (apiScope.UserClaims.Any())
                    {
                        affectedRows = await dbContext.ExecuteAsync($"insert into ApiScopeClaims values (@Type,@ApiScopeId);", apiScope.UserClaims.Select(x => new
                        {
                            x.Type,
                            ApiScopeId = id
                        }), transaction);
                    }

                    if (affectedRows == 0)
                        transaction.Rollback();
                    else
                        transaction.Commit();

                    return id;
                }
            }



            return await ManipulateApiScopeAsync(apiScope, apiResourceId, script);
        }

        private async Task RemoveApiScopeClaimsAsync(ApiScope apiScope)
        {
            using (var dbContext = ConnectionDatabase.Get(_configuration))
            {
                await dbContext.ExecuteAsync("delete from ApiScopeClaims where ApiScopeId = @Id;", new { apiScope.Id });
            }
        }

        public async Task<int> UpdateApiScopeAsync(int apiResourceId, ApiScope apiScope)
        {
            ////Remove old relations
            await RemoveApiScopeClaimsAsync(apiScope);

            var script = "update ApiScopes set Name=@Name, DisplayName=@DisplayName, Description=@Description, Required=@Required, Emphasize=@Emphasize, ShowInDiscoveryDocument=@ShowInDiscoveryDocument, ApiResourceId=@apiResourceId where Id=@Id;";

            return await ManipulateApiScopeAsync(apiScope, apiResourceId, script);
        }

        public async Task<int> DeleteApiScopeAsync(ApiScope apiScope)
        {
            using (var dbContext = ConnectionDatabase.Get(_configuration))
            {
                return await dbContext.ExecuteAsync("delete from ApiScopes where Id = @Id;", new { apiScope.Id });
            }
        }

        public async Task<PagedList<ApiSecret>> GetApiSecretsAsync(int apiResourceId, int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<ApiSecret>();

            using (var dbContext = ConnectionDatabase.Get(_configuration))
            {
                var apiScopes = await dbContext.QueryAsync<ApiSecret>($@"select * from ApiSecret where ApiResourceId = @apiResourceId OFFSET {page} ROWS FETCH NEXT {pageSize} ROWS ONLY", new { apiResourceId });

                pagedList.Data.AddRange(apiScopes);
                pagedList.TotalCount = apiScopes.Count();
                pagedList.PageSize = pageSize;

                return pagedList;
            }
        }

        public Task<ApiSecret> GetApiSecretAsync(int apiSecretId)
        {
            return Task.Run(() =>
            {
                using (var dbContext = ConnectionDatabase.Get(_configuration))
                {
                    var query = @"select * from ApiSecrets where Id = @apiSecretId;
                                  select * from ApiResources inner join ApiSecrets on ApiSecrets.ApiResourceId = ApiResources.Id  where ApiSecrets.Id = @apiSecretId;";

                    ApiSecret apiSecret;
                    using (var result = dbContext.QueryMultiple(query, new { apiSecretId }))
                    {
                        apiSecret = result.ReadFirstOrDefault<ApiSecret>();
                        if (apiSecret != null)
                            apiSecret.ApiResource = result.ReadFirst<ApiResource>();
                    }

                    return apiSecret;
                }
            });
        }

        public async Task<int> AddApiSecretAsync(int apiResourceId, ApiSecret apiSecret)
        {
            using (var dbContext = ConnectionDatabase.Get(_configuration))
            {
                return await dbContext.QueryFirstAsync<int>("insert into ApiSecrets values (@Expiration, @Description, @Value, @Type, @apiResourceId); select SCOPE_IDENTITY();", new
                {
                    apiSecret.Expiration,
                    apiSecret.Description,
                    apiSecret.Value,
                    apiSecret.Type,
                    apiResourceId
                });
            }
        }

        public async Task<int> DeleteApiSecretAsync(ApiSecret apiSecret)
        {
            using (var dbContext = ConnectionDatabase.Get(_configuration))
            {
                return await dbContext.ExecuteAsync("delete from ApiSecrets where Id = @Id;", new { apiSecret.Id });
            }
        }

        public async Task<string> GetApiResourceNameAsync(int apiResourceId)
        {
            using (var dbContext = ConnectionDatabase.Get(_configuration))
            {
                var result = await dbContext.QueryFirstAsync<ApiResource>("select * from ApiResources where Id = @apiResourceId", new { apiResourceId });
                return result.Name;
            }
        }

        private async Task<int> ManipulateApiScopeAsync(ApiScope apiScope, int apiResourceId, string script)
        {
            using (var dbContext = ConnectionDatabase.Get(_configuration))
            {
                using (var transaction = dbContext.BeginTransaction())
                {
                    var affectedRows = await dbContext.ExecuteAsync(script, new
                    {
                        apiScope.Id,
                        apiScope.Name,
                        apiScope.DisplayName,
                        apiScope.Description,
                        apiScope.Required,
                        apiScope.Emphasize,
                        apiScope.ShowInDiscoveryDocument,
                        apiResourceId
                    }, transaction);

                    if (affectedRows == 0) return affectedRows;

                    if (apiScope.UserClaims.Any())
                    {
                        affectedRows = await dbContext.ExecuteAsync($"insert into ApiScopeClaims values (@Type,@ApiScopeId);", apiScope.UserClaims.Select(x => new
                        {
                            x.Type,
                            ApiScopeId = apiScope.Id
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

        public Task<int> SaveAllChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
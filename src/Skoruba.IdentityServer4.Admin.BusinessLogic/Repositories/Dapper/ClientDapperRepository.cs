using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Constants;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Common;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Helpers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Repositories.Interface;
using Client = IdentityServer4.EntityFramework.Entities.Client;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Repositories.Dapper
{
    public class ClientDapperRepository : IClientRepository
    {
        private readonly IConfiguration _configuration;

        public ClientDapperRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<Client> GetClientAsync(int clientId)
        {
            return Task.Run(() =>
            {
                using (var dbContext = ConnectionDatabase.Get(_configuration))
                {
                    var query = @"select * from Clients where id = @clientId;
                                  select * from ClientGrantTypes where ClientId = @clientId;
                                  select * from ClientRedirectUris where ClientId = @clientId; 
                                  select * from ClientPostLogoutRedirectUris where ClientId = @clientId; 
                                  select * from ClientScopes where ClientId = @clientId; 
                                  select * from ClientSecrets where ClientId = @clientId; 
                                  select * from ClientClaims where ClientId = @clientId; 
                                  select * from ClientIdPRestrictions where ClientId = @clientId; 
                                  select * from ClientCorsOrigins where ClientId = @clientId; 
                                  select * from ClientProperties where ClientId = @clientId;";

                    Client client;
                    using (var result = dbContext.QueryMultiple(query, new { clientId }))
                    {
                        client = result.ReadFirstOrDefault<Client>();

                        if (client != null)
                        {
                            client.AllowedGrantTypes = result.Read<ClientGrantType>().ToList();
                            client.RedirectUris = result.Read<ClientRedirectUri>().ToList();
                            client.PostLogoutRedirectUris = result.Read<ClientPostLogoutRedirectUri>().ToList();
                            client.AllowedScopes = result.Read<ClientScope>().ToList();
                            client.ClientSecrets = result.Read<ClientSecret>().ToList();
                            client.Claims = result.Read<ClientClaim>().ToList();
                            client.IdentityProviderRestrictions = result.Read<ClientIdPRestriction>().ToList();
                            client.AllowedCorsOrigins = result.Read<ClientCorsOrigin>().ToList();
                            client.Properties = result.Read<ClientProperty>().ToList();
                        }
                    }

                    return client;
                }
            });
        }

        public async Task<PagedList<Client>> GetClientsAsync(string search = "", int page = 0, int pageSize = 10)
        {
            var pagedList = new PagedList<Client>();

            using (var dbContext = ConnectionDatabase.Get(_configuration))
            {
                var where = search.IsNullOrEmpty() ? "where ClientId like @search or ClientName like @search" : "";
                var apiResources = await dbContext.QueryAsync<Client>($"select * from Clients {where} order by ClientName OFFSET {page} ROWS FETCH NEXT {pageSize} ROWS ONLY;", new { search = $"%{search}%" });

                pagedList.Data.AddRange(apiResources);
                pagedList.TotalCount = apiResources.Count();
                pagedList.PageSize = pageSize;
            }

            return pagedList;
        }

        public async Task<List<string>> GetScopesAsync(string scope, int limit = 0)
        {
            using (var dbContext = ConnectionDatabase.Get(_configuration))
            {
                var scriptLimit = limit > 0 ? $"top {limit}" : "";

                var identityResources = await dbContext.QueryAsync<string>($"select {scriptLimit} IdentityResources.Name from IdentityResources where Name like @scope", new { scope = $"%{scope}%" });
                var apiResources = await dbContext.QueryAsync<string>($"select {scriptLimit} ApiResources.Name from ApiResources where Name like @scope", new { scope = $"%{scope}%" });

                var enumerable = identityResources.Concat(apiResources);

                return enumerable.ToList();
            }
        }

        public List<string> GetGrantTypes(string grant, int limit = 0)
        {
            var filteredGrants = ClientConsts.GetGrantTypes()
                .WhereIf(!string.IsNullOrWhiteSpace(grant), x => x.Contains(grant))
                .TakeIf(x => x, limit > 0, limit)
                .ToList();

            return filteredGrants;
        }

        public List<SelectItem> GetProtocolTypes()
        {
            return ClientConsts.GetProtocolTypes();
        }

        public List<SelectItem> GetSecretTypes()
        {
            var secrets = new List<SelectItem>();
            secrets.AddRange(ClientConsts.GetSecretTypes().Select(x => new SelectItem(x, x)));

            return secrets;
        }

        public List<string> GetStandardClaims(string claim, int limit = 0)
        {
            var filteredClaims = ClientConsts.GetStandardClaims()
                .WhereIf(!string.IsNullOrWhiteSpace(claim), x => x.Contains(claim))
                .TakeIf(x => x, limit > 0, limit)
                .ToList();

            return filteredClaims;
        }

        public List<SelectItem> GetAccessTokenTypes()
        {
            var accessTokenTypes = EnumHelpers.ToSelectList<AccessTokenType>();
            return accessTokenTypes;
        }

        public List<SelectItem> GetTokenExpirations()
        {
            var tokenExpirations = EnumHelpers.ToSelectList<TokenExpiration>();
            return tokenExpirations;
        }

        public List<SelectItem> GetTokenUsage()
        {
            var tokenUsage = EnumHelpers.ToSelectList<TokenUsage>();
            return tokenUsage;
        }

        public List<SelectItem> GetHashTypes()
        {
            var hashTypes = EnumHelpers.ToSelectList<HashType>();
            return hashTypes;
        }

        public async Task<int> AddClientSecretAsync(int clientId, ClientSecret clientSecret)
        {
            using (var dbContext = ConnectionDatabase.Get(_configuration))
            {
                return await dbContext.QueryFirstAsync<int>("insert into ClientSecrets values (@Expiration, @Description, @Value, @Type, @clientId);  select SCOPE_IDENTITY();", new
                {
                    clientSecret.Expiration,
                    clientSecret.Description,
                    clientSecret.Value,
                    clientSecret.Type,
                    clientId
                });
            }
        }

        public Task<ClientProperty> GetClientPropertyAsync(int clientPropertyId)
        {
            return Task.Run(() =>
            {
                using (var dbContext = ConnectionDatabase.Get(_configuration))
                {
                    ClientProperty clientProperty;
                    var query = @"select * from ClientProperties where id= @clientPropertyId; 
                                  select * from Clients inner join ClientProperties on ClientProperties.ClientId = Clients.Id  where ClientProperties.Id = @clientPropertyId;";

                    using (var result = dbContext.QueryMultiple(query, new { clientPropertyId }))
                    {
                        clientProperty = result.ReadFirstOrDefault<ClientProperty>();

                        if (clientProperty != null)
                            clientProperty.Client = result.ReadFirst<Client>();
                    }

                    return clientProperty;
                }
            });
        }

        public async Task<int> AddClientClaimAsync(int clientId, ClientClaim clientClaim)
        {
            using (var dbContext = ConnectionDatabase.Get(_configuration))
            {
                return await dbContext.QueryFirstAsync<int>("insert into ClientClaims values(@Type, @Value, @clientId); select SCOPE_IDENTITY();", new
                {
                    clientClaim.Type,
                    clientClaim.Value,
                    clientId
                });
            }
        }

        public async Task<int> AddClientPropertyAsync(int clientId, ClientProperty clientProperty)
        {
            using (var dbContext = ConnectionDatabase.Get(_configuration))
            {
                return await dbContext.QueryFirstAsync<int>("insert into ClientProperties values(@Key, @Value, @clientId); select SCOPE_IDENTITY();", new
                {
                    clientProperty.Key,
                    clientProperty.Value,
                    clientId
                });
            }
        }

        public async Task<(string ClientId, string ClientName)> GetClientIdAsync(int clientId)
        {
            using (var dbContext = ConnectionDatabase.Get(_configuration))
            {
                var client = await dbContext.QueryFirstOrDefaultAsync<Client>("select * from Clients where Id =@clientId", new { clientId });

                return (client?.ClientId, client?.ClientName);
            }
        }

        public async Task<PagedList<ClientSecret>> GetClientSecretsAsync(int clientId, int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<ClientSecret>();

            using (var dbContext = ConnectionDatabase.Get(_configuration))
            {
                var secrets = await dbContext.QueryAsync<ClientSecret>($@"select * from ClientSecrets where ClientId=@clientId  OFFSET {page} ROWS FETCH NEXT {pageSize} ROWS ONLY;", new { clientId });

                pagedList.Data.AddRange(secrets);
                pagedList.TotalCount = secrets.Count();
                pagedList.PageSize = pageSize;
            }

            return pagedList;
        }

        public Task<ClientSecret> GetClientSecretAsync(int clientSecretId)
        {
            return Task.Run(() =>
            {
                using (var dbContext = ConnectionDatabase.Get(_configuration))
                {
                    ClientSecret clientProperty;
                    var query = @"select * from ClientSecrets where id= @clientSecretId; 
                                  select * from Clients inner join ClientSecrets on ClientSecrets.ClientId = Clients.Id  where ClientSecrets.Id = @clientSecretId;";

                    using (var result = dbContext.QueryMultiple(query, new { clientSecretId }))
                    {
                        clientProperty = result.ReadFirstOrDefault<ClientSecret>();

                        if (clientProperty != null)
                            clientProperty.Client = result.ReadFirst<Client>();
                    }

                    return clientProperty;
                }
            });
        }

        public async Task<PagedList<ClientClaim>> GetClientClaimsAsync(int clientId, int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<ClientClaim>();

            using (var dbContext = ConnectionDatabase.Get(_configuration))
            {
                var clientClaims = await dbContext.QueryAsync<ClientClaim>($@"select * from ClientClaims where ClientId=@clientId  OFFSET {page} ROWS FETCH NEXT {pageSize} ROWS ONLY;", new { clientId });

                pagedList.Data.AddRange(clientClaims);
                pagedList.TotalCount = clientClaims.Count();
                pagedList.PageSize = pageSize;
            }

            return pagedList;
        }

        public async Task<PagedList<ClientProperty>> GetClientPropertiesAsync(int clientId, int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<ClientProperty>();

            using (var dbContext = ConnectionDatabase.Get(_configuration))
            {
                var properties = await dbContext.QueryAsync<ClientProperty>($@"select * from ClientProperties where ClientId=@clientId  OFFSET {page} ROWS FETCH NEXT {pageSize} ROWS ONLY;", new { clientId });

                pagedList.Data.AddRange(properties);
                pagedList.TotalCount = properties.Count();
                pagedList.PageSize = pageSize;
            }

            return pagedList;
        }

        public Task<ClientClaim> GetClientClaimAsync(int clientClaimId)
        {
            return Task.Run(() =>
            {
                using (var dbContext = ConnectionDatabase.Get(_configuration))
                {
                    ClientClaim clientClaim;
                    var query = @"select * from ClientClaims where id= @clientClaimId; 
                                  select * from Clients inner join ClientClaims on ClientClaims.ClientId = Clients.Id  where ClientClaims.Id = @clientClaimId;";

                    using (var result = dbContext.QueryMultiple(query, new { clientClaimId }))
                    {
                        clientClaim = result.ReadFirstOrDefault<ClientClaim>();

                        if (clientClaim != null)
                            clientClaim.Client = result.ReadFirst<Client>();
                    }

                    return clientClaim;
                }
            });
        }

        public async Task<int> DeleteClientSecretAsync(ClientSecret clientSecret)
        {
            using (var dbContext = ConnectionDatabase.Get(_configuration))
            {
                return await dbContext.ExecuteAsync("delete from ClientSecrets where id=@Id", new { clientSecret.Id });
            }
        }

        public async Task<int> DeleteClientClaimAsync(ClientClaim clientClaim)
        {
            using (var dbContext = ConnectionDatabase.Get(_configuration))
            {
                return await dbContext.ExecuteAsync("delete from ClientClaims where id=@Id", new { clientClaim.Id });
            }
        }

        public async Task<int> DeleteClientPropertyAsync(ClientProperty clientProperty)
        {
            using (var dbContext = ConnectionDatabase.Get(_configuration))
            {
                return await dbContext.ExecuteAsync("delete from ClientProperties where id=@Id", new { clientProperty.Id });
            }
        }

        public async Task<bool> CanInsertClientAsync(Client client, bool isCloned = false)
        {
            using (var dbContext = ConnectionDatabase.Get(_configuration))
            {
                if (client.Id == 0 || isCloned)
                {
                    var existsWithClientName = await dbContext.QueryFirstOrDefaultAsync<Client>("select * from clients where ClientId=@ClientId", new { client.ClientId });
                    return existsWithClientName == null;
                }
                else
                {
                    var existsWithClientName = await dbContext.QueryFirstOrDefaultAsync<Client>("select * from clients where ClientId=@ClientId and Id<>@Id", new { client.ClientId, client.Id });
                    return existsWithClientName == null;
                }
            }
        }

        /// <summary>
        /// Add new client, this method doesn't save client secrets, client claims, client properties
        /// </summary>
        /// <param name="client"></param>
        /// <returns>This method return new client id</returns>
        public async Task<int> AddClientAsync(Client client)
        {
            var script = @"insert into Clients 
                           values(@Enabled,
                                  @ClientId,
                                  @ProtocolType,
                                  @RequireClientSecret,
                                  @ClientName,
                                  @Description, 
                                  @ClientUri,
                                  @LogoUri,
                                  @RequireConsent,
                                  @AllowRememberConsent,
                                  @AlwaysIncludeUserClaimsInIdToken,
                                  @RequirePkce,
                                  @AllowPlainTextPkce,
                                  @AllowAccessTokensViaBrowser,
                                  @FrontChannelLogoutUri,
                                  @FrontChannelLogoutSessionRequired,
                                  @BackChannelLogoutUri,
                                  @BackChannelLogoutSessionRequired,
                                  @AllowOfflineAccess,
                                  @IdentityTokenLifetime,
                                  @AccessTokenLifetime,
                                  @AuthorizationCodeLifetime,
                                  @ConsentLifetime,
                                  @AbsoluteRefreshTokenLifetime,
                                  @SlidingRefreshTokenLifetime,
                                  @RefreshTokenUsage,
                                  @UpdateAccessTokenClaimsOnRefresh,
                                  @RefreshTokenExpiration,
                                  @AccessTokenType,
                                  @EnableLocalLogin,
                                  @IncludeJwtId,
                                  @AlwaysSendClientClaims,
                                  @ClientClaimsPrefix,
                                  @PairWiseSubjectSalt); ";


            return await ManipulateClientAsync(client, script, true);
        }

        public async Task<int> CloneClientAsync(Client client,
                                                bool cloneClientCorsOrigins = true,
                                                bool cloneClientGrantTypes = true,
                                                bool cloneClientIdPRestrictions = true,
                                                bool cloneClientPostLogoutRedirectUris = true,
                                                bool cloneClientScopes = true,
                                                bool cloneClientRedirectUris = true,
                                                bool cloneClientClaims = true,
                                                bool cloneClientProperties = true)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateProfile("CloneClient", expression => expression.CreateMap<Client, Client>())).CreateMapper();
            var clientToClone = mapper.Map<Client>(client);

            //Clean original ids
            clientToClone.Id = 0;
            clientToClone.ClientId = clientToClone.ClientId;
            clientToClone.AllowedCorsOrigins.ForEach(x => x.Id = 0);
            clientToClone.RedirectUris.ForEach(x => x.Id = 0);
            clientToClone.PostLogoutRedirectUris.ForEach(x => x.Id = 0);
            clientToClone.AllowedScopes.ForEach(x => x.Id = 0);
            clientToClone.ClientSecrets.ForEach(x => x.Id = 0);
            clientToClone.IdentityProviderRestrictions.ForEach(x => x.Id = 0);
            clientToClone.Claims.ForEach(x => x.Id = 0);
            clientToClone.AllowedGrantTypes.ForEach(x => x.Id = 0);
            clientToClone.Properties.ForEach(x => x.Id = 0);

            //Client secret will be skipped
            clientToClone.ClientSecrets.Clear();

            if (!cloneClientCorsOrigins)
            {
                clientToClone.AllowedCorsOrigins.Clear();
            }

            if (!cloneClientGrantTypes)
            {
                clientToClone.AllowedGrantTypes.Clear();
            }

            if (!cloneClientIdPRestrictions)
            {
                clientToClone.IdentityProviderRestrictions.Clear();
            }

            if (!cloneClientPostLogoutRedirectUris)
            {
                clientToClone.PostLogoutRedirectUris.Clear();
            }

            if (!cloneClientScopes)
            {
                clientToClone.AllowedScopes.Clear();
            }

            if (!cloneClientRedirectUris)
            {
                clientToClone.RedirectUris.Clear();
            }

            if (!cloneClientClaims)
            {
                clientToClone.Claims.Clear();
            }

            if (!cloneClientProperties)
            {
                clientToClone.Properties.Clear();
            }

            return await AddClientAsync(clientToClone);
        }

        private async Task RemoveClientRelationsAsync(Client client)
        {
            using (var dbContext = ConnectionDatabase.Get(_configuration))
            {
                await dbContext.ExecuteAsync(@"delete from ClientScopes where ClientId=@Id;
                                               delete from ClientGrantTypes where ClientId=@Id;
                                               delete from ClientRedirectUris where ClientId=@Id;
                                               delete from ClientCorsOrigins where ClientId=@Id;
                                               delete from ClientIdPRestrictions where ClientId=@Id;
                                               delete from ClientPostLogoutRedirectUris where ClientId=@Id;", new { client.Id });
            }
        }

        public async Task<int> UpdateClientAsync(Client client)
        {
            //Remove old relations
            await RemoveClientRelationsAsync(client);

            var script = @"update Clients 
                           set Enabled = @Enabled,
                               ClientId = @ClientId,
                               ProtocolType = @ProtocolType,
                               RequireClientSecret = @RequireClientSecret,
                               ClientName = @ClientName,
                               Description = @Description,
                               ClientUri = @ClientUri,
                               LogoUri = @LogoUri,
                               RequireConsent = @RequireConsent,
                               AllowRememberConsent = @AllowRememberConsent,
                               AlwaysIncludeUserClaimsInIdToken = @AlwaysIncludeUserClaimsInIdToken,
                               RequirePkce = @RequirePkce,
                               AllowPlainTextPkce = @AllowPlainTextPkce,
                               AllowAccessTokensViaBrowser = @AllowAccessTokensViaBrowser,
                               FrontChannelLogoutUri = @FrontChannelLogoutUri,
                               FrontChannelLogoutSessionRequired = @FrontChannelLogoutSessionRequired,
                               BackChannelLogoutUri = @BackChannelLogoutUri,
                               BackChannelLogoutSessionRequired = @BackChannelLogoutSessionRequired,
                               AllowOfflineAccess = @AllowOfflineAccess,
                               IdentityTokenLifetime = @IdentityTokenLifetime,
                               AccessTokenLifetime = @AccessTokenLifetime,
                               AuthorizationCodeLifetime = @AuthorizationCodeLifetime,
                               ConsentLifetime = @ConsentLifetime,
                               AbsoluteRefreshTokenLifetime = @AbsoluteRefreshTokenLifetime,
                               SlidingRefreshTokenLifetime = @SlidingRefreshTokenLifetime,
                               RefreshTokenUsage = @RefreshTokenUsage,
                               UpdateAccessTokenClaimsOnRefresh = @UpdateAccessTokenClaimsOnRefresh,
                               RefreshTokenExpiration = @RefreshTokenExpiration,
                               AccessTokenType = @AccessTokenType,
                               EnableLocalLogin = @EnableLocalLogin,
                               IncludeJwtId = @IncludeJwtId,
                               AlwaysSendClientClaims = @AlwaysSendClientClaims,
                               ClientClaimsPrefix = @ClientClaimsPrefix,
                               PairWiseSubjectSalt = @PairWiseSubjectSalt where Id=@Id; ";

            //Update with new data
            return await ManipulateClientAsync(client, script, false);
        }

        public async Task<int> RemoveClientAsync(Client client)
        {
            using (var dbContext = ConnectionDatabase.Get(_configuration))
            {
                return await dbContext.ExecuteAsync("delete from Clients where Id=@Id;", new { client.Id });
            }
        }

        private async Task<int> ManipulateClientAsync(Client client, string script, bool insert)
        {
            using (var dbContext = ConnectionDatabase.Get(_configuration))
            {
                using (var transaction = dbContext.BeginTransaction())
                {
                    int affectedRows, clientId = 0;

                    if (insert)
                    {
                        clientId = await dbContext.QueryFirstOrDefaultAsync<int>($"{script} select SCOPE_IDENTITY();", client, transaction);

                        if (clientId != 0)
                            affectedRows = 1;
                        else
                            return clientId;
                    }
                    else
                    {
                        affectedRows = await dbContext.ExecuteAsync(script, client, transaction);
                        clientId = client.Id;
                    }

                    if (affectedRows == 0) return affectedRows;
                    if (client.AllowedScopes.Any())
                    {
                        affectedRows = await dbContext.ExecuteAsync("insert into ClientScopes values (@Scope,@ClientId);", client.AllowedScopes.Select(x => new
                        {
                            x.Scope,
                            ClientId = clientId
                        }), transaction);
                    }

                    if (affectedRows == 0) { transaction.Rollback(); return affectedRows; }
                    if (client.AllowedGrantTypes.Any())
                    {
                        affectedRows = await dbContext.ExecuteAsync("insert into ClientGrantTypes values (@GrantType,@ClientId);", client.AllowedGrantTypes.Select(x => new
                        {
                            x.GrantType,
                            ClientId = clientId
                        }), transaction);
                    }

                    if (affectedRows == 0) { transaction.Rollback(); return affectedRows; }
                    if (client.RedirectUris.Any())
                    {
                        affectedRows = await dbContext.ExecuteAsync("insert into ClientRedirectUris values (@RedirectUri,@ClientId);", client.RedirectUris.Select(x => new
                        {
                            x.RedirectUri,
                            ClientId = clientId
                        }), transaction);
                    }

                    if (affectedRows == 0) { transaction.Rollback(); return affectedRows; }
                    if (client.AllowedCorsOrigins.Any())
                    {
                        affectedRows = await dbContext.ExecuteAsync("insert into ClientCorsOrigins values (@Origin,@ClientId);", client.AllowedCorsOrigins.Select(x => new
                        {
                            x.Origin,
                            ClientId = clientId
                        }), transaction);
                    }

                    if (affectedRows == 0) { transaction.Rollback(); return affectedRows; }
                    if (client.IdentityProviderRestrictions.Any())
                    {
                        affectedRows = await dbContext.ExecuteAsync("insert into ClientIdPRestrictions values (@Provider,@ClientId);", client.IdentityProviderRestrictions.Select(x => new
                        {
                            x.Provider,
                            ClientId = clientId
                        }), transaction);
                    }

                    if (affectedRows == 0) { transaction.Rollback(); return affectedRows; }
                    if (client.PostLogoutRedirectUris.Any())
                    {
                        affectedRows = await dbContext.ExecuteAsync("insert into ClientPostLogoutRedirectUris values (@PostLogoutRedirectUri,@ClientId);", client.PostLogoutRedirectUris.Select(x => new
                        {
                            x.PostLogoutRedirectUri,
                            ClientId = clientId
                        }), transaction);
                    }

                    if (affectedRows == 0)
                        transaction.Rollback();
                    else
                        transaction.Commit();

                    return insert ? clientId : affectedRows;
                }
            }
        }

        public async Task<int> SaveAllChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
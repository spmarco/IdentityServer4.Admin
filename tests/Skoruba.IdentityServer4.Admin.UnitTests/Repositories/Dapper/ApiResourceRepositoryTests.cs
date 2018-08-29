using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Repositories.Dapper;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Repositories.Interface;
using Skoruba.IdentityServer4.Admin.UnitTests.Mocks;
using Xunit;

namespace Skoruba.IdentityServer4.Admin.UnitTests.Repositories.Dapper
{
    public class ApiResourceDapperRepositoryTests
    {
        private IConfiguration _configuration;

        public ApiResourceDapperRepositoryTests()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName)
                .AddJsonFile("appsettings.test.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddUserSecrets<Startup>();

            _configuration = builder.Build();
        }

        [Fact]
        public async Task AddApiResourceAsync()
        {
            IApiResourceRepository apiResourceRepository = new ApiResourceDapperRepository(_configuration);

            //Generate random new api resource
            var apiResource = ApiResourceMock.GenerateRandomApiResource();

            //Add new api resource
            var apiResourceId = await apiResourceRepository.AddApiResourceAsync(apiResource);

            //Get new api resource
            var newApiResource = await apiResourceRepository.GetApiResourceAsync(apiResourceId);

            //Assert new api resource
            newApiResource.ShouldBeEquivalentTo(apiResource, options => options.Excluding(o => o.Id).Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "UserClaims\\[.+\\].Id")));
        }

        [Fact]
        public async Task GetApiResourceAsync()
        {
            IApiResourceRepository apiResourceRepository = new ApiResourceDapperRepository(_configuration);

            //Generate random new api resource
            var apiResource = ApiResourceMock.GenerateRandomApiResource();

            //Add new api resource
            var apiResourceId = await apiResourceRepository.AddApiResourceAsync(apiResource);

            //Get new api resource
            var newApiResource = await apiResourceRepository.GetApiResourceAsync(apiResourceId);

            //Assert new api resource
            newApiResource.ShouldBeEquivalentTo(apiResource, options => options.Excluding(o => o.Id).Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "UserClaims\\[.+\\].Id")));
        }

        [Fact]
        public async Task DeleteApiResourceAsync()
        {
            IApiResourceRepository apiResourceRepository = new ApiResourceDapperRepository(_configuration);

            //Generate random new api resource
            var apiResource = ApiResourceMock.GenerateRandomApiResource();

            //Add new api resource
            var apiResourceId = await apiResourceRepository.AddApiResourceAsync(apiResource);

            //Get new api resource
            var newApiResource = await apiResourceRepository.GetApiResourceAsync(apiResourceId);

            //Assert new api resource
            newApiResource.ShouldBeEquivalentTo(apiResource, options => options.Excluding(o => o.Id).Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "UserClaims\\[.+\\].Id")));

            //Delete api resource
            await apiResourceRepository.DeleteApiResourceAsync(newApiResource);

            //Get deleted api resource
            var deletedApiResource = await apiResourceRepository.GetApiResourceAsync(apiResourceId);

            //Assert if it not exist
            deletedApiResource.Should().BeNull();
        }

        [Fact]
        public async Task UpdateApiResourceAsync()
        {
            IApiResourceRepository apiResourceRepository = new ApiResourceDapperRepository(_configuration);

            //Generate random new api resource
            var apiResource = ApiResourceMock.GenerateRandomApiResource();

            //Add new api resource
            var apiResourceId = await apiResourceRepository.AddApiResourceAsync(apiResource);

            //Get new api resource
            var newApiResource = await apiResourceRepository.GetApiResourceAsync(apiResourceId);

            //Assert new api resource
            newApiResource.ShouldBeEquivalentTo(apiResource, options => options.Excluding(o => o.Id).Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "UserClaims\\[.+\\].Id")));


            //Generete new api resource with added item id
            var updatedApiResource = ApiResourceMock.GenerateRandomApiResource(newApiResource.Id);

            //Update api resource
            await apiResourceRepository.UpdateApiResourceAsync(updatedApiResource);

            //Get updated api resource
            var updatedApiResourceEntity = await apiResourceRepository.GetApiResourceAsync(updatedApiResource.Id);

            //Assert updated api resource
            updatedApiResource.ShouldBeEquivalentTo(updatedApiResourceEntity, options => options.Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "UserClaims\\[.+\\].Id"))); 
        }

        [Fact]
        public async Task AddApiScopeAsync()
        {
            IApiResourceRepository apiResourceRepository = new ApiResourceDapperRepository(_configuration);

            //Generate random new api resource
            var apiResource = ApiResourceMock.GenerateRandomApiResource();

            //Add new api resource
            var apiResourceId = await apiResourceRepository.AddApiResourceAsync(apiResource);

            //Generate random new api scope
            var apiScope = ApiResourceMock.GenerateRandomApiScope();

            //Add new api scope
            var apiScopeId = await apiResourceRepository.AddApiScopeAsync(apiResourceId, apiScope);

            //Get new api scope
            var newApiScopes = await apiResourceRepository.GetApiScopeAsync(apiResourceId, apiScopeId);

            //Assert new api scope
            newApiScopes.ShouldBeEquivalentTo(apiScope, options => options.Excluding(o => o.Id)
                                                                          .Excluding(o => o.ApiResource)
                                                                          .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "UserClaims\\[.+\\].Id")));
        }

        [Fact]
        public async Task UpdateApiScopeAsync()
        {
            IApiResourceRepository apiResourceRepository = new ApiResourceDapperRepository(_configuration);

            //Generate random new api resource
            var apiResource = ApiResourceMock.GenerateRandomApiResource();

            //Add new api resource
            var apiResourceId = await apiResourceRepository.AddApiResourceAsync(apiResource);

            //Get new api resource
            var newApiResource = await apiResourceRepository.GetApiResourceAsync(apiResourceId);

            //Assert new api resource
            newApiResource.ShouldBeEquivalentTo(apiResource, options => options.Excluding(o => o.Id).Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "UserClaims\\[.+\\].Id")));

            //Generate random new api scope
            var apiScope = ApiResourceMock.GenerateRandomApiScope();

            //Add new api scope
            var apiScopeId = await apiResourceRepository.AddApiScopeAsync(apiResourceId, apiScope);

            //Generete new api scope with added item id
            var updatedApiScope = ApiResourceMock.GenerateRandomApiScope(apiScopeId);

            //Update api scope
            await apiResourceRepository.UpdateApiScopeAsync(apiResourceId, updatedApiScope);

            //Get updated api scope
            var updatedApiScopeEntity = await apiResourceRepository.GetApiScopeAsync(apiResourceId, updatedApiScope.Id);

            //Assert updated api scope
            updatedApiScope.ShouldBeEquivalentTo(updatedApiScopeEntity, options => options.Excluding(o => o.ApiResource)
                                                                                          .Excluding(o => o.Id)
                                                                                          .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "UserClaims\\[.+\\].Id")));
        }

        [Fact]
        public async Task DeleteApiScopeAsync()
        {
            IApiResourceRepository apiResourceRepository = new ApiResourceDapperRepository(_configuration);

            //Generate random new api resource
            var apiResource = ApiResourceMock.GenerateRandomApiResource();

            //Add new api resource
            var apiResourceId = await apiResourceRepository.AddApiResourceAsync(apiResource);

            //Generate random new api scope
            var apiScope = ApiResourceMock.GenerateRandomApiScope();

            //Add new api resource
            var apiScopeId = await apiResourceRepository.AddApiScopeAsync(apiResourceId, apiScope);

            //Get new api resource
            var newApiScopes = await apiResourceRepository.GetApiScopeAsync(apiResourceId, apiScopeId);

            //Assert new api resource
            newApiScopes.ShouldBeEquivalentTo(apiScope, options => options.Excluding(o => o.Id)
                                                                          .Excluding(o => o.ApiResource)
                                                                          .Excluding(o => o.Id).Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "UserClaims\\[.+\\].Id")));

            //Try delete it
            await apiResourceRepository.DeleteApiScopeAsync(newApiScopes);

            //Get new api scope
            var deletedApiScopes = await apiResourceRepository.GetApiScopeAsync(apiResourceId, newApiScopes.Id);

            //Assert if it exist
            deletedApiScopes.Should().BeNull();
        }

        [Fact]
        public async Task GetApiScopeAsync()
        {
            IApiResourceRepository apiResourceRepository = new ApiResourceDapperRepository(_configuration);

            //Generate random new api resource
            var apiResource = ApiResourceMock.GenerateRandomApiResource();

            //Add new api resource
            var apiResourceId = await apiResourceRepository.AddApiResourceAsync(apiResource);

            //Generate random new api scope
            var apiScope = ApiResourceMock.GenerateRandomApiScope();

            //Add new api scope
            var apiScopeId = await apiResourceRepository.AddApiScopeAsync(apiResourceId, apiScope);

            //Get new api scope
            var newApiScopes = await apiResourceRepository.GetApiScopeAsync(apiResourceId, apiScopeId);

            //Assert new api resource
            newApiScopes.ShouldBeEquivalentTo(apiScope, options => options.Excluding(o => o.Id)
                                                                          .Excluding(o => o.ApiResource)
                                                                          .Excluding(o => o.Id).Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "UserClaims\\[.+\\].Id")));
        }

        [Fact]
        public async Task AddApiSecretAsync()
        {
            IApiResourceRepository apiResourceRepository = new ApiResourceDapperRepository(_configuration);

            //Generate random new api resource
            var apiResource = ApiResourceMock.GenerateRandomApiResource();

            //Add new api resource
            var apiResourceId = await apiResourceRepository.AddApiResourceAsync(apiResource);

            //Generate random new api secret
            var apiSecret = ApiResourceMock.GenerateRandomApiSecret();

            //Add new api secret
            var apiSecretId = await apiResourceRepository.AddApiSecretAsync(apiResourceId, apiSecret);

            //Get new api secret
            var newApiSecret = await apiResourceRepository.GetApiSecretAsync(apiSecretId);

            //Assert new api secret
            newApiSecret.ShouldBeEquivalentTo(apiSecret, options => options.Excluding(o => o.Id).Excluding(x => x.ApiResource).Excluding(x => x.Expiration));
        }

        [Fact]
        public async Task DeleteApiSecretAsync()
        {
            IApiResourceRepository apiResourceRepository = new ApiResourceDapperRepository(_configuration);

            //Generate random new api resource
            var apiResource = ApiResourceMock.GenerateRandomApiResource();

            //Add new api resource
            var apiResourceId = await apiResourceRepository.AddApiResourceAsync(apiResource);

            //Generate random new api scope
            var apiSecret = ApiResourceMock.GenerateRandomApiSecret();

            //Add new api secret
            var apiSecretId = await apiResourceRepository.AddApiSecretAsync(apiResourceId, apiSecret);

            //Get new api resource
            var newApiSecret = await apiResourceRepository.GetApiSecretAsync(apiSecretId);

            //Assert new api resource
            newApiSecret.ShouldBeEquivalentTo(apiSecret, options => options.Excluding(o => o.Id).Excluding(x => x.ApiResource).Excluding(x => x.Expiration));

            //Try delete it
            await apiResourceRepository.DeleteApiSecretAsync(newApiSecret);

            //Get deleted api secret
            var deletedApiSecret = await apiResourceRepository.GetApiSecretAsync(newApiSecret.Id);

            //Assert if it exist
            deletedApiSecret.Should().BeNull();
        }

        [Fact]
        public async Task GetApiSecretAsync()
        {
            IApiResourceRepository apiResourceRepository = new ApiResourceDapperRepository(_configuration);

            //Generate random new api resource
            var apiResource = ApiResourceMock.GenerateRandomApiResource();

            //Add new api resource
            var apiResourceId = await apiResourceRepository.AddApiResourceAsync(apiResource);

            //Generate random new api secret
            var apiSecret = ApiResourceMock.GenerateRandomApiSecret();

            //Add new api secret
            var apiSecretId = await apiResourceRepository.AddApiSecretAsync(apiResourceId, apiSecret);

            //Get new api secret
            var newApiSecret = await apiResourceRepository.GetApiSecretAsync(apiSecretId);

            //Assert new api secret
            newApiSecret.ShouldBeEquivalentTo(apiSecret, options => options.Excluding(o => o.Id).Excluding(x => x.ApiResource).Excluding(x => x.Expiration));
        }
    }
}
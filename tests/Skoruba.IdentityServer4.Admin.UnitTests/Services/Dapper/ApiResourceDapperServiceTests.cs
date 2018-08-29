using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Mappers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Repositories.Dapper;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Repositories.Interface;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Resources;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services;
using Skoruba.IdentityServer4.Admin.UnitTests.Mocks;
using Xunit;

namespace Skoruba.IdentityServer4.Admin.UnitTests.Services.Dapper
{
    public class ApiResourceDapperServiceTests
    {
        private IConfiguration _configuration;

        public ApiResourceDapperServiceTests()
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
            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            var localizerApiResourceMock = new Mock<IApiResourceServiceResources>();
            var localizerApiResource = localizerApiResourceMock.Object;

            var localizerClientResourceMock = new Mock<IClientServiceResources>();
            var localizerClientResource = localizerClientResourceMock.Object;

            IClientService clientService = new ClientService(clientRepository, localizerClientResource);
            IApiResourceService apiResourceService = new ApiResourceService(apiResourceRepository, localizerApiResource, clientService);

            //Generate random new api resource
            var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(0);
            var apiResourceDtoId = await apiResourceService.AddApiResourceAsync(apiResourceDto);

            //Get new api resource
            var apiResource = await apiResourceRepository.GetApiResourceAsync(apiResourceDtoId);
            var newApiResourceDto = await apiResourceService.GetApiResourceAsync(apiResource.Id);

            //Assert new api resource
            apiResourceDto.ShouldBeEquivalentTo(newApiResourceDto, options => options.Excluding(o => o.Id));

        }

        [Fact]
        public async Task GetApiResourceAsync()
        {
            IApiResourceRepository apiResourceRepository = new ApiResourceDapperRepository(_configuration);
            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            var localizerApiResourceMock = new Mock<IApiResourceServiceResources>();
            var localizerApiResource = localizerApiResourceMock.Object;

            var localizerClientResourceMock = new Mock<IClientServiceResources>();
            var localizerClientResource = localizerClientResourceMock.Object;

            IClientService clientService = new ClientService(clientRepository, localizerClientResource);
            IApiResourceService apiResourceService = new ApiResourceService(apiResourceRepository, localizerApiResource, clientService);

            //Generate random new api resource
            var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(0);
            var apiResourceDtoId = await apiResourceService.AddApiResourceAsync(apiResourceDto);

            //Get new api resource
            var apiResource = await apiResourceRepository.GetApiResourceAsync(apiResourceDtoId);

            var newApiResourceDto = await apiResourceService.GetApiResourceAsync(apiResource.Id);

            //Assert new api resource
            apiResourceDto.ShouldBeEquivalentTo(newApiResourceDto, options => options.Excluding(o => o.Id));

        }

        [Fact]
        public async Task RemoveApiResourceAsync()
        {
            IApiResourceRepository apiResourceRepository = new ApiResourceDapperRepository(_configuration);
            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            var localizerApiResourceMock = new Mock<IApiResourceServiceResources>();
            var localizerApiResource = localizerApiResourceMock.Object;

            var localizerClientResourceMock = new Mock<IClientServiceResources>();
            var localizerClientResource = localizerClientResourceMock.Object;

            IClientService clientService = new ClientService(clientRepository, localizerClientResource);
            IApiResourceService apiResourceService = new ApiResourceService(apiResourceRepository, localizerApiResource, clientService);

            //Generate random new api resource
            var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(0);

            var apiResourceDtoId = await apiResourceService.AddApiResourceAsync(apiResourceDto);

            //Get new api resource
            var newApiResourceDto = await apiResourceService.GetApiResourceAsync(apiResourceDtoId);

            //Assert new api resource
            apiResourceDto.ShouldBeEquivalentTo(newApiResourceDto, options => options.Excluding(o => o.Id));

            //Remove api resource
            await apiResourceService.DeleteApiResourceAsync(newApiResourceDto);

            //Try get removed api resource
            var removeApiResource = await apiResourceRepository.GetApiResourceAsync(apiResourceDtoId);

            //Assert removed api resource
            removeApiResource.Should().BeNull();

        }

        [Fact]
        public async Task UpdateApiResourceAsync()
        {
            IApiResourceRepository apiResourceRepository = new ApiResourceDapperRepository(_configuration);
            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            var localizerApiResourceMock = new Mock<IApiResourceServiceResources>();
            var localizerApiResource = localizerApiResourceMock.Object;

            var localizerClientResourceMock = new Mock<IClientServiceResources>();
            var localizerClientResource = localizerClientResourceMock.Object;

            IClientService clientService = new ClientService(clientRepository, localizerClientResource);
            IApiResourceService apiResourceService = new ApiResourceService(apiResourceRepository, localizerApiResource, clientService);

            //Generate random new api resource
            var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(0);

            var apiResourceDtoId = await apiResourceService.AddApiResourceAsync(apiResourceDto);

            //Get new api resource
            var newApiResourceDto = await apiResourceService.GetApiResourceAsync(apiResourceDtoId);

            //Assert new api resource
            apiResourceDto.ShouldBeEquivalentTo(newApiResourceDto, options => options.Excluding(o => o.Id));

            //Generete new api resuorce with added item id
            var updatedApiResource = ApiResourceDtoMock.GenerateRandomApiResource(newApiResourceDto.Id);

            //Update api resource
            await apiResourceService.UpdateApiResourceAsync(updatedApiResource);

            var updatedApiResourceDto = await apiResourceService.GetApiResourceAsync(newApiResourceDto.Id);

            //Assert updated api resuorce
            updatedApiResource.ShouldBeEquivalentTo(updatedApiResourceDto, options => options.Excluding(o => o.Id));

        }

        [Fact]
        public async Task AddApiScopeAsync()
        {
            IApiResourceRepository apiResourceRepository = new ApiResourceDapperRepository(_configuration);
            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            var localizerApiResourceMock = new Mock<IApiResourceServiceResources>();
            var localizerApiResource = localizerApiResourceMock.Object;

            var localizerClientResourceMock = new Mock<IClientServiceResources>();
            var localizerClientResource = localizerClientResourceMock.Object;

            IClientService clientService = new ClientService(clientRepository, localizerClientResource);
            IApiResourceService apiResourceService = new ApiResourceService(apiResourceRepository, localizerApiResource, clientService);

            //Generate random new api resource
            var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(0);

            var apiResourceDtoId = await apiResourceService.AddApiResourceAsync(apiResourceDto);

            //Get new api resource
            var newApiResourceDto = await apiResourceService.GetApiResourceAsync(apiResourceDtoId);

            //Assert new api resource
            apiResourceDto.ShouldBeEquivalentTo(newApiResourceDto, options => options.Excluding(o => o.Id));

            //Generate random new api scope
            var apiScopeDtoMock = ApiResourceDtoMock.GenerateRandomApiScope(0, newApiResourceDto.Id);

            //Add new api scope
            var apiScopeId = await apiResourceService.AddApiScopeAsync(apiScopeDtoMock);

            //Get inserted api scope
            var apiScope = await apiResourceRepository.GetApiScopeAsync(apiResourceDtoId, apiScopeId);

            //Map entity to model
            var apiScopesDto = apiScope.ToModel();

            //Get new api scope
            var newApiScope = await apiResourceService.GetApiScopeAsync(apiScopesDto.ApiResourceId, apiScopesDto.ApiScopeId);

            //Assert
            newApiScope.ShouldBeEquivalentTo(apiScopesDto, o => o.Excluding(x => x.ResourceName));

        }

        [Fact]
        public async Task GetApiScopeAsync()
        {

            IApiResourceRepository apiResourceRepository = new ApiResourceDapperRepository(_configuration);
            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            var localizerApiResourceMock = new Mock<IApiResourceServiceResources>();
            var localizerApiResource = localizerApiResourceMock.Object;

            var localizerClientResourceMock = new Mock<IClientServiceResources>();
            var localizerClientResource = localizerClientResourceMock.Object;

            IClientService clientService = new ClientService(clientRepository, localizerClientResource);
            IApiResourceService apiResourceService = new ApiResourceService(apiResourceRepository, localizerApiResource, clientService);

            //Generate random new api resource
            var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(0);

            var apiResourceDtoId = await apiResourceService.AddApiResourceAsync(apiResourceDto);

            //Get new api resource
            var newApiResourceDto = await apiResourceService.GetApiResourceAsync(apiResourceDtoId);

            //Assert new api resource
            apiResourceDto.ShouldBeEquivalentTo(newApiResourceDto, options => options.Excluding(o => o.Id));

            //Generate random new api scope
            var apiScopeDtoMock = ApiResourceDtoMock.GenerateRandomApiScope(0, newApiResourceDto.Id);

            //Add new api scope
            var apiScopeId = await apiResourceService.AddApiScopeAsync(apiScopeDtoMock);

            //Get inserted api scope
            var apiScope = await apiResourceRepository.GetApiScopeAsync(apiResourceDtoId, apiScopeId);

            //Map entity to model
            var apiScopesDto = apiScope.ToModel();

            //Get new api scope
            var newApiScope = await apiResourceService.GetApiScopeAsync(apiScopesDto.ApiResourceId, apiScopesDto.ApiScopeId);

            //Assert
            newApiScope.ShouldBeEquivalentTo(apiScopesDto, o => o.Excluding(x => x.ResourceName));

        }

        [Fact]
        public async Task UpdateApiScopeAsync()
        {
            IApiResourceRepository apiResourceRepository = new ApiResourceDapperRepository(_configuration);
            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            var localizerApiResourceMock = new Mock<IApiResourceServiceResources>();
            var localizerApiResource = localizerApiResourceMock.Object;

            var localizerClientResourceMock = new Mock<IClientServiceResources>();
            var localizerClientResource = localizerClientResourceMock.Object;

            IClientService clientService = new ClientService(clientRepository, localizerClientResource);
            IApiResourceService apiResourceService = new ApiResourceService(apiResourceRepository, localizerApiResource, clientService);

            //Generate random new api resource
            var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(0);

            var apiResourceDtoId = await apiResourceService.AddApiResourceAsync(apiResourceDto);

            //Get new api resource
            var newApiResourceDto = await apiResourceService.GetApiResourceAsync(apiResourceDtoId);

            //Assert new api resource
            apiResourceDto.ShouldBeEquivalentTo(newApiResourceDto, options => options.Excluding(o => o.Id));

            //Generate random new api scope
            var apiScopeDtoMock = ApiResourceDtoMock.GenerateRandomApiScope(0, newApiResourceDto.Id);

            //Add new api scope
            var apiScopeId = await apiResourceService.AddApiScopeAsync(apiScopeDtoMock);

            //Get inserted api scope
            var apiScope = await apiResourceRepository.GetApiScopeAsync(apiResourceDtoId, apiScopeId);

            //Map entity to model
            var apiScopesDto = apiScope.ToModel();

            //Get new api scope
            var newApiScope = await apiResourceService.GetApiScopeAsync(apiScopesDto.ApiResourceId, apiScopesDto.ApiScopeId);

            //Assert
            newApiScope.ShouldBeEquivalentTo(apiScopesDto, o => o.Excluding(x => x.ResourceName));
            
            //Update api scope
            var updatedApiScope = ApiResourceDtoMock.GenerateRandomApiScope(apiScopesDto.ApiScopeId, apiScopesDto.ApiResourceId);

            await apiResourceService.UpdateApiScopeAsync(updatedApiScope);

            var updatedApiScopeDto = await apiResourceService.GetApiScopeAsync(apiScopesDto.ApiResourceId, apiScopesDto.ApiScopeId);

            //Assert updated api scope
            updatedApiScope.ShouldBeEquivalentTo(updatedApiScopeDto, o => o.Excluding(x => x.ResourceName));
        }

        [Fact]
        public async Task DeleteApiScopeAsync()
        {

            IApiResourceRepository apiResourceRepository = new ApiResourceDapperRepository(_configuration);
            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            var localizerApiResourceMock = new Mock<IApiResourceServiceResources>();
            var localizerApiResource = localizerApiResourceMock.Object;

            var localizerClientResourceMock = new Mock<IClientServiceResources>();
            var localizerClientResource = localizerClientResourceMock.Object;

            IClientService clientService = new ClientService(clientRepository, localizerClientResource);
            IApiResourceService apiResourceService = new ApiResourceService(apiResourceRepository, localizerApiResource, clientService);

            //Generate random new api resource
            var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(0);

            var apiResourceDtoId = await apiResourceService.AddApiResourceAsync(apiResourceDto);

            //Get new api resource
            var newApiResourceDto = await apiResourceService.GetApiResourceAsync(apiResourceDtoId);

            //Assert new api resource
            apiResourceDto.ShouldBeEquivalentTo(newApiResourceDto, options => options.Excluding(o => o.Id));

            //Generate random new api scope
            var apiScopeDtoMock = ApiResourceDtoMock.GenerateRandomApiScope(0, newApiResourceDto.Id);

            //Add new api scope
            var apiScopeId = await apiResourceService.AddApiScopeAsync(apiScopeDtoMock);

            //Get inserted api scope
            var apiScope = await apiResourceRepository.GetApiScopeAsync(apiResourceDtoId, apiScopeId);

            //Map entity to model
            var apiScopesDto = apiScope.ToModel();

            //Get new api scope
            var newApiScope = await apiResourceService.GetApiScopeAsync(apiScopesDto.ApiResourceId, apiScopesDto.ApiScopeId);

            //Assert
            newApiScope.ShouldBeEquivalentTo(apiScopesDto, o => o.Excluding(x => x.ResourceName));

            //Delete it
            await apiResourceService.DeleteApiScopeAsync(newApiScope);

            var deletedApiScope = await apiResourceRepository.GetApiScopeAsync(apiScopesDto.ApiResourceId, apiScopesDto.ApiScopeId);

            //Assert after deleting
            deletedApiScope.Should().BeNull();
        }

        [Fact]
        public async Task AddApiSecretAsync()
        {
            IApiResourceRepository apiResourceRepository = new ApiResourceDapperRepository(_configuration);
            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            var localizerApiResourceMock = new Mock<IApiResourceServiceResources>();
            var localizerApiResource = localizerApiResourceMock.Object;

            var localizerClientResourceMock = new Mock<IClientServiceResources>();
            var localizerClientResource = localizerClientResourceMock.Object;

            IClientService clientService = new ClientService(clientRepository, localizerClientResource);
            IApiResourceService apiResourceService = new ApiResourceService(apiResourceRepository, localizerApiResource, clientService);

            //Generate random new api resource
            var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(0);

            var apiResourceDtoId = await apiResourceService.AddApiResourceAsync(apiResourceDto);

            //Get new api resource
            var newApiResourceDto = await apiResourceService.GetApiResourceAsync(apiResourceDtoId);

            //Assert new api resource
            apiResourceDto.ShouldBeEquivalentTo(newApiResourceDto, options => options.Excluding(o => o.Id));

            //Generate random new api secret
            var apiSecretsDto = ApiResourceDtoMock.GenerateRandomApiSecret(0, newApiResourceDto.Id);

            //Add new api secret
            var apiSecretId = await apiResourceService.AddApiSecretAsync(apiSecretsDto);

            //Get inserted api secret
            var apiSecret = await apiResourceRepository.GetApiSecretAsync(apiSecretId);

            //Map entity to model
            var secretsDto = apiSecret.ToModel();

            //Get new api secret    
            var newApiSecret = await apiResourceService.GetApiSecretAsync(secretsDto.ApiSecretId);

            //Assert
            newApiSecret.ShouldBeEquivalentTo(secretsDto, o => o.Excluding(x => x.ApiResourceName));
        }

        [Fact]
        public async Task DeleteApiSecretAsync()
        {
            IApiResourceRepository apiResourceRepository = new ApiResourceDapperRepository(_configuration);
            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            var localizerApiResourceMock = new Mock<IApiResourceServiceResources>();
            var localizerApiResource = localizerApiResourceMock.Object;

            var localizerClientResourceMock = new Mock<IClientServiceResources>();
            var localizerClientResource = localizerClientResourceMock.Object;

            IClientService clientService = new ClientService(clientRepository, localizerClientResource);
            IApiResourceService apiResourceService = new ApiResourceService(apiResourceRepository, localizerApiResource, clientService);

            //Generate random new api resource
            var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(0);

            var apiResourceDtoId = await apiResourceService.AddApiResourceAsync(apiResourceDto);

            //Get new api resource
            var newApiResourceDto = await apiResourceService.GetApiResourceAsync(apiResourceDtoId);

            //Assert new api resource
            apiResourceDto.ShouldBeEquivalentTo(newApiResourceDto, options => options.Excluding(o => o.Id));

            //Generate random new api secret
            var apiSecretsDtoMock = ApiResourceDtoMock.GenerateRandomApiSecret(0, newApiResourceDto.Id);

            //Add new api secret
            var apiSecretId = await apiResourceService.AddApiSecretAsync(apiSecretsDtoMock);

            //Get inserted api secret
            var apiSecret = await apiResourceRepository.GetApiSecretAsync(apiSecretId);

            //Map entity to model
            var apiSecretsDto = apiSecret.ToModel();

            //Get new api secret    
            var newApiSecret = await apiResourceService.GetApiSecretAsync(apiSecretsDto.ApiSecretId);

            //Assert
            newApiSecret.ShouldBeEquivalentTo(apiSecretsDto, o => o.Excluding(x => x.ApiResourceName));

            //Delete it
            await apiResourceService.DeleteApiSecretAsync(newApiSecret);

            var deletedApiSecret = await apiResourceRepository.GetApiSecretAsync(apiSecretId);

            //Assert after deleting
            deletedApiSecret.Should().BeNull();
        }
    }
}
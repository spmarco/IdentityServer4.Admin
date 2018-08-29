using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentAssertions;
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
    public class ClientServiceDapperTests
    {
        private IConfiguration _configuration;

        public ClientServiceDapperTests()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName)
                .AddJsonFile("appsettings.test.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddUserSecrets<Startup>();

            _configuration = builder.Build();
        }

        [Fact]
        public async Task AddClientAsync()
        {

            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            var localizerMock = new Mock<IClientServiceResources>();
            var localizer = localizerMock.Object;

            IClientService clientService = new ClientService(clientRepository, localizer);

            //Generate random new client
            var client = ClientDtoMock.GenerateRandomClient(0);

            var clientId = await clientService.AddClientAsync(client);

            //Get new client
            var clientEntity = await clientRepository.GetClientAsync(clientId);

            var clientDto = await clientService.GetClientAsync(clientEntity.Id);

            //Assert new client
            client.ShouldBeEquivalentTo(clientDto, options => options.Excluding(o => o.Id));
        }

        [Fact]
        public async Task CloneClientAsync()
        {
            int clonedClientId;

            //Generate random new client
            var clientDto = ClientDtoMock.GenerateRandomClient(0);

            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            var localizerMock = new Mock<IClientServiceResources>();
            var localizer = localizerMock.Object;

            IClientService clientService = new ClientService(clientRepository, localizer);

            //Add new client
            var clientId = await clientService.AddClientAsync(clientDto);

            var clientDtoToClone = await clientService.GetClientAsync(clientId);

            var clientCloneDto = ClientDtoMock.GenerateClientCloneDto(clientDtoToClone);

            //Try clone it
            clonedClientId = await clientService.CloneClientAsync(clientCloneDto);

            var cloneClientEntity = await clientRepository.GetClientAsync(clonedClientId);

            //Assert cloned client
            cloneClientEntity.ShouldBeEquivalentTo(clientDtoToClone,
                options => options.Excluding(o => o.Id)
                    .Excluding(o => o.ClientSecrets)
                    .Excluding(o => o.ClientId)
                    .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedGrantTypes\\[.+\\].Id"))
                    .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedGrantTypes\\[.+\\].Id"))
                    .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedGrantTypes\\[.+\\].GrantType"))
                    .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedGrantTypes\\[.+\\].Client"))
                    .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "RedirectUris\\[.+\\].Id"))
                    .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "RedirectUris\\[.+\\].RedirectUri"))
                    .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "RedirectUris\\[.+\\].Client"))
                    .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "PostLogoutRedirectUris\\[.+\\].Id"))
                    .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "PostLogoutRedirectUris\\[.+\\].PostLogoutRedirectUri"))
                    .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "PostLogoutRedirectUris\\[.+\\].Client"))
                    .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedScopes\\[.+\\].Id"))
                    .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedScopes\\[.+\\].Scope"))
                    .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedScopes\\[.+\\].Client"))
                    .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "IdentityProviderRestrictions\\[.+\\].Id"))
                    .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "IdentityProviderRestrictions\\[.+\\].Provider"))
                    .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "IdentityProviderRestrictions\\[.+\\].Client"))
                    .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "ClientSecrets\\[.+\\].Id"))
                    .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "Claims\\[.+\\].Id"))
                    .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "IdentityProviderRestrictions\\[.+\\].Id"))
                    .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedCorsOrigins\\[.+\\].Id"))
                    .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedCorsOrigins\\[.+\\].Origin"))
                    .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedCorsOrigins\\[.+\\].Client"))
                    .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "Properties\\[.+\\].Id")));

        }

        [Fact]
        public async Task UpdateClientAsync()
        {
            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            var localizerMock = new Mock<IClientServiceResources>();
            var localizer = localizerMock.Object;

            IClientService clientService = new ClientService(clientRepository, localizer);

            //Generate random new client without id
            var client = ClientDtoMock.GenerateRandomClient(0);

            //Add new client
            var clientId = await clientService.AddClientAsync(client);

            //Get new client
            var clientEntity = await clientRepository.GetClientAsync(clientId);

            var clientDto = await clientService.GetClientAsync(clientEntity.Id);

            //Assert new client
            client.ShouldBeEquivalentTo(clientDto, options => options.Excluding(o => o.Id));

            //Generete new client with added item id
            var updatedClient = ClientDtoMock.GenerateRandomClient(clientDto.Id);



            //Update client
            await clientService.UpdateClientAsync(updatedClient);

            //Get updated client
            var updatedClientEntity = await clientRepository.GetClientAsync(updatedClient.Id);

            var updatedClientDto = await clientService.GetClientAsync(updatedClientEntity.Id);

            //Assert updated client
            updatedClient.ShouldBeEquivalentTo(updatedClientDto, options => options.Excluding(o => o.Id));

        }

        [Fact]
        public async Task RemoveClientAsync()
        {

            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            var localizerMock = new Mock<IClientServiceResources>();
            var localizer = localizerMock.Object;

            IClientService clientService = new ClientService(clientRepository, localizer);

            //Generate random new client without id
            var client = ClientDtoMock.GenerateRandomClient(0);

            //Add new client
            var clientId = await clientService.AddClientAsync(client);

            //Get new client
            var clientEntity = await clientRepository.GetClientAsync(clientId);

            var clientDto = await clientService.GetClientAsync(clientEntity.Id);

            //Assert new client
            client.ShouldBeEquivalentTo(clientDto, options => options.Excluding(o => o.Id));

            //Remove client
            await clientService.RemoveClientAsync(clientDto);

            //Try Get Removed client
            var removeClientEntity = await clientRepository.GetClientAsync(clientEntity.Id);

            //Assert removed client - it might be null
            removeClientEntity.Should().BeNull();
        }

        [Fact]
        public async Task GetClientAsync()
        {

            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            var localizerMock = new Mock<IClientServiceResources>();
            var localizer = localizerMock.Object;

            IClientService clientService = new ClientService(clientRepository, localizer);

            //Generate random new client
            var client = ClientDtoMock.GenerateRandomClient(0);

            var clientId = await clientService.AddClientAsync(client);

            //Get new client
            var clientEntity = await clientRepository.GetClientAsync(clientId);

            var clientDto = await clientService.GetClientAsync(clientEntity.Id);

            //Assert new client
            client.ShouldBeEquivalentTo(clientDto, options => options.Excluding(o => o.Id));

        }

        [Fact]
        public async Task AddClientClaimAsync()
        {

            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            var localizerMock = new Mock<IClientServiceResources>();
            var localizer = localizerMock.Object;

            IClientService clientService = new ClientService(clientRepository, localizer);

            //Generate random new client
            var client = ClientDtoMock.GenerateRandomClient(0);

            var clientId = await clientService.AddClientAsync(client);

            //Get new client
            var clientEntity = await clientRepository.GetClientAsync(clientId);

            var clientDto = await clientService.GetClientAsync(clientEntity.Id);

            //Assert new client
            client.ShouldBeEquivalentTo(clientDto, options => options.Excluding(o => o.Id));

            //Generate random new Client Claim
            var clientClaim = ClientDtoMock.GenerateRandomClientClaim(0, clientEntity.Id);

            //Add new client claim
            var clientClaimId = await clientService.AddClientClaimAsync(clientClaim);

            //Get inserted client claims
            var claim = await clientRepository.GetClientClaimAsync(clientClaimId);

            //Map entity to model
            var claimsDto = claim.ToModel();

            //Get new client claim    
            var clientClaimsDto = await clientService.GetClientClaimAsync(claim.Id);

            //Assert
            clientClaimsDto.ShouldBeEquivalentTo(claimsDto, options => options.Excluding(o => o.ClientClaimId));
        }

        [Fact]
        public async Task DeleteClientClaimAsync()
        {

            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            var localizerMock = new Mock<IClientServiceResources>();
            var localizer = localizerMock.Object;

            IClientService clientService = new ClientService(clientRepository, localizer);

            //Generate random new client
            var client = ClientDtoMock.GenerateRandomClient(0);

            var clientId = await clientService.AddClientAsync(client);

            //Get new client
            var clientEntity = await clientRepository.GetClientAsync(clientId);

            var clientDto = await clientService.GetClientAsync(clientEntity.Id);

            //Assert new client
            client.ShouldBeEquivalentTo(clientDto, options => options.Excluding(o => o.Id));

            //Generate random new Client Claim
            var clientClaim = ClientDtoMock.GenerateRandomClientClaim(0, clientEntity.Id);

            //Add new client claim
            var clientClaimId = await clientService.AddClientClaimAsync(clientClaim);

            //Get inserted client claims
            var claim = await clientRepository.GetClientClaimAsync(clientClaimId);

            //Map entity to model
            var claimsDto = claim.ToModel();

            //Get new client claim    
            var clientClaimsDto = await clientService.GetClientClaimAsync(claim.Id);

            //Assert
            clientClaimsDto.ShouldBeEquivalentTo(claimsDto, options => options.Excluding(o => o.ClientClaimId));

            //Delete client claim
            await clientService.DeleteClientClaimAsync(clientClaimsDto);

            //Get removed client claim
            var deletedClientClaim = await clientRepository.GetClientClaimAsync(claim.Id);

            //Assert after delete it
            deletedClientClaim.Should().BeNull();

        }

        [Fact]
        public async Task GetClientClaimAsync()
        {

            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            var localizerMock = new Mock<IClientServiceResources>();
            var localizer = localizerMock.Object;

            IClientService clientService = new ClientService(clientRepository, localizer);

            //Generate random new client
            var client = ClientDtoMock.GenerateRandomClient(0);

            var clientId = await clientService.AddClientAsync(client);

            //Get new client
            var clientEntity = await clientRepository.GetClientAsync(clientId);

            var clientDto = await clientService.GetClientAsync(clientEntity.Id);

            //Assert new client
            client.ShouldBeEquivalentTo(clientDto, options => options.Excluding(o => o.Id));

            //Generate random new Client Claim
            var clientClaim = ClientDtoMock.GenerateRandomClientClaim(0, clientEntity.Id);

            //Add new client claim
            var clientClaimId = await clientService.AddClientClaimAsync(clientClaim);

            //Get inserted client claims
            var claim = await clientRepository.GetClientClaimAsync(clientClaimId);

            //Map entity to model
            var claimsDto = claim.ToModel();

            //Get new client claim    
            var clientClaimsDto = await clientService.GetClientClaimAsync(claim.Id);

            //Assert
            clientClaimsDto.ShouldBeEquivalentTo(claimsDto, options => options.Excluding(o => o.ClientClaimId));

        }

        [Fact]
        public async Task AddClientPropertyAsync()
        {
            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            var localizerMock = new Mock<IClientServiceResources>();
            var localizer = localizerMock.Object;

            IClientService clientService = new ClientService(clientRepository, localizer);

            //Generate random new client
            var client = ClientDtoMock.GenerateRandomClient(0);

            var clientId = await clientService.AddClientAsync(client);

            //Get new client
            var clientEntity = await clientRepository.GetClientAsync(clientId);

            var clientDto = await clientService.GetClientAsync(clientEntity.Id);

            //Assert new client
            client.ShouldBeEquivalentTo(clientDto, options => options.Excluding(o => o.Id));

            //Generate random new Client property
            var clientProperty = ClientDtoMock.GenerateRandomClientProperty(0, clientEntity.Id);

            //Add new client property
            var clientPropertyId = await clientService.AddClientPropertyAsync(clientProperty);

            //Get inserted client property
            var property = await clientRepository.GetClientPropertyAsync(clientPropertyId);

            //Map entity to model
            var propertyDto = property.ToModel();

            //Get new client property    
            var clientPropertiesDto = await clientService.GetClientPropertyAsync(property.Id);

            //Assert
            clientPropertiesDto.ShouldBeEquivalentTo(propertyDto, options => options.Excluding(o => o.ClientPropertyId));
        }

        [Fact]
        public async Task GetClientPropertyAsync()
        {
            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            var localizerMock = new Mock<IClientServiceResources>();
            var localizer = localizerMock.Object;

            IClientService clientService = new ClientService(clientRepository, localizer);

            //Generate random new client
            var client = ClientDtoMock.GenerateRandomClient(0);

            var clientId = await clientService.AddClientAsync(client);

            //Get new client
            var clientEntity = await clientRepository.GetClientAsync(clientId);

            var clientDto = await clientService.GetClientAsync(clientEntity.Id);

            //Assert new client
            client.ShouldBeEquivalentTo(clientDto, options => options.Excluding(o => o.Id));

            //Generate random new Client property
            var clientProperty = ClientDtoMock.GenerateRandomClientProperty(0, clientEntity.Id);

            //Add new client property
            var clientPropertyId = await clientService.AddClientPropertyAsync(clientProperty);

            //Get inserted client property
            var property = await clientRepository.GetClientPropertyAsync(clientPropertyId);

            //Map entity to model
            var propertyDto = property.ToModel();

            //Get new client property    
            var clientPropertiesDto = await clientService.GetClientPropertyAsync(property.Id);

            //Assert
            clientPropertiesDto.ShouldBeEquivalentTo(propertyDto, options => options.Excluding(o => o.ClientPropertyId));

        }

        [Fact]
        public async Task DeleteClientPropertyAsync()
        {
            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            var localizerMock = new Mock<IClientServiceResources>();
            var localizer = localizerMock.Object;

            IClientService clientService = new ClientService(clientRepository, localizer);

            //Generate random new client
            var client = ClientDtoMock.GenerateRandomClient(0);

            var clientId = await clientService.AddClientAsync(client);

            //Get new client
            var clientEntity = await clientRepository.GetClientAsync(clientId);

            var clientDto = await clientService.GetClientAsync(clientEntity.Id);

            //Assert new client
            client.ShouldBeEquivalentTo(clientDto, options => options.Excluding(o => o.Id));

            //Generate random new Client Property
            var clientProperty = ClientDtoMock.GenerateRandomClientProperty(0, clientEntity.Id);

            //Add new client Property
            var clientPropertyId = await clientService.AddClientPropertyAsync(clientProperty);

            //Get inserted client property
            var property = await clientRepository.GetClientPropertyAsync(clientPropertyId);

            //Map entity to model
            var propertiesDto = property.ToModel();

            //Get new client Property    
            var clientPropertiesDto = await clientService.GetClientPropertyAsync(property.Id);

            //Assert
            clientPropertiesDto.ShouldBeEquivalentTo(propertiesDto, options => options.Excluding(o => o.ClientPropertyId));

            //Delete client Property
            await clientService.DeleteClientPropertyAsync(clientPropertiesDto);

            //Get removed client Property
            var deletedClientProperty = await clientRepository.GetClientPropertyAsync(property.Id);

            //Assert after delete it
            deletedClientProperty.Should().BeNull();

        }

        [Fact]
        public async Task AddClientSecretAsync()
        {
            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            var localizerMock = new Mock<IClientServiceResources>();
            var localizer = localizerMock.Object;

            IClientService clientService = new ClientService(clientRepository, localizer);

            //Generate random new client
            var client = ClientDtoMock.GenerateRandomClient(0);

            var clientId = await clientService.AddClientAsync(client);

            //Get new client
            var clientEntity = await clientRepository.GetClientAsync(clientId);

            var clientDto = await clientService.GetClientAsync(clientEntity.Id);

            //Assert new client
            client.ShouldBeEquivalentTo(clientDto, options => options.Excluding(o => o.Id));

            //Generate random new Client secret
            var clientSecret = ClientDtoMock.GenerateRandomClientSecret(0, clientEntity.Id);

            //Add new client secret
            var clientSecretId = await clientService.AddClientSecretAsync(clientSecret);

            //Get inserted client property
            var secret = await clientRepository.GetClientSecretAsync(clientSecretId);

            //Map entity to model
            var clientSecretsDto = secret.ToModel();

            //Get new client secret    
            var secretsDto = await clientService.GetClientSecretAsync(secret.Id);

            //Assert
            secretsDto.ShouldBeEquivalentTo(clientSecretsDto, options => options.Excluding(o => o.ClientSecretId));

        }

        [Fact]
        public async Task GetClientSecretAsync()
        {
            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            var localizerMock = new Mock<IClientServiceResources>();
            var localizer = localizerMock.Object;

            IClientService clientService = new ClientService(clientRepository, localizer);

            //Generate random new client
            var client = ClientDtoMock.GenerateRandomClient(0);

            var clientId = await clientService.AddClientAsync(client);

            //Get new client
            var clientEntity = await clientRepository.GetClientAsync(clientId);

            var clientDto = await clientService.GetClientAsync(clientEntity.Id);

            //Assert new client
            client.ShouldBeEquivalentTo(clientDto, options => options.Excluding(o => o.Id));

            //Generate random new Client secret
            var clientSecret = ClientDtoMock.GenerateRandomClientSecret(0, clientEntity.Id);

            //Add new client secret
            var clientSecretId = await clientService.AddClientSecretAsync(clientSecret);

            //Get inserted client property
            var secret = await clientRepository.GetClientSecretAsync(clientSecretId);

            //Map entity to model
            var clientSecretsDto = secret.ToModel();

            //Get new client secret    
            var secretsDto = await clientService.GetClientSecretAsync(secret.Id);

            //Assert
            secretsDto.ShouldBeEquivalentTo(clientSecretsDto, options => options.Excluding(o => o.ClientSecretId));

        }

        [Fact]
        public async Task DeleteClientSecretAsync()
        {
            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            var localizerMock = new Mock<IClientServiceResources>();
            var localizer = localizerMock.Object;

            IClientService clientService = new ClientService(clientRepository, localizer);

            //Generate random new client
            var client = ClientDtoMock.GenerateRandomClient(0);

            var clientId = await clientService.AddClientAsync(client);

            //Get new client
            var clientEntity = await clientRepository.GetClientAsync(clientId);

            var clientDto = await clientService.GetClientAsync(clientEntity.Id);

            //Assert new client
            client.ShouldBeEquivalentTo(clientDto, options => options.Excluding(o => o.Id));

            //Generate random new Client secret
            var clientSecret = ClientDtoMock.GenerateRandomClientSecret(0, clientEntity.Id);

            //Add new client secret
            var clientSecretId = await clientService.AddClientSecretAsync(clientSecret);

            //Get inserted client property
            var secret = await clientRepository.GetClientSecretAsync(clientSecretId);

            //Map entity to model
            var secretsDto = secret.ToModel();

            //Get new client secret    
            var clientSecretsDto = await clientService.GetClientSecretAsync(secret.Id);

            //Assert
            clientSecretsDto.ShouldBeEquivalentTo(secretsDto, options => options.Excluding(o => o.ClientSecretId));

            //Delete client secret
            await clientService.DeleteClientSecretAsync(clientSecretsDto);

            //Get removed client secret
            var deleteClientSecret = await clientRepository.GetClientSecretAsync(secret.Id);

            //Assert after delete it
            deleteClientSecret.Should().BeNull();
        }
    }
}
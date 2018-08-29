using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.Extensions.Configuration;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Repositories.Dapper;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Repositories.Interface;
using Skoruba.IdentityServer4.Admin.UnitTests.Mocks;
using Xunit;

namespace Skoruba.IdentityServer4.Admin.UnitTests.Repositories.Dapper
{
    public class ClientRepositoryDapperTests
    {
        private readonly IConfiguration _configuration;

        public ClientRepositoryDapperTests()
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

            //Generate random new client
            var client = ClientMock.GenerateRandomClient();

            //Add new client
            var clientId = await clientRepository.AddClientAsync(client);

            //Get new client
            var clientEntity = await clientRepository.GetClientAsync(clientId);

            //Assert new client
            clientEntity.ShouldBeEquivalentTo(client, options => options.Excluding(o => o.Id)
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedGrantTypes\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "RedirectUris\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "PostLogoutRedirectUris\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedScopes\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "IdentityProviderRestrictions\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedCorsOrigins\\[.+\\].Id")));
        }

        [Fact]
        public async Task AddClientClaimAsync()
        {
            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            //Generate random new client without id
            var client = ClientMock.GenerateRandomClient();

            //Add new client
            var clientId = await clientRepository.AddClientAsync(client);

            //Get new client
            var clientEntity = await clientRepository.GetClientAsync(clientId);

            //Assert new client
            clientEntity.ShouldBeEquivalentTo(client, options => options.Excluding(o => o.Id)
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedGrantTypes\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "RedirectUris\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "PostLogoutRedirectUris\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedScopes\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "IdentityProviderRestrictions\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedCorsOrigins\\[.+\\].Id")));

            //Generate random new Client Claim
            var clientClaim = ClientMock.GenerateRandomClientClaim();

            //Add new client claim
            var clientClaimId = await clientRepository.AddClientClaimAsync(clientEntity.Id, clientClaim);

            //Get new client claim
            var newClientClaim = await clientRepository.GetClientClaimAsync(clientClaimId);

            newClientClaim.ShouldBeEquivalentTo(clientClaim, options => options.Excluding(o => o.Id).Excluding(x => x.Client));
        }

        [Fact]
        public async Task AddClientPropertyAsync()
        {
            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            //Generate random new client without id
            var client = ClientMock.GenerateRandomClient();

            //Add new client
            var clientId = await clientRepository.AddClientAsync(client);

            //Get new client
            var clientEntity = await clientRepository.GetClientAsync(clientId);

            //Assert new client
            clientEntity.ShouldBeEquivalentTo(client, options => options.Excluding(o => o.Id)
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedGrantTypes\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "RedirectUris\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "PostLogoutRedirectUris\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedScopes\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "IdentityProviderRestrictions\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedCorsOrigins\\[.+\\].Id")));

            //Generate random new Client property
            var clientProperty = ClientMock.GenerateRandomClientProperty();

            //Add new client property
            var clientPropertyId = await clientRepository.AddClientPropertyAsync(clientEntity.Id, clientProperty);

            //Get new client property
            var newClientProperty = await clientRepository.GetClientPropertyAsync(clientPropertyId);

            newClientProperty.ShouldBeEquivalentTo(clientProperty, options => options.Excluding(o => o.Id).Excluding(x => x.Client));
        }

        [Fact]
        public async Task AddClientSecretAsync()
        {
            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            //Generate random new client without id
            var client = ClientMock.GenerateRandomClient();

            //Add new client
            var clientId = await clientRepository.AddClientAsync(client);

            //Get new client
            var clientEntity = await clientRepository.GetClientAsync(clientId);

            //Assert new client
            clientEntity.ShouldBeEquivalentTo(client, options => options.Excluding(o => o.Id)
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedGrantTypes\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "RedirectUris\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "PostLogoutRedirectUris\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedScopes\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "IdentityProviderRestrictions\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedCorsOrigins\\[.+\\].Id")));
            //Generate random new Client Secret
            var clientSecret = ClientMock.GenerateRandomClientSecret();

            //Add new client secret
            var clientSecretId = await clientRepository.AddClientSecretAsync(clientEntity.Id, clientSecret);

            //Get new client secret
            var newSecret = await clientRepository.GetClientSecretAsync(clientSecretId);

            newSecret.ShouldBeEquivalentTo(clientSecret, options => options.Excluding(o => o.Id).Excluding(x => x.Client).Excluding(x => x.Expiration));
        }

        [Fact]
        public async Task CloneClientAsync()
        {
            //Generate random new client
            var client = ClientMock.GenerateRandomClient(0, generateClaims: true, generateProperties: true, generateSecrets: true);

            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            //Add new client
            var clientId = await clientRepository.AddClientAsync(client);

            var clientToClone = await clientRepository.GetClientAsync(clientId);
            clientToClone.ClientId = $"{clientToClone.ClientId}(Clone))";

            //Try clone it - all client collections without secrets
            var clonedClientId = await clientRepository.CloneClientAsync(clientToClone);

            var cloneClientEntity = await clientRepository.GetClientAsync(clonedClientId);
            var clientToCompare = await clientRepository.GetClientAsync(clientToClone.Id);

            ClientCloneCompare(cloneClientEntity, clientToCompare);
        }



        [Fact]
        public async Task CloneClientWithoutCorsAsync()
        {
            //Generate random new client
            var client = ClientMock.GenerateRandomClient(0, generateClaims: true, generateProperties: true, generateSecrets: true);

            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            //Add new client
            var clientId = await clientRepository.AddClientAsync(client);

            var clientToClone = await clientRepository.GetClientAsync(clientId);
            clientToClone.ClientId = $"{clientToClone.ClientId}(Clone))";
            //Try clone it
            var clonedClientId = await clientRepository.CloneClientAsync(clientToClone, cloneClientCorsOrigins: false);

            var cloneClientEntity = await clientRepository.GetClientAsync(clonedClientId);
            var clientToCompare = await clientRepository.GetClientAsync(clientToClone.Id);

            ClientCloneCompare(cloneClientEntity, clientToCompare, cloneClientCorsOrigins: false);
        }

        [Fact]
        public async Task CloneClientWithoutClaimsAsync()
        {
            //Generate random new client
            var client = ClientMock.GenerateRandomClient(0, generateClaims: true, generateProperties: true, generateSecrets: true);

            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            //Add new client
            var clientId = await clientRepository.AddClientAsync(client);

            var clientToClone = await clientRepository.GetClientAsync(clientId);
            clientToClone.ClientId = $"{clientToClone.ClientId}(Clone))";
            //Try clone it
            var clonedClientId = await clientRepository.CloneClientAsync(clientToClone, cloneClientClaims: false);

            var cloneClientEntity = await clientRepository.GetClientAsync(clonedClientId);
            var clientToCompare = await clientRepository.GetClientAsync(clientToClone.Id);

            ClientCloneCompare(cloneClientEntity, clientToCompare, cloneClientClaims: false);
        }

        [Fact]
        public async Task CloneClientWithoutPropertiesAsync()
        {
            //Generate random new client
            var client = ClientMock.GenerateRandomClient(0, generateClaims: true, generateProperties: true, generateSecrets: true);

            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            //Add new client
            var clientId = await clientRepository.AddClientAsync(client);

            var clientToClone = await clientRepository.GetClientAsync(clientId);
            clientToClone.ClientId = $"{clientToClone.ClientId}(Clone))";
            //Try clone it
            var clonedClientId = await clientRepository.CloneClientAsync(clientToClone, cloneClientProperties: false);

            var cloneClientEntity = await clientRepository.GetClientAsync(clonedClientId);
            var clientToCompare = await clientRepository.GetClientAsync(clientToClone.Id);

            ClientCloneCompare(cloneClientEntity, clientToCompare, cloneClientProperties: false);
        }

        [Fact]
        public async Task CloneClientWithoutGrantTypesAsync()
        {
            //Generate random new client
            var client = ClientMock.GenerateRandomClient(0, generateClaims: true, generateProperties: true, generateSecrets: true);

            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            //Add new client
            var clientId = await clientRepository.AddClientAsync(client);

            var clientToClone = await clientRepository.GetClientAsync(clientId);
            clientToClone.ClientId = $"{clientToClone.ClientId}(Clone))";
            //Try clone it
            var clonedClientId = await clientRepository.CloneClientAsync(clientToClone, cloneClientGrantTypes: false);

            var cloneClientEntity = await clientRepository.GetClientAsync(clonedClientId);
            var clientToCompare = await clientRepository.GetClientAsync(clientToClone.Id);

            ClientCloneCompare(cloneClientEntity, clientToCompare, cloneClientGrantTypes: false);
        }

        [Fact]
        public async Task CloneClientWithoutIdPRestrictionsAsync()
        {
            //Generate random new client
            var client = ClientMock.GenerateRandomClient(0, generateClaims: true, generateProperties: true, generateSecrets: true);

            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            //Add new client
            var clientId = await clientRepository.AddClientAsync(client);

            var clientToClone = await clientRepository.GetClientAsync(clientId);
            clientToClone.ClientId = $"{clientToClone.ClientId}(Clone))";
            //Try clone it
            var clonedClientId = await clientRepository.CloneClientAsync(clientToClone, cloneClientIdPRestrictions: false);

            var cloneClientEntity = await clientRepository.GetClientAsync(clonedClientId);
            var clientToCompare = await clientRepository.GetClientAsync(clientToClone.Id);

            ClientCloneCompare(cloneClientEntity, clientToCompare, cloneClientIdPRestrictions: false);
        }

        [Fact]
        public async Task CloneClientWithoutPostLogoutRedirectUrisAsync()
        {
            //Generate random new client
            var client = ClientMock.GenerateRandomClient(0, generateClaims: true, generateProperties: true, generateSecrets: true);

            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            //Add new client
            var clientId = await clientRepository.AddClientAsync(client);

            var clientToClone = await clientRepository.GetClientAsync(clientId);
            clientToClone.ClientId = $"{clientToClone.ClientId}(Clone))";
            //Try clone it
            var clonedClientId = await clientRepository.CloneClientAsync(clientToClone, cloneClientPostLogoutRedirectUris: false);

            var cloneClientEntity = await clientRepository.GetClientAsync(clonedClientId);
            var clientToCompare = await clientRepository.GetClientAsync(clientToClone.Id);

            ClientCloneCompare(cloneClientEntity, clientToCompare, cloneClientPostLogoutRedirectUris: false);
        }

        [Fact]
        public async Task CloneClientWithoutRedirectUrisAsync()
        {
            //Generate random new client
            var client = ClientMock.GenerateRandomClient(0, generateClaims: true, generateProperties: true, generateSecrets: true);

            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            //Add new client
            var clientId = await clientRepository.AddClientAsync(client);

            var clientToClone = await clientRepository.GetClientAsync(clientId);
            clientToClone.ClientId = $"{clientToClone.ClientId}(Clone))";
            //Try clone it
            var clonedClientId = await clientRepository.CloneClientAsync(clientToClone, cloneClientRedirectUris: false);

            var cloneClientEntity = await clientRepository.GetClientAsync(clonedClientId);
            var clientToCompare = await clientRepository.GetClientAsync(clientToClone.Id);

            ClientCloneCompare(cloneClientEntity, clientToCompare, cloneClientRedirectUris: false);
        }

        [Fact]
        public async Task CloneClientWithoutScopesAsync()
        {
            //Generate random new client
            var client = ClientMock.GenerateRandomClient(0, generateClaims: true, generateProperties: true, generateSecrets: true);

            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            //Add new client
            var clientId = await clientRepository.AddClientAsync(client);

            var clientToClone = await clientRepository.GetClientAsync(clientId);
            clientToClone.ClientId = $"{clientToClone.ClientId}(Clone))";
            //Try clone it
            var clonedClientId = await clientRepository.CloneClientAsync(clientToClone, cloneClientScopes: false);

            var cloneClientEntity = await clientRepository.GetClientAsync(clonedClientId);
            var clientToCompare = await clientRepository.GetClientAsync(clientToClone.Id);

            ClientCloneCompare(cloneClientEntity, clientToCompare, cloneClientScopes: false);
        }

        [Fact]
        public async Task DeleteClientClaimAsync()
        {
            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            //Generate random new client without id
            var client = ClientMock.GenerateRandomClient();

            //Add new client
            var clientId = await clientRepository.AddClientAsync(client);

            //Get new client
            var clientEntity = await clientRepository.GetClientAsync(clientId);

            //Assert new client
            clientEntity.ShouldBeEquivalentTo(client, options => options.Excluding(o => o.Id)
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedGrantTypes\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "RedirectUris\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "PostLogoutRedirectUris\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedScopes\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "IdentityProviderRestrictions\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedCorsOrigins\\[.+\\].Id")));

            //Generate random new Client Claim
            var clientClaim = ClientMock.GenerateRandomClientClaim();

            //Add new client claim
           var clientClaimId = await clientRepository.AddClientClaimAsync(clientEntity.Id, clientClaim);

            //Get new client claim
            var newClientClaim = await clientRepository.GetClientClaimAsync(clientClaimId);

            //Asert
            newClientClaim.ShouldBeEquivalentTo(clientClaim,
                options => options.Excluding(o => o.Id).Excluding(x => x.Client));

            //Try delete it
            await clientRepository.DeleteClientClaimAsync(newClientClaim);

            //Get new client claim
            var deletedClientClaim = await clientRepository.GetClientClaimAsync(clientClaimId);

            //Assert
            deletedClientClaim.Should().BeNull();
        }

        [Fact]
        public async Task DeleteClientPropertyAsync()
        {
            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            //Generate random new client without id
            var client = ClientMock.GenerateRandomClient();

            //Add new client
            var clientId = await clientRepository.AddClientAsync(client);

            //Get new client
            var clientEntity = await clientRepository.GetClientAsync(clientId);

            //Assert new client
            clientEntity.ShouldBeEquivalentTo(client, options => options.Excluding(o => o.Id)
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedGrantTypes\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "RedirectUris\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "PostLogoutRedirectUris\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedScopes\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "IdentityProviderRestrictions\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedCorsOrigins\\[.+\\].Id")));

            //Generate random new Client Property
            var clientProperty = ClientMock.GenerateRandomClientProperty();

            //Add new client property
            var clientPropertyId = await clientRepository.AddClientPropertyAsync(clientEntity.Id, clientProperty);

            //Get new client property
            var newClientProperties = await clientRepository.GetClientPropertyAsync(clientPropertyId);

            //Asert
            newClientProperties.ShouldBeEquivalentTo(clientProperty, options => options.Excluding(o => o.Id).Excluding(x => x.Client));

            //Try delete it
            await clientRepository.DeleteClientPropertyAsync(newClientProperties);

            //Get new client property
            var deletedClientProperty = await clientRepository.GetClientPropertyAsync(clientProperty.Id);

            //Assert
            deletedClientProperty.Should().BeNull();
        }

        [Fact]
        public async Task DeleteClientSecretAsync()
        {
            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            //Generate random new client without id
            var client = ClientMock.GenerateRandomClient();

            //Add new client
            var clientId = await clientRepository.AddClientAsync(client);

            //Get new client
            var clientEntity = await clientRepository.GetClientAsync(clientId);

            //Assert new client
            clientEntity.ShouldBeEquivalentTo(client, options => options.Excluding(o => o.Id)
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedGrantTypes\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "RedirectUris\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "PostLogoutRedirectUris\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedScopes\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "IdentityProviderRestrictions\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedCorsOrigins\\[.+\\].Id")));

            //Generate random new Client Secret
            var clientSecret = ClientMock.GenerateRandomClientSecret();

            //Add new client secret
            var clientSecretId = await clientRepository.AddClientSecretAsync(clientEntity.Id, clientSecret);

            //Get new client secret
            var newSecret = await clientRepository.GetClientSecretAsync(clientSecretId);

            //Asert
            newSecret.ShouldBeEquivalentTo(clientSecret, options => options.Excluding(o => o.Id).Excluding(x => x.Client).Excluding(x => x.Expiration));

            //Try delete it
            await clientRepository.DeleteClientSecretAsync(newSecret);

            //Get new client secret
            var deletedSecret = await clientRepository.GetClientSecretAsync(clientSecret.Id);

            //Assert
            deletedSecret.Should().BeNull();
        }

        [Fact]
        public async Task GetClientAsync()
        {
            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            //Generate random new client without id
            var client = ClientMock.GenerateRandomClient();

            //Add new client
            var clientId = await clientRepository.AddClientAsync(client);

            //Get new client
            var clientEntity = await clientRepository.GetClientAsync(clientId);

            //Assert new client
            clientEntity.ShouldBeEquivalentTo(client, options => options.Excluding(o => o.Id)
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedGrantTypes\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "RedirectUris\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "PostLogoutRedirectUris\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedScopes\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "IdentityProviderRestrictions\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedCorsOrigins\\[.+\\].Id")));
        }

        [Fact]
        public async Task GetClientClaimAsync()
        {
            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            //Generate random new client without id
            var client = ClientMock.GenerateRandomClient();

            //Add new client
            var clientId = await clientRepository.AddClientAsync(client);

            //Get new client
            var clientEntity = await clientRepository.GetClientAsync(clientId);

            //Assert new client
            clientEntity.ShouldBeEquivalentTo(client, options => options.Excluding(o => o.Id)
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedGrantTypes\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "RedirectUris\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "PostLogoutRedirectUris\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedScopes\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "IdentityProviderRestrictions\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedCorsOrigins\\[.+\\].Id")));

            //Generate random client claim
            var clientClaim = ClientMock.GenerateRandomClientClaim();

            //Add new client claim
            var clientClaimId = await clientRepository.AddClientClaimAsync(clientEntity.Id, clientClaim);

            //Get new client claim
            var newClientClaim = await clientRepository.GetClientClaimAsync(clientClaimId);

            newClientClaim.ShouldBeEquivalentTo(clientClaim, options => options.Excluding(o => o.Id).Excluding(x => x.Client));
        }

        [Fact]
        public async Task GetClientPropertyAsync()
        {
            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            //Generate random new client without id
            var client = ClientMock.GenerateRandomClient();

            //Add new client
            var clientId = await clientRepository.AddClientAsync(client);

            //Get new client
            var clientEntity = await clientRepository.GetClientAsync(clientId);

            //Assert new client
            clientEntity.ShouldBeEquivalentTo(client, options => options.Excluding(o => o.Id)
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedGrantTypes\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "RedirectUris\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "PostLogoutRedirectUris\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedScopes\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "IdentityProviderRestrictions\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedCorsOrigins\\[.+\\].Id")));

            //Generate random new Client Property
            var clientProperty = ClientMock.GenerateRandomClientProperty();

            //Add new client Property
            var clientPropertyId = await clientRepository.AddClientPropertyAsync(clientEntity.Id, clientProperty);

            //Get new client Property
            var newClientProperty = await clientRepository.GetClientPropertyAsync(clientPropertyId);

            newClientProperty.ShouldBeEquivalentTo(clientProperty, options => options.Excluding(o => o.Id).Excluding(x => x.Client));
        }

        [Fact]
        public async Task GetClientsAsync()
        {
            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            var rnd = new Random();
            var generateRows = rnd.Next(1, 10);

            //Generate random new clients
            var randomClients = ClientMock.GenerateRandomClients(0, generateRows);

            foreach (var client in randomClients)
                await clientRepository.AddClientAsync(client);

            var clients = await clientRepository.GetClientsAsync();

            //Assert clients count
            clients.Data.Count.Should().BeGreaterThan(randomClients.Count);
        }

        [Fact]
        public async Task GetClientSecretAsync()
        {
            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            //Generate random new client without id
            var client = ClientMock.GenerateRandomClient();

            //Add new client
            var clientId = await clientRepository.AddClientAsync(client);

            //Get new client
            var clientEntity = await clientRepository.GetClientAsync(clientId);

            //Assert new client
            clientEntity.ShouldBeEquivalentTo(client, options => options.Excluding(o => o.Id)
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedGrantTypes\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "RedirectUris\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "PostLogoutRedirectUris\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedScopes\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "IdentityProviderRestrictions\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedCorsOrigins\\[.+\\].Id")));

            //Generate random new Client Secret
            var clientSecret = ClientMock.GenerateRandomClientSecret();

            //Add new client secret
            var clientSecretId = await clientRepository.AddClientSecretAsync(clientEntity.Id, clientSecret);

            //Get new client secret
            var newSecret = await clientRepository.GetClientSecretAsync(clientSecretId);

            newSecret.ShouldBeEquivalentTo(clientSecret,options => options.Excluding(o => o.Id).Excluding(x => x.Client).Excluding(x => x.Expiration));
        }

        [Fact]
        public async Task RemoveClientAsync()
        {
            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            //Generate random new client without id
            var client = ClientMock.GenerateRandomClient();

            //Add new client
            var clientId = await clientRepository.AddClientAsync(client);

            //Get new client
            var clientEntity = await clientRepository.GetClientAsync(clientId);

            //Assert new client
            clientEntity.ShouldBeEquivalentTo(client, options => options.Excluding(o => o.Id)
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedGrantTypes\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "RedirectUris\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "PostLogoutRedirectUris\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedScopes\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "IdentityProviderRestrictions\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedCorsOrigins\\[.+\\].Id")));

            //Remove client
            await clientRepository.RemoveClientAsync(clientEntity);

            //Try Get Removed client
            var removeClientEntity = await clientRepository.GetClientAsync(clientId);

            //Assert removed client - it might be null
            removeClientEntity.Should().BeNull();
        }

        [Fact]
        public async Task UpdateClientAsync()
        {
            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            //Generate random new client without id
            var client = ClientMock.GenerateRandomClient();

            //Add new client
            var clientId = await clientRepository.AddClientAsync(client);

            //Get new client
            var clientEntity = await clientRepository.GetClientAsync(clientId);

            //Assert new client
            clientEntity.ShouldBeEquivalentTo(client, options => options.Excluding(o => o.Id)
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedGrantTypes\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "RedirectUris\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "PostLogoutRedirectUris\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedScopes\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "IdentityProviderRestrictions\\[.+\\].Id"))
                                                                        .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedCorsOrigins\\[.+\\].Id")));

            //Generete new client with added item id
            var updatedClient = ClientMock.GenerateRandomClient(clientEntity.Id);

            //Update client
            await clientRepository.UpdateClientAsync(updatedClient);

            //Get updated client
            var updatedClientEntity = await clientRepository.GetClientAsync(updatedClient.Id);

            //Assert updated client
            updatedClientEntity.ShouldBeEquivalentTo(updatedClient, options => options.Excluding(o => o.Id)
                                                                                      .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedGrantTypes\\[.+\\].Id"))
                                                                                      .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "RedirectUris\\[.+\\].Id"))
                                                                                      .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "PostLogoutRedirectUris\\[.+\\].Id"))
                                                                                      .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedScopes\\[.+\\].Id"))
                                                                                      .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "IdentityProviderRestrictions\\[.+\\].Id"))
                                                                                      .Excluding(x => Regex.IsMatch(x.SelectedMemberPath, "AllowedCorsOrigins\\[.+\\].Id")));
        }

        [Fact]
        public void GetGrantTypes()
        {
            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            //Try get some existing grant
            var randomClientGrantType = ClientMock.GenerateRandomClientGrantType();

            var grantTypes = clientRepository.GetGrantTypes(randomClientGrantType.GrantType);
            grantTypes[0].Should().Be(randomClientGrantType.GrantType);
        }

        [Fact]
        public void GetStandardClaims()
        {
            IClientRepository clientRepository = new ClientDapperRepository(_configuration);

            //Try get some existing claims
            var randomClientClaim = ClientMock.GenerateRandomClientClaim();

            var grantTypes = clientRepository.GetStandardClaims(randomClientClaim.Type);
            grantTypes.Contains(randomClientClaim.Type).Should().Be(true);
        }

        [Fact]
        public async Task GetScopesIdentityResourceAsync()
        {
            IClientRepository clientRepository = new ClientDapperRepository(_configuration);
            IIdentityResourceRepository identityResourceRepository = new IdentityResourceDapperRepository(_configuration);

            var identityResource = IdentityResourceMock.GenerateRandomIdentityResource();
            await identityResourceRepository.AddIdentityResourceAsync(identityResource);

            var identityScopes = await clientRepository.GetScopesAsync(identityResource.Name);

            identityScopes[0].Should().Be(identityResource.Name);
        }

        [Fact]
        public async Task GetScopesApiResourceAsync()
        {
            IClientRepository clientRepository = new ClientDapperRepository(_configuration);
            IApiResourceRepository apiResourceRepository = new ApiResourceDapperRepository(_configuration);

            var apiResource = ApiResourceMock.GenerateRandomApiResource();
            await apiResourceRepository.AddApiResourceAsync(apiResource);

            var apiScopes = await clientRepository.GetScopesAsync(apiResource.Name);

            apiScopes[0].Should().Be(apiResource.Name);
        }

        private void ClientCloneCompare(Client cloneClientEntity, Client clientToCompare, bool cloneClientCorsOrigins = true, bool cloneClientGrantTypes = true, bool cloneClientIdPRestrictions = true, bool cloneClientPostLogoutRedirectUris = true, bool cloneClientScopes = true, bool cloneClientRedirectUris = true, bool cloneClientClaims = true, bool cloneClientProperties = true)
        {
            //Assert cloned client
            cloneClientEntity.ShouldBeEquivalentTo(clientToCompare,
                options => options.Excluding(o => o.Id)
                    .Excluding(o => o.ClientSecrets)
                    .Excluding(o => o.ClientId)
                    .Excluding(o => o.ClientName)

                    //Skip the collections because is not possible ignore property in list :-(
                    //Note: I've found the solution above - try ignore property of the list using SelectedMemberPath                        
                    .Excluding(o => o.AllowedGrantTypes)
                    .Excluding(o => o.RedirectUris)
                    .Excluding(o => o.PostLogoutRedirectUris)
                    .Excluding(o => o.AllowedScopes)
                    .Excluding(o => o.ClientSecrets)
                    .Excluding(o => o.Claims)
                    .Excluding(o => o.IdentityProviderRestrictions)
                    .Excluding(o => o.AllowedCorsOrigins)
                    .Excluding(o => o.Properties)
            );


            //New client relations have new id's and client relations therefore is required ignore them
            if (cloneClientGrantTypes)
            {
                cloneClientEntity.AllowedGrantTypes.ShouldBeEquivalentTo(clientToCompare.AllowedGrantTypes,
                    option => option.Excluding(x => x.SelectedMemberPath.EndsWith("Id"))
                        .Excluding(x => x.SelectedMemberPath.EndsWith("Client")));
            }
            else
            {
                cloneClientEntity.AllowedGrantTypes.Should().BeEmpty();
            }

            if (cloneClientCorsOrigins)
            {
                cloneClientEntity.AllowedCorsOrigins.ShouldBeEquivalentTo(clientToCompare.AllowedCorsOrigins,
                    option => option.Excluding(x => x.SelectedMemberPath.EndsWith("Id"))
                        .Excluding(x => x.SelectedMemberPath.EndsWith("Client")));
            }
            else
            {
                cloneClientEntity.AllowedCorsOrigins.Should().BeEmpty();
            }

            if (cloneClientRedirectUris)
            {
                cloneClientEntity.RedirectUris.ShouldBeEquivalentTo(clientToCompare.RedirectUris,
                    option => option.Excluding(x => x.SelectedMemberPath.EndsWith("Id"))
                        .Excluding(x => x.SelectedMemberPath.EndsWith("Client")));
            }
            else
            {
                cloneClientEntity.RedirectUris.Should().BeEmpty();
            }

            if (cloneClientPostLogoutRedirectUris)
            {
                cloneClientEntity.PostLogoutRedirectUris.ShouldBeEquivalentTo(clientToCompare.PostLogoutRedirectUris,
                    option => option.Excluding(x => x.SelectedMemberPath.EndsWith("Id"))
                        .Excluding(x => x.SelectedMemberPath.EndsWith("Client")));
            }
            else
            {
                cloneClientEntity.PostLogoutRedirectUris.Should().BeEmpty();
            }

            if (cloneClientScopes)
            {
                cloneClientEntity.AllowedScopes.ShouldBeEquivalentTo(clientToCompare.AllowedScopes,
                    option => option.Excluding(x => x.SelectedMemberPath.EndsWith("Id"))
                        .Excluding(x => x.SelectedMemberPath.EndsWith("Client")));
            }
            else
            {
                cloneClientEntity.AllowedScopes.Should().BeEmpty();
            }

            if (cloneClientClaims)
            {
                cloneClientEntity.Claims.ShouldBeEquivalentTo(clientToCompare.Claims,
                    option => option.Excluding(x => x.SelectedMemberPath.EndsWith("Id"))
                        .Excluding(x => x.SelectedMemberPath.EndsWith("Client")));
            }
            else
            {
                cloneClientEntity.Claims.Should().BeEmpty();
            }

            if (cloneClientIdPRestrictions)
            {
                cloneClientEntity.IdentityProviderRestrictions.ShouldBeEquivalentTo(clientToCompare.IdentityProviderRestrictions,
                    option => option.Excluding(x => x.SelectedMemberPath.EndsWith("Id"))
                        .Excluding(x => x.SelectedMemberPath.EndsWith("Client")));
            }
            else
            {
                cloneClientEntity.IdentityProviderRestrictions.Should().BeEmpty();
            }

            if (cloneClientProperties)
            {
                cloneClientEntity.Properties.ShouldBeEquivalentTo(clientToCompare.Properties,
                    option => option.Excluding(x => x.SelectedMemberPath.EndsWith("Id"))
                        .Excluding(x => x.SelectedMemberPath.EndsWith("Client")));
            }
            else
            {
                cloneClientEntity.Properties.Should().BeEmpty();
            }

            cloneClientEntity.ClientSecrets.Should().BeEmpty();
        }
    }
}
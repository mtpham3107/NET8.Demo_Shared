using IdentityServer4;
using IdentityServer4.Models;

namespace NET8.Demo.GlobalAdmin;

public class IdentityServerConfig
{
    public static IEnumerable<IdentityResource> IdentityResources => [
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
    ];

    public static IEnumerable<ApiResource> ApiResources => [
        new ApiResource("GlobalAdmin", "GlobalAdmin api")
        {
            Scopes = { "GlobalAdminFullAccess" }
        },
        new ApiResource("TemplateServiceClient", "TemplateServiceClient api")
        {
            Scopes = { "TemplateServiceClientFullAccess" }
        },
    ];

    public static IEnumerable<ApiScope> ApiScopes => [
        new ApiScope("GlobalAdminFullAccess", "Full access to GlobalAdmin"),
        new ApiScope("TemplateServiceClientFullAccess", "Full access to TemplateServiceClient")
    ];

    public static IEnumerable<Client> Clients(Dictionary<string, string> clientUrls) => [
        new Client
        {
            ClientId = "GlobalAdminServiceClient",
            AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
            ClientSecrets = {
                new Secret("7Ecv5hTgKcPgeHpRX8TXxH5K4LoE3R0J".Sha256())
            },
            AllowedScopes = {
                "GlobalAdminFullAccess",
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
            }
        },
        new Client
        {
            ClientId = "TemplateServiceClient",
            AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
            ClientSecrets = {
                new Secret("Ho2bJ9z7LXDm22jTgfJdhFWe7GQmBUcA".Sha256())
            },
            AllowedScopes = {
                "TemplateServiceClientFullAccess",
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
            }
        },
        new Client
        {
            ClientId = "client_app1",
            AllowedGrantTypes = GrantTypes.Code,
            ClientSecrets = {
                new Secret("MZ8KUfgCkAgaVLWSkIWDnaxy4TVshc1q".Sha256())
            },
            AllowedScopes = {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                "TemplateServiceClientFullAccess", "GlobalAdminFullAccess"
            },
            RedirectUris = { $"{clientUrls["App1"]}/login-callback" },
            PostLogoutRedirectUris = { $"{clientUrls["App1"]}/logout-callback" },
            AllowedCorsOrigins = { $"{clientUrls["App1"]}" },
        },
        new Client
        {
            ClientId = "client_app2",
            AllowedGrantTypes = GrantTypes.Code,
            ClientSecrets = {
                new Secret("cYakBLjj9o3w9cI3GLpR4ryBHCRcEX3g".Sha256())
            },
            AllowedScopes = {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                "TemplateServiceClientFullAccess", "GlobalAdminFullAccess"
            },
            RedirectUris = { $"{clientUrls["App2"]}/login-callback" },
            PostLogoutRedirectUris = { $"{clientUrls["App2"]}/logout-callback" },
            AllowedCorsOrigins = { $"{clientUrls["App2"]}" },
        },
    ];
}

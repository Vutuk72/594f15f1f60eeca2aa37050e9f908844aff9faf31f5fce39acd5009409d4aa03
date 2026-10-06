using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Přihlašování přes Keycloak: API samo nikoho nepřihlašuje, jen ověřuje JWT token,
// který Angular dostal od Keycloaku a posílá v hlavičce "Authorization: Bearer ...".
var keycloak = builder.Configuration.GetSection("Keycloak");
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Kdo token vydal – musí přesně sedět s "iss" v tokenu.
        options.Authority = keycloak["Authority"];
        // V Dockeru se klíče stahují přes interní adresu (viz docker-compose.yml), lokálně z Authority.
        var metadataAddress = keycloak["MetadataAddress"];
        if (!string.IsNullOrEmpty(metadataAddress))
        {
            options.MetadataAddress = metadataAddress;
        }
        // Keycloak běží při vývoji na http, v produkci by tu mělo být https a true.
        options.RequireHttpsMetadata = false;
        // Pro koho je token určen – Keycloak ho přidává mapperem "audience-ekospol-api".
        options.Audience = keycloak["Audience"];
        // Nechat názvy claimů tak, jak je posílá Keycloak (bez převodu na dlouhé .NET názvy).
        options.MapInboundClaims = false;
        options.TokenValidationParameters.NameClaimType = "preferred_username";
        // Role jsou v claimu "roles" (mapper "realm-roles-flat") → funguje [Authorize(Roles = "admin")].
        options.TokenValidationParameters.RoleClaimType = "roles";
    });
builder.Services.AddAuthorization();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

using System.Text;
using AutoMapper;
using Core;
using Core.Interfaces;
using Core.MapperProfile;
using Core.Models;
using Core.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var jwtOpts = builder.Configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>()!;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOpts.Issuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOpts.Key)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

//Services
// Налаштування JWT
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));

// Налаштування AutoMapper
builder.Services.AddAutoMapper(typeof(AppProfile));

builder.Services.AddSingleton<IJwtService>(sp =>
{
    var jwtOptions = sp.GetRequiredService<IOptions<JwtOptions>>().Value;
    return new JwtService(jwtOptions);
});
builder.Services.AddSingleton<JwtService>(sp => (JwtService)sp.GetRequiredService<IJwtService>());

// Налаштування LingvoriaDbContext
builder.Services.AddSingleton(sp =>
{
    var connectionString = builder.Configuration["ConnectionString:String"];
    var databaseName = builder.Configuration["ConnectionString:Db"];
    return new LingvoriaDbContext(connectionString, databaseName);
});

// Налаштування PasswordHasher
builder.Services.AddSingleton<PasswordHasher>();

// Налаштування AccountService
builder.Services.AddSingleton<IAccountService, AccountService>(sp =>
{
    var hasher = sp.GetRequiredService<PasswordHasher>();
    var context = sp.GetRequiredService<LingvoriaDbContext>();
    var jwt = sp.GetRequiredService<JwtService>();
    return new AccountService(hasher, context, jwt);
});

builder.Services.AddSingleton<IWordService, WordService>(sp =>
{
    var context = sp.GetRequiredService<LingvoriaDbContext>();
    var mapper = sp.GetRequiredService<IMapper>();
    return new WordService(context, mapper);
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(cfg =>
{
    cfg.AllowAnyHeader();
    cfg.AllowAnyMethod();
    cfg.AllowAnyOrigin();
});

app.UseAuthorization();

app.MapControllers();

app.Run();

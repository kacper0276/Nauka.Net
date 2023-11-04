using WebApplication1;
using WebApplication1.Entities;
using System.Reflection;
using WebApplication1.Services;
using NLog.Web;
using WebApplication1.Middleware;
using Microsoft.AspNetCore.Identity;
using FluentValidation;
using WebApplication1.Models;
using WebApplication1.Models.Validators;
using FluentValidation.AspNetCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebApplication1.Authorization;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Dane u¿yte do JWT
var authenticationSettings = new AuthenticationSettings();
builder.Configuration.GetSection("Authentication").Bind(authenticationSettings);

builder.Services.AddSingleton(authenticationSettings);

// Konfiguracja autentykacji
builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = "Bearer";
    option.DefaultScheme = "Bearer";
    option.DefaultChallengeScheme = "Bearer";
}).AddJwtBearer(cfg =>
{
    cfg.RequireHttpsMetadata = false;
    cfg.SaveToken = true;
    cfg.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = authenticationSettings.JwtIssuer,
        ValidAudience = authenticationSettings.JwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey)),
    };
});

// W³asne warunki autoryzacji
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("HasNationality", builder => builder.RequireClaim("Nationality", "German", "Polish")); // Wartoœci jakie musi przyj¹æ
    options.AddPolicy("Atleast20", builder => builder.AddRequirements(new MinimumAgeRequirement(20))); // W³asny, musi mieæ conajmniej 20 lat
    options.AddPolicy("CreatedAtleast2Restaurants", builder => builder.AddRequirements(new CreatedMultipleRestaurantsRequirement(2))); // Co najmniej 2 restauracje
});
builder.Services.AddScoped<IAuthorizationHandler, MinimumAgeRequirementHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementHandler>();
builder.Services.AddScoped<IAuthorizationHandler, CreatedMultipleRestaurantsRequirementHandler>();

// Add services to the container.
builder.Services.AddTransient<IWeatherForecastService, WeatherForecastService>();
builder.Services.AddControllers().AddFluentValidation(); // FluentValidation <- paczka do tworzenia validacji
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IDishService, DishService>(); // Dodanie serwisu
builder.Services.AddScoped<RestaurantSeeder>(); // Zale¿noœæ typu Scoped
builder.Services.AddScoped<IRestaurantService, RestaurantService>(); // Dodanie Serwisu
builder.Services.AddScoped<IAccountService, AccountService>(); // Dodanie serwisu
builder.Services.AddScoped<IUserContextService,  UserContextService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDbContext<RestaurantDbContext>();
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

// Validator
builder.Services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
builder.Services.AddScoped<IValidator<RestaurantQuery>, RestaurantQueryValidator>();

// Hashowanie Has³a
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// Middleware
builder.Services.AddScoped<ErrorHandlingMiddleware>();
builder.Services.AddScoped<RequestTimeMiddleware>();

builder.Host.UseNLog();

var app = builder.Build();

var scope = app.Services.CreateScope();
var seeder = scope.ServiceProvider.GetRequiredService<RestaurantSeeder>();

seeder.Seed();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant API");
    });
}

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestTimeMiddleware>();
app.UseAuthentication(); // JWT
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

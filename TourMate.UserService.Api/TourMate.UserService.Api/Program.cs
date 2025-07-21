using TourMate.UserService.Repositories.IRepositories;
using TourMate.UserService.Repositories.Repositories;
using TourMate.UserService.Services.IServices;
using TourMate.UserService.Services.Services;
using TourMate.UserService.Services.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAccountService, AccountService>();

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>()
    ;
builder.Services.AddScoped<ITourGuideRepository, TourGuideRepository>();
builder.Services.AddScoped<ITourGuideService, TourGuideService>();

builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRoleService, RoleService>();

builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<IEmailSender, EmailSender>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

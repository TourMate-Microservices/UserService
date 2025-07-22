using Microsoft.OpenApi.Models;
using TourMate.UserService.Repositories.IRepositories;
using TourMate.UserService.Repositories.Repositories;
using TourMate.UserService.Services.IServices;
using TourMate.UserService.Services.Services;
using TourMate.UserService.Services.Utils;
using TourMate.UserService.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:5000");

// Đăng ký CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", builder =>
    {
        builder.WithOrigins(
            "http://localhost:3000"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

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

// Add gRPC client
builder.Services.AddScoped<ITourServiceGrpcClient, TourServiceGrpcClient>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "UserService API",
        Version = "v1"
    });

    // Thêm dòng này để Swagger hiểu base URL là /user-service
    c.AddServer(new OpenApiServer
    {
        Url = "/user-service"
    });
});

var app = builder.Build();


app.UsePathBase("/user-service");

app.UseRouting();

app.UseCors("AllowReactApp");


// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/user-service/swagger/v1/swagger.json", "UserService API V1");
    c.RoutePrefix = "swagger";
});


//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

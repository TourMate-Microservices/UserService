using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using TourMate.UserService.Api.Services;
using TourMate.UserService.Repositories.IRepositories;
using TourMate.UserService.Repositories.Repositories;
using TourMate.UserService.Services.IServices;
using TourMate.UserService.Services.Services;
using TourMate.UserService.Services.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:5000");

// Configure Kestrel to listen on both HTTP and gRPC ports
builder.WebHost.ConfigureKestrel(options =>
{
    // HTTP endpoint for REST API
    options.ListenAnyIP(5000, listenOptions =>
    {
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1;
    });

    // gRPC endpoint
    options.ListenAnyIP(9092, listenOptions =>
    {
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
    });
});

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
//builder.Services.AddScoped<IUserGrpcClient, UserGrpcClient>();
builder.Services.AddScoped<IFeedbackGrpcService, FeedbackGrpcService>();

// Add gRPC server and UserGrpcService
builder.Services.AddGrpc();
builder.Services.AddScoped<UserGrpcService>();
builder.Services.AddScoped<FeedbackGrpcService>();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
});

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

// Map gRPC service
app.MapGrpcService<UserGrpcService>();
app.MapGrpcService<FeedbackGrpcService>();

app.Run();

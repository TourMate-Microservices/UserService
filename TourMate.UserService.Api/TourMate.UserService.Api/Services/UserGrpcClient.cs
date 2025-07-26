using Grpc.Net.Client;
using userservice;

namespace TourMate.UserService.Api.Services
{
    public interface IUserGrpcClient
    {
        Task<TourGuideResponse> GetTourGuideByIdAsync(int id);
    }

    public class UserGrpcClient : IUserGrpcClient, IDisposable
    {
        private readonly GrpcChannel _channel;
        private readonly userservice.UserService.UserServiceClient _client;

        public UserGrpcClient(IConfiguration configuration)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var userServiceUrl = configuration["GrpcServices:UserService"] ?? "http://user-service:5000";
            _channel = GrpcChannel.ForAddress("http://localhost:9092");
            _client = new userservice.UserService.UserServiceClient(_channel);
        }

        public async Task<TourGuideResponse> GetTourGuideByIdAsync(int id)
        {
            var request = new GetTourGuideByIdRequest { TourGuideId = id };
            return await _client.GetTourGuideByIdAsync(request);
        }

        public void Dispose()
        {
            _channel?.Dispose();
        }
    }
}

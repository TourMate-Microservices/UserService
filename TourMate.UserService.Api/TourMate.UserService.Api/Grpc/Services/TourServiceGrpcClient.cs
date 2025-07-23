using Grpc.Net.Client;
using TourMate.Grpc;
using TourMate.UserService.Api.Grpc.IServices;

namespace TourMate.UserService.Api.Grpc.Services
{
    public class TourServiceGrpcClient : ITourServiceGrpcClient, IDisposable
    {
        private readonly GrpcChannel _channel;
        private readonly TourService.TourServiceClient _client;
        private readonly ILogger<TourServiceGrpcClient> _logger;

        public TourServiceGrpcClient(IConfiguration configuration, ILogger<TourServiceGrpcClient> logger)
        {
            _logger = logger;
            
            try 
            {
                var tourServiceUrl = configuration["GrpcServices:TourService"] ?? "http://tour-service:9091";
                
                _channel = GrpcChannel.ForAddress(tourServiceUrl);
                _client = new TourService.TourServiceClient(_channel);
                
                _logger.LogInformation("TourService gRPC client initialized with URL: {Url}", tourServiceUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize gRPC client");
                throw;
            }
        }

        public async Task<TourServiceList> GetToursByTourGuideIdAsync(int tourGuideId)
        {
            try
            {
                _logger.LogInformation("Calling GetByTourGuideId for tourGuideId: {TourGuideId}", tourGuideId);
                
                var request = new TourGuideIdRequest
                {
                    TourGuideId = tourGuideId
                };

                var response = await _client.GetByTourGuideIdAsync(request);
                
                _logger.LogInformation("Received {Count} tours for tourGuideId: {TourGuideId}", 
                    response.Items.Count, tourGuideId);
                
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling TourService gRPC for tourGuideId: {TourGuideId}", tourGuideId);
                throw;
            }
        }

        public async Task<TourServiceList> GetNumOfTourByTourGuideId(int tourGuideId, int numOfTours)
        {
            try
            {
                _logger.LogInformation("Calling GetByTourGuideId for tourGuideId: {TourGuideId}", tourGuideId);

                var request = new TourGuideIdRequestAnNumberOfTours
                {
                    TourGuideId = tourGuideId,
                    NumberOfTours = numOfTours
                };

                var response = await _client.GetNumOfTourByTourGuideIdAsync(request);

                _logger.LogInformation("Received {Count} tours for tourGuideId: {TourGuideId}",
                    response.Items.Count, tourGuideId);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling TourService gRPC for tourGuideId: {TourGuideId}", tourGuideId);
                throw;
            }
        }   

        public void Dispose()
        {
            _channel?.Dispose();
        }
    }
}

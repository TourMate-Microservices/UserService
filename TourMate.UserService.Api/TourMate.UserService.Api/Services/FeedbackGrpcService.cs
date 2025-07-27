using FeedbackServiceGrpc;
using Grpc.Net.Client;
using TourMate.Grpc;
using TourMate.UserService.Repositories.ResponseModels;
using TourMate.UserService.Services.IServices;
namespace TourMate.UserService.Api.Services
{
    public interface IFeedbackGrpcService
    {
        // Define methods that will be implemented in the FeedbackGrpcService class
        Task<PagedResult<Feedback>> GetFeedbacksForTourGuide(int tourGuideId, int page, int size);
    }
    public class FeedbackGrpcService: IFeedbackGrpcService, IDisposable
    {
        private readonly ITourGuideService _tourGuideService;
        private readonly ILogger<FeedbackGrpcService> _logger;
        private readonly GrpcChannel _channel;
        private readonly FeedbackService.FeedbackServiceClient _client;

        public FeedbackGrpcService(ITourGuideService tourGuideService, ILogger<FeedbackGrpcService> logger, IConfiguration configuration)
        {
            _tourGuideService = tourGuideService ?? throw new ArgumentNullException(nameof(tourGuideService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            try
            {
                var feedbackUrl = configuration["GrpcServices:FeedbackService"] ?? "http://payment-service:9093";

                _channel = GrpcChannel.ForAddress(feedbackUrl);
                _client = new FeedbackService.FeedbackServiceClient(_channel);

                _logger.LogInformation("FeedbackService gRPC client initialized with URL: {Url}", feedbackUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize gRPC client");
            }

        }

        public void Dispose()
        {
            _channel?.Dispose();
        }

        public async Task<PagedResult<Feedback>> GetFeedbacksForTourGuide(int tourGuideId, int page, int size)
        {
            try
            {
                _logger.LogInformation("Calling GetFeedbackOfTourGuide for tourGuideId: {tourGuideId}\n", tourGuideId);

                var request = new TourGuideRequest
                {
                    TourGuideId = tourGuideId
                };

                var response = await _client.GetFeedbackOfTourGuideAsync(request);

                _logger.LogInformation("Response is: {response}\n", response);

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling GetFeedbackOfTourGuide gRPC for tourGuideId: {tourGuideId}", tourGuideId);                
            }
            return null;
        }
    }
}

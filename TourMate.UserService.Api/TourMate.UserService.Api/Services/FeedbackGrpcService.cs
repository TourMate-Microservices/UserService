using FeedbackServiceGrpc;
using Grpc.Net.Client;
using TourMate.Grpc;
using TourMate.UserService.Repositories.Models;
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
        private readonly ICustomerService _customerService;
        private readonly ILogger<FeedbackGrpcService> _logger;
        private readonly GrpcChannel _channel;
        private readonly FeedbackService.FeedbackServiceClient _client;

        public FeedbackGrpcService(ILogger<FeedbackGrpcService> logger, IConfiguration configuration, ICustomerService customerService)
        {
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
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
                if (response == null)
                {
                    return null;
                }
                var feedbacks = response.Data.Select(f => new Feedback
                {
                    FeedbackId = f.FeedbackId,
                    CustomerId = f.CustomerId,
                    Date = DateTime.Parse(f.CreatedDate),
                    Rating = f.Rating,
                    InvoiceId = f.InvoiceId,
                    Content = f.Content,
                    ServiceId = f.ServiceId
                }).ToList();
                var customerCache = feedbacks.Select(x => x.CustomerId).ToHashSet();
                var customers = await _customerService.GetCustomersFromIds(customerCache);
                for ( var i = 0; i < feedbacks.Count; i++)
                {
                    if (customers.TryGetValue(feedbacks[i].CustomerId, out var customer))
                    {
                        feedbacks[i].CustomerAvatar = customer.Image;
                        feedbacks[i].CustomerName = customer.FullName;
                    }
                    else
                    {
                        _logger.LogWarning("Customer with ID {CustomerId} not found for feedback ID {FeedbackId}", feedbacks[i].CustomerId, feedbacks[i].FeedbackId);
                        feedbacks[i].CustomerAvatar = "";
                        feedbacks[i].CustomerName = "";
                    }
                }
                return new PagedResult<Feedback>
                {
                    data = feedbacks,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling GetFeedbackOfTourGuide gRPC for tourGuideId: {tourGuideId}", tourGuideId);                
            }
            return null;
        }
    }
}

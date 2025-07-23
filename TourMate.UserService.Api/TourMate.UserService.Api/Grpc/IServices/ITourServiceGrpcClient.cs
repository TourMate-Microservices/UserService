using TourMate.Grpc;

namespace TourMate.UserService.Api.Grpc.IServices
{
    public interface ITourServiceGrpcClient
    {
        Task<TourServiceList> GetToursByTourGuideIdAsync(int tourGuideId);
        Task<TourServiceList> GetNumOfTourByTourGuideId(int tourGuideId, int numOfTours);
    }
}

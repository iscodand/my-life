namespace MyLifeApp.Application.Dtos.Responses.Profile.Followers
{
    public class GetTotalFollowersResponse
    {
        public ICollection<GetProfileResponse> Followers { get; set; }
        public int FollowersCount { get; set; }
    }
}

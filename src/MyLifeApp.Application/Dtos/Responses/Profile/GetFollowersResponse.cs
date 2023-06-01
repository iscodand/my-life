namespace MyLifeApp.Application.Dtos.Responses.Profile
{
    public class GetFollowingsResponse : BaseResponse
    {
        public int Total { get; set; }
        public ICollection<GetProfileResponse>? Profiles { get; set; }
    }
}

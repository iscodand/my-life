namespace MyLifeApp.Application.Dtos.Responses.Profile
{
    public class GetProfileResponse : BaseResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public bool IsPrivate { get; set; }
    }
}

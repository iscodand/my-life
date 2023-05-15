namespace Identity.Infrastructure.Data.DTOs.Response
{
    public class RefreshTokenResponse : BaseResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}

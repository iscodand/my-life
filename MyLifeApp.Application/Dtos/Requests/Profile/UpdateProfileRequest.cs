namespace MyLifeApp.Application.Dtos.Requests.Profile
{
    public class UpdateProfileRequest
    {
        public string? Bio { get; set; }
        public string? Location { get; set; }
        public DateTime BirthDate { get; set; }
        public bool IsPrivate { get; set; }
    }
}

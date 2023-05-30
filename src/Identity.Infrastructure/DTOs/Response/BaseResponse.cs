using System.Text.Json.Serialization;

namespace Identity.Infrastructure.DTOs.Response
{
    public class BaseResponse
    {
        public string? Message { get; set; }
        public bool IsSuccess { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<string>? Errors { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public int StatusCode { get; set; }
    }
}

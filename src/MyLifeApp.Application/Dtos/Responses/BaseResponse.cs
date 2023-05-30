using System.Text.Json.Serialization;

namespace MyLifeApp.Application.Dtos.Responses
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

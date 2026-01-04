using System.Text.Json;

namespace Application.DTOs.Error
{
    public class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string? Trace { get; set; } // اختياري: بيظهر فقط في الـ Development

        public override string ToString() => JsonSerializer.Serialize(this);
    }
}

namespace Canteen.Shared.Dtos.Response;

public class ResponseDto
{
    public int Id { get; set; }
    public int StatusCode { get; set; }
    public string? Message { get; set; }
    public bool IsSuccess { get; set; }
    public string? MediaUrl { get; set; }
}
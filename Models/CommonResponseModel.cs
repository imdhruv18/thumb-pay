namespace Thubpay.Models
{
    public class CommonResponseModel
    {
        public bool success { get; set; }
        public string? message { get; set; }
        public object? data { get; set; }
    }

    public class TokenResponseModel
    {
        public int UserId { get; set; }
        public string? RoleName { get; set; }
    }
}

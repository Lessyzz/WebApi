
namespace ForgotPasswordPhoneProject.Dtos
{
    public class VerifyCodeRequest
    {
        public string Number { get; set; }
        public string Code { get; set; }
        public string NewPassword { get; set; }
    }
}

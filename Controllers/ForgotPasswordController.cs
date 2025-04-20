
using System.Security.Cryptography;
using System.Text;
using ForgotPasswordPhoneProject.Dtos;
using ForgotPasswordPhoneProject.Services;
using Microsoft.AspNetCore.Mvc;
using WebApi.Data;
using WebApi.Models;

namespace WebApi.Controllers
{
    
    public class ForgotPasswordController(EfContext context) : Microsoft.AspNetCore.Mvc.Controller
    {
        

        private readonly MockSmsService _smsService = new MockSmsService();

        [HttpPost("/ForgotPassword/request-reset")]
        public IActionResult RequestReset([FromBody] ResetPasswordRequest request)
        {
            var user = context.Users.FirstOrDefault(u => u.Number == request.Number);
            if (user == null)
                return NotFound("Kullanıcı bulunamadı");

            var code = new Random().Next(100000, 999999).ToString();
            user.ResetCode = code;
            user.ResetCodeExpires = DateTime.Now.AddMinutes(5);
            context.SaveChanges();

            _smsService.SendSms(user.Number, $"Şifre sıfırlama kodun: {code}");

            return Ok("Kod gönderildi");
        }

        [HttpPost("/ForgotPassword/verify-code")]
        public IActionResult VerifyCode([FromBody] VerifyCodeRequest request)
        {
            var user = context.Users.FirstOrDefault(u => u.Number == request.Number && u.ResetCode == request.Code);
            if (user == null || user.ResetCodeExpires < DateTime.Now)
                return BadRequest("Kod geçersiz veya süresi dolmuş");

            user.Password = ComputeSha256Hash(request.NewPassword);
            user.ResetCode = null;
            user.ResetCodeExpires = null;
            context.SaveChanges();

            return Ok("Şifre başarıyla güncellendi");
        }
        
        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}


using System;

namespace ForgotPasswordPhoneProject.Services
{
    public class MockSmsService
    {
        public void SendSms(string phoneNumber, string message)
        {
            Console.WriteLine($">>> SMS to {phoneNumber}: {message}");
        }
    }
}

using WebApi.Models;

namespace WebApi.Dto
{
    public class UserDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Roles { get; set; }
        public string Number { get; set; }
    }
}

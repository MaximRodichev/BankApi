using System.ComponentModel.DataAnnotations;

namespace BankApi.Models.DTO
{
    public class UserDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Length(6,15)]
        public string Password { get; set; }
    }
}

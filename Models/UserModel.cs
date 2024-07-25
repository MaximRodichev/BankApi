using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankApi.Models
{
    public class UserModel
    {
        [Key]
        [Required]
        public int UserId { get; set; }
        public string Role { get; set; } = "User";
        public string FirstName {  get;set; }
        public string LastName { get;set; }
        public string Email { get;set; }
        public string Password { get;set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}

using BankApi.Models;
using System.ComponentModel.DataAnnotations;

namespace BankApi.Models
{
    public static class BillSetting {
        public static readonly string BankCode = "2220676";

        public static readonly Dictionary<string, string> Settings = new Dictionary<string, string>()
        {
            { "EUR", "978" },
            { "USD", "840" },
            { "RUP", "000" },
            { "RUB", "643" },
            { "UAH", "980" },
            { "MDL", "498" }
        };
        }
    public class BillModel
    {
    
        public string number { get; set; }
        public string CurrencyAllowed {  get; set; }
        [Required]
        public int idHolder { get; set; }
        public decimal Balance { get; set; } = 10.342M;
        public DateTime DateOpen { get; set; } = DateTime.Now;
        public DateTime? DateClose { get; set; }
        public bool IsActive { get; set; } = true;
        public string? cardLinked { get; set; } = null;
    }
}

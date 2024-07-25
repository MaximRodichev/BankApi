using System.ComponentModel.DataAnnotations;

namespace BankApi.Models.DTO
{
    public class PaymentDTO
    {
        public string Target { get; set; }
        [Required]
        public string CurrencyAmount { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public string SenderBill { get; set; }
        [Required]
        public string ReceiverCard_or_Bill { get; set; }

    }
}

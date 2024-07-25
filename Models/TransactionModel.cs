using System.Runtime.InteropServices;

namespace BankApi.Models
{
    public class TransactionModel
    {
        public Guid id { get; set; } = new Guid();
        public int idSender { get; set; }
        public int idReceiver { get; set; }
        public string? SenderBill { get; set; }
        public string? ReceiverBill { get; set; }
        public string Target { get; set; } = "Giveaway";
        public decimal Amount { get; set; } = 0;
        public DateTime Date { get;set; } = DateTime.Now;

    }
}

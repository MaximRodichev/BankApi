namespace BankApi.Models.DTO
{
    public class CardDTO
    {
        public string? CardNumber { get; set; }
        public CardTypes CardType { get; set; }
        public DateTime dateIssue { get; set; }
        public DateTime dateClose { get; set; }
        public string BillMain { get; set; }
    }
}

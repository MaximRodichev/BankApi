using System.ComponentModel.DataAnnotations;

namespace BankApi.Models
{
    public class CardSettings
    {
        public static readonly string CardBankCode = "910401";
        public static readonly string VisaCode = "883941";
        public static readonly string PieceCode = "000111";
    }
    public enum CardTypes
    {
        _Visa, 
        _Piece,
        _1xBank 
    }
    public class CardModel
    {
        [Required]
        public int idHolder {  get; set; }
        public CardTypes CardType { get; set; }
        public string? CardNumber { get; set; }
        public DateTime dateIssue { get; set; } 
        public DateTime dateClose { get; set; } 
        public short PIN { get; set; } = Convert.ToInt16(new Random().Next(1000, 9999));
        public short CVC { get; set; } = Convert.ToInt16(new Random().Next(100, 999));
        public string? BillMain { get; set; } 
    }
}

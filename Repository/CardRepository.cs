using BankApi.EntityFramework;
using BankApi.Models;
using BankApi.Models.DTO;

namespace BankApi.Repository
{
    public class CardRepository
    {
        public static CardModel OpenCard(CardDTO card, int idHolder)
        {
            string prefix = CardSettings.CardBankCode;
            switch (card.CardType)
            {
                case CardTypes._Visa: prefix = CardSettings.VisaCode; break;
                case CardTypes._1xBank: prefix = CardSettings.CardBankCode; break;
                case CardTypes._Piece: prefix = CardSettings.PieceCode; break;
            }
            CardModel cardModel = new CardModel()
            {
                BillMain = card.BillMain,
                CardNumber = prefix + $"{new Random().Next(1000, 9999)}" + $"{new Random().Next(1000, 9999)}" + $"{new Random().Next(10, 99)}",
                CardType = card.CardType,
                dateIssue = card.dateIssue,
                dateClose = card.dateClose,
                idHolder = idHolder
            };
            return cardModel;
        }
    }
}

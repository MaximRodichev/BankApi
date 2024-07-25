using BankApi.Models;
using BankApi.Models.DTO;

namespace BankApi.Repository
{

    public class BillRepository
    {
        public static BillModel OpenBill(string currencyString, int userId)
        {
            string number = BillSetting.BankCode + BillSetting.Settings[currencyString] + $"{new Random().Next(100000, 1000000 - 1)}";
            var bill = new BillModel()
            {
                idHolder = userId,
                CurrencyAllowed = currencyString,
                number = number
            };
            return bill;
        }
    }
}

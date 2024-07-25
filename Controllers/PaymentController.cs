using BankApi.EntityFramework;
using BankApi.Models;
using BankApi.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net.Security;
using System.Security.Claims;
using System.Transactions;

namespace BankApi.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly CardDbContext _cardDbContext;
        private readonly BillDbContext _billDbContext;
        private readonly TransactionDbContext _transactionDbContext;
        public PaymentController(IConfiguration configuration, CardDbContext cardDbContext, BillDbContext billDbContext, TransactionDbContext transactionDbContext)
        {
            _configuration = configuration;
            _cardDbContext = cardDbContext;
            _billDbContext = billDbContext;
            _transactionDbContext = transactionDbContext;
        }
        private ActionResult billToBill(Models.DTO.PaymentDTO paymentFunction)
        {
            var billSender = _billDbContext.Bills.First(x => x.number == paymentFunction.SenderBill);
            var billReceiver = _billDbContext.Bills.First(x => x.number == paymentFunction.ReceiverCard_or_Bill);
            if (billReceiver.IsActive == false || billSender.IsActive == false)
            {
                return BadRequest("Счет отправителя или получателя не активен");
            }
            if (billSender.Balance < paymentFunction.Amount)
            {
                return BadRequest(paymentFunction);
            }
            billSender.Balance -= paymentFunction.Amount;
            billReceiver.Balance += paymentFunction.Amount;
            Models.TransactionModel transaction = new Models.TransactionModel()
            {
                Amount = paymentFunction.Amount,
                Date = DateTime.Now,
                id = Guid.NewGuid(),
                idSender = billSender.idHolder,
                idReceiver = billReceiver.idHolder,
                ReceiverBill = billReceiver.number,
                SenderBill = billSender.number,
                Target = paymentFunction.Target
            };

            _billDbContext.Update(billReceiver);
            _billDbContext.Update(billSender);
            _transactionDbContext.Add(transaction);
            _billDbContext.SaveChanges();
            _transactionDbContext.SaveChanges();
            return Ok();
        }

        [HttpPost]
        [Route("send")]
        public IActionResult Send(Models.DTO.PaymentDTO payment)
        {
            if (User.FindFirstValue(ClaimTypes.Name) == null)
            {
                return BadRequest("Claims not found");
            }
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));
            //Sender == BILL
            if (payment.ReceiverCard_or_Bill.StartsWith(Models.BillSetting.BankCode))
            {
                var billSender = _billDbContext.Bills.First(x=> x.number == payment.SenderBill);
                var billReceiver = _billDbContext.Bills.First(x => x.number == payment.ReceiverCard_or_Bill);

                if (billSender.CurrencyAllowed == billReceiver.CurrencyAllowed)
                {
                    return billToBill(payment);
                }
                else
                {
                    //Если не совпадает валюта требуется купить нужной валюты и отправить уже куда нужно
                    //USD -> EUR (Мы покупаем EUR и отправляем их)
                    // Курс валюты будет следующий USD * ((USD/RUP)/(EUR/RUP))
                    // Формула: Сумма * ((ВалютаОтправки/ПМРубль)/(ВалютаПолучения/ПМРубль))
                    // Осуществляются следующие платежи:
                    // 1. Payment(billReceiver, billWithThisCurrency, FullAmount)
                    // 2. Payment(billSender, billWithSenderCurrency, AmountInCurrent)
                    BillModel? Bank_bill_Buy = _billDbContext.Bills.FirstOrDefault(x => x.number == BillSetting.BankCode + BillSetting.Settings[billSender.CurrencyAllowed] + "000000");
                    BillModel? Bank_bill_Send = _billDbContext.Bills.FirstOrDefault(x => x.number == BillSetting.BankCode + BillSetting.Settings[billReceiver.CurrencyAllowed] + "000000");
                    if (Bank_bill_Buy == null || Bank_bill_Send == null)
                    {
                        return NoContent();
                    }

                    if (billSender.CurrencyAllowed == payment.CurrencyAmount)
                    {
                        PaymentDTO payment1 = new PaymentDTO()
                        {
                            Target = payment.Target,
                            CurrencyAmount = payment.CurrencyAmount,
                            Amount = payment.Amount,
                            SenderBill = payment.SenderBill,
                            ReceiverCard_or_Bill = Bank_bill_Buy.number
                        };
                        PaymentDTO payment2 = new PaymentDTO()
                        {
                            Target = $"Покупка {billReceiver.CurrencyAllowed}",
                            CurrencyAmount = $"{billReceiver.CurrencyAllowed}",
                            Amount = payment.Amount * ((decimal)KursController.CurrencyKurs[billSender.CurrencyAllowed] / (decimal)KursController.CurrencyKurs[billReceiver.CurrencyAllowed]),
                            SenderBill = Bank_bill_Send.number,
                            ReceiverCard_or_Bill = payment.ReceiverCard_or_Bill
                        };
                        var a = billToBill(payment1);
                        var b = billToBill(payment2);
                        if (b == Ok())
                        {
                            return Ok();
                        }
                        else
                        {
                            return b;
                        }
                        
                    }
                    else
                    {
                        PaymentDTO payment1 = new PaymentDTO()
                        {
                            Target = payment.Target,
                            CurrencyAmount = payment.CurrencyAmount,
                            Amount = payment.Amount / ((decimal)KursController.CurrencyKurs[billSender.CurrencyAllowed] / (decimal)KursController.CurrencyKurs[billReceiver.CurrencyAllowed]),
                            
                            SenderBill = payment.SenderBill,
                            ReceiverCard_or_Bill = Bank_bill_Buy.number
                        };
                        PaymentDTO payment2 = new PaymentDTO()
                        {
                            Target = $"Покупка {billReceiver.CurrencyAllowed}",
                            
                            CurrencyAmount = $"{billSender.CurrencyAllowed}",
                            Amount = payment.Amount,
                            SenderBill = Bank_bill_Send.number,
                            ReceiverCard_or_Bill = payment.ReceiverCard_or_Bill
                        };

                        var a = billToBill(payment1);
                        var b = billToBill(payment2);
                        if (b == Ok())
                        {
                            return Ok();
                        }
                        else
                        {
                            return b;
                        }
                    }



                }
            }
            //Sender == CARD
            else
            {
                var billSender = _billDbContext.Bills.First(x => x.number == payment.SenderBill);
                var cardReceiver = _cardDbContext.Cards.First(x => x.CardNumber == payment.ReceiverCard_or_Bill);

                var billReceiver = _billDbContext.Bills.First(x => x.cardLinked == cardReceiver.CardNumber && x.CurrencyAllowed == billSender.CurrencyAllowed);
                
                if (billReceiver != null)
                {
                    return billToBill( payment);
                }
                else
                {
                    return BadRequest();
                }
            }
            return BadRequest();
        }
    }
}

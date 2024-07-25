using BankApi.EntityFramework;
using BankApi.Models;
using BankApi.Models.DTO;
using BankApi.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BankApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class CardController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly CardDbContext _cardDbContext;
        private readonly BillDbContext _billDbContext;
        public CardController(IConfiguration configuration, CardDbContext cardDbContext, BillDbContext billDbContext)
        {
            _configuration = configuration;
            _cardDbContext = cardDbContext;
            _billDbContext = billDbContext;
        }

        [HttpPost]
        [Route("close")]
        public ActionResult CloseCard(CardDTO card)
        {
            try
            {
                var thisCard = _cardDbContext.Cards.FirstOrDefault(x => x.CardNumber == card.CardNumber);
                if (thisCard == null) return NoContent();
                var checkedBills = _billDbContext.Bills.Where(x => x.cardLinked == thisCard.CardNumber);
                foreach (var bill in checkedBills) {
                    bill.cardLinked = null;
                    _billDbContext.Bills.Update(bill);
                }
                thisCard.dateClose = DateTime.Now;
                _cardDbContext.Update(thisCard);
                _billDbContext.SaveChanges();
                _cardDbContext.SaveChanges();
                return Ok();
            }
            catch (Exception ex) {
                return BadRequest();    
            }
        }

        [HttpPost]
        [Route("open")]
        public ActionResult OpenCard(CardDTO card)
        {
            if (User.FindFirstValue(ClaimTypes.Name) == null)
            {
                return BadRequest("Claims not found");
            }
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));
            if(_cardDbContext.Cards.Where(x=>x.idHolder == userId && x.dateClose > DateTime.Now).Count() > 5)
            {
                return BadRequest("You open 4 cards it's your limit, to open new please close any");
            }
            try
            {
                CardModel result = CardRepository.OpenCard(card, userId);
                var res = _billDbContext.Bills.First(x => x.number == result.BillMain);
                res.cardLinked = result.CardNumber;
                _billDbContext.Bills.Update(res);
                _cardDbContext.Cards.Add(result);
                _cardDbContext.SaveChanges();
                _billDbContext.SaveChanges();
                return Ok();
            }
            catch (Exception ex) { BadRequest($"{ex}"); }
            return NoContent();
        }

        [HttpGet]
        [Route("secureData/{number}")]
        public ActionResult GetSecureData(string number)
        {
            if (User.FindFirstValue(ClaimTypes.Name) == null)
            {
                return BadRequest("Claims not found");
            }
            CardModel data = _cardDbContext.Cards.FirstOrDefault(x => x.CardNumber == number);
            if(data == null)
            {
               return NoContent();
            }
            else
            {
                string response = $"CVC: {data.CVC}\nPIN: {data.PIN}";
                return Ok(response);
            }
        }

        [HttpGet]
        [Route("get")]
        public ActionResult Get()
        {
            if (User.FindFirstValue(ClaimTypes.Name) == null)
            {
                return BadRequest("Claims not found");
            }
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));
            return Ok(_cardDbContext.Cards.Where(x=> x.idHolder == userId && x.dateClose > DateTime.Now).ToList());
        }
        [HttpGet]
        [Route("get/{cardNumber}")]
        public ActionResult Get(string cardNumber)
        {
            if(User.FindFirstValue(ClaimTypes.Name) == null)
            {
                return BadRequest("Claims not found");
            }
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));
            var card = _cardDbContext.Cards.FirstOrDefault(x => x.CardNumber == cardNumber && x.idHolder == userId);
            return card!=null? Ok(card) : BadRequest();
        }
    }
}

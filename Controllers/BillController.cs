using BankApi.EntityFramework;
using BankApi.Models;
using BankApi.Models.DTO;
using BankApi.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BankApi.Controllers
{

    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class BillController : ControllerBase
    {
        private readonly BillDbContext _billDbContext;
        private readonly IConfiguration _configuration;
        public BillController(BillDbContext billDbContext, IConfiguration configuration)
        {
            this._billDbContext = billDbContext;
            this._configuration = configuration; 
        }
        

        [HttpPost]
        [Route("open/{currency}")]
        public ActionResult OpenBill(string currency)
        { 
            if(BillSetting.Settings.ContainsKey(currency))
            {
               
                if (User.FindFirstValue(ClaimTypes.Name) == null)
                {
                    return BadRequest("Not found claims of user");
                } 
                var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));
                if (_billDbContext.Bills.Where(x=>x.idHolder == userId && x.IsActive == true).ToArray().Length > 10)
                {
                    return BadRequest("Your count of bills over 10, please close any to create a new");
                }
                var bill = BillRepository.OpenBill(currency, userId);
                _billDbContext.Bills.Add(bill);
                _billDbContext.SaveChanges();
                return Ok();
            }
            else
            {
                return BadRequest("Not Allowed currency");
            }
        }
        [HttpGet]
        [Route("get/number={number}")]
        public ActionResult GetBill(string number)
        {
            var bill = _billDbContext.Bills.FirstOrDefault(x => x.number == number);
            if (User.FindFirstValue(ClaimTypes.Name) == null)
            {
                return BadRequest("Please restart the app and login again");
            }
            var userIdResponse = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));
            if (bill == null)
            {
                return NotFound("Not found Bill by number");
            }
            else
            {
                if (bill.idHolder == userIdResponse)
                {
                    return Ok(bill);
                }
                else
                {
                    return BadRequest("Bill holder and your id is not compared");
                }
            }
        }
        [HttpGet]
        [Route("get/currency={currency}")]
        public ActionResult GetBillbyCurrency(string currency)
        {
            if (User.FindFirstValue(ClaimTypes.Name) == null)
            {
                return BadRequest("Not found claims of user");
            }
            var userIdResponse = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));
            var bill = _billDbContext.Bills.FirstOrDefault(x => x.CurrencyAllowed == currency && x.idHolder == userIdResponse);
            if (bill == null)
            {
                return NotFound("Not found Bill by number");
            }
            else
            {
                return Ok(bill);
            }
        }
        [HttpGet]
        [Route("get")]
        public ActionResult Get()
        {
            if (User.FindFirstValue(ClaimTypes.Name) == null)
            {
                return BadRequest("Not found claims of user");
            }
            var userIdResponse = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));
            var bill = _billDbContext.Bills.Where(x => x.idHolder == userIdResponse).ToList();
            if (bill == null)
            {
                return NotFound("Not found Bills by holder");
            }
            else
            {
                return Ok(bill);
            }
        }


















        [Authorize(Policy = "AdminOnly")]
        [HttpDelete]
        [Route("admin/close/{number}")]
        public ActionResult CloseBill(string number)
        {
            try
            {
                var bill = _billDbContext.Bills.FirstOrDefault(x => x.number == number);
                if (bill == null)
                {
                    return BadRequest("Not found a bill or this bill isn't yours");
                }
                bill.IsActive = false;
                bill.DateClose = DateTime.Now;
                _billDbContext.Update(bill);
                _billDbContext.SaveChanges();
                return Ok();
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet]
        [Route("admin/get")]
        public ActionResult AdminGets()
        {
            return Ok(_billDbContext.Bills.ToList());
        }
        [Authorize(Policy = "AdminOnly")]
        [HttpGet]
        [Route("admin/get/{userId}")]
        public ActionResult AdminGets(string userId)
        {
            if (User.FindFirstValue(ClaimTypes.Name) == null)
            {
                return BadRequest("Not found claims of user");
            }
            var userIdResponse = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));
            return Ok(_billDbContext.Bills.Where(x=>x.idHolder == userIdResponse));
        }

    }
}

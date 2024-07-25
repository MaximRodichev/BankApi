using BankApi.EntityFramework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Security;
using System.Security.Claims;
using System.Transactions;

namespace BankApi.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly TransactionDbContext _transactionDbContext;
        public TransactionController(IConfiguration configuration, TransactionDbContext transactionDbContext)
        {
            _configuration = configuration;
            _transactionDbContext = transactionDbContext;
        }

        [HttpGet]
        [Route("get")]
        public IActionResult Get()
        {
            if (User.FindFirstValue(ClaimTypes.Name) == null)
            {
                return BadRequest("Claims not found");
            }
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));
            return Ok(_transactionDbContext.Transactions.Where(x => x.idSender == userId || x.idReceiver == userId).ToList());
        }
    }
}

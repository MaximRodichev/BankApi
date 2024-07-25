using Microsoft.AspNetCore.Mvc;

namespace BankApi.Controllers
{ 
    [Route("[controller]")]
    [ApiController]
   
    public class KursController : ControllerBase
    {
        public static Dictionary<string, double> CurrencyKurs = new Dictionary<string, double>()
            {
                {"EUR", 17.45},
                {"USD", 16.35},
                {"RUP", 1.0 },
                {"MDL", 0.965 },
                {"UAH", 0.368 },
                {"RUB", 0.17 }
            };


        [HttpGet]
        [Route("get")]
        public IActionResult GetKurs()
        {
            return Ok(CurrencyKurs);
        }
    }
}

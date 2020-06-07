using Microsoft.AspNetCore.Mvc;

namespace DynamoDbLocalSample.Controllers
{
    [ApiController]
    [Route("/")]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public ContentResult Get()
        {
            return Content("Welcome to DynamoDB");
        }
    }
}
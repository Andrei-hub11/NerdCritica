using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NerdCritica.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {

        [HttpGet("teste")]
        public string Get()
        {
            return "a";
        }
    }
}

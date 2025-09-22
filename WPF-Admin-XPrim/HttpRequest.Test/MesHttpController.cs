using Microsoft.AspNetCore.Mvc;

namespace HttpRequest.Test {
    public record class TestData(string code, string recipe);

    [ApiController]
    [Route("api/[controller]")]
    public class MesHttpController : ControllerBase {
        [HttpGet()]
        public IActionResult Get([FromQuery] string code) {
            return Ok(new { request= new { Result = "OK"} });
        }

        [HttpPost]
        [Route("post")]
        public IActionResult Post([FromQuery] string code, [FromBody]TestData dto) {
            return Ok(new { Code = code, Recipe = dto.recipe });
        }
    }
}
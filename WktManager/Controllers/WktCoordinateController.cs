using Microsoft.AspNetCore.Mvc;
using WktManager.DTOs;
using WktManager.Repositories;


namespace WktManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WktCoordinateController : ControllerBase
    {
        private readonly IWktCoordinateService _service;

        public WktCoordinateController(IWktCoordinateService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _service.GetAll();

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message, data = result.Data });
        }

        [HttpGet("getrange")]
        public IActionResult GetRange(int startIndex, int count)
        {
            var result = _service.GetRange(startIndex, count);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message, data = result.Data });
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var result = _service.GetById(id);

            if (!result.Success)
                return NotFound(new { message = result.Message });

            return Ok(new { message = result.Message, data = result.Data });
        }

      
        [HttpPost]
        public IActionResult Add([FromBody] WktCoordinateDto dto)
        {
            var result = _service.Add(dto);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message, data = result.Data });
        }


        [HttpPost("addrange")]
        public IActionResult AddRange([FromBody] List<WktCoordinateDto> dtos)
        {
            var result = _service.AddRange(dtos);

            if (!result.Success)
                return BadRequest(new { message = result.Message });



            return Ok(new { message = result.Message, data = result.Data });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] WktCoordinateDto dto)
        {
            var result = _service.Update(id, dto);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message, data = result.Data });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _service.Delete(id);

            if (!result.Success)
                return NotFound(new { message = result.Message });

            return Ok(new { message = result.Message, data = result.Data });
        }
    }
}

//trycatch yaz add de
//rsut ları buraya yaz 
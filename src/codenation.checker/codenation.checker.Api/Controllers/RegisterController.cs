using codenation.checker.Api.Context;
using codenation.checker.Api.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace codenation.checker.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly DatabaseContext _context;
        public RegisterController(DatabaseContext context)
        {
            _context = context;
        }


        [HttpGet]
        [Route("/{email}")]
        public async Task<IActionResult> Email([FromQuery] string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                var user = _context.CodenationUsers
                    .AsNoTracking()
                    .FirstOrDefault(x => x.Email.Equals(email));

                if (user != null)
                {
                    await _context.CodenationUsers.AddAsync(new CodenationUser() { Email = email });
                    return Ok("Email saved.");
                }

                return BadRequest("Email already exist");
            }

            return BadRequest();
        }

    }
}
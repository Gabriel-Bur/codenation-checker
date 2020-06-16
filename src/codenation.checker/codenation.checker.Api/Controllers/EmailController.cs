using System.Linq;
using System.Threading.Tasks;
using codenation.checker.Api.Context;
using codenation.checker.Api.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace codenation.checker.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {

        private readonly DatabaseContext _context;
        public EmailController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpGet("{email}")]
        public async Task<IActionResult> Add(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                var user = _context.CodenationUsers
                    .AsNoTracking()
                    .FirstOrDefault(x => x.Email.Equals(email));

                if (user == null)
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
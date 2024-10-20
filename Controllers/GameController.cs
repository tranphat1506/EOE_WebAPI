using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EOE_WebAPI.Models;
using Microsoft.AspNetCore.Mvc.Formatters;
using EOE_WebAPI.Models.ResponsePayload;
using Microsoft.AspNetCore.Authorization;

namespace EOE_WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GameController : ControllerBase
    {
        private readonly EOEDbContext _context;

        public GameController(EOEDbContext context)
        {
            _context = context;
        }

        // GET: api/Game
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetGameResponse>>> GetGames()
        {
            // Lấy danh sách game, bao gồm cả thông tin GameImage từ Media
            var games = await _context.Game
                .Include(g => g.GameImage) // Bao gồm Media liên kết với GameImageId
                .Select(g => new
                Game{
                    GameId = g.GameId,
                    Game_Name = g.Game_Name,
                    Game_Desc = g.Game_Desc,
                    Game_Image = g.Game_Image,
                    GameImageId = g.GameImageId,
                    GameImage = g.GameImage
                })
                .ToListAsync();
            return Ok(games); // Trả về danh sách game
        }
    }
}

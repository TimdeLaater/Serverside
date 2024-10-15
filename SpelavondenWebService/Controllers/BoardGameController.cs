using Application.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SpelavondenWebService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BoardGameController : ControllerBase
    {
        private readonly IBoardGameRepository _boardGameRepository;

        public BoardGameController(IBoardGameRepository boardGameRepository)
        {
            _boardGameRepository = boardGameRepository;
        }

        // GET: api/boardgame
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BoardGame>>> GetAllBoardGames()
        {
            var boardGames = await _boardGameRepository.GetAllAsync();
            if (boardGames == null)
            {
                return NotFound("No board games found.");
            }
            return Ok(boardGames);
        }

        // GET: api/boardgame/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<BoardGame>> GetBoardGameById(int id)
        {
            var boardGame = await _boardGameRepository.GetByIdAsync(id);
            if (boardGame == null)
            {
                return NotFound($"Board game with ID {id} not found.");
            }
            return Ok(boardGame);
        }

    }
}

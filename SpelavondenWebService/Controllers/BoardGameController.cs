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

        // POST: api/boardgame
        [HttpPost]
        public async Task<ActionResult<BoardGame>> CreateBoardGame(BoardGame boardGame)
        {
            if (boardGame == null)
            {
                return BadRequest("Board game cannot be null.");
            }

            var createdBoardGame = await _boardGameRepository.AddAsync(boardGame);
            return CreatedAtAction(nameof(GetBoardGameById), new { id = createdBoardGame.BoardGameId }, createdBoardGame);
        }

        // PUT: api/boardgame/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateBoardGame(int id, BoardGame boardGame)
        {
            if (id != boardGame.BoardGameId)
            {
                return BadRequest("Board game ID mismatch.");
            }

            var existingBoardGame = await _boardGameRepository.GetByIdAsync(id);
            if (existingBoardGame == null)
            {
                return NotFound($"Board game with ID {id} not found.");
            }

            await _boardGameRepository.UpdateAsync(boardGame);
            return NoContent();
        }

        // DELETE: api/boardgame/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBoardGame(int id)
        {
            var boardGame = await _boardGameRepository.GetByIdAsync(id);
            if (boardGame == null)
            {
                return NotFound($"Board game with ID {id} not found.");
            }

            await _boardGameRepository.DeleteAsync(boardGame);
            return NoContent();
        }
    }
}

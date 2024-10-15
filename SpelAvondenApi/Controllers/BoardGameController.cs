using Application.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpelAvondenApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoardGameController : ControllerBase
    {
        private readonly IBoardGameRepository _boardGameRepository;

        public BoardGameController(IBoardGameRepository boardGameRepository)
        {
            _boardGameRepository = boardGameRepository;
        }

        // GET: api/BoardGame
        [HttpGet(Name = "GetBoardGames")]
        public async Task<ActionResult<IEnumerable<BoardGame>>> Get()
        {
            var boardGames = await _boardGameRepository.GetAllAsync();
            return Ok(boardGames); // Return a 200 OK response with the list of board games
        }

        // GET: api/BoardGame/{id}
        [HttpGet("{id}", Name = "GetBoardGameById")]
        public async Task<ActionResult<BoardGame>> GetById(int id)
        {
            var boardGame = await _boardGameRepository.GetByIdAsync(id);
            if (boardGame == null)
            {
                return NotFound(); // Return a 404 if the board game is not found
            }
            return Ok(boardGame); // Return the specific board game with a 200 OK
        }
    }
}

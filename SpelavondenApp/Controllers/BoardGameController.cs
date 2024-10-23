using Application.Interfaces;
using Domain.Models;
using GraphQL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpelavondenApp.Models;

namespace SpelavondenApp.Controllers
{
    public class BoardGameController : BaseController
    {
        private readonly IBoardGameRepository _boardGameRepository;

        public BoardGameController(IBoardGameRepository boardGameRepository)
        {
            _boardGameRepository = boardGameRepository;
        }
        // GET: HomeController1
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var boardGames = await _boardGameRepository.GetAllAsync();
            return View(boardGames);
        }

        // GET: HomeController1/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HomeController1/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BoardGameViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var boardGame = new BoardGame
                    {
                        Name = model.Name,
                        Description = model.Description,
                        Genre = model.Genre,
                        Is18Plus = model.Is18Plus,
                        GameType = model.GameType
                    };

                    // Handle the image upload
                    if (model.ImageFile != null)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await model.ImageFile.CopyToAsync(memoryStream);
                            boardGame.Image = memoryStream.ToArray();  // Save the image as byte[]
                        }
                    }

                    // Save to repository
                    await _boardGameRepository.AddAsync(boardGame);

                    return RedirectToAction("Index");
                }

                return View(model);
            }
            catch
            {
                return View();
            }
        }


        // POST: HomeController1/Delete/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var BoardGame = await _boardGameRepository.GetByIdAsync(id);
            if (BoardGame != null)
            {
                 await _boardGameRepository.DeleteAsync(BoardGame);
                return RedirectToAction("Index");
            }
            return NotFound();
        }
    }
}

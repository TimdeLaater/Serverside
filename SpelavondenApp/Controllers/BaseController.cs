using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SpelavondenApp.Controllers
{
    public class BaseController : Controller
    {
        // Reusable method to get the PersonId from claims
        protected int? GetPersonIdFromClaims()
        {
            var personIdClaim = User.FindFirst("PersonId");
            if (personIdClaim != null && int.TryParse(personIdClaim.Value, out var personId))
            {
                return personId;
            }
            return null;
        }
    }
}
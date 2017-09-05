using System.Threading.Tasks;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using Inshapardaz.Identity.Quickstart;
using Inshapardaz.Identity.Quickstart.Home;
using Microsoft.AspNetCore.Mvc;

namespace Inshapardaz.Identity.Controllers
{
    [SecurityHeaders]
    public class HomeController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;

        public HomeController(IIdentityServerInteractionService interaction)
        {
            _interaction = interaction;
        }

        public IActionResult Index()
        {
            if (User.IsAuthenticated())
            {
                return View();
            }

            return RedirectToAction("Login", "Account");
        }

        public async Task<IActionResult> Error(string errorId)
        {
            var vm = new ErrorViewModel();

            // retrieve error details from identityserver
            var message = await _interaction.GetErrorContextAsync(errorId);
            if (message != null)
            {
                vm.Error = message;
            }

            return View("Error", vm);
        }
    }
}

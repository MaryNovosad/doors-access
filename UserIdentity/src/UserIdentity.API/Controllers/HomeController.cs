using IdentityServer4;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserIdentity.API.ViewModels;

namespace UserIdentity.API.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly TestUserStore _users;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IEventService _events;

        public HomeController(
            IIdentityServerInteractionService interaction,
            IEventService events,
            TestUserStore users)
        {
            _users = users;
            _interaction = interaction;
            _events = events;
        }

        [HttpGet]
        public async Task<IActionResult> Login(string? returnUrl)
        {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);

            var model = new LoginInputModel
            {
                ReturnUrl = returnUrl,
                Username = context?.LoginHint
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model)
        {
            var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);

            if (ModelState.IsValid)
            {
                if (_users.ValidateCredentials(model.Username, model.Password))
                {
                    var user = _users.FindByUsername(model.Username);
                    await _events.RaiseAsync(new UserLoginSuccessEvent(user.Username, user.SubjectId, user.Username, clientId: context?.Client.ClientId));

                    var identityServerUser = new IdentityServerUser(user.SubjectId)
                    {
                        DisplayName = user.Username
                    };

                    await HttpContext.SignInAsync(identityServerUser, null);

                    if (string.IsNullOrWhiteSpace(model.ReturnUrl))
                    {
                        return View("LoggedIn", model.Username);
                    }

                    if (context != null)
                    {
                        return Redirect(model.ReturnUrl);
                    }

                    if (Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }

                    return BadRequest();
                }

                await _events.RaiseAsync(new UserLoginFailureEvent(model.Username, "invalid credentials", clientId: context?.Client.ClientId));
                ModelState.AddModelError(string.Empty, "Invalid username or password");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            var model = await BuildLogoutViewModelAsync(logoutId);

            if (!model.ShowLogoutPrompt)
            {
                return await Logout(model);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutInputModel model)
        {
            var viewModel = await BuildLoggedOutViewModelAsync(model.LogoutId);

            if (User.Identity?.IsAuthenticated == true)
            {
                await HttpContext.SignOutAsync();
                await _events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));
            }

            return View("LoggedOut", viewModel);
        }

        private async Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId)
        {
            var model = new LogoutViewModel { LogoutId = logoutId, ShowLogoutPrompt = true };

            if (User.Identity?.IsAuthenticated != true)
            {
                model.ShowLogoutPrompt = false;
                return model;
            }

            var context = await _interaction.GetLogoutContextAsync(logoutId);
            if (context?.ShowSignoutPrompt == false)
            {
                model.ShowLogoutPrompt = false;
                return model;
            }

            return model;
        }

        private async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId)
        {
            var logout = await _interaction.GetLogoutContextAsync(logoutId);

            var model = new LoggedOutViewModel
            {
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                ClientName = string.IsNullOrWhiteSpace(logout?.ClientName) ? logout?.ClientId : logout.ClientName,
                LogoutId = logoutId
            };

            return model;
        }
    }
}

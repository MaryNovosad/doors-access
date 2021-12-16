namespace UserIdentity.API.ViewModels
{
    public class LoggedOutViewModel
    {
        public string? PostLogoutRedirectUri { get; set; }
        public string? ClientName { get; set; }

        public string? LogoutId { get; set; }
    }
}
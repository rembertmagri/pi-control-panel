namespace PiControlPanel.Domain.Models.Authentication
{
    using System.Collections.Generic;

    public class LoginResponse
    {
        public string Username { get; set; }

        public string JsonWebToken { get; set; }

        public IList<string> Roles { get; set; }
    }
}

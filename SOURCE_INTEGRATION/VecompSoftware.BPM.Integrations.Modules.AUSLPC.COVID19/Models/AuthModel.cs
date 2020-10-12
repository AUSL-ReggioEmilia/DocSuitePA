namespace VecompSoftware.BPM.Integrations.Modules.AUSLPC.COVID19.Models
{
    public class AuthModel
    {
        public AuthModel(string username, string password)
        {
            Username = username;
            Password = password;
        }
        public string Username { get; private set; }
        public string Password { get; private set; }
    }
}

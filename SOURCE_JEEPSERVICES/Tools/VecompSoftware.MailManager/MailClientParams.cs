using System;
namespace VecompSoftware.MailManager
{
  public class MailClientParams
  {
    public int MaxMailsForSession { get; set; }
    public bool DebugModeEnabled { get; set; }
    public bool DeleteMailFromServer { get; set; }
    public string DropFolder { get; set; }

    public string IncomingServer { get; set; }
    public int Port { get; set; }
    public bool UseSsl { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }

    public MailClientParams()
    {
      MaxMailsForSession = 1;
      DebugModeEnabled = true;
      DeleteMailFromServer = false;
      DropFolder = String.Empty;

      IncomingServer = String.Empty;
      Port = 0;
      UseSsl = false;
      Username = String.Empty;
      Password = String.Empty;
    }
  }
}

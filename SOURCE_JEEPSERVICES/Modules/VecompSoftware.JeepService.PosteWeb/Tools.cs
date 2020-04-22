using VecompSoftware.Services.Logging;

namespace VecompSoftware.JeepService.PosteWeb
{
    public static class Tools
    {
        #region [ Properties ]

        public static string ServiceUser
        {
            get { return "POLService"; }
        }

        public static string ModuleName
        {
            get { return "PosteWeb"; }
        }

        #endregion
        
        #region [ Methods ]

        /// <summary> Divide il nome compatibilmente con le richieste del servizio. </summary>
        public static string[] SplitName(string name)
        {
            const int complementoNominativo = 44;
            const int ragioneSociale = 44;

            string fullNominativo = name.Trim();
            if (fullNominativo.Length > complementoNominativo + ragioneSociale)
            {
                FileLogger.Warn(ModuleName,
                    string.Format("Nominativo [{0}] maggiore di {1} caratteri.", fullNominativo,
                        complementoNominativo + ragioneSociale));
            }

            // Il primo nominativo stà in 44 caratteri
            string[] tor;
            if (fullNominativo.Length > ragioneSociale)
            {
                tor = new string[2];
                tor[0] = fullNominativo.Substring(0, ragioneSociale);
                // se eccede dev'essere spostato nella seconda parte da 44 caratteri (NON 50 COME DICE LA DOCUMENTAZIONE UFFICIALE)
                tor[1] = fullNominativo.Substring(ragioneSociale, fullNominativo.Length < ragioneSociale + complementoNominativo ? fullNominativo.Length - ragioneSociale : complementoNominativo);
            }
            else
            {
                tor = new string[1];
                tor[0] = fullNominativo;
            }
            return tor;
        }

        #endregion

    }
}

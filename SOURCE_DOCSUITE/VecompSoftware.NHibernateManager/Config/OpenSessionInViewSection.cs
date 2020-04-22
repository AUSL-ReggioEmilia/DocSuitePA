using System.Configuration;

namespace VecompSoftware.NHibernateManager.Config
{
    /// <summary>
    /// Encapsulates a section of Web/App.config
    /// to declare which session factories are to be created.
    /// Kudos go out to 
    /// http://msdn2.microsoft.com/en-us/library/
    ///    system.configuration.configurationcollectionattribute.aspx
    /// for this technique - it was by far the best overview of the subject.
    /// </summary>
    /// <remarks>http://www.codeproject.com/KB/aspnet/NHibernateMultipleDBs.aspx</remarks>
    public class OpenSessionInViewSection : ConfigurationSection
    {
        [ConfigurationProperty("sessionFactories", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(SessionFactoriesCollection), AddItemName = "sessionFactory", ClearItemsName = "clearFactories")]
        public SessionFactoriesCollection SessionFactories
        {
            get { return (SessionFactoriesCollection)base["sessionFactories"]; }
        }

        public SessionFactoryElement DefaultSessionFactory
        {
            get { return SessionFactories[SessionFactories.DefaultSessionFactory]; }
        }

    }
}

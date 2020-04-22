using NHibernate.Cfg;
using VecompSoftware.NHibernateManager.Config;

namespace VecompSoftware.NHibernateManager.SessionFactory
{
    public interface INHibernateConfiguration
    {
        Configuration GetConfigurationFor(SessionFactoryElement factoryName);

        /// <summary>
        /// Aggiunti per verifica mapping in UtltCheckDBMapping
        /// </summary>
        Configuration GetSingleConfigurationFor(SessionFactoryElement factoryName);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using BiblosDS.Library.Common.DB;
using System.Reflection;
using System.IO;
using BiblosDS.Library.Common.Objects;

namespace BiblosDS.Library.Common.Services
{
    public abstract class ServiceBase
    {        
        /// <summary>
        /// Db Provider to access the database
        /// </summary>
        /// <remarks>
        /// Istance new provider to provide
        /// access to new db like oracle
        /// </remarks>
        protected static EntityProvider DbProvider
        {
            get
            {
                return new EntityProvider();
            }
        }

        /// <summary>
        /// Db Admin function Provider to access the database
        /// </summary>
        /// <remarks>
        /// Istance new provider to provide
        /// access to new db like oracle
        /// </remarks>
        internal static EntityAdminProvider DbAdminProvider
        {
            get
            {
                return new EntityAdminProvider();
            }
        }


        ///// <summary>
        ///// Path di transito in cui salvare i file 
        ///// prima dell'upload sul server.
        ///// </summary>
        //internal static string TransitoLocalPath
        //{
        //    get
        //    {
        //        if (ConfigurationManager.AppSettings["TransitoLocalPath"] == null)
        //            throw new Exception("Nessuna chiave specificata per \"TransitoLocalPath\".");                
        //        else
        //            return ConfigurationManager.AppSettings["TransitoLocalPath"].ToString();
        //    }
        //}       
    }
}

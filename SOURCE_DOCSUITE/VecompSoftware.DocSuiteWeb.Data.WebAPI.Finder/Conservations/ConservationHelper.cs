using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Conservations;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Conservations
{
    public static class ConservationHelper
    {
        public static IReadOnlyDictionary<ConservationStatus, string> StatusDescription = new Dictionary<ConservationStatus, string>
        {
            {
                ConservationStatus.Ready, "Inviato e pronto per essere conservato"
            },
            {
                ConservationStatus.Conservated, "Conservato"
            },
            {
                ConservationStatus.InProgress, "Conservazione in corso"
            },
            {
                ConservationStatus.Unconservable, "Non conservabile"
            },
            {
                ConservationStatus.Error, "In errore"
            },
            {
                ConservationStatus.Discarded, "Conservato"
            }
        };

        public static IReadOnlyDictionary<ConservationStatus, string> StatusSmallIcon = new Dictionary<ConservationStatus, string>
        {
            {
                ConservationStatus.Conservated, "../App_Themes/DocSuite2008/imgset16/conservated16.png"
            },
            {
                ConservationStatus.InProgress, "../App_Themes/DocSuite2008/imgset16/inprogress16.png"
            },
            {
                ConservationStatus.Error, "../App_Themes/DocSuite2008/imgset16/error16.png"
            },
            {
                ConservationStatus.Unconservable, "../App_Themes/DocSuite2008/imgset16/unconservable16.png"
            },
            {
                ConservationStatus.Ready, "../App_Themes/DocSuite2008/imgset16/ready16.png"
            },
            {
                ConservationStatus.Discarded, "../App_Themes/DocSuite2008/imgset16/conservated16.png"
            }
        };

        public static IReadOnlyDictionary<ConservationStatus, string> StatusBigIcon = new Dictionary<ConservationStatus, string>
        {
            {
                ConservationStatus.Conservated, "../App_Themes/DocSuite2008/imgset32/conservated32.png"
            },
            {
                ConservationStatus.InProgress, "../App_Themes/DocSuite2008/imgset32/inprogress32.png"
            },
            {
                ConservationStatus.Error, "../App_Themes/DocSuite2008/imgset32/error32.png"
            },
            {
                ConservationStatus.Unconservable, "../App_Themes/DocSuite2008/imgset32/unconservable32.png"
            },
            {
                ConservationStatus.Ready, "../App_Themes/DocSuite2008/imgset32/ready32.png"
            },
            {
                ConservationStatus.Discarded, "../App_Themes/DocSuite2008/imgset32/conservated32.png"
            }
        };
    }
}

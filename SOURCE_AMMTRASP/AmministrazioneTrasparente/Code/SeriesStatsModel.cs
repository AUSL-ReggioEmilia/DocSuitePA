using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AmministrazioneTrasparente.Code
{
    public class SeriesStatsModel
    {

        public SeriesStatsModel(string serie, int numeroVisite)
        {
            _serie = serie;
            _numeroVisite = numeroVisite;
        }

        private string _serie;

        public string Serie
        {
            get { return _serie; }
        }

        private int _numeroVisite;

        public int NumeroVisite
        {
            get { return _numeroVisite; }
        }


    }
}
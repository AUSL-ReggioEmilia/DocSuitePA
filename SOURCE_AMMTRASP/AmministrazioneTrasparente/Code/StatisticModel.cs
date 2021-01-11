using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AmministrazioneTrasparente.Code
{
    public class StatisticModel
    {

        public StatisticModel(string anno, string visitatori, string visitatoriunici)
        {
            _anno = anno;
            _visitatori = visitatori;
            _visitatoriUnici = visitatoriunici;
        }

        private string _anno;
        public string Anno
        {
            get
            {
                return _anno;
            }
        }

        private string _visitatori;
        public string Visitatori
        {
            get
            {
                return _visitatori;
            }
        }

        private string _visitatoriUnici;
        public string VisitatoriUnici
        {
            get
            {
                return _visitatoriUnici;
            }
        }
    }
}
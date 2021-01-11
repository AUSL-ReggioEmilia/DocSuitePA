using System;
using System.Web;
using VecompSoftware.DocSuiteWeb.DTO.WSSeries;
using VecompSoftware.Helpers;

namespace AmministrazioneTrasparente
{
    public class BaseSeriesPage : BasePage
    {
        #region [ Fields ]

        private DocumentSeriesWSO _seriesWso;
        private ResultArchiveAttributeWSO _structure;

        #endregion

        #region [ Properties ]

        public int IdSeries
        {
            get
            {
                if (ViewState["IdSeries"] == null)
                {
                    if (Request.QueryString["idSeries"] != null)
                    {
                        ViewState["IdSeries"] = int.Parse(Request.QueryString["idSeries"]);
                    }
                }
                if (ViewState["IdSeries"] == null)
                {
                    throw new Exception("Nessuna serie documentale richiesta");
                }
                return (int)ViewState["IdSeries"];
            }
            set { ViewState["IdSeries"] = value; }
        }

        public int? IdSubSection
        {
            get
            {
                if (ViewState["IdSubSection"] == null)
                {
                    int value;
                    if (int.TryParse(HttpContext.Current.Request.QueryString["IdSubSection"], out value))
                    {
                        ViewState["IdSubSection"] = value;
                    }
                }

                return (int?)ViewState["IdSubSection"];
            }
            set { ViewState["IdSubSection"] = value; }
        }

        protected DocumentSeriesWSO SeriesWso
        {
            get
            {
                if (Cache[string.Concat("SeriesWso_", IdSeries)] == null)
                {
                    var temp = MyMaster.Client.GetDocumentSeries(IdSeries, true, ArchiveRestriction);
                    Cache.Insert(string.Concat("SeriesWso_", IdSeries), SerializationHelper.SerializeFromString<DocumentSeriesWSO>(temp), null, DateTime.Now.AddMinutes(CacheExpiration), TimeSpan.Zero);
                    MyMaster.Client.Close();
                }
                return (DocumentSeriesWSO)Cache[string.Concat("SeriesWso_", IdSeries)];
            }
        }

        private string SerializedStructure
        {
            get
            {
                if (Cache[string.Concat("SerializedStructure_", IdSeries)] == null)
                {
                    Cache.Insert(string.Concat("SerializedStructure_", IdSeries), MyMaster.Client.GetDynamicData(IdSeries), null, DateTime.Now.AddMinutes(CacheExpiration), TimeSpan.Zero);
                    MyMaster.Client.Close();
                }
                return (string)Cache[string.Concat("SerializedStructure_", IdSeries)];
            }
        }

        protected ResultArchiveAttributeWSO Structure
        {
            get
            {
                if (_structure == null)
                {
                    _structure = SerializationHelper.SerializeFromString<ResultArchiveAttributeWSO>(SerializedStructure);
                }
                return _structure;
            }
        }

        #endregion

        #region [ Events ]

        protected override void OnLoad(EventArgs e)
        {
            MyMaster.SelectedFamily = MyMaster.GetFamilyBySeries(SeriesWso.Id);
            base.OnLoad(e);
        }

        #endregion

    }
}
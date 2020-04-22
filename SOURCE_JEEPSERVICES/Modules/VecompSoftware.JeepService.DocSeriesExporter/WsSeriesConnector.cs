using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.DTO.WSSeries;
using VecompSoftware.JeepService.DocSeriesExporter.WsSeriesService;
using VecompSoftware.Helpers;

namespace VecompSoftware.JeepService.DocSeriesExporter
{
    public class WsSeriesConnector
    {
        #region Fields

        private string _wsUrl;
        private WSSeriesClient _client;
        private const string CONFIGURATION_NAME = "VecompSoftware.DocSuiteWeb.Services.WSSeries";
        private string _address;
        private string _impersonatingUser;

        #endregion

        #region Properties

        public string WsSeriesUrl
        {
            get { return _wsUrl; }
            set { _wsUrl = value; }
        }

        protected WSSeriesClient Client
        {
            get { return _client; }
        }

        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }

        public string ImpersonatingUser
        {
            get { return _impersonatingUser; }
            set { _impersonatingUser = value; }
        }
        #endregion

        #region Properties

        public WsSeriesConnector(string address)
        {
            Address = address;
            this._client = CreateClient();
        }
        #endregion

        #region Methods

        public bool IsAlive()
        {
            return this.Client.IsAlive();
        }

        public IList<DocumentSeriesFamilyWSO> GetFamilyWsos()
        {
            var families = this.Client.GetFamilies(true, true, true);
            var deserializeFamilies = SerializationHelper.SerializeFromString<ResultDocumentSeriesFamilyWSO>(families);
            return deserializeFamilies.DocumentSeriesFamilies;
        }

        public IList<ArchiveAttributeWSO> GetDynamicAttributeWsos(int idSeries)
        {
            var archiveAttributes = this.Client.GetDynamicData(idSeries);
            var deserializeArchiveAttributes =
                SerializationHelper.SerializeFromString<ResultArchiveAttributeWSO>(archiveAttributes);
            return deserializeArchiveAttributes.ArchiveAttributes;
        }

        public DocumentSeriesItemWSO GetFullItemWso(int idDocumentSeriesItem)
        {
            var response = this.Client.GetDocumentSeriesItem(idDocumentSeriesItem, true, true, false, false);
            return SerializationHelper.SerializeFromString<DocumentSeriesItemWSO>(response);
        }

        public IList<DocumentSeriesItemWSO> GetSeriesWsosById(int idSeries)
        {
            var countingFinder = GetCountingFinderWso(idSeries);
            var serializeFinder = SerializationHelper.SerializeToString(countingFinder);
            var countSeriesItem = this.Client.SearchCount(serializeFinder);

            var response = new List<DocumentSeriesItemWSO>();
            var seriesItemFinder = GetSeriesItemFinderWso(idSeries);
            while (response.Count < countSeriesItem)
            {
                var serializeItemFinder = SerializationHelper.SerializeToString(seriesItemFinder);
                var finderResponse = this.Client.Search(serializeItemFinder, true);
                var deserializedResponse =
                    SerializationHelper.SerializeFromString<DocumentSeriesItemResultWSO>(finderResponse);

                response.AddRange(deserializedResponse.DocumentSeriesItems);
                seriesItemFinder.Skip += seriesItemFinder.Take;
            }
            return response;
        }

        private DocumentSeriesItemFinderWSO GetCountingFinderWso(int idSeries)
        {
            var finder = new DocumentSeriesItemFinderWSO();

            finder.IdDocumentSeries = idSeries;
            finder.IsPublished = true;
            finder.IsRetired = false;
            finder.ImpersonatingUser = ImpersonatingUser;
            finder.EnablePaging = false;
            finder.IncludeSubsections = true;

            return finder;
        }

        private DocumentSeriesItemFinderWSO GetSeriesItemFinderWso(int idSeries)
        {
            var finder = new DocumentSeriesItemFinderWSO();

            finder.IdDocumentSeries = idSeries;
            finder.IsPublished = true;
            finder.IsRetired = false;
            finder.ImpersonatingUser = ImpersonatingUser;
            finder.EnablePaging = false;
            finder.EnablePaging = true;
            finder.IncludeSubsections = true;
            finder.Skip = 0;
            finder.Take = 50;

            return finder;
        }

        private WSSeriesClient CreateClient()
        {
            return new WSSeriesClient(CONFIGURATION_NAME, Address);
        }
        #endregion
    }
}

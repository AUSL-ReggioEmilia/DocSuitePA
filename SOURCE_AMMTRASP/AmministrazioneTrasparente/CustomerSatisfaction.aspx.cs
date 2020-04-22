using AmministrazioneTrasparente.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Services;
using VecompSoftware.DocSuiteWeb.DTO.WSSeries;
using VecompSoftware.Helpers;

namespace AmministrazioneTrasparente
{
    public partial class CustomerSatisfaction : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        [WebMethod]
        public static void SendXml(string json)
        {
            DocumentSeriesItemWSO documentSeries = new DocumentSeriesItemWSO();
            documentSeries.DynamicData = JsonConvert.DeserializeObject<List<AttributeWSO>>(json);
            HandleXml insertXml = new HandleXml();
            insertXml.InsertXml(documentSeries);
        }
    }
    public class HandleXml : BasePage
    {
        private readonly ParameterService _parameterService = new ParameterService();
        public void InsertXml(DocumentSeriesItemWSO keyValue)
        {
            if (_parameterService.GetBoolean("CustomerSatisfactionEnabled"))
                keyValue.IdDocumentSeries = _parameterService.GetInteger("CustomerSatisfactionSeriedId");
            var xml = SerializationHelper.SerializeToString(keyValue);
            MyMaster.Client.Insert(xml);
        }
    }
}
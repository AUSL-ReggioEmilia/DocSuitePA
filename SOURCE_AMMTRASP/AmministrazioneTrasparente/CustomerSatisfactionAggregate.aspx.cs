using AmministrazioneTrasparente.Extensions;
using AmministrazioneTrasparente.Models;
using AmministrazioneTrasparente.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Telerik.Web.UI;
using Telerik.Web.UI.Calendar;
using VecompSoftware.DocSuiteWeb.DTO.WSSeries;
using VecompSoftware.Helpers;

namespace AmministrazioneTrasparente
{
    public partial class CustomerSatisfactionAggregate : BasePage
    {
        #region [ Fields ]
        private readonly ParameterService _parameterService = new ParameterService();
        #endregion

        #region [ Events ]
        protected void Page_Load(object sender, EventArgs e)
        {
            rdpReportDate.MaxDate = DateTime.UtcNow;
        }

        protected void rdpReportDate_SelectedDateChanged(object sender, SelectedDateChangedEventArgs e)
        {
            string path = @"app_data\customer_report";

            string month = rdpReportDate.SelectedDate.Value.AddMonths(-1).Month.ToString();
            string year = rdpReportDate.SelectedDate.Value.Month == 1
                ? rdpReportDate.SelectedDate.Value.AddYears(-1).Year.ToString()
                : rdpReportDate.SelectedDate.Value.Year.ToString();

            string reportFileName = $"{year}{month}.json";

            if (!Directory.Exists(Server.MapPath(path)))
            {
                Directory.CreateDirectory(Server.MapPath(path));
            }
            if (!File.Exists(Path.Combine(Server.MapPath(path), reportFileName)))
            {
                List<ReportModel> report = GenerateReport(rdpReportDate.SelectedDate.Value);
                using (StreamWriter file = File.CreateText(Path.Combine(Server.MapPath(path), reportFileName)))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, report);
                }
            }
            //TODO: Display report values
            string getReport = File.ReadAllText(Path.Combine(Server.MapPath(path), reportFileName));
            List<ReportModel> json = JsonConvert.DeserializeObject<List<ReportModel>>(getReport);

            reportGrid.DataSource = json;
            reportGrid.DataBind();
        }

        protected void reportGrid_DetailTableDataBind(object sender, GridDetailTableDataBindEventArgs e)
        {
            GridDataItem gridDataItem = e.DetailTableView.ParentItem;
            switch (e.DetailTableView.Name)
            {
                case "Responses":
                    {
                        string question = gridDataItem.GetDataKeyValue("Question").ToString();
                        e.DetailTableView.DataSource = LoadResponses(question);
                        break;
                    }
            }
        }
        #endregion

        #region [ Methods ]
        private List<ResponseModel> LoadResponses(string question)
        {
            string month = rdpReportDate.SelectedDate.Value.AddMonths(-1).Month.ToString();
            string year = rdpReportDate.SelectedDate.Value.Month == 1
                ? rdpReportDate.SelectedDate.Value.AddYears(-1).Year.ToString()
                : rdpReportDate.SelectedDate.Value.Year.ToString();

            string path = @"app_data\customer_report";
            string reportFileName = $"{year}{month}.json";
            string getReport = File.ReadAllText(Path.Combine(Server.MapPath(path), reportFileName));
            List<ReportModel> json = JsonConvert.DeserializeObject<List<ReportModel>>(getReport);
            return json.FirstOrDefault(X => X.Question == question).Responses;
        }

        private List<ReportModel> GenerateReport(DateTime reportDate)
        {
            int idDocumentSeriesItem = _parameterService.GetInteger("CustomerSatisfactionSeriedId");
            string dynamicDataXml = MyMaster.Client.GetDocumentSeriesItem(idDocumentSeriesItem, false, false, false, false, false);
            DocumentSeriesItemWSO documentSeriesItemWSO
                = SerializationHelper.SerializeFromString<DocumentSeriesItemWSO>(dynamicDataXml);

            int year = reportDate.Month == 1
               ? reportDate.AddYears(-1).Year
               : reportDate.Year;
            
            List<AttributeWSO> attributeWSOs = new List<AttributeWSO>();
            foreach (AttributeWSO attributeWSO in documentSeriesItemWSO.DynamicData)
            {
                attributeWSOs.Add(attributeWSO);
            }

            List<ReportModel> reports = new List<ReportModel>();
            IEnumerable<IGrouping<string, AttributeWSO>> rep = attributeWSOs.GroupBy(x => x.Key);
            Dictionary<string, IOrderedEnumerable<KeyValuePair<string, int>>> keyValuePairs = new Dictionary<string, IOrderedEnumerable<KeyValuePair<string, int>>>();
            foreach (IGrouping<string, AttributeWSO> item in rep)
            {
                string question = item.Key;
                IOrderedEnumerable<KeyValuePair<string, int>> answers =
                item.GroupBy(x => x.Value)
                    .ToDictionary(y => y.Key, y => y.Count())
                   .OrderByDescending(z => z.Value);
                keyValuePairs.Add(question, answers);
            }
            foreach (KeyValuePair<string, IOrderedEnumerable<KeyValuePair<string, int>>> question in keyValuePairs)
            {
                List<ResponseModel> responseModels = new List<ResponseModel>();
                foreach (KeyValuePair<string, int> answer in question.Value)
                {
                    string response = answer.Key;
                    switch (question.Key)
                    {
                        case "conoscenzaDiQuestaDelSito":
                            {
                                response = Enum.Parse(typeof(ConoscenzaDiQuestaDelSito), answer.Key).DescriptionAttr();
                                break;
                            }
                        case "frequenzaConsulta":
                            {
                                response = Enum.Parse(typeof(FrequenzaConsulta), answer.Key).DescriptionAttr();
                                break;
                            }
                        case "motivoHaConsultato":
                            {
                                response = Enum.Parse(typeof(MotivoHaConsultato), answer.Key).DescriptionAttr();
                                break;
                            }
                        case "favoritoAccessoAlleInformazioni":
                            {
                                response = Enum.Parse(typeof(FavoritoAccessoAlleInformazioni), answer.Key).DescriptionAttr();
                                break;
                            }
                        case "qualitàDiQuestaSezione":
                            {
                                response = Enum.Parse(typeof(QualitàDiQuestaSezione), answer.Key).DescriptionAttr();
                                break;
                            }
                        case "fasciaDiEtà":
                            {
                                response = Enum.Parse(typeof(FasciaDiEtà), answer.Key).DescriptionAttr();
                                break;
                            }
                        case "attualeOccupazione":
                            {
                                response = Enum.Parse(typeof(AttualeOccupazione), answer.Key).DescriptionAttr();
                                break;
                            }
                        case "titoloDiStudio":
                            {
                                response = Enum.Parse(typeof(TitoloDiStudio), answer.Key).DescriptionAttr();
                                break;
                            }
                        case "doveRisiede":
                            {
                                response = Enum.Parse(typeof(DoveRisiede), answer.Key).DescriptionAttr();
                                break;
                            }
                    }
                    float percentage = answer.Value * 100 / question.Value.Sum(x => x.Value);
                    responseModels.Add(new ResponseModel
                    {
                        Response = response,
                        PercentageResponse = percentage + "%"
                    });
                }
                reports.Add(new ReportModel
                {
                    Question = Enum.Parse(typeof(TranslateQuestions), question.Key).DescriptionAttr(),
                    Responses = responseModels
                });
            }
            return reports;
        }
        #endregion
    }
}
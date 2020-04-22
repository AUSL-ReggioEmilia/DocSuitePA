using Newtonsoft.Json;

namespace VecompSoftware.DocSuiteWeb.DTO.UDS
{/// <summary>
/// fieldName deve essere univoco
/// </summary>
    public class UDSFinderExpressionDto
    {
        [JsonProperty("fieldName")]

        public string FieldName { get; set; }
        [JsonProperty("expression")]
        public string Expression { get; set; }
    }
}

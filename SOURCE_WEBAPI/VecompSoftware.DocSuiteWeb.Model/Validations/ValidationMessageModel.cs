
namespace VecompSoftware.DocSuiteWeb.Model.Validations
{
    public class ValidationMessageModel
    {
        public int MessageCode { get; set; }
        public string Message { get; set; }
        public string Key { get; set; }

        public override string ToString()
        {
            return $"{Key}({MessageCode}):{Message}";
        }
    }
}

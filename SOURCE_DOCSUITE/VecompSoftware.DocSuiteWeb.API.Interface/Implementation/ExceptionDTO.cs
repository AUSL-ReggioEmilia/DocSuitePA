using System;
using Newtonsoft.Json;

namespace VecompSoftware.DocSuiteWeb.API
{
    public class ExceptionDTO : IExceptionDTO
    {
        #region [ Constructors ]

        public ExceptionDTO()
        {
        }

        public ExceptionDTO(Exception ex)
        {
            this.TypeName = ex.GetType().Name;
            this.Message = ex.Message;
            this.StackTrace = ex.StackTrace;
            if (ex.InnerException != null)
                this.InnerException = new ExceptionDTO(ex.InnerException);
        }

        #endregion

        #region [ Properties ]

        public string TypeName { get; set; }

        public string Message { get; set; }

        public string StackTrace { get; set; }

        [JsonConverter(typeof(APIArgumentConverter<ExceptionDTO, IExceptionDTO>))]
        public IExceptionDTO InnerException { get; set; }

        #endregion

        #region [ Methods ]

        public Type GetExceptionType()
        {
            if (string.IsNullOrWhiteSpace(this.TypeName))
                return null;

            return Type.GetType(this.TypeName);
        }

        public Exception GetException()
        {
            if (this.InnerException == null)
                return new Exception(this.Message);

            var inner = (ExceptionDTO)this.InnerException;
            return new Exception(this.Message, inner.GetException());
        }

        #endregion
    }
}
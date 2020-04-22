using System;
using Newtonsoft.Json;

namespace VecompSoftware.DocSuiteWeb.API
{
    public class APIResponse<T> : IAPIResponse<T>
    {
        #region [ Constructors ]

        public APIResponse()
        {
        }

        public APIResponse(T argument, Exception ex)
        {
            this.TypeName = typeof(T).Name;
            this.Argument = argument;
            if (ex != null)
                this.Exception = new ExceptionDTO(ex);
        }

        public APIResponse(T argument)
            : this(argument, null)
        {
        }

        public APIResponse(Exception ex)
            : this(default(T), ex)
        {
        }

        #endregion

        #region [ Properties ]

        public string TypeName { get; set; }

        public T Argument { get; set; }

        [JsonConverter(typeof(APIArgumentConverter<ExceptionDTO, IExceptionDTO>))]
        public IExceptionDTO Exception { get; set; }

        #endregion

        #region [ Methods ]

        public Type GetResponseType()
        {
            if (string.IsNullOrWhiteSpace(this.TypeName))
                return null;

            return Type.GetType(this.TypeName);
        }

        #endregion
    }
}
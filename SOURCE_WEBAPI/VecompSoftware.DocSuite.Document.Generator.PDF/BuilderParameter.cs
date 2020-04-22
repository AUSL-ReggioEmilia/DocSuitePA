using System.Collections.Generic;
using System.Net;
using VecompSoftware.Commons.Interfaces.DocumentGenerator.Models;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Model.DocumentGenerator.Parameters;

namespace VecompSoftware.DocSuite.Document.Generator.PDF
{
    public class BuilderParameter
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public BuilderParameter(ICollection<IDocumentGeneratorParameter> parameters)
        {
            Parameters = parameters;
            MappedParameters = new List<SC.BuildValueModel>();
            foreach (IDocumentGeneratorParameter item in parameters)
            {
                Mapper(item);
            }
        }
        #endregion

        #region [ Properties ]

        public ICollection<IDocumentGeneratorParameter> Parameters { get; private set; }
        public ICollection<SC.BuildValueModel> MappedParameters { get; private set; }
        #endregion

        #region [ Methods ]
        private void Mapper(IDocumentGeneratorParameter visitable)
        {
            if (visitable is BooleanParameter)
            {
                BooleanParameter v = visitable as BooleanParameter;
                MappedParameters.Add(new SC.BuildValueModel()
                {
                    IsHTML = false,
                    Name = v.Name,
                    Value = v.Value.ToString()
                });
                return;
            }
            if (visitable is CharParameter)
            {
                CharParameter v = visitable as CharParameter;
                MappedParameters.Add(new SC.BuildValueModel()
                {
                    IsHTML = false,
                    Name = v.Name,
                    Value = v.Value.ToString()
                });
                return;
            }
            if (visitable is FloatParameter)
            {
                FloatParameter v = visitable as FloatParameter;
                MappedParameters.Add(new SC.BuildValueModel()
                {
                    IsHTML = false,
                    Name = v.Name,
                    Value = v.Value.ToString()
                });
                return;
            }
            if (visitable is GuidParameter)
            {
                GuidParameter v = visitable as GuidParameter;
                MappedParameters.Add(new SC.BuildValueModel()
                {
                    IsHTML = false,
                    Name = v.Name,
                    Value = v.Value.ToString()
                });
                return;
            }
            if (visitable is IntParameter)
            {
                IntParameter v = visitable as IntParameter;
                MappedParameters.Add(new SC.BuildValueModel()
                {
                    IsHTML = false,
                    Name = v.Name,
                    Value = v.Value.ToString()
                });
                return;
            }
            if (visitable is StringParameter)
            {
                StringParameter v = visitable as StringParameter;
                MappedParameters.Add(new SC.BuildValueModel()
                {
                    IsHTML = v.HasHtmlValue,
                    Name = v.Name,
                    Value = WebUtility.HtmlDecode(v.Value)
                });
                return;
            }
            if (visitable is DateTimeParameter)
            {
                DateTimeParameter v = visitable as DateTimeParameter;
                MappedParameters.Add(new SC.BuildValueModel()
                {
                    IsHTML = false,
                    Name = v.Name,
                    Value = v.Value.ToShortDateString()
                });
                return;
            }

            throw new DSWException(string.Concat("Parameter '", visitable.GetType().Name, "' is not correct"), null, DSWExceptionCode.Invalid);
        }

        
        #endregion
    }
}

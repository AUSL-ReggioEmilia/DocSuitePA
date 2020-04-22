using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VecompSoftware.JeepService
{
    public class SignatureDocumentValidationException : Exception
    {
        public string FilePath { get; set; }
        public ICollection<string> Errors { get; set; }
        public bool ToRetry { get; set; }

        public SignatureDocumentValidationException()
        {
        }

        public SignatureDocumentValidationException(string message)
            : base(message)
        {
        }

        public SignatureDocumentValidationException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public SignatureDocumentValidationException(string filepath, ICollection<string> errors, bool toRetry)
            :this(string.Format("Errore nella validazione del documento {0}. {1}", filepath, string.Join(", ", errors)))
        {
            FilePath = filepath;
            Errors = errors;
            ToRetry = toRetry;
        }
    }
}

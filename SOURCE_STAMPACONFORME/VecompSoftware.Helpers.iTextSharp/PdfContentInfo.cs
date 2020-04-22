using System.Collections.Generic;
using System.IO;
using System.Linq;
using iTextSharp.text.pdf;
using VecompSoftware.Commons.BiblosInterfaces;
using VecompSoftware.Commons.CommonEx;
using VecompSoftware.Commons.Infos;
using VecompSoftware.Helpers.iTextSharp.iTextSharpEx;

namespace VecompSoftware.Helpers.iTextSharp
{
    public class PdfContentInfo : ContentInfo, IPdfContent
    {

        #region [ Constructors ]

        public PdfContentInfo(string fullName, byte[] content)
            : base(Path.ChangeExtension(fullName, ".pdf"), content) { }
        public PdfContentInfo(byte[] content)
            : base(string.Empty.ToPath().ChangeExtension(".pdf"), content) { }
        public PdfContentInfo(string fullName, bool eager)
            : base(Path.ChangeExtension(fullName, ".pdf"), eager) { }
        public PdfContentInfo(string fullName)
            : base(Path.ChangeExtension(fullName, ".pdf"), false) { }

        public PdfContentInfo(IContent contentInfo) : this(contentInfo.FileName, contentInfo.Content) { }

        #endregion

        #region [ Fields ]

        private bool? _isEncrypted;

        private int? _numberOfPages;

        private bool? _xfaPresent;

        private bool? _hasAnnots;

        private bool? _hasSignatures;

        private IEnumerable<SignInfo> _signatures;

        private string _sha1;

        #endregion

        #region [ Properties ]

        public bool IsEncrypted
        {
            get
            {
                if (!_isEncrypted.HasValue)
                    using (var reader = GetReader())
                        _isEncrypted = reader.IsEncrypted();
                return _isEncrypted.Value;
            }
        }

        public int NumberOfPages
        {
            get
            {
                if (!_numberOfPages.HasValue)
                    using (var reader = GetReader())
                        _numberOfPages = reader.NumberOfPages;
                return _numberOfPages.Value;
            }
        }

        public bool XfaPresent
        {
            get
            {
                if (!_xfaPresent.HasValue)
                    using (var reader = GetReader())
                        _xfaPresent = reader.XfaPresent();
                return _xfaPresent.Value;
            }
        }

        public bool HasAnnots
        {
            get
            {
                if (!_hasAnnots.HasValue)
                    using (var reader = GetReader())
                        _hasAnnots = reader.HasAnnotations();
                return _hasAnnots.Value;
            }
        }

        public bool HasSignatures
        {
            get
            {
                if (!_hasSignatures.HasValue && _signatures != null)
                    _hasSignatures = Signatures.Any();

                if (!_hasSignatures.HasValue)
                    using (var reader = GetReader())
                        _hasSignatures = !reader.AcroFields
                            .GetSignatureNames()
                            .IsNullOrEmpty();
                return _hasSignatures.Value;
            }
        }
        public IEnumerable<SignInfo> Signatures
        {
            get
            {
                if (_signatures == null && _hasSignatures.HasValue(false))
                    _signatures = Enumerable.Empty<SignInfo>();

                if (_signatures == null)
                    using (var reader = GetReader())
                    {
                        var af = reader.AcroFields;
                        _signatures = af.GetSignatureNames()
                            .Select(n => new PdfSignInfo(af.VerifySignature(n)))
                            .ToList();
                    }
                return _signatures;
            }
        }

        #endregion

        #region [ Methods ]

        public PdfReader GetReader()
        {
            if (IsDirty)
                return new PdfReader(FullName);
            return new PdfReader(Content);
        }

        #endregion

    }
}

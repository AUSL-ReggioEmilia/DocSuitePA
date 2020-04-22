using System.Collections.Generic;
using System.Linq;
using VecompSoftware.Common;

namespace VecompSoftware.CompEd
{
    public class CompEdContentInfo : ContentInfo, ISignContent
    {

        #region [ Constructors ]

        public CompEdContentInfo(string fullName, byte[] content)
            : base(fullName, content) { }
        public CompEdContentInfo(byte[] content)
            : base(content) { }
        public CompEdContentInfo(string fullName, bool eager)
            : base(fullName, eager) { }
        public CompEdContentInfo(string fullName)
            : base(fullName) { }

        public CompEdContentInfo(IContent contentInfo) : base(contentInfo) { }

        #endregion

        #region [ Fields ]

        private bool? _hasParent;
        private ContentInfo _parent;
        private ContentInfo _root;

        private bool? _hasP7kSignatures;
        private IEnumerable<SignInfo> _p7kSignatures;

        private bool? _hasP7xSignatures;
        private IEnumerable<SignInfo> _p7xSignatures;

        private bool? _hasPadesSignatures;
        private IEnumerable<SignInfo> _padesSignatures;

        private bool? _hasSignatures;
        private IEnumerable<SignInfo> _signatures;

        #endregion

        #region [ Properties ]

        public bool HasParent
        {
            get
            {
                if (!_hasParent.HasValue)
                    _hasParent = !Parent.IsNullOrEmpty();
                return _hasParent.Value;
            }
        }
        public ContentInfo Parent
        {
            get
            {
                if (_parent == null && _hasParent.HasValue(false))
                    return null;

                if (_parent == null)
                    using (var digest = new DigestSession(this))
                        _parent = digest.GetParent();
                return _parent;
            }
        }
        public ContentInfo Root
        {
            get
            {
                if (_root == null && _hasParent.HasValue(false))
                    return null;

                if (_root == null)
                {
                    var container = this;
                    while (container.Parent != null)
                        container = container.Parent.ToCompEdContent();
                    _root = new ContentInfo(container);
                }
                return _root;
            }
        }

        public bool HasP7kSignatures
        {
            get
            {
                if (!_hasP7kSignatures.HasValue && _p7kSignatures != null)
                    _hasP7kSignatures = P7kSignatures.Any();

                if (!_hasP7kSignatures.HasValue)
                    using (var digest = new DigestSession(this))
                    {
                        _hasP7kSignatures = digest.HasP7kSignatures()
                            || HasParent && Parent.ToCompEdContent().HasP7kSignatures;
                    }

                return _hasP7kSignatures.Value;
            }
        }
        public IEnumerable<SignInfo> P7kSignatures
        {
            get
            {
                if (_p7kSignatures == null && _hasP7kSignatures.HasValue(false))
                    _p7kSignatures = Enumerable.Empty<SignInfo>();

                if (_p7kSignatures == null)
                {
                    using (var digest = new DigestSession(this))
                        _p7kSignatures = digest.GetP7kSignatures();
                    if (HasParent)
                        _p7kSignatures = _p7kSignatures.Concat(Parent.ToCompEdContent().P7kSignatures);
                }

                return _p7kSignatures;
            }
        }

        public bool HasP7xSignatures
        {
            get
            {
                if (!_hasP7xSignatures.HasValue && _p7xSignatures != null)
                    _hasP7xSignatures = P7xSignatures.Any();

                if (!_hasP7xSignatures.HasValue)
                    using (var digest = new DigestSession(this))
                        _hasP7xSignatures = digest.HasP7xSignatures();

                return _hasP7xSignatures.Value;
            }
        }
        public IEnumerable<SignInfo> P7xSignatures
        {
            get
            {
                if (_p7xSignatures == null && _hasP7xSignatures.HasValue(false))
                    _p7xSignatures = Enumerable.Empty<SignInfo>();

                if (_p7xSignatures == null)
                    using (var digest = new DigestSession(this))
                        _p7xSignatures = digest.GetP7xSignatures();

                return _p7xSignatures;
            }
        }

        public bool HasPADESSignatures
        {
            get
            {
                if (!_hasPadesSignatures.HasValue && _padesSignatures != null)
                    _hasPadesSignatures = PADESSignatures.Any();

                if (!_hasPadesSignatures.HasValue)
                    using (var digest = new DigestSession(this))
                        _hasPadesSignatures = digest.HasPADESSignatures();

                return _hasPadesSignatures.Value;
            }
        }
        public IEnumerable<SignInfo> PADESSignatures
        {
            get
            {
                if (_padesSignatures == null && _hasPadesSignatures.HasValue(false))
                    _padesSignatures = Enumerable.Empty<SignInfo>();

                if (_padesSignatures == null)
                    using (var digest = new DigestSession(this))
                        _padesSignatures = digest.GetPADESSignatures();

                return _padesSignatures;
            }
        }

        public bool HasSignatures
        {
            get
            {
                _hasSignatures = _hasSignatures ?? HasP7xSignatures || HasPADESSignatures || HasP7kSignatures;
                return _hasSignatures.Value;
            }
        }
        public IEnumerable<SignInfo> Signatures
        {
            get
            {
                _signatures = _signatures ?? P7xSignatures.Concat(P7kSignatures).Concat(PADESSignatures);
                return _signatures;
            }
        }

        #endregion

    }
}

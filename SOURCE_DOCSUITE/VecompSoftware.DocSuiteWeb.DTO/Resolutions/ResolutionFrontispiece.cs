using System.IO;

namespace VecompSoftware.DocSuiteWeb.DTO.Resolutions
{
    public class ResolutionFrontispiece
    {
        #region [ Constructor ]
        public ResolutionFrontispiece() { }
        #endregion

        #region [ Properties ]
        public string Name { get; set; }

        public string Path { get; set; }

        public bool IsPrivacy { get; set; }
        #endregion
    }
}

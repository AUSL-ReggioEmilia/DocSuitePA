
namespace VecompSoftware.Common
{
    public interface IContent
    {

        #region [ Properties ]

        string FullName { get; }
        byte[] Content { get; }
        string FileName { get; }
        string Extension { get; }
        bool IsDirty { get; }

        #endregion

        #region [ Methods ]

        void WriteAllBytes(string destination);

        #endregion

    }
}

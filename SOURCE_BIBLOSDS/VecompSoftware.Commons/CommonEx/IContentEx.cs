
namespace VecompSoftware.Common
{
    public static class IContentEx
    {

        public static bool IsNullOrEmpty(this IContent contentInfo)
        {
            return contentInfo == null || contentInfo.Content.IsNullOrEmpty();
        }

    }
}

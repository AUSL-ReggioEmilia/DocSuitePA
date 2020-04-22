namespace VecompSoftware.DocSuiteWeb.API
{
    public static class IAPIArgumentEx
    {
        public static APIResponse<T> ToResponse<T>(this T source) where T: IAPIArgument
        {
            return new APIResponse<T>(source);
        }

        public static APIResponse<T[]> ToResponse<T>(this T[] source) where T : IAPIArgument
        {
            return new APIResponse<T[]>(source);
        }

        public static string SerializeAsResponse<T>(this T source) where T : IAPIArgument
        {
            return source.ToResponse().Serialize();
        }

        public static string SerializeAsResponse<T>(this T[] source) where T : IAPIArgument
        {
            return source.ToResponse().Serialize();
        }

        public static APIResponse<T> DeserializeAsResponse<T>(this string source)
        {
            return source.Deserialize<APIResponse<T>>();
        }
    }
}

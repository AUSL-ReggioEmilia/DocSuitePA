namespace VecompSoftware.WebAPIManager
{
    public interface IODATAQueryManager
    {
        IODATAQueryManager Function(string function);
        IODATAQueryManager Skip(int skip);
        IODATAQueryManager Top(int top);
        IODATAQueryManager Filter(string filter);
        IODATAQueryManager Sorting(string sorting);
        IODATAQueryManager Expand(string expand);
        string Compile();
    }
}

namespace VecompSoftware.ServiceBus.Module.UDS
{
    public class UDSConfig
    {
        public string ConnectionString { get; set; }
        public string DbSchema { get; set; }
        public string SolutionPath { get; set; }
        public string ProjectName { get; set; }
        public string CompilationLoggerPath { get; set; }
        public string OutputDLLPath { get; set; }
        public string WebAPI_UDS_Path { get; set; }
        public string WebAPI_PoolName { get; set; }
        public string BiblosDS_Storage_MainPath { get; set; }
        public string BiblosDS_Storage_StorageType { get; set; }
    }
}

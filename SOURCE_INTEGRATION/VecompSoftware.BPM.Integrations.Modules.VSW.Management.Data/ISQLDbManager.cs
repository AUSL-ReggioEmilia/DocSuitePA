using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace VecompSoftware.BPM.Integrations.Modules.VSW.Management.Data
{
    public interface ISQLDbManager
    {
        Task<int> ExecuteSqlCommandAsync(string commandText, CommandType commandType, SqlParameter[] parameters);
    }
}

using Microsoft.Data.SqlClient;

namespace SolucionWebGranoVivo.Services
{
    public class DatabaseRestoreService
    {
        private readonly string _connectionString;

        public DatabaseRestoreService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void RestoreDatabase(string backupFilePath)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = $@"
                USE master;
                ALTER DATABASE [GranoVivoSelectoDB] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                RESTORE DATABASE [GranoVivoSelectoDB]
                FROM DISK = N'C:\Backups\GranoVivoDB.bak'
                WITH REPLACE;
                ALTER DATABASE [GranoVivoSelectoDB] SET MULTI_USER;";

            cmd.ExecuteNonQuery();
        }
    }
}

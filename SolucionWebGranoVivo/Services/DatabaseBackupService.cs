using Microsoft.Data.SqlClient;

namespace SolucionWebGranoVivo.Services
{
    public class DatabaseBackupService
    {
        private readonly string _connectionString;

        public DatabaseBackupService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void BackupDatabase(string backupFilePath)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            // Obtiene el nombre de la base desde la cadena de conexión
            var dbName = connection.Database;

            using var cmd = connection.CreateCommand();
            cmd.CommandText = $@"
                BACKUP DATABASE [{dbName}]
                TO DISK = N'{backupFilePath}'
                WITH FORMAT, INIT, NAME = N'Backup completo de {dbName}'";

            cmd.ExecuteNonQuery();
        }
    }
}

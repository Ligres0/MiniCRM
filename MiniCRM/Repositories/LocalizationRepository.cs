using Dapper;
using Microsoft.Data.SqlClient;
using MiniCRM.Models;
using System.Data;

namespace MiniCRM.Repositories
{
    public class LocalizationRepository: ILocalizationRepository
    {
        private readonly string _connectionString;

        public LocalizationRepository(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "DefaultConnection bağlantısı bulunamadı.");
        }

        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public List<Localization> GetByCulture(string culture)
        {
            using var connection = CreateConnection();
            const string sql = """
                SELECT Id, [Key], Culture, Value
                FROM Localizations
                WHERE Culture = @Culture
                """;
            return connection.Query<Localization>(
                sql,
                new { Culture = culture }
                ).ToList();
        }
    }
}

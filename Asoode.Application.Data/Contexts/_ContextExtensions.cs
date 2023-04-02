using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Asoode.Application.Data.Contexts
{
    public static class ContextExtensions
    {
        public static async Task<Dictionary<string, List<Dictionary<string, object>>>> GetMultipleResultSets(this DbContext context,string commandText, params NpgsqlParameter[] parameters)
        {
            // Get the connection from DbContext
            var connection = context.Database.GetDbConnection();

            // Open the connection if isn't open
            if (connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();

            using (var command = connection.CreateCommand())
            {
                var sets = new Dictionary<string, List<Dictionary<string, object>>>();
                command.CommandText = commandText;
                command.Connection = connection;

                if (parameters?.Length > 0)
                {
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.Add(parameter);
                    }
                }

                using (var dataReader = await command.ExecuteReaderAsync())
                {
                    int tableCount = 1;
                    do
                    {
                        var names = new List<string>();
                        var result = new List<Dictionary<string, object>>();
                        if (dataReader.HasRows)
                        {
                            for (var i = 0; i < dataReader.VisibleFieldCount; i++)
                            {
                                names.Add(dataReader.GetName(i));
                            }
                        
                            while (await dataReader.ReadAsync())
                            {
                                var row = new Dictionary<string, object>();
                                foreach (var name in names)
                                {
                                    row.Add(name, dataReader[name]);
                                }
                                result.Add(row);
                            }
                        }
                        
                        sets.Add("Table-" + tableCount, result);
                        tableCount++;
                    } while (await dataReader.NextResultAsync());
                    
                }

                return sets;
            }
        }
    }
}
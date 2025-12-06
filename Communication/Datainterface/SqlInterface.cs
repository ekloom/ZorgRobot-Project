using System.Reflection;
using Microsoft.Data.SqlClient;


namespace RobotProject.Services.Datainterface
{
    /// <summary>
    /// Base class for saving data to a sql data base and getting data 
    /// </summary>
    public class SqlInterface
    {
        private readonly string _connectionString;

        public SqlInterface()
        {
            _connectionString = AppConfig.Configuration["ConnectionString"];
        }

        public void DeleteData(string tableName, string condition)
        {

            var deleteQuery = $"DELETE FROM [{tableName}] WHERE {condition}";

            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            using var command = new SqlCommand(deleteQuery, connection);

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Returns the data with type 'T'
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition"></param>
        /// <returns></returns>
        public T GetData<T>(string tableName, string condition) where T : class, new()
        {
            var type = typeof(T);  // Gets the type of 'T'
            tableName = string.IsNullOrEmpty(tableName) ? type.Name : tableName;  // Uses the class name as the table name. Example: User or the tablename in param
            var properties = type.GetProperties(BindingFlags.Public); // Gets the properties in the class 'T'


            /* This will form a query with the variables: tableName, columns and parameters.
              Example: SELECT * FROM [User] VALUES (@Name, @Age, @IsActive)
            */
            var selectQuery = $"SELECT * FROM [{tableName}] {condition}";

            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            using var command = new SqlCommand(selectQuery, connection);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                // Creates a new object of type 'T' this is why T needs to be able to be Instantiable
                var obj = new T();


                foreach (var prop in properties)
                {
                    // Als de data niet null is dan wordt deze if statement true
                    if (!reader.IsDBNull(reader.GetOrdinal(prop.Name)))
                    {
                        // gets the value which has the property name in reader and assigns the value to the matching property in 'obj'
                        prop.SetValue(obj, reader[prop.Name]);
                    }
                }

                // Returns the object with values
                return obj;
            }
            return default;
        }

        public List<T> GetListOfData<T>(string tableName, string condition) where T : class, new()
        {
            var type = typeof(T);  // Gets the type of 'T'
            tableName = string.IsNullOrEmpty(tableName) ? type.Name : tableName;  // Uses the class name as the table name. Example: User or the tablename in param
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance); // Gets the properties in the class 'T'


            /* This will form a query with the variables: tableName, columns and parameters.
              Example: SELECT * FROM [User] VALUES (@Name, @Age, @IsActive)
            */
            var selectQuery = $"SELECT * FROM [{tableName}] {condition}";

            var resultList = new List<T>();

            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            using var command = new SqlCommand(selectQuery, connection);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                // Creates a new object of type 'T' this is why T needs to be able to be Instantiable
                var obj = new T();


                foreach (var prop in properties)
                {
                    // Als de data niet null is dan pas wordt deze if statement gerunned
                    if (!reader.IsDBNull(reader.GetOrdinal(prop.Name)))
                    {
                        // gets the value which has the property name in reader and assigns the value to the matching property in 'obj'
                        prop.SetValue(obj, reader[prop.Name]);
                    }
                }

                // adds the object with values to the list
                resultList.Add(obj);
            }
            return resultList;
        }

        public void SaveData<T>(string tableName, T data) where T : class, new()
        {
            var type = typeof(T);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            //  .Where(p => p.Name != "Id") // Exclude 'Id' from INSERT
            //  .ToArray();

            tableName = string.IsNullOrEmpty(tableName) ? type.Name : tableName;

            var columns = string.Join(", ", properties.Select(p => p.Name));
            var parameters = string.Join(", ", properties.Select(p => $"@{p.Name}"));

            var insertQuery = $"INSERT INTO [{tableName}] ({columns}) VALUES ({parameters})";

            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            using var command = new SqlCommand(insertQuery, connection);

            foreach (var prop in properties)
            {
                var value = prop.GetValue(data) ?? DBNull.Value;
                command.Parameters.AddWithValue($"@{prop.Name}", value);
            }

            command.ExecuteNonQuery();
        }

    }
}

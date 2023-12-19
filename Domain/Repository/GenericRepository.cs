using Microsoft.Data.SqlClient;
using System.Reflection;

namespace SpargoApp.Domain.Repository
{
    internal class GenericRepository<TModel> : IDisposable where TModel : class, new()
    {
        readonly SqlConnection _connection;

        public GenericRepository(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
            _connection.Open();
        }

        public IEnumerable<TModel> GetAll(string tableName, Func<TModel, bool> filter = null)
        {
            PropertyInfo[] propertyInfo = typeof(TModel).GetProperties();
            List<TModel> list = [];

            using SqlCommand getAll = new()
            {
                Connection = _connection,
                CommandText = $"select * from {tableName};"
            };

            using SqlDataReader reader = getAll.ExecuteReader();
            while (reader.Read())
            {
                TModel model = new();
                for (int i = 0; i < propertyInfo.Length; i++)
                {
                    propertyInfo[i].SetValue(model, reader.GetValue(i));
                }
                list.Add(model);
            }
            if (filter != null)
            {
                return list.Where(filter);
            }
            return list;
        }

        public bool Insert(string tableName, TModel model)
        {
            List<PropertyInfo> properties = [.. typeof(TModel).GetProperties()];
            properties.RemoveAt(0);

            string[] parameters = new string[properties.Count];

            for (int i = 0; i < parameters.Length; i++)
            {
                parameters[i] = $"@parameter{i + 1}";
            }
            using SqlCommand insert = new()
            {
                Connection = _connection,
                CommandText = $"insert into {tableName} values({string.Join(", ", parameters)});"
            };
            SqlParameter[] sqlParameters = properties
                .Select((p, i) => new SqlParameter(parameters[i], p.GetValue(model)))
                .ToArray();
            insert.Parameters.AddRange(sqlParameters);

            return insert.ExecuteNonQuery() == 1;
        }

        public bool Delete(string tableName, object id)
        {
            using SqlCommand delete = new()
            {
                Connection = _connection,
                CommandText = $"delete from {tableName} where id = {id}"
            };
            return delete.ExecuteNonQuery() == 1;
        }

        public bool Save(string tableName, IEnumerable<TModel> models)
        {
            bool result = true;
            foreach (TModel model in models)
            {
                result = Insert(tableName, model);
            }
            return result;
        }

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _connection.Close();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

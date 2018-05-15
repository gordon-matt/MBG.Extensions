using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Transactions;
using MBG.Extensions.Core;

namespace MBG.Extensions.Data.SqlClient
{
    public static class SqlConnectionExtensions
    {
        public static void CreateColumn(
            this SqlConnection connection,
            string tableName,
            string columnName,
            SqlDbType type,
            string maxLength,
            bool nullable)
        {
            using (TransactionScope transactionScope = new TransactionScope())
            {
                const string CMD_CREATE_COLUMN_FORMAT =
    @"IF NOT EXISTS
(
    SELECT Name FROM sys.columns WHERE Name = N'{1}'
        AND OBJECT_ID = OBJECT_ID(N'[dbo].[{0}]')
)
ALTER TABLE dbo.[{0}] ADD [{1}] {2}{3} {4};";

                string commandText = string.Empty;

                switch (type)
                {
                    case SqlDbType.Decimal:
                        commandText = string.Format(
                            CMD_CREATE_COLUMN_FORMAT,
                            tableName,
                            columnName,
                            type,
                            "(18)",
                            (nullable ? "NULL" : "NOT NULL"));
                        break;
                    case SqlDbType.Date:
                    case SqlDbType.DateTime:
                    case SqlDbType.BigInt:
                    case SqlDbType.Bit:
                    case SqlDbType.Float:
                    case SqlDbType.Image:
                    case SqlDbType.Int:
                    case SqlDbType.UniqueIdentifier:
                    case SqlDbType.Money:
                    case SqlDbType.NText:
                        commandText = string.Format(
                            CMD_CREATE_COLUMN_FORMAT,
                            tableName,
                            columnName,
                            type,
                            string.Empty,
                            (nullable ? "NULL" : "NOT NULL"));
                        break;
                    default:
                        int max = 255;
                        int.TryParse(maxLength, out max);
                        if (max < 255)
                        {
                            maxLength = "(255)";
                        }
                        else if (!maxLength.ContainsAll('(', ')'))
                        {
                            maxLength = string.Format("({0})", maxLength.Trim('(', ')'));
                        }
                        commandText = string.Format(
                            CMD_CREATE_COLUMN_FORMAT,
                            tableName,
                            columnName,
                            type,
                            maxLength,
                            (nullable ? "NULL" : "NOT NULL"));
                        break;
                }

                connection.Open();
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
                connection.Close();

                transactionScope.Complete();
            }
        }

        public static void CreateTable(this SqlConnection connection, string tableName)
        {
            connection.CreateTable(tableName, "ID", SqlDbType.Int, true);
        }

        public static void CreateTable(
            this SqlConnection connection,
            string tableName,
            string pkColumnName,
            SqlDbType pkDataType,
            bool pkIsIdentity)
        {
            using (TransactionScope transactionScope = new TransactionScope())
            {
                const string CMD_CREATE_TABLE_FORMAT = "CREATE TABLE [dbo].[{0}]({1} [{2}] {3} NOT NULL CONSTRAINT [PK{0}] PRIMARY KEY )";
                //const string CMD_CREATE_TABLE_FORMAT = "CREATE TABLE [dbo].[{0}]({1} [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK{0}] PRIMARY KEY )";
                string commandText = string.Format(
                    CMD_CREATE_TABLE_FORMAT,
                    tableName,
                    pkColumnName,
                    pkDataType,
                    pkIsIdentity ? "IDENTITY(1,1)" : string.Empty);

                connection.Open();
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
                connection.Close();

                transactionScope.Complete();
            }
        }

        public static IEnumerable<string> GetDatabaseNames(this SqlConnection connection)
        {
            const string CMB_SELECT_DATABASE_NAMES = "SELECT NAME FROM SYS.DATABASES ORDER BY NAME";
            List<string> databaseNames = new List<string>();

            connection.Open();
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = CMB_SELECT_DATABASE_NAMES;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        databaseNames.Add(reader.GetString(0));
                    }
                }
            }
            connection.Close();

            return databaseNames;
        }

        public static ForeignKeyInfoCollection GetForeignKeyData(this SqlConnection connection, string tableName)
        {
            const string CMD_FOREIGN_KEYS_FORMAT =
@"SELECT FK_Table = FK.TABLE_NAME,
    FK_Column = CU.COLUMN_NAME,
	PK_Table = PK.TABLE_NAME,
    PK_Column = PT.COLUMN_NAME,
	Constraint_Name = C.CONSTRAINT_NAME
FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS C
INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS FK ON C.CONSTRAINT_NAME = FK.CONSTRAINT_NAME
INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS PK ON C.UNIQUE_CONSTRAINT_NAME = PK.CONSTRAINT_NAME
INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE CU ON C.CONSTRAINT_NAME = CU.CONSTRAINT_NAME
INNER JOIN
(
	SELECT i1.TABLE_NAME, i2.COLUMN_NAME
	FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS i1
	INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE i2 ON
		i1.CONSTRAINT_NAME = i2.CONSTRAINT_NAME
	WHERE i1.CONSTRAINT_TYPE = 'PRIMARY KEY'
) PT ON PT.TABLE_NAME = PK.TABLE_NAME
WHERE FK.TABLE_NAME = '{0}'
ORDER BY 1,2,3,4";

            ForeignKeyInfoCollection foreignKeyData = new ForeignKeyInfoCollection();

            connection.Open();
            using (SqlCommand command = new SqlCommand(string.Format(CMD_FOREIGN_KEYS_FORMAT, tableName), connection))
            {
                command.CommandType = CommandType.Text;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        foreignKeyData.Add(new ForeignKeyInfo(
                            reader.GetString(0),
                            reader.GetString(1),
                            reader.GetString(2),
                            reader.GetString(3),
                            reader.GetString(4)));
                    }
                }
            }
            connection.Close();
            return foreignKeyData;
        }

        public static IEnumerable<string> GetTableNames(this SqlConnection connection)
        {
            if (!string.IsNullOrEmpty(connection.Database))
            {
                return connection.GetTableNames(connection.Database);
            }
            else { return new List<string>(); }
        }

        public static IEnumerable<string> GetTableNames(this SqlConnection connection, string databaseName)
        {
            string query = string.Concat("USE ", databaseName, " SELECT * FROM sys.Tables");

            List<string> tables = new List<string>();

            connection.Open();
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = query;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tables.Add(reader.GetString(0));
                    }
                }
            }
            connection.Close();

            return tables;
        }

        public static bool Validate(this SqlConnection connection)
        {
            try
            {
                connection.Open();
                byte numberOfTries = 1;
                while (connection.State == ConnectionState.Connecting && numberOfTries <= 10)
                {
                    System.Threading.Thread.Sleep(100);
                    numberOfTries++;
                }
                bool valid = connection.State == ConnectionState.Open;
                connection.Close();
                return valid;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            catch (SqlException)
            {
                return false;
            }
        }

        public static IEnumerable<ColumnInfo> GetColumnData(this SqlConnection connection, string tableName)
        {
            const string CMD_COLUMN_INFO_FORMAT =
@"SELECT COLUMN_NAME, COLUMN_DEFAULT, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = '{0}'";

            List<ColumnInfo> list = new List<ColumnInfo>();
            try
            {
                ForeignKeyInfoCollection foreignKeyColumns = connection.GetForeignKeyData(tableName);

                connection.Open();

                SqlCommand command = new SqlCommand(string.Format(CMD_COLUMN_INFO_FORMAT, tableName), connection);
                command.CommandType = CommandType.Text;
                SqlDataReader reader = command.ExecuteReader();
                ColumnInfo columnInfo = null;

                //ForeignKeyInfoCollection foreignKeyColumns = new ForeignKeyInfoCollection();

                //using (SqlConnection connection2 = new SqlConnection(connection.ConnectionString))
                //{
                //    //Must use separate connection for this
                //    foreignKeyColumns = connection2.GetForeignKeyData(tableName);
                //}

                while (reader.Read())
                {
                    columnInfo = new ColumnInfo();

                    if (!reader.IsDBNull(0))
                    { columnInfo.Name = reader.GetString(0); }

                    if (!reader.IsDBNull(1))
                    { columnInfo.DefaultValue = reader.GetString(1); }
                    else
                    { columnInfo.DefaultValue = string.Empty; }

                    if (foreignKeyColumns.Contains(columnInfo.Name))
                    {
                        columnInfo.IsForeignKeyColumn = true;
                    }
                    else
                    {
                        try
                        {
                            columnInfo.Type = reader.GetString(2).ToEnum<SqlDbType>(true);
                        }
                        catch (ArgumentNullException)
                        {
                            columnInfo.Type = SqlDbType.Variant;
                        }
                        catch (ArgumentException)
                        {
                            columnInfo.Type = SqlDbType.Variant;
                        }
                    }

                    if (!reader.IsDBNull(3))
                    { columnInfo.MaxLength = reader.GetInt32(3); }

                    if (!reader.IsDBNull(4))
                    {
                        if (reader.GetString(4).ToUpperInvariant().Equals("NO"))
                        { columnInfo.IsRequired = false; }
                        else
                        { columnInfo.IsRequired = true; }
                    }

                    list.Add(columnInfo);
                }
            }
            finally
            {
                if (connection.State != ConnectionState.Closed)
                { connection.Close(); }
            }
            return list;
        }

        public static IEnumerable<ColumnInfo> GetColumnData(this SqlConnection connection, string tableName, IEnumerable<string> columnNames)
        {
            return (from x in connection.GetColumnData(tableName)
                    where columnNames.Contains(x.Name)
                    select x).ToList();
        }

        private static string GetFormattedValue(Type type, object value)
        {
            if (value == null)
            {
                return "NULL";
            }

            switch (type.Name)
            {
                case "Boolean": return (bool)value ? "1" : "0";

                case "String": return ((string)value).Replace("'", "''").AddSingleQuotes();
                case "DateTime": return ((DateTime)value).ToISO8601DateString().AddSingleQuotes();

                case "Byte":
                case "Decimal":
                case "Double":
                case "Int16":
                case "Int32":
                case "Int64":
                case "SByte":
                case "Single":
                case "UInt16":
                case "UInt32":
                case "UInt64": return value.ToString();

                case "DBNull": return "NULL";

                default: return value.ToString().AddSingleQuotes();
            }
        }

        /// <summary>
        /// <para>Inserts the specified entity into the specified Table. Property names are used to match</para>
        /// <para>with Sql Column Names.</para>
        /// </summary>
        /// <typeparam name="T">The type of entity to persist to Sql database.</typeparam>
        /// <param name="connection">This SqlConnection.</param>
        /// <param name="entity">The entity to persist to Sql database.</param>
        /// <param name="tableName">The table to insert the entity into.</param>
        /// <returns>Number of rows affected.</returns>
        public static int Insert<T>(this SqlConnection connection, T entity, string tableName)
        {
            Dictionary<string, string> mappings = typeof(T).GetProperties()
                .ToDictionary(k => k.Name, v => v.Name);

            return connection.Insert(entity, tableName, mappings);
        }

        /// <summary>
        /// <para>Inserts the specified entity into the specified Table. Properties are matched</para>
        /// <para>with Sql Column Names by using the specified mappings dictionary.</para>
        /// </summary>
        /// <typeparam name="T">The type of entity to persist to Sql database.</typeparam>
        /// <param name="connection">This SqlConnection.</param>
        /// <param name="entity">The entity to persist to Sql database.</param>
        /// <param name="tableName">The table to insert the entity into.</param>
        /// <param name="mappings">
        ///     <para>A Dictionary to use to map properties to Sql columns.</para>
        ///     <para>Key = Property Name, Value = Sql Column Name.</para>
        /// </param>
        /// <returns>Number of rows affected.</returns>
        public static int Insert<T>(
            this SqlConnection connection,
            T entity,
            string tableName,
            IDictionary<string, string> mappings)
        {
            using (TransactionScope transactionScope = new TransactionScope())
            {
                const string INSERT_INTO_FORMAT = "INSERT INTO {0} ({1}) VALUES ({2})";

                IEnumerable<ColumnInfo> columnData = connection.GetColumnData(tableName);

                StringBuilder sbNames = new StringBuilder(100);
                StringBuilder sbValues = new StringBuilder(100);

                bool flag = false;
                foreach (PropertyInfo property in typeof(T).GetProperties())
                {
                    string columnName = mappings[property.Name];
                    ColumnInfo columnInfo = columnData.SingleOrDefault(x => x.Name == mappings[property.Name]);

                    if (columnInfo == null)
                    { continue; }

                    if (flag)
                    {
                        sbNames.Append(',');
                        sbValues.Append(',');
                    }

                    sbNames.Append(columnName);
                    sbValues.Append(GetFormattedValue(
                        property.PropertyType,
                        property.GetValue(entity, null)));

                    flag = true;
                }

                using (SqlCommand command = connection.CreateCommand())
                {
                    string commandText = string.Format(INSERT_INTO_FORMAT, tableName, sbNames.ToString(), sbValues.ToString());

                    command.CommandType = CommandType.Text;
                    command.CommandText = commandText;

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    connection.Close();

                    transactionScope.Complete();
                    return rowsAffected;
                }
            }
        }

        /// <summary>
        /// <para>Inserts the specified entities into the specified Table. Property names are used to match</para>
        /// <para>with Sql Column Names.</para>
        /// </summary>
        /// <typeparam name="T">The type of entity to persist to Sql database.</typeparam>
        /// <param name="connection">This SqlConnection.</param>
        /// <param name="entities">The entities to persist to Sql database.</param>
        /// <param name="tableName">The table to insert the entities into.</param>
        public static void InsertCollection<T>(this SqlConnection connection, IEnumerable<T> entities, string tableName)
        {
            using (TransactionScope transactionScope = new TransactionScope())
            {
                foreach (T entity in entities)
                {
                    connection.Insert(entity, tableName);
                }
                transactionScope.Complete();
            }
        }

        /// <summary>
        /// <para>Inserts the specified entities into the specified Table. Properties are matched</para>
        /// <para>with Sql Column Names by using the specified mappings dictionary.</para>
        /// </summary>
        /// <typeparam name="T">The type of entity to persist to Sql database.</typeparam>
        /// <param name="connection">This SqlConnection.</param>
        /// <param name="entities">The entities to persist to Sql database.</param>
        /// <param name="tableName">The table to insert the entities into.</param>
        /// <param name="mappings">
        ///     <para>A Dictionary to use to map properties to Sql columns.</para>
        ///     <para>Key = Property Name, Value = Sql Column Name.</para>
        /// </param>
        public static void InsertCollection<T>(
            this SqlConnection connection,
            IEnumerable<T> entities,
            string tableName,
            IDictionary<string, string> mappings)
        {
            using (TransactionScope transactionScope = new TransactionScope())
            {
                foreach (T entity in entities)
                {
                    connection.Insert(entity, tableName, mappings);
                }
                transactionScope.Complete();
            }
        }
    }
}
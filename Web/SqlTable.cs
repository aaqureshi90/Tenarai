using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
  Library designed for streamlining SQL interactions with the database
*/

namespace SqlLibrary
{
    public class SqlTable : IDisposable
    {
        /* public declarations */
        public StatementType QueryType;

        /* private declarations */
        private string query;
        private SqlConnection connection;

        private SqlCommand cmd;

        /* reader vars */
        private SqlDataReader reader;
        private Boolean readerIsSetup; // This must be true before any SqlDataReader functions can be called
        private string[] columnNames;
        private int fieldCount;

        public enum StatementType
        {
            ALTER,
            CREATE,
            CREATE_DROP,
            DELETE,
            DROP,
            INSERT,
            INVALID,
            SELECT,
            UPDATE
        }

        /* constructors */
        public SqlTable(string connectionString)
        {
            this.connection = new SqlConnection(connectionString);
            this.query = null;
            QueryType = StatementType.INVALID;
            readerIsSetup = false;
            columnNames = null;
            fieldCount = 0;
        }

        /* clean up */
        public void Dispose()
        {
            if (readerIsSetup)
            {
                connection.Dispose();
                reader.Dispose();
            }
        }

        /* public methods*/
        /**
         * Sets query string that will be executed by the database. Must be set before execution methods are called.
         * 
         * @param query     Exact text of query to be executed.
         * **/
        public void SetQuery(string query)
        {
            this.query = query;
        }

        /**
         * Executes query string that is set by setQuery method.
         * 
         * @return          Whether the execution was successful.
         * **/
        public Boolean ExecuteQuery()
        {
            Boolean retval = true;

            if (!String.IsNullOrEmpty(query))
            {
                switch (QueryType)
                {
                    case StatementType.DELETE:
                    case StatementType.INSERT:
                    case StatementType.UPDATE:
                        retval = ExecuteNonQuery();
                        break;
                    case StatementType.SELECT:
                        retval = SetupReader();
                        break;
                    default:
                        retval = false;
                        break;
                }
            }
            else
            {
                retval = false;
            }
            return retval;
        }

        /**
         * Retrieves the next record from a SELECT query.
         * 
         * @return          Whether the read was successful.
         * **/
        public Boolean NextRecord()
        {
            Boolean retval = false;

            if (readerIsSetup)
                retval = reader.Read();

            return retval;
        }

        /**
         * Retrieves column data as a string given exact column name as an input.
         * 
         * @return          Column data value as a string. Null return may be either empty value or invalid column name.
         * **/
        public string ColumnToString(string exactColumnName)
        {
            string retval = null;

            if (readerIsSetup)
            {
                for (int i = 0; i < fieldCount; i++)
                    if (String.Compare(columnNames[i], exactColumnName) == 0)
                        //                        retval = reader.ToString(i);
                        switch (reader.GetDataTypeName(i))
                        {
                            case "int":
                                retval = reader.GetInt32(i).ToString();
                                break;
                            case "string":
                            default:
                                retval = reader.GetString(i);
                                break;
                        }
            }
            return retval;
        }

        /* private methods */
        private Boolean ExecuteNonQuery()
        {
            Boolean retval = true;
            try
            {
                this.connection.Open();
                using (cmd = new SqlCommand(query, connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                retval = false;
            }
            finally
            {
                connection.Close();
            }
            return retval;
        }

        private Boolean SetupReader()
        {
            Boolean retval = true;
            try
            {
                connection.Open();
                using (cmd = new SqlCommand(query, connection))
                {
                    reader = cmd.ExecuteReader();
                    readerIsSetup = true;
                    fieldCount = reader.FieldCount;

                    if (fieldCount > 0)
                    {
                        columnNames = new string[fieldCount];

                        for (int i = 0; i < fieldCount; i++)
                            columnNames[i] = reader.GetName(i);
                    }
                }
            }
            catch
            {
                retval = false;
                connection.Close();
            }
            return retval;
        }

    }
}

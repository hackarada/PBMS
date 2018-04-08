using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    /// <summary>
    /// this is A General Purpose DataAcess Class for all application Needs
    /// </summary>
    public class DataAccess
    {
        private SqlConnection _connection;
        private SqlCommand _command;

        private static string ConnectionString
        {
            get
            {
                string ConnectionString1 = (ConfigurationManager.ConnectionStrings["DALconnection"]).ToString();
                return ConnectionString1;
            }
        }

        private void Init(string commandText)
        {
            createCommand();
            _command.CommandText = commandText;
        }

        private SqlCommand createCommand()
        {
            if (_command == null)
            {
                _command = new SqlCommand();
            }
            else
            {
                if (_command.Transaction != null)
                {
                    return _command;
                }
                else
                {
                    _command = new SqlCommand();
                }
            }

            _command.Connection = createConnection();
            return _command;
        }

        private SqlConnection createConnection()
        {
            if (_connection == null)
            {
                _connection = new SqlConnection();
                _connection.ConnectionString = ConnectionString;
                _connection.Open();
            }
            else
            {
                if (_connection.State != ConnectionState.Open)
                {
                    _connection.Open();
                }
            }

            return _connection;
        }

        public IDataReader loadDataByText(string commandText)
        {
            try
            {
                if (commandText.Equals(string.Empty))
                    return null;

                Init(commandText);
                _command.CommandType = CommandType.Text;

                return _command.ExecuteReader();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IDataReader loadDataByText(string commandText, SqlParameter[] paramList)
        {
            try
            {
                if (commandText.Equals(string.Empty) || paramList == null)
                    return null;

                Init(commandText);
                _command.CommandType = CommandType.Text;
                _command.Parameters.AddRange(paramList);

                return _command.ExecuteReader();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                //closeConnection();
            }
        }

        public void saveByText(string commandText, SqlParameter[] paramList)
        {
            try
            {
                //  int status = 0;
                if (commandText.Equals(string.Empty) || paramList == null)
                    return;

                Init(commandText);
                _command.CommandType = CommandType.Text;
                _command.Parameters.Clear();
                _command.Parameters.AddRange(paramList);

                _command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void ExcecuteProcedure(string ProcedureName, SqlParameter[] paramList)
        {
            try
            {
                //  int status = 0;
                if (ProcedureName.Equals(string.Empty) || paramList == null)
                    return;

                Init(ProcedureName);
                _command.CommandType = CommandType.StoredProcedure;
                _command.Parameters.Clear();
                _command.Parameters.AddRange(paramList);

                _command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void ExcecuteProcedure(string ProcedureName)
        {
            try
            {
                //  int status = 0;
                if (ProcedureName.Equals(string.Empty))
                    return;

                Init(ProcedureName);
                _command.CommandType = CommandType.StoredProcedure;
                _command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void saveByText(string commandText)
        {
            //    int affected;
            try
            {
                if (commandText.Equals(string.Empty))
                    return;
                Init(commandText);
                _command.CommandType = CommandType.Text;
                _command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public object ExcecuteScalar(string commandText, SqlParameter[] paramlist)
        {
            object x = new object();
            try
            {
                if (commandText.Length != 0)
                {
                    Init(commandText);
                    _command.CommandType = CommandType.Text;
                    _command.Parameters.AddRange(paramlist);
                    x = (_command.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return x;
        }

        public object ExcecuteScalar(string commandText)
        {
            object x = new object();
            try
            {
                if (commandText.Length != 0)
                {
                    Init(commandText);
                    _command.CommandType = CommandType.Text;

                    x = (_command.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return x;
        }

        public void closeConnection()
        {
            try
            {
                if (_connection == null)
                    return;

                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
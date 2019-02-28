using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FingerprintClass
{
    class Repositorio<T> where T : class

    {
        public SqlConnection con;

        private void connection()
        {
            con = new SqlConnection($@"{ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString}");
        }

        public List<T> returnListClass(string query, DynamicParameters param)
        {
            try
            {
                connection();
                con.Open();
                IList<T> Tlista = SqlMapper.Query<T>(con, query, param, null, true, null, commandType: CommandType.Text).ToList();
                con.Close();
                return Tlista.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public T returnClass(string query, DynamicParameters param)
        {
            try
            {
                connection();
                con.Open();

                T Tlista = SqlMapper.Query<T>(con, query, param, null, true, null, commandType: CommandType.Text).FirstOrDefault();

                con.Close();

                return Tlista;

            }

            catch (Exception)

            {

                throw;

            }

        }

        public int ExecuteNonQuery(string sql, dynamic param)
        {
            try
            {
                connection();
                con.Open();
                int resultado = SqlMapper.Execute(con, sql, param);
                con.Close();
                return resultado;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}

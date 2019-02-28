using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportarArquivosLote
{
    public enum BancoUtilizado
    {
        PortalEducacional,
        WebEscola
    }

    class Repositorio<T> where T : class

    {
        public SqlConnection con;
        private string _connectionString;

        public Repositorio(BancoUtilizado banco)
        {
            _connectionString = banco == BancoUtilizado.WebEscola ? "ConnectionStringWebEscola": "ConnectionStringPortal";
        }

        private void connection()
        {
            con = new SqlConnection($@"{ConfigurationManager.ConnectionStrings[_connectionString].ConnectionString}");
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

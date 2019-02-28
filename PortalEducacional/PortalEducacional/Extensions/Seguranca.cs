using System;
using System.Data;
using System.Linq;
using PortalEducacional.Data;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using PortalEducacional.Models;

namespace PortalEducacional.Extensions
{
    public class Seguranca
    {
        private readonly ApplicationDbContext _context;

        public Seguranca(ApplicationDbContext context)
        {
            _context = context;
        }

        public static DataTable RetornaPermissoesTelas()
        {
            using (var dsPastas = new DataSet())
            {
                using (var dtPermissoes = new DataTable())
                {
                    using (var dataColumn = new DataColumn("Classe", Type.GetType("System.String")))
                    {
                        dtPermissoes.Columns.Add(dataColumn);
                    }
                    using (var dataColumn = new DataColumn("Modulo", Type.GetType("System.String")))
                    {
                        dtPermissoes.Columns.Add(dataColumn);
                    }
                    using (var dataColumn = new DataColumn("Menu", Type.GetType("System.String")))
                    {
                        dtPermissoes.Columns.Add(dataColumn);
                    }
                    using (var dataColumn = new DataColumn("Descricao", Type.GetType("System.String")))
                    {
                        dtPermissoes.Columns.Add(dataColumn);
                    }
                    using (var dataColumn = new DataColumn("Permissoes", Type.GetType("System.String")))
                    {
                        dtPermissoes.Columns.Add(dataColumn);
                    }

                    var asm = Assembly.GetExecutingAssembly();

                    var controller = asm.GetTypes()
                                .Where(type => typeof(Controller).IsAssignableFrom(type)).ToList();

                    foreach (var item in controller)
                    {
                        var attributes = item.GetCustomAttributes();

                        foreach (var item2 in attributes)
                        {
                            if (item2.GetType().Name == typeof(Permissao).GetTypeInfo().Name)
                            {
                                var drPermissao = dtPermissoes.NewRow();
                                drPermissao["Classe"] = ((Permissao)item2).Controller;
                                drPermissao["Modulo"] = ((Permissao)item2).Modulo;
                                drPermissao["Menu"] = ((Permissao)item2).Menu;
                                drPermissao["Descricao"] = ((Permissao)item2).DescricaoTela;
                                drPermissao["Permissoes"] = ((Permissao)item2).Acao;
                                dtPermissoes.Rows.Add(drPermissao);
                                break;
                            }
                        }
                    }

                    return dtPermissoes.Select("", "Modulo, Menu, Descricao").CopyToDataTable();
                }
            }
        }
    }
}

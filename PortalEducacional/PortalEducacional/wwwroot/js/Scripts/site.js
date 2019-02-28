function pageLoad() {
    DataTable();
    maskInit();
    Select2();
    SomenteNumeros(this);
}

//MASCARAS
function maskInit() {
    $(".mask-cep").mask('99999-999');
    $(".mask-cep").attr("placeholder", "99999-999");

    $(".mask-cpf").mask('999.999.999-99');
    $(".mask-cpf").attr("placeholder", "999.999.999-99");

    $(".mask-cnpj").mask('99.999.999/9999-99');
    $(".mask-cnpj").attr("placeholder", "99.999.999/9999-99");

    $('.mask-data').mask('99/99/9999', { clearIfNotMatch: true });
    $('.mask-dataMesAno').mask('99/9999', { clearIfNotMatch: true });

    $('.mask-altura').mask("#0,00", { reverse: true });
    $('.mask-peso').mask("#00,0", { reverse: true });

    $('.monetario').mask("#9,99", { reverse: true, maxlength: false });

    $(".mask-telefone").mask('(00) 0000-00009');
    $(".mask-telefone").attr("placeholder", "(99) 99999-9999");
    $(".mask-telefone").blur(function (event) {
        var target = (event.currentTarget) ? event.currentTarget : event.srcElement;
        var phone = target.value.replace(/\D/g, '');
        var element = $(target);
        element.unmask();
        if (phone.length == 11) {
            element.mask("(00) 00000-0009");
        } else {
            element.mask("(00) 0000-00009");
        }
    });
}

//VALIDA CPF QUANDO O CAMPO PERDE O FOCO
$(function () {
    $("#CPF").blur(function () {

        if ($("#CPF").val() === "")
            return false;

        if (!validarCPF($("#CPF").val())) {
            swal({
                title: "ATENÇÃO!",
                text: "CPF INVÁLIDO: " + $("#CPF").val(),
                icon: "error",
            });
            $("#CPF").val("");
            return false;
        }
    });
});

//VALIDA CPF
function validarCPF(cpf) {

    cpf = cpf.replace(/[^\d]+/g, '');

    if (cpf.length != 11 ||
        cpf == "00000000000" ||
        cpf == "11111111111" ||
        cpf == "22222222222" ||
        cpf == "33333333333" ||
        cpf == "44444444444" ||
        cpf == "55555555555" ||
        cpf == "66666666666" ||
        cpf == "77777777777" ||
        cpf == "88888888888" ||
        cpf == "99999999999")
        return false;

    add = 0;

    for (i = 0; i < 9; i++)
        add += parseInt(cpf.charAt(i)) * (10 - i);
    rev = 11 - (add % 11);
    if (rev == 10 || rev == 11)
        rev = 0;
    if (rev != parseInt(cpf.charAt(9)))
        return false;
    add = 0;
    for (i = 0; i < 10; i++)
        add += parseInt(cpf.charAt(i)) * (11 - i);
    rev = 11 - (add % 11);
    if (rev == 10 || rev == 11)
        rev = 0;
    if (rev != parseInt(cpf.charAt(10)))
        return false;

    return true;
}

//VALIDA CNPJ QUANDO O CAMPO PERDE O FOCO
$(function () {
    $("#CNPJ").blur(function () {

        if ($("#CNPJ").val() === "")
            return false;

        if (!validarCNPJ($("#CNPJ").val())) {
            swal({
                title: "ATENÇÃO!",
                text: "CNPJ INVÁLIDO: " + $("#CNPJ").val(),
                icon: "error",
            });
            $("#CNPJ").val("");
            return false;
        }
    });
});

//VALIDA CNPJ
function validarCNPJ(cnpj) {

    cnpj = cnpj.replace(/[^\d]+/g, '');

    if (cnpj == '') return false;

    if (cnpj.length != 14)
        return false;

    // Elimina CNPJs invalidos conhecidos
    if (cnpj == "00000000000000" ||
        cnpj == "11111111111111" ||
        cnpj == "22222222222222" ||
        cnpj == "33333333333333" ||
        cnpj == "44444444444444" ||
        cnpj == "55555555555555" ||
        cnpj == "66666666666666" ||
        cnpj == "77777777777777" ||
        cnpj == "88888888888888" ||
        cnpj == "99999999999999")
        return false;

    // Valida DVs
    tamanho = cnpj.length - 2
    numeros = cnpj.substring(0, tamanho);
    digitos = cnpj.substring(tamanho);
    soma = 0;
    pos = tamanho - 7;
    for (i = tamanho; i >= 1; i--) {
        soma += numeros.charAt(tamanho - i) * pos--;
        if (pos < 2)
            pos = 9;
    }
    resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
    if (resultado != digitos.charAt(0))
        return false;

    tamanho = tamanho + 1;
    numeros = cnpj.substring(0, tamanho);
    soma = 0;
    pos = tamanho - 7;
    for (i = tamanho; i >= 1; i--) {
        soma += numeros.charAt(tamanho - i) * pos--;
        if (pos < 2)
            pos = 9;
    }
    resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
    if (resultado != digitos.charAt(1))
        return false;

    return true;

}

//VALIDA EMAIL QUANDO O CAMPO PERDE O FOCO
$(function () {
    $("#Email").blur(function () {

        if ($("#Email").val() === "")
            return false;

        if (!validateEmail($("#Email").val())) {
            swal({
                title: "ATENÇÃO!",
                text: "E-MAIL INVÁLIDO: " + $("#Email").val(),
                icon: "error",
            });
            $("#Email").val("");
            return false;
        }
    });
});

//MASCARA PARA VALIDAR EMAIL
function validateEmail(email) {
    var expr = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
    return expr.test(email);
};

//SOMENTE NÚMEROS
function SomenteNumeros(num) {
    var er = /[^0-9]/;
    er.lastIndex = 0;
    var campo = num;
    if (er.test(campo.value)) {
        campo.value = "";
    }
}

//DATA_TABLE
function DataTable() {
    $(".datatable").dataTable({
        "sDom": "<'top'i><'dtFilter'f><'dtTables'rt><'dtBottom'<'dtShowPer'l><'dtPagination'p>><'clear'>",
        //"bStateSave": true,
        "oLanguage": {
            "sLengthMenu": " _MENU_ ",
            "sSearch": "",
            "oPaginate": {
                "sFirst": "Primeiro",
                "sPrevious": "Anterior",
                "sNext": "Pr&oacute;ximo",
                "sLast": "&Uacute;ltimo"
            },
            "sZeroRecords": "Nenhum registro encontrado de acordo com a sua pesquisa.",
            "sEmptyTable": "Nenhum registro Cadastrado!",
            "sInfo": "Visualizando _START_ até _END_ de _TOTAL_ registros:",
            "sInfoEmpty": "Visualizando 0 até 0 de 0 registros.",
            "sInfoFiltered": "(Pesquisados em _MAX_ registros no Total.)",
            "sInfoPostFix": "",
            "sInfoThousands": ","
        },
        "bSort": false,
        //"aaSorting": [],
        "iDisplayLength": 10,
        "bDestroy": true
    });

    $('.dataTables_filter input').addClass('form-control').attr('placeholder', 'PESQUISAR NA TABELA');
    $('.dataTables_length select').addClass('form-control');
}

//Select2
function Select2() {
    if ($(".select2").length > 0) {
        $(".select2").select2('destroy');
        $(".select2").select2({
            width: '100%'
        });
    }
}

//Tooltips
function Toltips() {
    $('.ttip, [data-toggle="tooltip"]').tooltip();
}

function VerificaQtdeProduto(id) {
    $.ajax({
        type: "POST",
        url: "/Estoque/RetornaControleProduto",
        dataType: "json",
        data: { Dado: id },
        success: function (data) {
            var res = jQuery.parseJSON(JSON.stringify(data));

            if (res.length === 0) {
                $("#EstoqueEmpresa_ValorCompra").val('');
                $("#EstoqueEmpresa_QuantidadeMinima").val(1);
                $("#EstoqueEmpresa_QuantidadeMaxima").val('');
                $("#EstoqueEmpresa_Validade").val('');
            }
            else {
                $("#EstoqueEmpresa_ValorCompra").val(res[0].ValorCompra);
                $("#EstoqueEmpresa_QuantidadeMinima").val(res[0].QuantidadeMinima);
                $("#EstoqueEmpresa_QuantidadeMaxima").val(res[0].QuantidadeMaxima);
                $("#EstoqueEmpresa_Validade").val(res[0].Validade);
            }
        },
        error: function (xhr, err) {
            $("#EstoqueEmpresa_ValorCompra").val('');
            $("#EstoqueEmpresa_QuantidadeMinima").val(1);
            $("#EstoqueEmpresa_QuantidadeMaxima").val('');
            $("#EstoqueEmpresa_Validade").val('');
        }
    })
}

function SubstituirAcentosEmPalavras(palavra) {
    palavra = palavra.replace(new RegExp('[àáâãäå]', 'g'), 'a');
    palavra = palavra.replace(new RegExp('[èéêë]', 'g'), 'e');
    palavra = palavra.replace(new RegExp('[íìî]', 'g'), 'i');
    palavra = palavra.replace(new RegExp('[óòôõ]', 'g'), 'o');
    palavra = palavra.replace(new RegExp('[úùû]', 'g'), 'u');
    palavra = palavra.replace(new RegExp('[ÀÁÂÃÄÅ]', 'gi'), 'A');
    palavra = palavra.replace(new RegExp('[ÈÉÊË]', 'gi'), 'E');
    palavra = palavra.replace(new RegExp('[ÍÌÎ]', 'gi'), 'I');
    palavra = palavra.replace(new RegExp('[ÓÒÔÕ]', 'gi'), 'O');
    palavra = palavra.replace(new RegExp('[ÚÙÛ]', 'gi'), 'U');
    return palavra;
}

function CalculoIMC() {
    var peso = $("#Peso").val();
    var altura = $("#Altura").val();

    if (peso == null && altura == null) return;

    var altura_new = altura.replace(',', '.') * 0.01; //CM - MT

    var resultado = peso.replace(',', '.') / (altura_new * altura_new);

    document.getElementById("Resultado").value = parseFloat(resultado.toFixed(2));

}

$(document).ready(function () {

    var data = new Date().toLocaleDateString();
    if (data != null) {
        if (document.getElementById("DataCadastro") != null) {
            document.getElementById("DataCadastro").value = data;
        }
        else if (document.getElementById("Estoque_DataCadastro") != null) {
            document.getElementById("Estoque_DataCadastro").value = data;
        }
    }
});

//AUTO_COMPLETE_ESCOLA
$(document).ready(function () {
    $("#Escola").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: "/Escola/AutoComplete",
                type: "POST",
                dataType: "json",
                data: { Dado: request.term },
                success: function (data) {
                    response($.map(data, function (item) {
                        if (data.length === 0) {
                            return { label: "No Results." }
                        } else {
                            return { label: item.CNPJ + " | " + item.Nome, value: item.Nome, id: item.EscolaID };
                        }
                    }))
                }
            })
        },
        select: function (event, ui) {
            $("#Escola_EscolaID").val(ui.item.id);
            $("#Aluno_Escola_EscolaID").val(ui.item.id);
        }
    });
})

//AUTO_COMPLETE_COD_PRODUTO
$(document).ready(function () {
    $("#CodProduto").autocomplete({
        source: function (request, response) {

            var idescola = document.getElementById('Estoque_EscolaID').value;

            if (idescola === undefined)
                idescola = 0;

            $.ajax({
                url: "/Produto/AutoComplete",
                type: "POST",
                dataType: "json",
                data: { Dado: request.term, EscolaID: idescola },
                success: function (data) {
                    response($.map(data, function (item) {
                        if (data.length === 0) {
                            return { label: "No Results." }
                        } else {
                            return { label: item.Codigo + " | " + item.Descricao, value: item.Codigo, text: item.Descricao, id: item.ProdutoID };
                        }
                    }))
                }
            })
        },
        select: function (event, ui) {

            if (document.getElementById("Estoque_Produto_Descricao") != null) {
                $("#Estoque_Produto_Descricao").val(ui.item.text);
                $("#Estoque_Produto_ProdutoID").val(ui.item.id);

                VerificaQtdeProduto(ui.item.id);
            }
            else {
                $("#Produto_Descricao").val(ui.item.text);
                $("#Produto_ProdutoID").val(ui.item.id);
            }
        }
    });
})

//AUTO_COMPLETE_ALUNO
$(document).ready(function () {

    $("#Aluno").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: "/Aluno/AutoComplete",
                type: "POST",
                dataType: "json",
                data: { Dado: request.term, IDescola: $("#Aluno_Escola_EscolaID").val() },
                success: function (data) {
                    response($.map(data, function (item) {
                        if (data.length === 0) {
                            return { label: "No Results." }
                        } else {
                            return { label: item.RA + " | " + item.NomeAluno, value: item.NomeAluno, id: item.IdAluno, EscolaID: item.IdEscola, EscolaNome: item.NomeEscola, Sexo: item.Sexo, Serie: item.Serie, IdSerie: item.IdSerie, Nascimento: item.Nascimento };
                        }
                    }))
                }
            })
        },
        select: function (event, ui) {
            $("#Aluno_AlunoID").val(ui.item.id);
            $("#Escola").val(ui.item.EscolaNome);
            $('#Escola').attr('readonly', true);
            $("#Serie").val(ui.item.Serie);
            $("#Nascimento").val(ui.item.Nascimento);
            $("#Sexo").val(ui.item.Sexo);
        }

    });

})

//ADD Nutrição no Produto
$(document).ready(function () {
    var i = 0;
    $("#btnAdd").click(function () {
        var porcao = $("#Porcao").val();
        var valorDiario = $("#ValorDiario").val();
        var tipoNutricional = $("#TipoNutricional option:selected").text();
        var tipoNutricional_val = $("#TipoNutricional").val();
        if (tipoNutricional_val === "--") return;
        var tr = $('#tbNutricao tbody > tr:last');
        if (tr.length != 0) {
            var identificador = tr.attr('id').substr(7);
            if (identificador != null) i = parseInt(identificador) + 1;
        }


        $('#tbNutricao tbody').append('<tr id="LinhaTR' + i + '">' +
            '<input type="hidden" name="DadosNutricionais[' + i + '].Porcao" value="' + porcao + '"/>' +
            '<input type="hidden" name="DadosNutricionais[' + i + '].ValorDiario" value="' + valorDiario + '"/>' +
            '<input type="hidden" name="DadosNutricionais[' + i + '].TipoNutricionalID" value="' + tipoNutricional_val + '"/>' +
            '<input id="Deletado' + i + '" type="hidden" name="DadosNutricionais[' + i + '].Deletado" value="false" />' +
            '<td><a class="btn btn-danger btn-trans btn-xs" onclick="remove(this)"><i class="fa fa-remove"></i></a></td>' +
            '<td> ' + tipoNutricional + '</td > <td>' + porcao + '</td> <td>' + valorDiario + '</td>' +
            '</tr > '
        );
        $("#TipoNutricional").val($("#TipoNutricional option:first").val());
        $("#ValorDiario").val('');
        $("#Porcao").val('');
    });

    (function ($) {
        remove = function (item) {
            var tr = $(item).closest('tr');
            var identificador = tr.attr('id').substr(7);
            $("#Deletado" + identificador).val('true');
            tr.fadeOut(400, function () { tr.hide(); });
            return false;
        }
    })(jQuery);

    $(".onlyNumber").keydown(function (e) {
        // Allow: backspace, delete, tab, escape, enter and .
        if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
            // Allow: Ctrl+A, Command+A
            (e.keyCode === 65 && (e.ctrlKey === true || e.metaKey === true)) ||
            // Allow: home, end, left, right, down, up
            (e.keyCode >= 35 && e.keyCode <= 40)) {
            // let it happen, don't do anything
            return;
        }
        // Ensure that it is a number and stop the keypress
        if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
            e.preventDefault();
        }
    });

});

//ADD RESPONSÁVEL_FINANCEIRO
$(document).ready(function () {
    var i = 0;
    $("#btnAddResponsavel").click(function () {

        if ($("#CPF").val() == "" || $("#Nome").val() == "" || $("#Email").val() == "" || $("#Telefone").val() == "") {
            swal({
                title: "ATENÇÃO!",
                text: "Todos os campos devem ser preenchidos!",
                icon: "error",
            });
        }
        else {

            var cpf = $("#CPF").val();
            var nome = $("#Nome").val();
            var email = $("#Email").val();
            var telefone = $("#Telefone").val();
            var data = $("#DataCadastro").val();
            var tr = $('#tbody-responsavel tbody > tr:last');
            if (tr.length != 0) {
                var identificador = tr.attr('id').substr(7);
                if (identificador != null) i = parseInt(identificador) + 1;
            }
            $('#tbody-responsavel').append('<tr id="LinhaTR' + i + '">' +
                '<input type="hidden" name="ResponsavelFinanceiro[' + i + '].CPF" value="' + cpf + '"/>' +
                '<input type="hidden" name="ResponsavelFinanceiro[' + i + '].Nome" value="' + nome + '"/>' +
                '<input type="hidden" name="ResponsavelFinanceiro[' + i + '].Email" value="' + email + '"/>' +
                '<input type="hidden" name="ResponsavelFinanceiro[' + i + '].Telefone" value="' + telefone + '"/>' +
                '<input type="hidden" name="ResponsavelFinanceiro[' + i + '].DataCadastro" value="' + data + '"/>' +
                '<input id="Deletado' + i + '" type="hidden" name="ResponsavelFinanceiro[' + i + '].Deletado" value="false" />' +
                '<td> <a class="btn btn-trans btn-danger btn-xs" onclick="remove(this)"><i class="fa fa-remove"></i></a> </td>' +
                '<td> ' + cpf + '</td > <td>' + nome + '</td> <td>' + email + '</td> <td>' + telefone + ' </td></tr > ');
            i++;

            $("#CPF").val("");
            $("#Nome").val("");
            $("#Email").val("");
            $("#Telefone").val("");
        }
    });

    (function ($) {
        remove = function (item) {
            var tr = $(item).closest('tr');
            var identificador = tr.attr('id').substr(7);
            $("#Deletado" + identificador).val('true');
            tr.fadeOut(400, function () { tr.hide(); });
            return false;
        }
    })(jQuery);
});

/*
 * AGUARDE...
 */
$(document).ready(function () {
    $('#btnPesquisar').click(
        function () {
            $('#inProgress').show();
            $('#inProgress2').show();
            $.get('<%= Url.Action("UpdateResource") %>', {},
                function (data) {
                    $('#result').html(data);
                    $('#inProgress').hide();
                    $('#inProgress2').hide();
                });
        }
    );
    $("#btnSalvar").click(
        function () {
            $('#inProgress').show();
            $('#inProgress2').show();
            $.get('<%= Url.Action("UpdateResource") %>', {},
                function (data) {
                    $('#result').html(data);
                    $('#inProgress').hide();
                    $('#inProgress2').hide();
                });
        });
    $("#Salvar").click(
        function () {
            $('#inProgress').show();
            $('#inProgress2').show();
            $.get('<%= Url.Action("UpdateResource") %>', {},
                function (data) {
                    $('#result').html(data);
                    $('#inProgress').hide();
                    $('#inProgress2').hide();
                });
        });
});

/*
*CONTROLE PERMISSÕES
*/
function MarcaOpcaoFuncionalidade(Object) {

    var id = Object.id;

    var opcao = id.toString().substring(0, 6);

    var i = id.toString().substr(6, 3);

    if (opcao === "bloque") {
        document.getElementById(id).className = "btn btn-danger btn-xs";

        if (document.getElementById('editar' + i).className != "btn btn btn-default btn-trans btn-xs")
            document.getElementById('editar' + i).className = "btn btn btn-default btn-trans btn-xs";

        $("#status" + i).val("BLOQUEADO");
    }

    if (opcao === "editar") {
        document.getElementById(id).className = "btn btn-success btn-xs";

        if (document.getElementById('bloque' + i).className != "btn btn btn-default btn-trans btn-xs")
            document.getElementById('bloque' + i).className = "btn btn btn-default btn-trans btn-xs";

        $("#status" + i).val("EDITAR");
    }
}

function MarcaTodos(Object) {
    var opcao = Object.id.toString();

    var i = 0;
    $('#tbody-funcionalidades tr').each(function () {

        if (opcao === "aBloquearTodos") {
            document.getElementById('bloque' + i).className = "btn btn-danger btn-xs";
            document.getElementById('editar' + i).className = "btn btn-default btn-xs";

            $("#status" + i).val("BLOQUEADO");
        }

        if (opcao === "aEditarTodos") {
            document.getElementById('editar' + i).className = "btn btn-success btn-xs";
            document.getElementById('bloque' + i).className = "btn btn-default btn-xs";

            $("#status" + i).val("EDITAR");
        }

        i++;
    });
}

function BuscaPermissoesPorPerfil() {

    var id = $("#PerfilID").val();

    if (id === "0")
        return;

    $.ajax({
        type: "POST",
        url: "/ApplicationUser/RetornaPermissoes",
        dataType: "json",
        data: { Dado: id },
        success: function (data) {
            var res = jQuery.parseJSON(JSON.stringify(data));

            var l = res.length;

            for (var i = 0; i < l; i++) {

                var permissao = res[i].Permissao;
                var controller = res[i].Controller;

                $('#tbody-funcionalidades tr').each(function () {

                    var tela = $(this).find('input#tela' + i + ':nth-child(3)').val();

                    if (tela === controller) {

                        if (permissao === "BLOQUEADO") {
                            document.getElementById('bloque' + i).className = "btn btn-danger btn-xs";

                            if (document.getElementById('editar' + i).className != "btn btn btn-default btn-trans btn-xs")
                                document.getElementById('editar' + i).className = "btn btn btn-default btn-trans btn-xs";

                            $("#status" + i).val("BLOQUEADO");
                        }

                        if (permissao === "EDITAR") {
                            document.getElementById('editar' + i).className = "btn btn-success btn-xs";

                            if (document.getElementById('bloque' + i).className != "btn btn btn-default btn-trans btn-xs")
                                document.getElementById('bloque' + i).className = "btn btn btn-default btn-trans btn-xs";

                            $("#status" + i).val("EDITAR");
                        }
                    }
                });
            }
        },
        error: function (xhr, err) {
            swal({
                title: "ATENÇÃO!",
                text: "Erro ao consultar permissões. Entre em contato com o Suporte Técnico!",
                icon: "error",
            });
        }
    })
}

/*
 *MODULO CANTINA
 */

//CARREGA CATEGORIAS
$(document).ready(function () {
    var id = 0;
    var alunoID = $("#Aluno_AlunoID").val();

    if (document.getElementById("listaCategoria") != null) {
        $.ajax({
            type: "POST",
            url: "/Cantina/RetornaCategoria",
            dataType: "json",
            data: { Dado: id, AlunoID: alunoID },
            success: function (data) {
                var res = jQuery.parseJSON(JSON.stringify(data));

                var l = res.length;

                $('#listaCategoria').append('<div class="icone-categoria" id="0" onclick="VerificaPorCategoria(this);">' +
                    '<img src="../images/Cantina/icone-all.png" />' +
                    '<label class="control-label text-center" id="0" onclick="VerificaPorCategoria(this);">TODOS</label>' +
                    '</div>');

                for (var i = 0; i < l; i++) {

                    var id = res[i].CategoriaID;
                    var descricao = res[i].Descricao;
                    var imagem = res[i].Imagem;

                    //$('#tbody-categorias').append('<tr>' +
                    //    '<td> <a class="btn btn-info btn-trans btn-xs" id="' + id + '" onclick="VerificaPorCategoria(this);"><i class="fa fa-search"></i></a> </td>' +
                    //    '<td> ' + descricao + '</td ></tr> ');

                    $('#listaCategoria').append('<div class="icone-categoria" id="' + id + '" onclick="VerificaPorCategoria(this);">' +
                        '<img src="' + imagem + '" />' +
                        '<label class="control-label text-center" id="' + id + '" onclick="VerificaPorCategoria(this);">' + descricao + '</label>' +
                        '</div>');
                }

                VerificaPorCategoria(null);
            },
            error: function (xhr, err) {
                swal({
                    title: "ATENÇÃO!",
                    text: "Erro ao consultar produtos. Entre em contato com o Suporte Técnico!",
                    icon: "error",
                });
            }
        })
    }
});

//ADD PRODUTO PDV_CANTINA
function AdicionaProdutoCantina(Object, id = null) {

    var identificador = "";

    if (Object === null)
        identificador = id.toString();
    else
        identificador = Object.id;

    $.ajax({
        type: "POST",
        url: "/Cantina/RetornaDadosProduto",
        dataType: "json",
        data: { Dado: identificador },
        success: function (data) {
            var res = jQuery.parseJSON(JSON.stringify(data));

            if (res.length === 0) {
                return null;
            }
            else {
                var id = res[0].ProdutoID;
                var codigo = res[0].Codigo;
                var descricao = res[0].Descricao;
                var valor = res[0].Valor.toString().replace('.', ',');
                var qtde = "1";
                var idProduto = 0;

                //Verifica qtde de linha na tbody
                var tabela = document.getElementById('tbody-produtos');
                var linhas = tabela.getElementsByTagName('tr');
                var i = linhas.length;

                //td numeros - posição
                //Ação - 7
                //Produto - 8
                //Qtde - 9
                //Valor - 10
                //SubTotal - 11
                //Id - 12

                //caso linha = 0, só adiciona | se não insere quantidade já existente na linha + 1;
                if (linhas.length === 0) {
                    $('#tbody-produtos').append('<tr id="LinhaTR' + i + '">' +
                        '<input type="hidden" name="PedidoItens[' + i + '].Produto.ProdutoID" value="' + id + '"/>' +
                        '<input type="hidden" name="PedidoItens[' + i + '].Produto.Codigo" value="' + codigo + '"/>' +
                        '<input type="hidden" name="PedidoItens[' + i + '].Produto.Descricao" value="' + descricao + '"/>' +
                        '<input id="idProdValor' + id + '" type="hidden" name="PedidoItens[' + i + '].Valor" value="' + valor + '"/>' +
                        '<input id="idProdQtde' + id + '" type="hidden" name="PedidoItens[' + i + '].Quantidade" value="' + qtde + '"/>' +
                        '<input id="Deletado' + i + '" type="hidden" name="PedidoItens[' + i + '].Deletado" value="false" />' +
                        '<td> <a class="btn btn-trans btn-danger btn-xs" onclick="RemoveItemCantina(this, ' + id +')"><i class="fa fa-remove"></i></a> </td>' +
                        '<td> ' + codigo + ' - ' + descricao + '</td > <td>' + qtde + '</td> <td>' + valor + ' </td> <td>' + valor + ' </td> <td id="idProd' + id + '">' + id + ' </td></tr> ');

                    $('#idProd' + id + '').hide();

                    CalculaValoresFinais(null);
                }
                else {
                    var i = 0;
                    var exists = 0;

                    $('#tbody-produtos tr').each(function () {

                        idProduto = $(this).find('td:nth-child(12)').text().replace(' ', '');//ID

                        var nomeDeletado = '#Deletado' + i;

                        var deletado = $(this).find('input#Deletado' + i + ':nth-child(6)').val();

                        if (identificador === idProduto) {
                            var quantidade = parseFloat($(this).find('td:nth-child(9)').text());
                            var valorUnitario = parseFloat($(this).find('td:nth-child(10)').text().replace(',', '.'));
                            var subTotal = parseFloat($(this).find('td:nth-child(11)').text().replace(',', '.'));
                            var qtdeAtualizada = quantidade + 1;
                            var total = subTotal + valorUnitario;

                            document.querySelectorAll("td:nth-child(9)")[i].innerText = qtdeAtualizada;
                            document.querySelectorAll("td:nth-child(11)")[i].innerText = total.toString().replace('.', ',');

                            if (deletado === "true") {
                                $(nomeDeletado).val('false');

                                var tr = $(this).closest('tr');
                                tr.fadeIn(400, function () { tr.show(); });
                            }

                            $('#idProdValor' + id + '').val(total.toString().replace('.', ','));
                            $('#idProdQtde' + id + '').val(qtdeAtualizada);

                            CalculaValoresFinais(null);

                            exists++;
                        }

                        i++;
                    });

                    if (exists === 0) {
                        $('#tbody-produtos').append('<tr id="LinhaTR' + i + '">' +
                            '<input type="hidden" name="PedidoItens[' + i + '].Produto.ProdutoID" value="' + id + '"/>' +
                            '<input type="hidden" name="PedidoItens[' + i + '].Produto.Codigo" value="' + codigo + '"/>' +
                            '<input type="hidden" name="PedidoItens[' + i + '].Produto.Descricao" value="' + descricao + '"/>' +
                            '<input id="idProdValor' + id + '" type="hidden" name="PedidoItens[' + i + '].Valor" value="' + valor + '"/>' +
                            '<input id="idProdQtde' + id + '" type="hidden" name="PedidoItens[' + i + '].Quantidade" value="' + qtde + '"/>' +
                            '<input id="Deletado' + i + '" type="hidden" name="PedidoItens[' + i + '].Deletado" value="false" />' +
                            '<td> <a class="btn btn-trans btn-danger btn-xs" onclick="RemoveItemCantina(this, '+ id +')"><i class="fa fa-remove"></i></a> </td>' +
                            '<td> ' + codigo + ' - ' + descricao + '</td > <td>' + qtde + '</td> <td>' + valor + ' </td> <td>' + valor + ' </td> <td id="idProd' + id + '">' + id + ' </td></tr > ');

                        $('#idProd' + id + '').hide();

                        CalculaValoresFinais(null);
                    }
                }
            }
        },
        error: function (xhr, err) {
            return null;
        }
    })
}

//REMOVE ITEM CANTINA_PDV
function RemoveItemCantina(item, idItem) {

    var tr = $(item).closest('tr');
    var identificador = tr.attr('id').substr(7);

    var i = 0;
    var qtde = 0;
    var valor = 0;
    var subTotal = 0;

    $('#tbody-produtos tr').each(function () {

        var element = this;

        var idProduto = element.id.substr(7);

        if (idProduto === identificador) {
            qtde = parseFloat($(this).find('td:nth-child(9)').text());
            subTotal = parseFloat($(this).find('td:nth-child(11)').text().replace(',', '.'));
            valor = parseFloat($(this).find('td:nth-child(10)').text().replace(',', '.'));

            document.getElementById('idProdValor' + idItem).value = subTotal - valor;
            document.getElementById('idProdQtde' + idItem).value = qtde - 1;

            CalculaValoresFinais(null);

            if (qtde > 1) {
                document.querySelectorAll("td:nth-child(9)")[i].innerText = qtde - 1;
                document.querySelectorAll("td:nth-child(11)")[i].innerText = subTotal - valor;

                CalculaValoresFinais(null);
            }
            else {
                document.querySelectorAll("td:nth-child(9)")[i].innerText = 0;
                document.querySelectorAll("td:nth-child(11)")[i].innerText = 0, 00;

                $("#Deletado" + identificador).val('true');
                tr.fadeOut(400, function () { tr.hide(); });

                CalculaValoresFinais(identificador);
            }
        }

        i++;
    });
}

//CALCULA TOTAIS A PAGAR
function CalculaValoresFinais(idProduto) {

    var totalProds = 0;
    var desconto = 0;
    var Total = 0;
    var subTotal = 0;

    $('#tbody-produtos tr').each(function () {

        subTotal = parseFloat($(this).find('td:nth-child(11)').text().replace(',', '.'));

        totalProds = totalProds + subTotal;
        Total = Total + subTotal;
    });

    document.getElementById('totalItens').innerText = totalProds.toString().replace('.', ',');
    document.getElementById('totalPagar').innerText = Total.toString().replace('.', ',');
}

//VENDAS DIA
function VisibleVendasDia(item) {

    var i = 0;
    var tr = $(item).closest('tr');
    var identificador = tr.attr('id').substr(7);

    $('#tbody-vendasDia tr').each(function () {

        if (i === 0) {
            if ($("#LinhaItens" + identificador).is(":visible")) {
                $("#LinhaItens" + identificador).hide();
                $("#mais" + identificador).show();
                $("#menos" + identificador).hide();
            }
            else {
                $("#LinhaItens" + identificador).show();
                $("#mais" + identificador).hide();
                $("#menos" + identificador).show();
            }
        }

        i++;
    });
}

//BUSCA PRODUTO POR CÓDIGO DE BARRAS
function VerificaCodigoBarras(codigo) {

    var campo = codigo.value;
    var alunoID = $("#Aluno_AlunoID").val();

    campo = campo.toString().replace(/\s{2,}/g, ' ');

    if (campo === "")
        return;

    $.ajax({
        type: "POST",
        url: "/Cantina/RetornaDadosProdutoCodigoBarras",
        dataType: "json",
        data: { Dado: campo, AlunoID: alunoID },
        success: function (data) {
            var res = jQuery.parseJSON(JSON.stringify(data));

            if (res.length === 0) {
                swal({
                    title: "ATENÇÃO!",
                    text: "Nenhum produto cadastrado com o Código de Barras: " + campo + ", ou o produto não pode ser consumido pelo ALUNO.",
                    icon: "error",
                });
                $("#CodigoBarras").val('');
            }
            else {
                var id = res[0].ProdutoID;
                AdicionaProdutoCantina(null, id);
                $("#CodigoBarras").val('');
            }
        },
        error: function (xhr, err) {
            swal({
                title: "ATENÇÃO!",
                text: "Nenhum produto cadastrado com o Código de Barras: " + campo,
                icon: "error",
            });
            $("#CodigoBarras").val('');
        }
    })
}

//VERIFICA PRODUTO POR CATEGORIA
function VerificaPorCategoria(Object) {

    var id = 0;
    var alunoID = $("#Aluno_AlunoID").val();

    if (Object != null)
        id = Object.id;

    $.ajax({
        type: "POST",
        url: "/Cantina/RetornaDadosProdutoCategoria",
        dataType: "json",
        data: { Dado: id, AlunoID: alunoID },
        success: function (data) {
            var res = jQuery.parseJSON(JSON.stringify(data));

            var l = res.length;

            $('#listaProdutos').empty();

            for (var i = 0; i < l; i++) {

                var id = res[i].ProdutoID;
                var codigo = res[i].Codigo;
                var descricao = res[i].Descricao;
                var valor = res[i].Valor.toString().replace('.', ',');
                var imagem = res[i].Imagem;

                $('#listaProdutos').append('<div class="icone-produtos" id="' + id + '" onclick="AdicionaProdutoCantina(this);">' +
                    '<img src="' + imagem + '" />' +
                    '<div class="fix"></div>' +
                    '<label class="control-label text-center" id="' + id + '" onclick="AdicionaProdutoCantina(this);">' + descricao + ' <br /> R$ ' + valor + '</label>' +
                    '</div>');

                //$('#tbody-produtos-cadastrados').append('<tr id="LinhaTR' + i + '">' +
                //    '<td> <a class="btn btn-info btn-trans btn-xs" id="' + id + '" onclick="AdicionaProdutoCantina(this);"><i class="fa fa-plus"></i></a> </td>' +
                //    '<td> ' + codigo + ' - ' + descricao + '</td > <td>' + valor + ' </td></tr > ');
            }
        },
        error: function (xhr, err) {
            swal({
                title: "ATENÇÃO!",
                text: "Erro ao consultar produtos. Entre em contato com o Suporte Técnico!",
                icon: "error",
            });
        }
    })
}

//OPEN MODAL SANGRIA
function OpenModalSangria(acao) {

    if (acao === true)
        $("#modalSangria").modal('show');
    else
        $("#modalSangria").modal('hide');

    $('#valorSangria').val('');
    $('#motivoSangria').val('');
}

//OPEN MODAL RECEBIMENTO
function OpenModalRecebimento(acao, Obj) {

    var tipoRecebimento = Obj.value;

    if (tipoRecebimento === "dinheiro" && acao) {
        if (acao === true)
            $("#modalRecebimento").modal('show');
        else
            $("#modalRecebimento").modal('hide');
    }
    else {
        $("#modalRecebimento").modal('hide');
        document.getElementById('formaPagemento').value = "credito";

        $('#valorSangria').val('');
        $('#motivoSangria').val('');
    }
}

//OPEN MODAL SELECIONA CLIENTE
function OpenModalSelecionaCliente(acao) {

    if (acao === true)
        $("#modalSelecionaCliente").modal('show');
    else
        $("#modalSelecionaCliente").modal('hide');
}

//OPEN MODAL SELECIONA ALUNO
function OpenModalSelecionaAluno(acao) {

    if (acao === true)
        $("#modalAlteraAluno").modal('show');
    else
        $("#modalAlteraAluno").modal('hide');
}

//PREENCHER CLIENTE
function PreencheCliente(tipo) {
    var identificador = tipo.value.toString();

    if (identificador === "0")
        return;

    $('#TipoCliente').val(identificador);

    $.ajax({
        type: "POST",
        url: "/Cantina/RetornaCliente",
        dataType: "json",
        data: { Dado: identificador },
        success: function (data) {
            var res = jQuery.parseJSON(JSON.stringify(data));

            var l = res.length;

            if (l === 0) {
                swal({
                    title: "ATENÇÃO!",
                    text: "Nenhum cliente encontrado com desse tipo!",
                    icon: "error",
                });

                $('#listaCliente').empty();
                $("#listaCliente").append('<option value= "0">NENHUM REGISTRO ENCONTRADO</option>');

                return;
            }

            $('#listaCliente').empty();

            for (var i = 0; i < l; i++) {

                var id = res[i].ID;
                var nome = res[i].Nome;



                if (i === 0)
                    $('#ClienteID').val(id);


                $("#listaCliente").append('<option value= "' + id + '" > ' + nome + '</option>');
            }
        },
        error: function (xhr, err) {
            swal({
                title: "ATENÇÃO!",
                text: "Erro ao consultar o cliente. Entre em contato com o Suporte Técnico!",
                icon: "error",
            });
        }
    })
}

//PREENCHE ELEMENTOS HIDDEN CLIENTE
function SelecionaCliente(Object) {
    var id = Object.value;

    $('#ClienteID').val(id);
}

//PREENCHE ELEMENTOS HIDDEN ALUNO
function SelecionaAluno(Object) {
    var id = Object.value;

    $('#Aluno_AlunoID').val(id);
}

/*
* COMPRA CRÉDITO
*/

//VERIFICA CARTÃO
function VerificaCartao() {

    var NumeroCartao = document.getElementById("FaturamentoCartao_NumeroCartao").value;

    var regexVisa = /^4/;
    var regexMaster = /^5/;
    var regexAmex = /^3[47]{1}/;
    var regexDinersClub = /^3(?:0[0-5]|[68][0-9])[0-9]{11}/;
    var regexDiscover = /^6(?:011|5[0-9]{2})[0-9]{12}/;
    var regexJCB = /^35(?:2[89]|[3-8]\d)\d{12}$/;
    var regexAura = /^(5078\d{2})(\d{2})(\d{11})$/;
    var regexHiperCard = /^38|^60/;
    var invalidos = /^[0126789]|^5[06-9]{1}|^3[1235689]/;

    if (NumeroCartao == "") {
        document.getElementById("imgCartao").src = "../images/Cartoes/img_default_card.png";
        $("#BandeiraID").val("");
        return false;
    }

    if (regexVisa.test(NumeroCartao)) {
        document.getElementById("imgCartao").src = "../images/Cartoes/img_visa.png";
        $("#BandeiraID").val("0"); //0 = Visa
        return false;
    }

    if (checkElo(parseInt(NumeroCartao.substr(0, 6)))) {
        return false;
    }

    if (regexMaster.test(NumeroCartao)) {
        document.getElementById("imgCartao").src = "../images/Cartoes/img_mastercard.png";
        $("#BandeiraID").val("1"); //1 = Master
        return false;
    }

    if (regexAmex.test(NumeroCartao)) {
        document.getElementById("imgCartao").src = "../images/Cartoes/img_amex.png";
        $("#BandeiraID").val("2"); //2 = Amex
        return false;
    }

    if (regexAura.test(NumeroCartao)) {
        document.getElementById("imgCartao").src = "../images/Cartoes/img_aura.png";
        $("#BandeiraID").val("4"); //4 = Aura
        return false;
    }

    if (regexJCB.test(NumeroCartao)) {
        document.getElementById("imgCartao").src = "../images/Cartoes/img_jcb.png";
        $("#BandeiraID").val("5"); //5 = JCB
        return false;
    }

    if (regexDinersClub.test(NumeroCartao)) {
        document.getElementById("imgCartao").src = "../images/Cartoes/img_diners.png";
        $("#BandeiraID").val("6"); //6 = Diners
        return false;
    }

    if (regexDiscover.test(NumeroCartao)) {
        document.getElementById("imgCartao").src = "../images/Cartoes/img_discover.png";
        $("#BandeiraID").val("7"); //7 = Discover
        return false;
    }

    if (invalidos.test(NumeroCartao)) {
        document.getElementById("imgCartao").src = "../images/Cartoes/img_default_card.png";
        $("#BandeiraID").val("");
        return false;
    }

    function checkElo(bin) {
        var eloBIN = [401178, 401179, 431274, 438935, 451416, 457393,
            457631, 457632, 504175, 627780, 636297, 636368,
            655000, 655001, 651652, 651653, 651654, 650485,
            650486, 650487, 650488, 655002, 655003, 650489,
            650490, 650491, 650492, 650493, 650494];


        if (eloBIN.indexOf(bin) >= 0 ||
            (bin >= 506699 && bin <= 506778) ||
            (bin >= 509000 && bin <= 509999) ||
            (bin >= 650031 && bin <= 650033) ||
            (bin >= 650035 && bin <= 650051) ||
            (bin >= 650405 && bin <= 650439) ||
            (bin >= 650485 && bin <= 650538) ||
            (bin >= 650541 && bin <= 650598) ||
            (bin >= 650700 && bin <= 650718) ||
            (bin >= 650720 && bin <= 650727) ||
            (bin >= 650901 && bin <= 650920) ||
            (bin >= 651652 && bin <= 651679) ||
            (bin >= 655000 && bin <= 655019) ||
            (bin >= 655021 && bin <= 655058)
        ) {
            document.getElementById("imgCartao").src = "../images/Cartoes/img_elo.png";
            $("#BandeiraID").val("3"); //3 = Elo
            return true;
        }
    }
}

//CARREGA PRODUTOS POR ESCOLA
$(document).ready(function () {
    var id = 0;

    if (document.getElementById("tbody-produtos-escola") != null) {

        id = $("#Aluno_EscolaID").val();

        $.ajax({
            type: "POST",
            url: "/CompraCredito/RetornaProdutos",
            dataType: "json",
            data: { Dado: id },
            success: function (data) {
                var res = jQuery.parseJSON(JSON.stringify(data));

                var l = res.length;

                for (var i = 0; i < l; i++) {

                    var id = res[i].ProdutoID;
                    var cod = res[i].Codigo;
                    var descricao = res[i].Descricao;
                    var valor = res[i].Valor;

                    valor = valor.toFixed(2);

                    $('#tbody-produtos-escola').append('<tr>' +
                        '<td> <a class="btn btn-info btn-trans btn-xs" id="' + id + '" name="' + cod + ' - ' + descricao + '" onclick= "IncluiListaAluno(this);" > <i class="fa fa-plus"></i></a > </td > ' +
                        '<td> ' + cod + ' - ' + descricao + '</td ><td> ' + valor.toString().replace('.', ',') + '</td></tr> ');
                }
            },
            error: function (xhr, err) {
                swal({
                    title: "ATENÇÃO!",
                    text: "Erro ao consultar produtos. Entre em contato com o Suporte Técnico!",
                    icon: "error",
                });
            }
        })
    }
});

//ADICIONA NA PERMISSÃO DE ALUNO
function IncluiListaAluno(Object) {
    var id = 0;
    var descricao = "";
    var idAlunoProduto = 0;

    id = Object.id;
    descricao = Object.name;

    //Verifica qtde de linha na tbody
    var tabela = document.getElementById('tbody-permissao-aluno');
    var linhas = tabela.getElementsByTagName('tr');
    var i = linhas.length;

    if (linhas.length === 0) {
        $('#tbody-permissao-aluno').append('<tr id="LinhaTR' + i + '">' +
            '<input type="hidden" name="ListaProdutosAluno[' + i + '].ProdutoID" value="' + id + '"/>' +
            '<input id="Deletado' + i + '" type="hidden" name="ListaProdutosAluno[' + i + '].Deletado" value="false" />' +
            '<input type="hidden" name="ListaProdutosAluno[' + i + '].AlunoProdutos.AlunoProdutosID" value="' + idAlunoProduto + '" />' +
            '<td> <a class="btn btn-trans btn-danger btn-xs" onclick="RemoveListaAluno(this)"><i class="fa fa-minus"></i></a> </td>' +
            '<td> ' + descricao + '</td > <td id="idProd' + id + '">' + id + ' </td></tr > ');

        $('#idProd' + id + '').hide();

    }
    else {

        var i = 0;
        var exists = 0;

        $('#tbody-permissao-aluno tr').each(function () {

            idProduto = $(this).find('td:nth-child(6)').text().replace(' ', '');//ID

            var nomeDeletado = '#Deletado' + i;

            var deletado = $(this).find('input#Deletado' + i + ':nth-child(2)').val();

            if (id === idProduto && deletado) {

                if (deletado === "true") {
                    $(nomeDeletado).val('false');

                    var tr = $(this).closest('tr');
                    tr.fadeIn(400, function () { tr.show(); });
                }

                exists++;
            }

            i++;
        });

        if (exists === 0) {
            $('#tbody-permissao-aluno').append('<tr id="LinhaTR' + i + '">' +
                '<input type="hidden" name="ListaProdutosAluno[' + i + '].ProdutoID" value="' + id + '"/>' +
                '<input id="Deletado' + i + '" type="hidden" name="ListaProdutosAluno[' + i + '].Deletado" value="false" />' +
                '<input type="hidden" name="ListaProdutosAluno[' + i + '].AlunoProdutos.AlunoProdutosID" value="' + idAlunoProduto + '" />' +
                '<td> <a class="btn btn-trans btn-danger btn-xs" onclick="RemoveListaAluno(this)"><i class="fa fa-minus"></i></a> </td>' +
                '<td> ' + descricao + '</td > <td id="idProd' + id + '">' + id + ' </td></tr > ');

            $('#idProd' + id + '').hide();
        }
    }
}

function RemoveListaAluno(item) {

    var tr = $(item).closest('tr');
    var identificador = tr.attr('id').substr(7);

    $("#Deletado" + identificador).val('true');
    tr.fadeOut(400, function () { tr.hide(); });
}

//CARREGA CATEGORIAS
$(document).ready(function () {
    var id = 0;

    if (document.getElementById("tbody-categorias") != null) {
        $.ajax({
            type: "POST",
            url: "/CompraCredito/RetornaCategoria",
            dataType: "json",
            data: { Dado: id },
            success: function (data) {
                var res = jQuery.parseJSON(JSON.stringify(data));

                var l = res.length;

                for (var i = 0; i < l; i++) {

                    var id = res[i].CategoriaID;
                    var descricao = res[i].Descricao;

                    $('#tbody-categorias').append('<tr>' +
                        '<td> <a class="btn btn-warning btn-trans btn-xs" id="' + id + '" onclick="VerificaPorCategoriaPermissaoAluno(this);"><i class="fa fa-search"></i></a> </td>' +
                        '<td> ' + descricao + '</td ></tr> ');
                }

                VerificaPorCategoriaPermissaoAluno(null);
            },
            error: function (xhr, err) {
                swal({
                    title: "ATENÇÃO!",
                    text: "Erro ao consultar Categorias. Entre em contato com o Suporte Técnico!",
                    icon: "error",
                });
            }
        })
    }
});

//CRIA POP-UP CVV
$(document).ready(function () {

    if (document.getElementById("tbody-categorias") != null) {
        $('.fa-info-circle').tooltip({ title: "<p><h4 style='width: 180px;'><strong>Código de Segurança.</strong></h4></p> <br /><p style='width: 180px;'>Para os cartões: <strong><br />- Visa <br />- Mastercard<br />- DinersClub<br />- Hipercard<br />- Elo<br /></strong> O código encontra-se no verso de seu cartão.</p><br /><p style='width: 180px;'>Já para o <strong>American Express</strong> o código encontra-se na frente de seu cartão.</p>", html: true, placement: "top", style: "width: 300px;" });
    }
});

//VERIFICA PRODUTO POR CATEGORIA
function VerificaPorCategoriaPermissaoAluno(Object) {

    var id = 0;

    if (Object != null)
        id = Object.id;

    $.ajax({
        type: "POST",
        url: "/CompraCredito/RetornaDadosProdutoCategoria",
        dataType: "json",
        data: { Dado: id },
        success: function (data) {
            var res = jQuery.parseJSON(JSON.stringify(data));

            var l = res.length;

            $('#tbody-produtos-escola').empty();

            for (var i = 0; i < l; i++) {

                var id = res[i].ProdutoID;
                var codigo = res[i].Codigo;
                var descricao = res[i].Descricao;
                var valor = res[i].Valor;

                valor = valor.toFixed(2);

                $('#tbody-produtos-escola').append('<tr>' +
                    '<td> <a class="btn btn-info btn-trans btn-xs" id="' + id + '" name="' + codigo + ' - ' + descricao + '" onclick= "IncluiListaAluno(this);" > <i class="fa fa-plus"></i></a > </td > ' +
                    '<td> ' + codigo + ' - ' + descricao + '</td ><td>' + valor.toString().replace('.', ',') + '</td></tr> ');
            }
        },
        error: function (xhr, err) {
            swal({
                title: "ATENÇÃO!",
                text: "Erro ao consultar produtos. Entre em contato com o Suporte Técnico!",
                icon: "error",
            });
        }
    })
}

//PRODUTOS LIBERADOS AO ALUNO
$(document).ready(function () {

    if (document.getElementById("tbody-permissao-aluno") != null) {

        var id = $("#Aluno_AlunoID").val();

        if (id === 0)
            return;

        $.ajax({
            type: "POST",
            url: "/CompraCredito/RetornaProdutosAluno",
            dataType: "json",
            data: { Dado: id },
            success: function (data) {
                var res = jQuery.parseJSON(JSON.stringify(data));

                var l = res.length;

                for (var i = 0; i < l; i++) {

                    var id = res[i].ProdutoID;
                    var cod = res[i].Codigo;
                    var descricao = res[i].Descricao;
                    var idAlunoProduto = res[i].AlunoProdutosID;

                    $('#tbody-permissao-aluno').append('<tr id="LinhaTR' + i + '">' +
                        '<input type="hidden" name="ListaProdutosAluno[' + i + '].ProdutoID" value="' + id + '"/>' +
                        '<input id="Deletado' + i + '" type="hidden" name="ListaProdutosAluno[' + i + '].Deletado" value="false" />' +
                        '<input type="hidden" name="ListaProdutosAluno[' + i + '].AlunoProdutosID" value="' + idAlunoProduto + '" />' +
                        '<td> <a class="btn btn-trans btn-danger btn-xs" onclick="RemoveListaAluno(this)"><i class="fa fa-minus"></i></a> </td>' +
                        '<td> ' + cod + ' - ' + descricao + '</td > <td id="idProd' + id + '">' + id + ' </td></tr > ');

                    $('#idProd' + id + '').hide();
                }
            },
            error: function (xhr, err) {
                swal({
                    title: "ATENÇÃO!",
                    text: "Erro ao consultar Produtos do Aluno. Entre em contato com o Suporte Técnico!",
                    icon: "error",
                });
            }
        })
    }

});

/*
*ALUNO
*/

//VERIFICA RESPONSAVEL EXISTENTE
$(function () {
    $("#cpfResponsavel").blur(function () {

        if ($("#cpfResponsavel").val() === "")
            return false;

        var cpf = $("#cpfResponsavel").val().toString().replace(".", "").replace("-", "");

        if (!validarCPF($("#cpfResponsavel").val())) {
            swal({
                title: "ATENÇÃO!",
                text: "CPF INVÁLIDO: " + $("#cpfResponsavel").val(),
                icon: "error",
            });
            $("#cpfResponsavel").val("");
            return false;
        }
        else {
            $.ajax({
                type: "POST",
                url: "/Responsavel/RetornaResponsavel",
                dataType: "json",
                data: { Dado: cpf },
                success: function (data) {
                    var res = jQuery.parseJSON(JSON.stringify(data));

                    var l = res.length;

                    if (l === 0) {
                        $("#ResponsavelLegal_ResponsavelID").val('0');
                        return;
                    }

                    for (var i = 0; i < l; i++) {

                        var id = res[i].ResponsavelID;
                        var nome = res[i].Nome;
                        var email = res[i].Email;
                        var telefone = res[i].Telefone;

                        $("#ResponsavelLegal_ResponsavelID").val(id);
                        $("#ResponsavelLegal_Nome").val(nome);
                        $("#emailResp").val(email);
                        $("#ResponsavelLegal_Telefone").val(telefone);
                    }
                },
                error: function (xhr, err) {
                    swal({
                        title: "ATENÇÃO!",
                        text: "Erro ao consultar Responsavél. Entre em contato com o Suporte Técnico!",
                        icon: "error",
                    });
                }
            })
        }
    });
});

//VERIFICA RESPONSAVEL FINANCEIRO EXISTENTE
$(function () {
    $("#cpfResponsavelFin").blur(function () {

        if ($("#cpfResponsavelFin").val() === "")
            return false;

        var cpf = $("#cpfResponsavelFin").val().toString().replace(".", "").replace("-", "");

        if (!validarCPF($("#cpfResponsavelFin").val())) {
            swal({
                title: "ATENÇÃO!",
                text: "CPF INVÁLIDO: " + $("#cpfResponsavelFin").val(),
                icon: "error",
            });
            $("#cpfResponsavelFin").val("");
            return false;
        }
        else {
            $.ajax({
                type: "POST",
                url: "/Responsavel/RetornaResponsavel",
                dataType: "json",
                data: { Dado: cpf },
                success: function (data) {
                    var res = jQuery.parseJSON(JSON.stringify(data));

                    var l = res.length;

                    if (l === 0) {
                        $("#ResponsavelFinanceiro_ResponsavelID").val('0');
                        return;
                    }

                    for (var i = 0; i < l; i++) {

                        var id = res[i].ResponsavelID;
                        var nome = res[i].Nome;
                        var email = res[i].Email;
                        var telefone = res[i].Telefone;

                        $("#ResponsavelFinanceiro_ResponsavelID").val(id);
                        $("#ResponsavelFinanceiro_Nome").val(nome);
                        $("#emailRespFin").val(email);
                        $("#ResponsavelFinanceiro_Telefone").val(telefone);
                    }
                },
                error: function (xhr, err) {
                    swal({
                        title: "ATENÇÃO!",
                        text: "Erro ao consultar Responsavél. Entre em contato com o Suporte Técnico!",
                        icon: "error",
                    });
                }
            })
        }
    });
});

//DESABILITA FECHAMENTO CAIXA SE O CAIXA JÁ ESTIVER FECHADO
$(document).ready(function () {

    if (document.getElementById("CaixaFechamentoFechado") != undefined) {

        if (document.getElementById("CaixaFechamentoFechado").value === "True") {
            document.getElementById("btnSalvar").disabled = true;
            document.getElementById("reabrirCaixa").disabled = false;
        }
        else {
            document.getElementById("btnSalvar").disabled = false;
            document.getElementById("reabrirCaixa").disabled = true;
        }
    }
});
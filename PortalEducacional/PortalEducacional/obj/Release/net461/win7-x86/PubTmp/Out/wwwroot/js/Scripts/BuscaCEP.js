$(function () {
    $('[id$="EstadoId"]').change(function (ConsultaCidade) {
        var endereco = $(this).parents('div#endereco');
        $('[id$="CidadeId"]').empty();

        // Carrega Cidades de acordo o Estado Selecionado
        $.get('/ListaCidadeporEstado/', { estadoId: $(this).val() })
            .done(function (cidade) {
                $(cidade).each(function () {
                    //adiciona cidades de acordo com o retorno obtido
                    endereco.find('[id$="CidadeId"]').append("<option value='" + this.cidadeId + "'>" + this.nome + "</option>");
                });
            })
            .fail(function (xhr) {
                console.log(xhr.statusText);
            });
    });

    // Carrega Endereço de acordo com o CEP informado
    $(document).on('blur', '.cep', function (e) {
        var cep = $(this).val().replace("-", "");
        var endereco = $(this).parents('div#endereco');
        var cidadeId = endereco.find('[id$="CidadeId"]').attr('id');
        var estadoId = endereco.find('[id$="EstadoId"]').attr('id');

        $.getJSON('https://api.postmon.com.br/v1/cep/' + cep, function (dados) {
            endereco.find('[id$="Endereco_Logradouro"]').val(dados.logradouro);
            endereco.find('[id$="Endereco_Bairro"]').val(dados.bairro);
            endereco.find('[id$="EstadoId"] option:contains("' + dados.estado_info.nome.toUpperCase() + '")').attr({ selected: "selected" });
            //$('#Endereco_Cidade_CidadeId').attr('disabled', true);
            //endereco.find('[id$="EstadoId"]').attr('disabled', true);
            $('#Endereco_Numero').focus();

            // Percorre opções (option) do elemento Select
            endereco.find('[id$="CidadeId"] > option').each(function () {
                var cidade = SubstituirAcentosEmPalavras($(this).text());
                if (cidade == SubstituirAcentosEmPalavras(dados.cidade.toUpperCase())) {
                    $(this).attr({ selected: "selected" });
                    return false;
                }
            });
        })
            .fail(function (xhr) {
                swal("CEP não encontrado.", "Por Favor Verificar o CEP Digitado");
                //$(".cep").val("");
            });
    });
});


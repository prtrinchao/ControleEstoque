function set_Dados_form(dados) {
    $('#id_cadastro').val(dados.Id);
    $('#txt_nome').val(dados.Nome);
    $('#txt_sigla').val(dados.Sigla);
    $('#cbx_ativo').prop('checked', dados.Ativo);

}

function setaFocus() {

    $('#txt_nome').focus();
}

function set_Dados_Grid(dados) {

    return '<td>' + dados.Nome + '</td>' +
        '<td>' + dados.Sigla + '</td>' +
        '<td>' + (dados.Ativo ? 'SIM' : 'NÃO') + '</td>';

}

function seta_Botoes() {
    return '<a class="btn btn-primary btn-alterar" role="button" style="margin-right: 3px"><i class="glyphicon glyphicon-pencil"></i> Alterar</a>' +
        '<a class="btn btn-danger btn-excluir" role="button"><i class="glyphicon glyphicon-trash"></i> Excluir</a>';
}


$("#grid_cadastro > tr > td").change(function () {
    alerts("call");

});

function get_dados_form() {
    return { Id: 0, Nome: '', Sigla: '', Ativo: true }
}

function get_Param_Form() {
    return {
        Id: $('#id_cadastro').val(),
        Nome: $('#txt_nome').val(),
        Sigla: $('#txt_sigla').val(),
        Ativo: $('#cbx_ativo').prop('checked')
    }
}

function seta_Linha_Grid(linha, param) {

    linha
        .eq(0).html(param.Nome).end()
        .eq(1).html(param.Sigla).end()
        .eq(2).html(param.Ativo ? 'SIM' : 'NÃO')
}

createPagination($('#paginador_table'), QtdPaginas);



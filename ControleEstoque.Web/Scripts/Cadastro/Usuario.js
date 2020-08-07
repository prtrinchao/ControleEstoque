function set_Dados_form(dados) {
    $('#id_cadastro').val(dados.Id);
    $('#txt_login').val(dados.Login);
    $('#txt_nome').val(dados.Nome);
    $('#txt_senha').val(dados.Senha);

    var lista_perfil = $('#lista_perfil');
    lista_perfil.find('input[type=checkbox]').prop('checked', false);

    if (dados.Perfis) {
        for (var i = 0; i < dados.Perfis.length; i++) {
            var perfil = dados.Perfis[i];
            var cbx = lista_perfil.find('input[data-id-perfil=' + perfil.Id + ']');
            cbx.prop('checked', true);

        }
    }
}

function setaFocus() {

    var id = $('#id_cadastro').val()
    if (id == 0) {
        $('#txt_login').focus();
        $('#txt_login').prop("disabled", false);
        $('#senha_group').show();

    }
    else {
        $('#txt_login').prop("disabled", true);
        $('#txt_nome').focus();
        $('#senha_group').hide();

    }
}

function set_Dados_Grid(dados) {

    return '<td>' + dados.Login + '</td>' +
        '<td>' + dados.Nome + '</td>';
}


function seta_Botoes() {
    return '<a class="btn btn-warning btn-reset" role="button" style="margin-right: 3px"><i class="glyphicon glyphicon-edit"></i> Reset</a>' +
        '<a class="btn btn-primary btn-alterar" role="button" style="margin-right: 3px"><i class="glyphicon glyphicon-pencil"></i> Alterar</a>' +
        '<a class="btn btn-danger btn-excluir" role="button"><i class="glyphicon glyphicon-trash"></i> Excluir</a>';
}

$("#grid_cadastro > tr > td").change(function () {
    alerts("call");

});

function get_dados_form() {
    return { Id: 0, Login: '', Nome: '', Senha: '' }
}

function get_Param_Form() {
    return {

        Id: $('#id_cadastro').val(),
        Login: $('#txt_login').val(),
        Nome: $('#txt_nome').val(),
        Senha: $('#txt_senha').val()
       
    }
}

function seta_Linha_Grid(linha, param) {

    linha
        .eq(0).html(param.Login).end()
        .eq(1).html(param.Nome);
}

$(document).on('click', '.btn-reset', function () {
    var btn = $(this),
        tr = btn.closest('tr'),
        id = tr.attr('data-id'),
        url = urlResetSenha,
        param = {
            'id': id,
            'senha': 123
        };

    bootbox.confirm({
        message: "Realmente deseja Resetar a Senha do Usuário?",
        buttons: {
            confirm: {
                label: 'Sim',
                className: 'btn-danger'
            },
            cancel: {
                label: 'Não',
                className: 'btn-success'
            }
        },
        callback: function (result) {
            if (result) {
                $.post(url, add_anti_forgery_token(param), function (response) {
                    if (response) {
                        bootbox.alert('Senha do Usuário atualizada');
                    }
                });
            }
        }
    });
});


createPagination($('#paginador_table'), QtdPaginas);



function add_anti_forgery_token(data) {
    data.__RequestVerificationToken = $('[name=__RequestVerificationToken]').val();
    return data;
}

function formatar_mensagem_aviso(mensagens) {
    var ret = '';
    for (var i = 0; i < mensagens.length; i++) {
        ret += '<li>' + mensagens[i] + '</li>';
    }
    return '<ul>' + ret + '</ul>';
}

function createPagination(divPag, qtdPag) {

    if (qtdPag > 1) {

        var cabecalho = ' <nav> ' +
            '  	<ul class="pagination"> ' +
            '      <li class="page-item"> ' +
            '         <a class="page-link" href="#" aria-label="Previous"> ' +
            '              <span aria-hidden="true">&laquo;</span> ' +
            '              <span class="sr-only">Previous</span> ' +
            '          </a> ' +
            '      </li>  ';

        var paginas = '';

        for (var i = 1; i <= qtdPag; i++) {
            paginas += '	<li class="page-item"> ' +
                '	<a class="page-link" href="#"> ' +
                '    <span> ' + i + ' </span> ' +
                ' </a> ' +
                '	 </li> ';

        }

        var rodape = '	<li class="page-item"> ' +
            '     <a class="page-link" href="#" aria-label="Next"> ' +
            '             <span aria-hidden="true">&raquo;</span> ' +
            '                <span class="sr-only">Next</span>	' +
            '            </a> ' +
            '        </li>	  ' +
            '   	 </ul>    ' +
            '	</nav>        ';

        var paginador = cabecalho + paginas + rodape;

        divPag.empty().append(paginador);

    }
    else {
        divPag.empty();
    }

}

function abrir_form(dados) {

    set_Dados_form(dados);

    var modal_cadastro = $('#modal_cadastro');
    $('#msg_mensagem_aviso').empty();
    $('#msg_aviso').hide();
    $('#msg_mensagem_aviso').hide();
    $('#msg_erro').hide();
    bootbox.dialog({
        title: '<h3> Cadastro de ' + TituloPagina + '</h3>',
        message: modal_cadastro
    })
        .on('shown.bs.modal', function () {
            modal_cadastro.show(0, function () {
                setaFocus();
            });
        })
        .on('hidden.bs.modal', function () {
            modal_cadastro.hide().appendTo('body');
            createPagination($('#paginador_table'), QtdPaginas);
        });

}


function criar_linha_grid(dados) {
    var ret =
        '<tr data-id=' + dados.Id + '>' +
        set_Dados_Grid(dados) +
        '<td>' +
        seta_Botoes() +
        '</td>' +
        '</tr>';
    return ret;
}



$(document).on('click', '#btn_incluir', function () {
    abrir_form(get_dados_form());
}).on('click', '.btn-alterar', function () {
    var btn = $(this),
        id = btn.closest('tr').attr('data-id'),
        url = urlRecuperar,
        param = { 'id': id };
    $.post(url, add_anti_forgery_token(param), function (response) {
        if (response) {
            abrir_form(response);
        }
    });
})
    .on('click', '.btn-excluir', function () {
        var btn = $(this),
            tr = btn.closest('tr'),
            id = tr.attr('data-id'),
            url = urlExcluir,
            paginador = $('#paginador_table');
        param = { 'id': id };
        bootbox.confirm({
            message: "<h3>Realmente deseja excluir o" + TituloPagina + "? </h3>",
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
                            tr.remove();
                            createPagination($('#paginador_table'), QtdPaginas);

                            if ($('#grid_cadastro > tbody > tr').length == 0) {
                                $('#grid_cadastro').addClass('invisivel');
                                $('#mensagem_grid').removeClass('invisivel');

                            }
                        }
                    });

                }
            }
        });
    })
    .on('click', '#btn_confirmar', function () {
        var btn = $(this),
            url = urlConfirmar,
            param = get_Param_Form();
        $.post(url, add_anti_forgery_token(param), function (response) {
            if (response.Resultado == "OK") {
                if (param.Id == 0) {
                    param.Id = response.IdSalvo;
                    var table = $('#grid_cadastro').find('tbody'),
                        linha = criar_linha_grid(param);
                    table.append(linha);

                    $('#grid_cadastro').removeClass('invisivel');
                    $('#mensagem_grid').addClass('invisivel');


                    // createPagination(QtdPaginas);
                }
                else {
                    var linha = $('#grid_cadastro').find('tr[data-id=' + param.Id + ']').find('td');
                    seta_Linha_Grid(linha, param);

                }
                $('#modal_cadastro').parents('.bootbox').modal('hide');
            }
            else if (response.Resultado == "ERRO") {
                $('#msg_aviso').hide();
                $('#msg_mensagem_aviso').hide();
                $('#msg_erro').show();
            }
            else if (response.Resultado == "AVISO") {
                $('#msg_mensagem_aviso').html(formatar_mensagem_aviso(response.Mensagens));
                $('#msg_aviso').show();
                $('#msg_mensagem_aviso').show();
                $('#msg_erro').hide();
            }
        });
    })
    .on('click', '.page-link', function () {

        var btn = $(this),
            url = urlPaginacao,
            numero = btn.children().eq(0).text(),
            param = {
                'pagina': $.isNumeric(numero) ? numero : ((numero == '«') ? 1 : QtdPaginas),
                'maxPag': $('#ddl_tam_pag').val(),
                'filtro': $('#txt_search').val()
            }
        


        $('a.page-link').removeClass('active');
        btn.addClass('active');

        $.post(url, add_anti_forgery_token(param), function (response) {
            if (response) {

                var table = $('#grid_cadastro').find('tbody');
                table.empty();
                for (var i = 0; i < response.length; i++) {
                    table.append(criar_linha_grid(response[i]));

                }

                if ($('#grid_cadastro > tbody > tr').length == 0) {
                    $('#grid_cadastro').addClass('invisivel');
                    $('#mensagem_grid').removeClass('invisivel');

                }
            }
        });

    })
    .on('change', '#ddl_tam_pag', function () {
        var url = urlPaginacao,
            numero = 1,
            param = {
                'pagina': $.isNumeric(numero) ? numero : ((numero == '«') ? 1 : QtdPaginas),
                'maxPag': $('#ddl_tam_pag').val(),
                'filtro': $('#txt_search').val()
            }


        $.post(url, add_anti_forgery_token(param), function (response) {
            if (response) {

                var table = $('#grid_cadastro').find('tbody');
                table.empty();
                for (var i = 0; i < response.length; i++) {
                    table.append(criar_linha_grid(response[i]));

                }

                if ($('#grid_cadastro > tbody > tr').length == 0) {
                    $('#grid_cadastro').addClass('invisivel');
                    $('#mensagem_grid').removeClass('invisivel');

                }
            }
        });

    })
    .on('keyup', '#txt_search', function () {
        var url = urlPaginacao,
            numero = 1,
            param = {
                'pagina': $.isNumeric(numero) ? numero : ((numero == '«') ? 1 : QtdPaginas),
                'maxPag': $('#ddl_tam_pag').val(),
                'filtro': $('#txt_search').val()
            }

        $.post(url, add_anti_forgery_token(param), function (response) {
            if (response) {

                var table = $('#grid_cadastro').find('tbody');
                table.empty();
                for (var i = 0; i < response.length; i++) {
                    table.append(criar_linha_grid(response[i]));

                }

                if ($('#grid_cadastro > tbody > tr').length == 0) {
                    $('#grid_cadastro').addClass('invisivel');
                    $('#mensagem_grid').removeClass('invisivel');

                }
                else {
                    $('#grid_cadastro').removeClass('invisivel');
                    $('#mensagem_grid').addClass('invisivel');

                }
            }
        });

    });

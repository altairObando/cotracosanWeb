// DataTable Generica para todos las acciones basicas
var dataTable = null;
// Mostrar un loading para el crud
$(function () {
    var loading = $("#loading").hide();
    $(document).ajaxStart(function () {
        $("#parcialCreateUpdate").hide();
        loading.show();
    }).ajaxStop(function () {
        loading.hide();
        $("#parcialCreateUpdate").show();
    });
});
function SubmitForm(form) {
    $.validator.unobtrusive.parse(form);
    if ($(form).valid()) {
        $.ajax({
            type: "POST",
            url: form.action,
            data: $(form).serialize(),
            success: function (data) {
                if (data.success) {
                    dataTable
                        .ajax
                        .reload();
                    dataTable.search('')
                        .columns()
                        .search('')
                        .draw();
                }
                $("#modal-default .close").click(); //Ocultar modal
                $.notify({
                    // Opciones
                    message: data.mensaje,
                    allow_dismiss: true
                },
            {
                // Ajustes
                type: data.type,
                allow_dismiss: true,
                placement: {
                    from: "top",
                    align: "center"
                },
            });
            }
        });
    } else {
        return false;
    }
    return false;
}
function getHtmlData(uri) {
    $("#parcialCreateUpdate").html(""); //Limpiar el contenido para evitar errores
    $.ajax({
        type: "GET",
        url: uri,
        success: function (data) {
            $("#parcialCreateUpdate").html(data);
        }
    });
}
function generarDataTable(dataUrl, indexId, tableId, columnDefs, actionsUri) {
    columnas = [];
    for (var i = 0; i < columnDefs.length; i++) {
        columnas.push({ 'data': columnDefs[i] });
    }
    dataTable = $("#" + tableId).DataTable({
        "ajax": {
            "url": dataUrl,
            "type": "POST",
            "dataType": "JSON"
        }, "columns": columnas,
        
        "columnDefs": [
        {
            "targets": indexId, "render": function (data) {
                var buttons = ' <a href="#" onclick=getHtmlData("' + actionsUri[0] + '/' + data + '") class="btn btn-sm btn-warning" data-toggle="modal" data-target="#modal-default"> Editar </a> |';
                buttons += ' <a href="#" onclick=getHtmlData("' + actionsUri[1] + '/' + data + '") class="btn btn-sm btn-info" data-toggle="modal" data-target="#modal-default"> Detalle </a> |';
                buttons += ' <a href="#" onclick=getHtmlData("' + actionsUri[2] + '/' + data + '") class="btn btn-sm btn-danger" data-toggle="modal" data-target="#modal-default">Eliminar </a>';
                return buttons;
            }, "className": "text-center",
        }
        ], "language": {
            "sProcessing": "Procesando...",
            "sLengthMenu": "Mostrar _MENU_ registros",
            "sZeroRecords": "No se encontraron resultados",
            "sEmptyTable": "Ningún dato disponible en esta tabla",
            "sInfo": "Mostrando registros del _START_ al _END_ de un total de _TOTAL_ registros",
            "sInfoEmpty": "Mostrando registros del 0 al 0 de un total de 0 registros",
            "sInfoFiltered": "(filtrado de un total de _MAX_ registros)",
            "sInfoPostFix": "",
            "sSearch": "Buscar:",
            "sUrl": "",
            "sInfoThousands": ",",
            "sLoadingRecords": "Cargando...",
            "oPaginate": {
                "sFirst": "Primero",
                "sLast": "Último",
                "sNext": "Siguiente",
                "sPrevious": "Anterior"
            },
            "oAria": {
                "sSortAscending": ": Activar para ordenar la columna de manera ascendente",
                "sSortDescending": ": Activar para ordenar la columna de manera descendente"
            }
        }
    });
}
// DataTable Generica para todos las acciones basicas
var dataTable = null;
// Mostrar un spinner de carga en el datatable
$(function () {
    var loading = $("#spinner").hide();
    $(document).ajaxStart(function () {
        $("#parcialCreateUpdate").hide();
        $("#loading").show();
        loading.show();
    }).ajaxStop(function () {
        loading.hide();
        $("#loading").hide();
        $("#parcialCreateUpdate").show();
    });
});
$(document).ready(function () {
    dataTable.ajax.reload();
});
function CambiarUrl() {
    // Capturar la fecha seleccionada
    let fecha = $("#SelectorFecha").val();
    let uri = "/Carreras/GetCarreras?Fecha=" + fecha;
    dataTable.clear().draw(false);
    dataTable.ajax.url(uri);
    dataTable.ajax.reload();
    dataTable.search('')
                        .columns()
                        .search('')
                        .draw();
}
function CargarTodo() {
    $("#SelectorFecha").val();
    dataTable.clear().draw(false);
    dataTable.ajax.url("/Carreras/GetCarreras?Todo=True");
    dataTable.ajax.reload();
    dataTable.search('')
                        .columns()
                        .search('')
                        .draw();
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
function generarDataTable(dataUrl, indexId, tableId, columnDefs) {
    columnas = [];
    for (var i = 0; i < columnDefs.length; i++) {
        columnas.push({ 'data': columnDefs[i] });
    }
    dataTable = $("#" + tableId).DataTable({
        "lengthMenu": [5, 10, 20, 50],
        "deferLoading": 0,
        "ajax": {
            "url": dataUrl,
            "type": "POST",
            "dataType": "JSON"
        },
        "columns": columnas,
        "columnDefs": [
        {
            "targets": indexId, "render": function (data) {
                let buttons = '<a href="#" onclick=getHtmlData("/Carreras/Details/' + data + '") class="btn btn-sm btn-info" data-toggle="modal" data-target="#modal-default">Detalles </a>';
                buttons += ' | <a href="#" onclick=getHtmlData("/Carreras/Delete/' + data + '") class="btn btn-sm btn-danger" data-toggle="modal" data-target="#modal-default">Eliminar </a>';
                return buttons;
            }, "className": "text-center",
        }, {
            "targets": 4, "className": "text-center", "render": function (data) {
                return "C$ " + data;
            }
        },
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

function loadSelect2() {
    $("#ConductorId,#LugarFinalDeRecorridoId,#TurnoId,#VehiculoId").select2({
        width: "225px"
    });
}
function addListeners() {
    $("#Multa").val('0');

    $("#HoraRealDeLlegada").change(function () {
        if ($("#TurnoId").val())
            $("#TurnoId").change();
    })

    $("#TurnoId").change(function () {
        var idTurno = $("#TurnoId").val();
        var horaReal = $("#HoraRealDeLlegada").val();
        // Haciendo el calculoe en el servidor :v
        if (idTurno && idTurno > 0 && horaReal && horaReal != "") {
            $.ajax({
                url: "/Carreras/GetMulta/",
                type: "POST",
                dataType: "JSON",
                data: { 'id': idTurno, "horaRealLlegada": horaReal },
                success: function (data) {
                    $("#Multa").val(data.data);
                }
            });
        } else {
            alert("Ingrese la hora de llegada");
        }
    });
}
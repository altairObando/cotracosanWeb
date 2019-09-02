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
    generarDataTable();
    dataTable.ajax.reload();
});

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
function generarDataTable() {
    dataTable = $("#tableCreditos").DataTable({
        "ajax": {
            "url": "/Creditos/GetCreditos/",
            "type": "GET",
            "dataType": "JSON"
        },
        "columns": [
            { 'data': 'CodigoCredito' },
            { 'data': 'FechaDeCredito' },
            { 'data': 'Vehiculo' },
            { 'data': 'MontoTotal' },
            {
                'data': 'CreditoAnulado', "render": function (data) {

                    return !data ? '<span class="badge badge-success">Sin Eliminar</span>' : '<span class="badge badge-danger">Eliminado</span>';
                }
            },
            {
                'data': 'IdCredito', "render": function (data) {
                    let buttons = '<a href="#" onclick=getHtmlData("/Creditos/Abonos/' + data + '/") class="btn btn-sm btn-info" data-toggle="modal" data-target="#modal-default"> Ver Abonos </a> |';
                    buttons += ' <a href="/Abonos/Agregar?CreditoId='+data+'" class="btn btn-sm btn-success"> Agregar Abonos </a> | ';
                    buttons += ' <a href="#" onclick=getHtmlData("/Creditos/Delete/'+data+'/") class="btn btn-sm btn-danger" data-toggle="modal" data-target="#modal-default"> Eliminar Credito </a>';
                return buttons; }}
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
var dataTableRoles = null;
$edicion = false;
$idRolEditable = "none";
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
function loadDataTable() {
    // Cargar la tabla de roles
    dataTableRoles = $("#tableRoles").DataTable({
        "ajax": {
            "url": '/Roles/GetRoles/',
            "type": "GET",
            "dataType": "JSON"
        },
        'columns': [
            { 'data': 'descripcion' },
            { 'data': 'usuarios' },
            {
                'data': 'id', 'render': function (id) {

                    let buttons = "<button class='btn btn-sm btn-warning btn-editar'>Editar</button> | ";
                    buttons += '<a href="#" onclick=getHtmlData("/Roles/Delete/'+id+'")  class="btn btn-sm btn-danger" data-toggle="modal" data-target="#modal-default"><span> <i class="fa fa-trash"></i> </span> Eliminar </a>';
                    return buttons;
                }
            }
        ],
        "searching": false,
        "language": {
            "emptyTable": "No hay roles en el sistema",
            "paginate": {
                "first": "Primero",
                "last": "Ultimo",
                "next": "Siguiente",
                "previous": "Anterior"
            },
            "info": "Mostrando _START_ de _END_ en _TOTAL_ registros",
            "infoEmpty": "Mostrando 0 de 0 en 0 registros",
            "lengthMenu": "Mostar _MENU_ registros",
        }
    });
    // Evento editar
    $("#tableRoles tbody").on("click", "button.btn-editar", function () {
        var data = dataTableRoles.row($(this).parents('tr')).data();
        $idRolEditable = data.id;
        $edicion = true;
        $("#Descripcion").val(data.descripcion);
        $("#tableRoles").find("input,button,textarea,select").attr("disabled", "false");
    });
}
function cancelarEdicion() {
    $("#tableRoles").find("input,button,textarea,select").removeAttr("disabled");
    $idRolEditable = null;
    $edicion = false;
    $("#Descripcion").val("");
}

function SubmitForm(form) {
    $.validator.unobtrusive.parse(form);
    if ($(form).valid()) {
        $.ajax({
            type: "POST",
            url: form.action,
            data: $(form).serialize() + "&idRol=" + $idRolEditable,
            success: function (data) {
                if (data.success) {
                    dataTableRoles.ajax.reload();
                    cancelarEdicion();
                }
                $.notify({
                    // Opciones
                    message: data.mensaje,
                    title: "Resultado.",
                    allow_dismiss: true,
                    placement: {
                        from: "top",
                        align: "center"
                    },
                },
                {
                    // Ajustes
                    type: data.success ? 'success' : 'danger',
                    allow_dismiss: true,
                    placement: {
                        from: "top",
                        align: "center"
                    },
                });
            },
            error: function (data) {
                $.notify({
                    // Opciones
                    message: data.mensaje,
                    title: "Error  ",
                    allow_dismiss: true,
                    placement: {
                        from: "top",
                        align: "center"
                    },
                },
                {
                    // Ajustes
                    type: 'danger',
                    allow_dismiss: true,
                    placement: {
                        from: "top",
                        align: "center"
                    },
                });
            }
        });
    }
    return false;
}
function deleteRoles(form) {
    $.ajax({
        type: "POST",
        dataType: "JSON",
        url: form.action,
        data: $(form).serialize(), 
        success: function (data) {
            if (data.success) {
                dataTableRoles.ajax.reload();
                cancelarEdicion();
            }
            $("#modal-default .close").click(); //Ocultar modal
            $.notify({
                // Opciones
                message: data.mensaje
            },
            {
                // Ajustes
                type: data.success ? 'success' : 'danger',
                allow_dismiss: true,
                placement: {
                    from: "top",
                    align: "center"
                },
            });
            
        },
        error: function (data) {
            $("#modal-default .close").click(); //Ocultar modal
            $.notify({
                // Opciones
                message: data.mensaje,
                allow_dismiss: true
            },
            {
                // Ajustes
                type: 'danger',
                allow_dismiss: true,
                placement: {
                    from: "top",
                    align: "center"
                },
            });
        }
    });
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
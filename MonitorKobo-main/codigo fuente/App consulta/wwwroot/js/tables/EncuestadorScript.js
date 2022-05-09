//*********************************** VARIABLES ******************************************
var root;

//*********************************** funcLE ******************************************

var funcLE = {
    loadLocations: function () {
        $(".selectLocation").change(function () {
            var selected = $(this).children("option:selected").val();
            var childId = $(this).data('child');

            $("#" + childId).find('option').remove();
            $("#" + childId).append("<option value=''>Seleccione una opción</option>");
            $("#" + childId).attr('disabled', 'disabled');

            if (selected) {
                var fullurl = root + "Encuestador/LocationsAjax/";
                $.post(fullurl, { IdParent: selected }).
                    done(function (data) {
                        if (data != null) {
                            $("#" + childId).removeAttr('disabled');
                            for (var i = 0; i < data.length; i++) {
                                $("#" + childId).append("<option value='" + data[i].id + "'>" + data[i].name + "</option>");
                            }
                        } else {
                            funcGenerico.mostrarMensaje("Error en la respuesta del servidor.", "error");
                        }
                    }).fail(function (data) {
                        funcGenerico.mostrarMensaje("Error al solicitar la operación.", "error");
                    });
            }
            
        });
    },
    init: function () {
        // Carga las variables de configuración.
        root = $('#Root').val();
        funcLE.loadLocations();
    }
};

//************************************** ON READY **********************************************
$(function () {

    DevExpress.localization.locale("es-US");
    funcLE.init();
});







//*********************************** VARIABLES ******************************************
var root_print;
var idsPrint = [];
//*********************************** funcLE ******************************************

var funcPrint = {
    changeState: function () {
        if (idsPrint.length > 0) {
            var fullurl = root_print + "Formalizacion/Imprimir/";
            var idsParam = idsPrint.join();
            $.post(fullurl, { ids: idsParam }).
                done(function (data) {
                    if (data != null) {
                        console.log(data);
                    } else {
                        console.log("Error en la respuesta del servidor.");
                    }
                }).fail(function (data) {
                    console.log("Error al solicitar la operación.");
                });
        }
    },
    init: function () {
        // Carga las variables de configuración.
        root_print = $('#Root').val();

        if (typeof myIdsPrint !== "undefined") {
            idsPrint = myIdsPrint;
        }
    }
};

//************************************** ON READY **********************************************
$(function () {
    funcPrint.init();

    window.onbeforeprint = function () {
        console.log("Print");
        funcPrint.changeState();
    };

});







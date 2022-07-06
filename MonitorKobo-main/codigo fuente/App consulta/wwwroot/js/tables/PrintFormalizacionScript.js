//*********************************** VARIABLES ******************************************
var root_print;
var idsPrint = [];
var project = 0;
//*********************************** funcLE ******************************************

var funcPrint = {
    changeState: function () {
        if (idsPrint.length > 0) {
            var fullurl = root_print + "Validation/Imprimir/";
            var idsParam = idsPrint.join();
            $.post(fullurl, { ids: idsParam, project: project }).
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
    LoadPrintExtraPages: function () {
        $('body').on('change', '#print_ck', function (e) {
            var value = $('input#print_ck:checkbox:checked').length;
            if (value == 1) {
                $('.pageExtra').removeClass('d-print-block');
            } else {
                $('.pageExtra').addClass('d-print-block');
            }
        });
    },
    init: function () {
        // Carga las variables de configuración.
        root_print = $('#Root').val();

        if (typeof myIdsPrint !== "undefined") {
            idsPrint = myIdsPrint;
        }
        if (typeof myProject !== "undefined") {
            project = myProject;
        }
        funcPrint.LoadPrintExtraPages();
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







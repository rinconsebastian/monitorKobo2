//*********************************** VARIABLES ******************************************
var root_v;
var idForm = 0;
var waitClose = false;
//*********************************** funcLE ******************************************

var funcVali = {
    showPrint: function (url) {
        var myWindow = window.open(url, "_blank", "toolbar=no,titlebar=no,menubar=no,scrollbars=yes,resizable=no,top=0,left=386,width=1000,height=700");

        myWindow.onload = function () {
            myWindow.onunload = function () {
                if (waitClose) {
                    goBack();
                }
            };
        }
    },
    loadPrintFormlz: function () {
        $('body').on('click', '.btn-print-formlz', function (e) {
            var url = $(this).attr("data-link");
            funcVali.showPrint(url);
        });
    },
    loadFinalizar: function () {
        $('body').on('click', '.btn-finalizar', function (e) {
            var button = $(this);
            var accion = button.data('accion');
            console.log(accion);

            var print = "";
            var estado = 0;

            switch (accion) {
                case "finalizar":
                    estado = 2;
                    break;
                case "imprimir":
                    estado = 2;
                    print = $(this).data("link");
                    break;
                case "cancelar":
                    estado = 3;
                    break;
            }

            if (estado != 0) {
                $(this).find('i').addClass('fa-cog fa-spin');
                $('.btn-finalizar').attr('disabled', 'disabled');

                var fullurl = root_v + "Formalizacion/CambiarEstado/";
                $.post(fullurl, { id: idForm, estado: estado }).
                    done(function (data) {
                        button.find('i').removeClass('fa-cog fa-spin');
                        $('.btn-finalizar').removeAttr('disabled');
                        if (data != null) {
                            if (data.success) {
                                if (print != "") {
                                    waitClose = true;
                                    funcVali.showPrint(print);
                                } else {
                                    goBack();
                                  // funcGenerico.mostrarMensaje(data.message, "success");
                                }
                            } else {
                                funcGenerico.mostrarMensaje(data.message, "error");
                            }
                        } else {
                            funcGenerico.mostrarMensaje("Error en la respuesta del servidor.", "error");
                        }
                    }).fail(function (data) {
                        button.find('i').removeClass('fa-cog fa-spin');
                        $('.btn-finalizar').removeAttr('disabled');
                        funcGenerico.mostrarMensaje("Error al solicitar la operación.", "error");
                    });
            }
        });
    },

    init: function () {
        // Carga las variables de configuración.
        root_v = $('#Root').val();
        idForm = $('#IdFormalizacion').val();
        funcVali.loadFinalizar();
        funcVali.loadPrintFormlz();
    }
};

//************************************** ON READY **********************************************
$(function () {

    funcVali.init();
});


function goBack() {
    if ('referrer' in document) {
        window.location = document.referrer;
    } else {
        window.history.back();
    }
}




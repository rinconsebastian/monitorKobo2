//*********************************** VARIABLES ******************************************

//*********************************** FUNCTIONS ******************************************

var funcGenerico = {
    //General
    mostrarMensaje: function (mensaje, alerta) {
        if (mensaje != "") {
            $("#toastGenerico").dxToast("instance").option("message", mensaje);
            $("#toastGenerico").dxToast("instance").option("type", alerta);
            $("#toastGenerico").dxToast("show");
        }
    },
    mostrarMensajeReload: function (mensaje) {
        if (mensaje != "") {
            $("#toastGenericoReload").dxToast("instance").option("message", mensaje);
            $("#toastGenericoReload").dxToast("show");
        }
    },
    loadToasts: function () {
        var container = "#formCons";
        $("#toastGenerico").dxToast({
            type: "error",
            position: {
                my: "center",
                at: "center",
                of: container
            },
            width: 400,
            height: 50,
            displayTime: 3000,
            closeOnClick: true,
            closeOnOutsideClick: true
        });

        $("#toastGenericoReload").dxToast({
            type: "success",
            position: {
                my: "center",
                at: "center",
                of: container
            },
            width: 500,
            displayTime: 3000,
            closeOnClick: true,
            closeOnOutsideClick: true,
            onHidden: function (e) {
                window.location.reload();
            }
        });
    },

    init: function () {

        funcGenerico.loadToasts();
    }
};

//************************************** ON READY **********************************************
$(function () {
    funcGenerico.init();
});


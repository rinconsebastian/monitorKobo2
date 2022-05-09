var root;

var funcUpload = {
    loadUploadFile: function () {
        $('body').on('submit', '#formSolicitud', function (e) {
            e.preventDefault();
            var formulario = $(this).closest('form');

            // Serialize your form
            var formData = new FormData(formulario[0]);
            var time = funcUpload.getFormatDate("Solic");
            formData.append("time", time);


            $('#info').empty();
            $('#uploading').html('Cargando <img src="' + root + 'images/ajax-loader.gif">');
            $('.btn-form').attr('disabled', 'disabled');

            $.ajax({
                type: "POST",
                url: formulario.attr('action'),
                data: formData,
                dataType: 'json',
                contentType: false,
                processData: false,
                success: function (response) {
                    if (response == '') {
                        window.location.replace(root + "Solicitud");
                    }
                    else {
                        $('#info').html(response);
                    }
                    $('#uploading').empty();
                    $('.btn-form').removeAttr('disabled');
                },
                error: function (error) {
                    $('#uploading').empty();
                    $('.btn-form').removeAttr('disabled');
                    $('#info').html("Error al realizar la operación.");
                }
            });

        });
    },
    getFormatDate: function (key) {
        var dt = new Date();
        var day = dt.getDate();
        var month = dt.getMonth() + 1;
        var hour = dt.getHours();
        var minute = dt.getMinutes();
        var seconds = dt.getSeconds();
        if (day < 10) { day = "0" + day; }
        if (month < 10) { month = "0" + month; }
        if (hour < 10) { hour = "0" + hour; }
        if (minute < 10) { minute = "0" + minute; }
        if (seconds < 10) { seconds = "0" + seconds; }
        return key + "-" + dt.getFullYear() + "_" + month + "_" + day + "-" + hour + "_" + minute + "_" + seconds;
    },
    showSelection: function () {
        $('body').on('change', 'input[type="file"].custom-file-input', function (e) {
            if (e.target.files.length > 0) {
                var fileName = e.target.files[0].name;
                $(this).next('.custom-file-label').html(fileName);
            } else {
                $(this).next('.custom-file-label').html('Seleccionar Archivo');
            }
        });
    },
  
    init: function () {
        console.log("Adjunto Script load");
        root = $('#Root').val();
        funcUpload.loadUploadFile();

        funcUpload.showSelection();
    }
};


//************************************** ON READY **********************************************
$(function () {
    funcUpload.init();
});



//*********************************** VARIABLES ******************************************
var root;
var image;
var cropper;
var result;

var filter;

var pathStorage = "";

var currentImg;

var img;
//*********************************** funcImg ******************************************

var funcImg = {
    //Set image
    loadRotar: function () {
        $('body').on('click', '.btn-rotar', function (e) {
            var degree = $(this).data('option');
            cropper.rotate(degree);
            funcImg.updateRotate(true);

        });
    },
    loadZoom: function () {
        $('body').on('click', '.btn-zoom', function (e) {
            var val = $(this).data('option');
            if (val == "1") {
                cropper.zoom(0.1);
            } else {
                cropper.zoom(-0.1)
            }
            console.log(e.type, e.detail.ratio);

        });
    },
    loadColor: function () {
        $('body').on('change', '.filter-color', function (e) {
            funcImg.updateFilter();
        });
    },
    loadRecortar: function () {
        $('body').on('click', '.btn-recortar', function (e) {
            funcImg.updateFilter();

            var canvas = cropper.getCroppedCanvas({
                maxWidth: 1280,
                maxHeight: 1280,
                fillColor: '#fff',
                imageSmoothingEnabled: false,
                imageSmoothingQuality: 'medium'
            });
            var croppedImageDataURL = canvas.toDataURL("image/jpg", 0.5);
            img.src = croppedImageDataURL;

            $('.btn-save').removeAttr('disabled');

        });
    },
    loadRotate: function () {
        $('body').on('change', '#angle', function (e) {
            var angle = $('#angle').val();
            cropper.rotateTo(angle);
            funcImg.updateRotate(false);
        });
    },
    loadReset: function () {
        $('body').on('click', '.btn-reset', function (e) {
            funcImg.cleanResult();
            funcImg.resetFilter();
            cropper.reset();
            funcImg.updateRotate(true);

        });
    },
    //Save
    loadCropper: function () {
        image = document.getElementById('image');
        cropper = new Cropper(image, {
            minContainerWidth: 450,
            minContainerHeight: 450,
            autoCropArea: 1,
            movable: false,
            scalable: false,
            zoomable: true,
            zoomOnWheel: false,
            toggleDragModeOnDblclick: false,
            autoCrop: false,
            ready() {
                funcImg.changeStateBtns(true);
                this.cropper.crop();
            },
        });
    },
    loadSave: function () {
        $('body').on('submit', '#formUpload', function (e) {
            e.preventDefault();
            var formulario = $(this).closest('form');

            var canvas = document.querySelector("canvas");
            canvas.toBlob((blob) => {

                var formData = new FormData(formulario[0]);
                var filename = $('#inputFilename').val();

                $('#uploadingInfo').html('Cargando <img src="' + root + 'images/ajax-loader.gif">');
                $('.btn-save').attr('disabled', 'disabled');

                formData.append('file', blob);
                $.ajax({
                    type: "POST",
                    url: formulario.attr('action'),
                    data: formData,
                    dataType: 'json',
                    contentType: false,
                    processData: false,
                    success: function (data) {
                        if (data != null) {
                            if (data.success) {
                                $('#modalEditImage').modal('hide');
                                funcImg.reloadImageOrFile(currentImg, filename, "Imagen actualizada correctamente.");
                                cropper.destroy();
                            } else {
                                $('#uploadingInfo').html('<div class="text-danger">' + data.message + '</div>');
                                $('.btn-save').removeAttr('disabled');
                            }
                        } else {
                            $('#uploadingInfo').html('<div class="text-danger">Error en la respuesta del servidor.</div>');
                            $('.btn-save').removeAttr('disabled');
                        }
                    },
                    error: function (error) {
                        $('.btn-save').removeAttr('disabled');
                        $('#uploadingInfo').html('<div class="text-danger">Error al cargar el archivo.</div>');
                    }
                });
            }, "image/jpeg"); //"image/webp"
        });
    },
    //Load show modals
    loadShowModals: function () {
        //modal edit
        $('body').on('click', '.showEditModal', function (e) {
            var filename = $(this).data('filename');
            var d = new Date();

            currentImg = $(this).data('img');

            $('#image').attr('src', pathStorage + filename + "&time=" + d.getTime());
            $('#inputFilename').val(filename);
            funcImg.cleanResult();
            funcImg.resetFilter();
            funcImg.loadCropper();
            funcImg.updateRotate(true);
            $('#modalEditImage').modal('show');
        });
        $('#modalEditImage').on('hidden.bs.modal', function (e) {
            cropper.destroy();
            funcImg.changeStateBtns(false);
        });
        //modal reset
        $('body').on('click', '.btn-image-reset', function (e) {
            var name = $(this).data('name');
            var label = $(this).data('label');
            var container = $(this).data('container');

            $('#img-reset-name').html(label);

            $('#BtnConfirmReset').data("container", container);
            $('#BtnConfirmReset').data("name", name);
            $('#BtnConfirmReset').data("label", label);

            $('#modalConfirmReset').modal('show');
        });
        //modal delete
        $('body').on('click', '.btn-image-delete', function (e) {
            var name = $(this).data('name');
            var label = $(this).data('label');
            var container = $(this).data('container');

            $('#img-delete-name').html(label);

            $('#BtnConfirmDelete').data("container", container);
            $('#BtnConfirmDelete').data("name", name);
            $('#BtnConfirmDelete').data("label", label);

            $('#modalConfirmDelete').modal('show');
        });
        //modal load file
        $('body').on('click', '.btn-image-load', function (e) {
            var name = $(this).data('name');
            var label = $(this).data('label');
            var container = $(this).data('container');
            var type = $(this).data('type');

            if (type == "img") {
                $('#fileLoad').attr("accept", "image/png, image/gif, image/jpeg");
            } else {
                $('#fileLoad').attr("accept", "image/png, image/gif, image/jpeg, application/pdf, .zip");
            }

            //Reset inputs
            $('#fileLoad').val('');
            $('#fileLoad').next('.custom-file-label').html('Seleccionar Archivo');
            $('#img-load-name').html(label);

            $('#inputNameLoad').val(name);
            $('#BtnConfirmLoad').data("container", container);
            $('#BtnConfirmLoad').data("label", label);

            $('#modalUploadImage').modal('show');
        });
    },
    //Manage files
    loadFileUpload: function () {
        $('body').on('submit', '#formLoad', function (e) {

            e.preventDefault();

            var formulario = $(this).closest('form');

            var button = $('#BtnConfirmLoad');
            var container = button.data('container');
            var label = button.data('label');

            $('#loadInfo').html('Cargando <img src="' + root + 'images/ajax-loader.gif">');
            button.find('i').addClass('fa-cog fa-spin');
            $('.btn-modal-load').attr('disabled', 'disabled');

            var formData = new FormData(formulario[0]);
            $.ajax({
                type: "POST",
                url: formulario.attr('action'),
                data: formData,
                dataType: 'json',
                contentType: false,
                processData: false,
                success: function (data) {
                    button.find('i').removeClass('fa-cog fa-spin');
                    $('.btn-modal-load').removeAttr('disabled');
                    $('#loadInfo').html('');
                    if (data != null) {
                        if (data.success) {
                            $('#modalUploadImage').modal('hide');
                            funcImg.reloadImageOrFile(container, data.message, "Archivo '" + label + "'cargado correctamente.");
                        } else {
                            $('#loadInfo').html('<div class="alert alert-danger">' + data.message + '</div>');
                        }
                    } else {
                        $('#loadInfo').html('<div class="alert alert-danger">Error en la respuesta del servidor.</div>');
                    }
                },
                error: function (error) {
                    button.find('i').removeClass('fa-cog fa-spin');
                    $('.btn-modal-load').removeAttr('disabled');
                    $('#loadInfo').html('<div class="alert alert-danger">Error al solicitar la operación.</div>');
                }
            });

        });
    },
    loadImageReset: function () {
        $('body').on('click', '#BtnConfirmReset', function (e) {

            var button = $(this);
            var label = $(this).data('label');
            var container = $(this).data('container');
            var name = $(this).data('name');

            var idValidation = $('#IdValidation').val();
            var idProject = $('#IdProject').val();

            var fullurl = root + "KoBoData/ResetImage/";

            $('#resetInfo').html('Cargando <img src="' + root + 'images/ajax-loader.gif">');
            button.find('i').addClass('fa-cog fa-spin');
            $('.btn-modal-reset').attr('disabled', 'disabled');

            $.post(fullurl, { id: idValidation, projectid: idProject, name: name }).
                done(function (data) {
                    button.find('i').removeClass('fa-cog fa-spin');
                    $('.btn-modal-reset').removeAttr('disabled');
                    $('#resetInfo').html('');
                    if (data != null) {
                        if (data.success) {
                            $('#modalConfirmReset').modal('hide');
                            funcImg.reloadImageOrFile(container, data.message, "Archivo '" + label + "' restablecido correctamente.");
                        } else {
                            $('#resetInfo').html('<div class="alert alert-danger">' + data.message + '</div>');
                        }
                    } else {
                        $('#resetInfo').html('<div class="alert alert-danger">Error en la respuesta del servidor.</div>');
                    }

                }).fail(function (data) {
                    button.find('i').removeClass('fa-cog fa-spin');
                    $('.btn-modal-reset').removeAttr('disabled');
                    $('#resetInfo').html('<div class="alert alert-danger">Error al solicitar la operación.</div>');
                });
        });
    },
    loadImageDelete: function () {
        $('body').on('click', '#BtnConfirmDelete', function (e) {

            var button = $(this);
            var label = $(this).data('label');
            var container = $(this).data('container');
            var name = $(this).data('name');

            var idValidation = $('#IdValidation').val();
            var idProject = $('#IdProject').val();

            var fullurl = root + "Validation/DeleteNewFile/";

            $('#deleteInfo').html('Cargando <img src="' + root + 'images/ajax-loader.gif">');
            button.find('i').addClass('fa-cog fa-spin');
            $('.btn-modal-delete').attr('disabled', 'disabled');

            $.post(fullurl, { id: idValidation, projectid: idProject, name: name }).
                done(function (data) {
                    button.find('i').removeClass('fa-cog fa-spin');
                    $('.btn-modal-delete').removeAttr('disabled');
                    $('#deleteInfo').html('');
                    if (data != null) {
                        if (data.success) {
                            $('#modalConfirmDelete').modal('hide');
                            funcImg.reloadImageOrFile(container, data.message, "Archivo '" + label + "' borrado correctamente.");
                        } else {
                            $('#deleteInfo').html('<div class="alert alert-danger">' + data.message + '</div>');
                        }
                    } else {
                        $('#deleteInfo').html('<div class="alert alert-danger">Error en la respuesta del servidor.</div>');
                    }

                }).fail(function (data) {
                    button.find('i').removeClass('fa-cog fa-spin');
                    $('.btn-modal-delete').removeAttr('disabled');
                    $('#deleteInfo').html('<div class="alert alert-danger">Error al solicitar la operación.</div>');
                });
        });
    },
    //Load image canvas
    draw: function () {
        var canvas = document.querySelector("canvas"),
            ctx = canvas.getContext("2d");

        canvas.width = this.width;
        canvas.height = this.height;

        if (typeof ctx.filter !== "undefined") {
            ctx.filter = filter != "" ? filter : "none";
            ctx.drawImage(this, 0, 0);
        }
        else {
            ctx.drawImage(this, 0, 0);
        }
        var croppedImageDataURL = canvas.toDataURL("image/jpg");
        result.html($('<img>').attr('src', croppedImageDataURL).addClass('img-edit'));

    },
    //Extra
    reloadImageOrFile: function (id, newVal, text) {
        if (newVal != null) {
            var d = new Date();
            var extension = newVal.substr((newVal.lastIndexOf('.') + 1));
            if ($("img#" + id).length > 0) {
                if (extension == 'jpg' || extension == 'png' || extension == 'jpeg') {
                    $("#" + id).attr("src", pathStorage + newVal + "&time=" + d.getTime());
                    $('#btn-edit_'+id).data("filename", newVal);
                    funcGenerico.mostrarMensaje(text, "success");
                    return;
                }
            } else if ($("embed#" + id).length > 0) {
                if (extension == 'pdf') {
                    $("#" + id).attr("src", pathStorage + newVal + "&time=" + d.getTime());
                    funcGenerico.mostrarMensaje(text, "success");
                    return;
                }
            }
        }
        $("div#cont_" + id).html("<div class='alert alert-success'><b>" + text + "</b><br>Recargue la página para ver los cambios.</div>");
        funcGenerico.mostrarMensaje(" Recargue la página para ver los cambios.", "success");
    },
    updateFilter: function () {
        var brightness = $('#brightness').val();
        var contrast = $('#contrast').val();
        var saturate = $('#saturate').val();

        filter = "brightness(" + brightness + "%) " + "contrast(" + contrast + "%) " + " saturate(" + saturate + "%)";

        $(".cropper-container.cropper-bg").css("filter", filter);
        //$("#result img").css("filter", filter);

    },
    resetFilter: function () {
        $('#brightness').val(100);
        $('#contrast').val(100);
        $('#saturate').val(100);
        funcImg.updateFilter();
    },
    updateRotate: function (bar) {
        var rotate = cropper.getData().rotate;
        if (bar) {
            $('#angle').val(rotate)
        }
        $('#dataRotate').html(rotate + "°");
    },

    changeStateBtns: function (state) {

        if (state) {
            $('.btn-img').removeAttr('disabled');
        } else {
            $('.btn-img').attr('disabled', 'disabled');
        }
    },
    cleanResult: function () {
        $('#uploadingInfo').html('');
        $('.btn-save').attr('disabled', 'disabled');
        $('#result').html('<div class="preview">VISTA PREVIA</div>');
    },
    showSelectionFileInput: function () {
        $('body').on('change', 'input[type="file"].custom-file-input', function (e) {
            if (e.target.files.length > 0) {
                var fileName = e.target.files[0].name;
                $(this).next('.custom-file-label').html(fileName);
            } else {
                $(this).next('.custom-file-label').html('Seleccionar Archivo');
            }
        });
    },
    LoadValidImages: function () {
        $('body').on('change', '.ck_field', function (e) {
            var basic = $('#ck_bar').data('basic');
            var total = $('input.ck_field:checkbox').length;
            var selected = $('input.ck_field:checkbox:checked').length;
            var percent = basic + ((100 - basic) * selected / total);

            var tab = $(this).data('tab');
            $('#' + tab).removeClass('pill-complete');
            if ($(this).is(':checked')) {
                $('#' + tab).addClass('pill-complete');
            }

            if (percent > 95) {
                $('.ck_button').prop('disabled', false);
                $('#ck_panel').attr('style', 'display:none !important');
            } else {
                $('.ck_button').prop('disabled', true);
                $('#ck_panel').show();
            }
            $('#ck_bar').css('width', percent + '%').attr('aria-valuenow', percent);

        });
    },
    LoadSetAllValid: function () {
        document.addEventListener("keydown", function (event) {
            if (event.altKey && event.code === "KeyY") {
                $('input.ck_field:checkbox').attr('checked', 'checked');

                $('.pill-item.not-null').addClass('pill-complete');
                $('.ck_button').prop('disabled', false);
                $('#ck_panel').attr('style', 'display:none !important');

                $('#ck_bar').css('width', '100%').attr('aria-valuenow', 100);

                event.preventDefault();
            }
        });
        $('body').on('change', '.ck_field', function (e) {
            var basic = $('#ck_bar').data('basic');
            var total = $('input.ck_field:checkbox').length;
            var selected = $('input.ck_field:checkbox:checked').length;
            var percent = basic + ((100 - basic) * selected / total);

            var tab = $(this).data('tab');
            $('#' + tab).removeClass('pill-complete');
            if ($(this).is(':checked')) {
                $('#' + tab).addClass('pill-complete');
            }

            if (percent > 95) {
                $('.ck_button').prop('disabled', false);
                $('#ck_panel').attr('style', 'display:none !important');
            } else {
                $('.ck_button').prop('disabled', true);
                $('#ck_panel').show();
            }
            $('#ck_bar').css('width', percent + '%').attr('aria-valuenow', percent);

        });
    },
    init: function () {
        // Carga las variables de configuración.
        root = $('#Root').val();

        pathStorage = $('#PathStorage').val() + "?filename=";

        result = $('#result');

        //funcImg.loadCropper();
        funcImg.loadRotar();
        funcImg.loadRotate();
        funcImg.loadZoom();
        funcImg.loadRecortar();
        funcImg.loadReset();
        funcImg.loadSave();
        funcImg.loadColor();

        //modals
        funcImg.loadShowModals();
        //files
        funcImg.loadImageReset();
        funcImg.loadFileUpload();
        funcImg.loadImageDelete();

        //Extra functions
        funcImg.LoadValidImages();
        funcImg.LoadSetAllValid();
        funcImg.showSelectionFileInput();

        //Load image
        img = new Image();
        img.crossOrigin = "";
        img.onload = funcImg.draw;
    }
};

//************************************** ON READY **********************************************
$(function () {
    funcImg.init();
});



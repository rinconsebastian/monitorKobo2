//*********************************** VARIABLES ******************************************
var root;
var dataGrid;
var source = "";

var showExport = false;
var showDNI = false;
var showValidation = false;
var loadValidation = false;
var code = "";
//*********************************** funcLE ******************************************

var funcLE = {


    instanceDataGrid: function () {
        dataGrid = $("#gridContainer").dxDataGrid({
            dataSource: source,
            selection: {
                mode: "none",
                showCheckBoxesMode: "always",
                selectAllMode: "allPages"
            },
            noDataText: "No hay datos disponibles.",
            export: {
                enabled: showExport,
                fileName: "Listado_caracterizaciones_" + moment().format("DD-MM-YYYY_hh-mm-ss"),
                allowExportSelectedData: false
            },
            onExporting: function (e) {
                e.component.beginUpdate();
                if (showValidation) {
                    e.component.columnOption("Opciones", "visible", false);
                }

                if (!showDNI) {
                    if (showValidation) {
                        e.component.columnOption("status", "visible", true);
                    }
                    e.component.columnOption("user", "visible", true);
                    e.component.columnOption("userName", "visible", true);
                }
                
            },
            onExported: function (e) {
                if (showValidation) {
                    e.component.columnOption("Opciones", "visible", true);
                }
                if (!showDNI) {
                    e.component.columnOption("status", "visible", false);
                    e.component.columnOption("user", "visible", false);
                    e.component.columnOption("userName", "visible", false);
                }
                e.component.endUpdate();
            },

            stateStoring: {
                enabled: true,
                type: "localStorage",
                storageKey: "storage"
            },
            loadPanel: {
                enabled: true
            },

            paging: {
                pageSize: 50
            },

            scrolling: {
                mode: "virtual"
            },
            height: '72vh',
            width: '100%',
            columnFixing: {
                enabled: true
            },
            wordWrapEnabled: false,
            rowAlternationEnabled: false,
            columnHidingEnabled: true,
            showRowLines: true,
            grouping: {
                contextMenuEnabled: true,
                expandMode: "rowClick"
            },
            groupPanel: {
                emptyPanelText: "Haga clic derecho en una columna para agruparla",
                visible: true
            },
            pager: {
                showPageSizeSelector: true,
                allowedPageSizes: [10, 20, 50, 100, 1000]
            },
            columnChooser: {
                enabled: false
            },
            allowColumnReordering: false,
            allowColumnResizing: true,
            columnAutoWidth: false,
            showBorders: true,
            filterRow: {
                visible: true,
                applyFilter: "auto"
            },
            searchPanel: {
                visible: true,
                width: 160,
                placeholder: "Buscar..."
            },
            headerFilter: {
                visible: true
            },
            columns: [

                {
                    dataField: "dni",
                    caption: "Cedula\r\nPescador",
                    headerCellTemplate: function (header, info) {
                        $("<div>").html(info.column.caption.replace(/\r\n/g, "<br/>")).appendTo(header);
                    },
                    alignment: "center",
                    width: '120',
                    hidingPriority: 10
                },

                {
                    dataField: "name",
                    caption: "Nombre\r\nPescador",
                    headerCellTemplate: function (header, info) {
                        $("<div>").html(info.column.caption.replace(/\r\n/g, "<br/>")).appendTo(header);
                    },
                    alignment: "center",
                    width: '120',
                    hidingPriority: 5
                },
              
                {
                    dataField: "mun",
                    caption: "Municipio",
                    alignment: "center",
                    width: '120',
                    hidingPriority: 8
                },
                {
                    dataField: "dep",
                    caption: "Departamento",
                    alignment: "center",
                    width: '120',
                    hidingPriority: 2
                },
               
                {
                    dataField: "datetime",
                    caption: "Fecha",
                    alignment: "center",
                    width: '100',
                    hidingPriority: 9,
                    dataType: "date",
                    calculateFilterExpression: function (value, selectedFilterOperations, target) {
                        if (target === "headerFilter" && value === "weekends") {
                            return [[getOrderDay, "=", 0], "or", [getOrderDay, "=", 6]];
                        }
                        return this.defaultCalculateFilterExpression.apply(this, arguments);
                    }
                },

                {
                    dataField: "user",
                    caption: "Cedula\r\nEncuestador",
                    headerCellTemplate: function (header, info) {
                        $("<div>").html(info.column.caption.replace(/\r\n/g, "<br/>")).appendTo(header);
                    },
                    alignment: "center",
                    visible: showDNI,
                    width: '120',
                    hidingPriority: 3
                },

                {
                    dataField: "userName",
                    caption: "Nombre\r\nEncuestador",
                    headerCellTemplate: function (header, info) {
                        $("<div>").html(info.column.caption.replace(/\r\n/g, "<br/>")).appendTo(header);
                    },
                    alignment: "center",
                    visible: showDNI,
                    width: '200',
                    hidingPriority: 1
                },
                {
                    dataField: "carnet",
                    caption: "Carnet",
                    visible: (showValidation),
                    alignment: "center",
                    width: '100',
                    hidingPriority: 4
                },
                {
                    dataField: "status",
                    caption: "Formalización",
                    visible: (showValidation && showDNI),
                    alignment: "center",
                    width: '100',
                    hidingPriority: 6
                },
                {
                    
                    caption: "Opciones",
                    visible: showValidation,
                    alignment: "center",
                    hidingPriority: 7,
                    width: '80',
                    cellTemplate: function (container, options) {

                        var idKobo = options.data.idKobo;
                        var val = options.data.validation;
                        var formId = options.data.formalizacionId;
                        var status = options.data.formalizacionEstado;

                        var contenido = "No";
                        if (formId != 0) {
                            contenido = '<a href="' + root + 'Formalizacion/Details/' + formId + '" title="Detalles" class="btn btn-outline-info btn-xs ml-1" ><i class="fas fa-file-alt"></i></a>'
                            if (loadValidation && status == 1) {
                                contenido += '<a href="' + root + 'Formalizacion/Edit/' + formId + '" title="Editar " class="btn btn-outline-warning btn-xs ml-1" ><i class="fas fa-edit"></i></a>'
                            }
                        } else if (val && loadValidation) {
                            contenido = '<button class="btn btn-outline-success btn-xs ml-1 load-formlz" data-id="' + idKobo + '" title="Cargar datos"><i class="fas fa-download"></i></button>';
                        }

                        $("<div class='preventSelection'>")
                            .append(contenido)
                            .appendTo(container);
                    }

                },
                
            ],
            summary: {
                totalItems: [{
                    column: "user",
                    summaryType: "count",
                    showInColumn: "dni",
                    displayFormat: "Total: {0}",
                }],

            },

            onToolbarPreparing: function (e) {
                var dataGrid = e.component;
                e.toolbarOptions.items.unshift(
                    {
                        location: "after",
                        widget: "dxButton",
                        options: {
                            icon: "refresh",
                            hint: "Actualizar",
                            onClick: function () {
                                dataGrid.refresh();
                            }
                        }
                    },
                    {
                        location: "after",
                        widget: "dxButton",
                        options: {
                            icon: "clearformat",
                            hint: "Borrar filtros",
                            onClick: function () {
                                dataGrid.state(null);
                            }
                        }
                    }
                );
            },
            onRowPrepared: function (e) {
                if (e.rowType === "data") {
                    if (e.data.status == "Pend.") {
                        e.rowElement.css('background-color', '#ffe6de');
                    } else if (e.data.status == "Si") {
                        e.rowElement.css('background-color', '#d7ffd1');
                    }
                }

            }
        }).dxDataGrid('instance');
    },

    loadFormalizacion: function () {
        $('body').on('click', '.load-formlz', function (e) {
            var idKobo = $(this).data('id');
            var btn = $(this);
            $(this).find('i').removeClass('fa-download');
            $(this).find('i').addClass('fa-cog fa-spin');
            $(this).attr('disabled', 'disabled');

            var fullurl = root + "Formalizacion/Cargar/";
            
            $.post(fullurl, { idKobo: idKobo }).
                done(function (data) {
                    btn.find('i').addClass('fa-download');
                    btn.find('i').removeClass('fa-cog fa-spin');
                    btn.removeAttr('disabled');
                    if (data != null) {
                        if (data.success) {
                            if (data.url != null) {
                                window.location.href = root + data.url;
                            } else {
                                funcGenerico.mostrarMensaje(data.message, "success");
                            }
                        } else {
                            funcGenerico.mostrarMensaje(data.message, "error");
                        }
                    } else {
                        funcGenerico.mostrarMensaje("Error en la respuesta del servidor.", "error");
                    }
                }).fail(function (data) {
                    $(this).find('i').addClass('fa-download');
                    $(this).find('i').removeClass('fa-cog fa-spin');
                    $(this).removeAttr('disabled');
                    funcGenerico.mostrarMensaje("Error al solicitar la operación.", "error");
                });
        });
    },

    init: function () {
        // Carga las variables de configuración.
        root = $('#Root').val();
        code = $('#code').val();

        showExport = $('#showExport').val() == 1;

        if (typeof myShowDni !== "undefined") {
            showDNI = myShowDni;
        }
        if (typeof myShowValidation !== "undefined") {
            showValidation = myShowValidation;
        }
        if (typeof myLoadValidation !== "undefined") {
            loadValidation = myLoadValidation;
        }
       
        if (code != "") {
            source = root + "Kobo/ListadoEncuestasUsuario/?code=" + code;
        } else {
            source = root + "Kobo/ListadoEncuestas";
        }
        

        funcLE.instanceDataGrid();
        funcLE.loadFormalizacion();
    }
};

//************************************** ON READY **********************************************
$(function () {

    DevExpress.localization.locale("es-US");
    funcLE.init();
});








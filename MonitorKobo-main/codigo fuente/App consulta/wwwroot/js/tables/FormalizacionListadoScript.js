//*********************************** VARIABLES ******************************************
var root;
var dataGrid;
var source = "";

var showExport = false;
var ShowValidar = false;
var showPrint = false;
var urlPrint = "";

//*********************************** funcForm ******************************************

var funcForm = {
    imprimirMultiple: function (seleccion) {
        if (seleccion.length > 0) {
            var ids = $.map(seleccion, function (n, i) {
                return n.id+"";
            });
            var url = urlPrint + "?ids=" + ids.join('&ids=');
            var myWindow = window.open(url, "_blank", "toolbar=no,titlebar=no,menubar=no,scrollbars=yes,resizable=no,top=0,left=386,width=1000,height=700");
        } else {
            DevExpress.ui.notify("No hay formalizaciones seleccionadas.", 'warning', 3000);
        }
    },

    instanceDataGrid: function () {
        dataGrid = $("#gridContainer").dxDataGrid({
            dataSource: source,
            selection: {
                mode: showPrint ? "multiple" : "none",
                showCheckBoxesMode: "always",
                selectAllMode: "allPages"
            },
            noDataText: "No hay datos disponibles.",
            export: {
                enabled: showExport,
                fileName: "Listado_formalizaciones_" + moment().format("DD-MM-YYYY_hh-mm-ss"),
                allowExportSelectedData: false
            },
            onExporting: function (e) {
                e.component.beginUpdate();
                e.component.columnOption("Opciones", "visible", false);
            },
            onExported: function (e) {
                e.component.columnOption("Opciones", "visible", true);
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
            showRowLines: true,
            pager: {
                showPageSizeSelector: true,
                allowedPageSizes: [10, 20, 50, 100, 1000]
            },
            columnChooser: {
                enabled: false
            },
            allowColumnReordering: false,
            allowColumnResizing: true,
            columnAutoWidth: true,
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
                    dataField: "registro",
                    caption: "Número\r\nRegistro",
                    headerCellTemplate: function (header, info) {
                        $("<div>").html(info.column.caption.replace(/\r\n/g, "<br/>")).appendTo(header);
                    },
                    alignment: "center",
                    width: '80',
                    hidingPriority: 9,
                },
                {
                    dataField: "cedula",
                    caption: "Cedula\r\nPescador",
                    headerCellTemplate: function (header, info) {
                        $("<div>").html(info.column.caption.replace(/\r\n/g, "<br/>")).appendTo(header);
                    },
                    alignment: "center",
                    width: '80',
                    hidingPriority: 8,
                },
                {
                    dataField: "nombre",
                    caption: "Nombre\r\nPescador",
                    headerCellTemplate: function (header, info) {
                        $("<div>").html(info.column.caption.replace(/\r\n/g, "<br/>")).appendTo(header);
                    },
                    alignment: "center",
                    width: '180',
                    hidingPriority: 6,
                },
                {
                    dataField: "municipio",
                    caption: "Muninicipio",
                    alignment: "center",
                    width: '130',
                    hidingPriority: 5
                },
                {
                    dataField: "departamento",
                    caption: "Departamento",
                    alignment: "center",
                    width: '130',
                    hidingPriority: 1
                },
                {
                    dataField: "coordinacion",
                    caption: "Coordinación",
                    alignment: "center",
                    width: '190',
                    hidingPriority: 3
                },
                {
                    dataField: "fecha",
                    caption: "Fecha",
                    alignment: "center",
                    width: '80',
                    hidingPriority: 4,
                    dataType: "date",
                    calculateFilterExpression: function (value, selectedFilterOperations, target) {
                        if (target === "headerFilter" && value === "weekends") {
                            return [[getOrderDay, "=", 0], "or", [getOrderDay, "=", 6]];
                        }
                        return this.defaultCalculateFilterExpression.apply(this, arguments);
                    }
                },
                {
                    dataField: "nombreEstado",
                    caption: "Estado",
                    alignment: "center",
                    width: '70',
                    hidingPriority: 2,
                    cellTemplate: function (container, options) {
                        var nombre = options.data.nombreEstado;
                        var color = "";
                        switch (options.data.estado) {
                            case 1:
                                color = "bg-warning text-dark";
                                break;
                            case 2:
                                color = "bg-success text-white";
                                break;
                            case 3:
                            case 5:
                                color = "bg-danger text-white";
                                break;
                            case 4:
                                color = "bg-info text-white";
                                break;
                        }
                        contenido = '<h6 class="mb-0"><span class="badge ' + color + '">' + nombre + '</span></h6>';
                        $("<div class='preventSelection'>")
                            .append(contenido)
                            .appendTo(container);
                    }
                },

                {
                    dataField: "Opciones",
                    caption: "Opciones",
                    alignment: "center",
                    allowHeaderFiltering: false,
                    width: '70',
                    hidingPriority: 7,
                    cellTemplate: function (container, options) {

                        var formId = options.data.id;
                        var status = options.data.estado;

                        var contenido = "";
                        if (formId != 0) {
                            contenido = '<a href="' + root + 'Formalizacion/Details/' + formId + '" title="Detalles" class="btn btn-outline-info btn-xs ml-1" ><i class="fas fa-file-alt"></i></a>'
                            if (ShowValidar && status == 1) {
                                contenido += '<a href="' + root + 'Formalizacion/Edit/' + formId + '" title="Editar " class="btn btn-outline-warning btn-xs ml-1" ><i class="fas fa-edit"></i></a>'
                            }
                        }

                        $("<div class='preventSelection'>")
                            .append(contenido)
                            .appendTo(container);
                    }

                }
            ],
            summary: {
                totalItems: [{
                    column: "id",
                    summaryType: "count",
                    showInColumn: "cedula",
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
                    },
                    {
                        location: "after",
                        widget: "dxButton",
                        options: {
                            icon: "print",
                            hint: "Imprimir",
                            visible: showPrint,
                            onClick: function () {
                                var seleccion = dataGrid.option('selectedRowKeys');
                                funcForm.imprimirMultiple(seleccion);
                            }
                        }
                    }
                );
            }
        }).dxDataGrid('instance');
    },

    init: function () {
        // Carga las variables de configuración.
        root = $('#Root').val();
        source = root + "Formalizacion/Ajax/";
        showExport = $('#showExport').val() == 1;
        showPrint = $('#showPrint').val() == 1;
        urlPrint = $('#urlPrint').val();

        ShowValidar = $('#ShowValidate').val() == 1;

        funcForm.instanceDataGrid();
    }
};

//************************************** ON READY **********************************************
$(function () {

    DevExpress.localization.locale("es-US");
    funcForm.init();
});
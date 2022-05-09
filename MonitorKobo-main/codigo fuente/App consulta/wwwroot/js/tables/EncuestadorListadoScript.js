//*********************************** VARIABLES ******************************************
var root;
var dataGrid;
var source = "";

var showExport = false;
var showEditar = false;
var showDelete = false;

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
                fileName: "Listado_encuestadores_" + moment().format("DD-MM-YYYY_hh-mm-ss"),
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
            columnHidingEnabled: true,
            rowAlternationEnabled: true,
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
                    dataField: "cedula",
                    caption: "Cedula",
                    alignment: "center",
                    width: '100',
                    dataType: "string",
                    hidingPriority: 9
                },
                {
                    dataField: "nombre",
                    caption: "Nombre",
                    alignment: "center",
                    width: '220',
                    hidingPriority: 8
                },
                {
                    dataField: "municipio",
                    caption: "Municipio",
                    alignment: "center",
                    width: '130',
                    hidingPriority: 3

                },
                {
                    dataField: "departamento",
                    caption: "Departamento",
                    alignment: "center",
                    width: '100',
                    hidingPriority: 4
                },
                {
                    dataField: "coordinacion",
                    caption: "Coordinación",
                    alignment: "center",
                    width: '150',
                    hidingPriority: 5
                },
                {
                    dataField: "telefono",
                    caption: "Teléfono",
                    alignment: "center",
                    width: '120',
                    hidingPriority: 2
                },
                {
                    dataField: "email",
                    caption: "Correo electrónico",
                    alignment: "center",
                    width: '120',
                    hidingPriority: 1
                },
                {
                    dataField: "numeroEncuestas",
                    caption: "Nº\r\nCaracterizaciones",
                    headerCellTemplate: function (header, info) {
                        $("<div>").html(info.column.caption.replace(/\r\n/g, "<br/>")).appendTo(header);
                    },
                    alignment: "center",
                    width: '100',
                    hidingPriority: 6
                },
                {
                    dataField: "numeroAsociaciones",
                    caption: "Nº\r\nAsociaciones",
                    headerCellTemplate: function (header, info) {
                        $("<div>").html(info.column.caption.replace(/\r\n/g, "<br/>")).appendTo(header);
                    },
                    alignment: "center",
                    width: '100',
                    hidingPriority: 7
                },
                {
                    dataField: "Opciones",
                    hidingPriority: 10,
                    caption: "Opciones",
                    alignment: "left",
                    allowHeaderFiltering: false,
                    width: '80',
                    cellTemplate: function (container, options) {

                        var idEnc = options.data.id;
                        var nombre = options.data.nombre;
                        var contenido = '<a href="' + root + 'Encuestador/Details/' + idEnc + '" title="Detalles encuestador ' + nombre + '" class="btn btn-outline-info btn-xs ml-1" ><i class="fas fa-file-alt"></i></a>'
                        if (showEditar) {
                            contenido += '<a href="' + root + 'Encuestador/Edit/' + idEnc + '" title="Editar encuestador ' + nombre + '" class="btn btn-outline-success btn-xs ml-1" ><i class="fas fa-edit"></i></a>'
                        }
                        if (showDelete) {
                            contenido += '<a href="' + root + 'Encuestador/Delete/' + idEnc + '" title="Borrar encuestador ' + nombre + '" class="btn btn-outline-danger btn-xs ml-1" ><i class="fas fa-trash"></i></a>'
                        }

                        $("<div class='preventSelection'>")
                            .append(contenido)
                            .appendTo(container);
                    }

                }
            ],
            summary: {
                totalItems: [
                    {
                        column: "id",
                        summaryType: "count",
                        showInColumn: "cedula",
                        displayFormat: "Total: {0}",
                    },
                    {
                        column: "numeroAsociaciones",
                        summaryType: "sum",
                        displayFormat: "{0}",
                    },
                    {
                        column: "numeroEncuestas",
                        summaryType: "sum",
                        displayFormat: "{0}",
                    },
                ],

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
                        },
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
                    });
            }
        }).dxDataGrid('instance');
    },

    init: function () {
        // Carga las variables de configuración.
        root = $('#Root').val();
        source = root + "Encuestador/ListAjax/";
        showExport = $('#showExport').val() == 1;

        showEditar = $('#ShowEdit').val() == 1;
        showDelete = $('#ShowDelete').val() == 1;

        funcLE.instanceDataGrid();
    }
};

//************************************** ON READY **********************************************
$(function () {

    DevExpress.localization.locale("es-US");
    funcLE.init();
});
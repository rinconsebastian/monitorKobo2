
//*********************************** VARIABLES ******************************************
var root;
var dataGrid;
var source = "";

//*********************************** funcLogs ******************************************

var funcLogs = {


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
                enabled: false,
                fileName: "Listado_encuestadores_" + moment().format("DD-MM-YYYY_hh-mm-ss"),
                allowExportSelectedData: false
            },

            stateStoring: {
                enabled: false,
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
                    dataField: "usuario",
                    caption: "Usuario",
                    alignment: "center",
                    width: '25%'
                },
                {
                    dataField: "modelo",
                    caption: "Modelo",
                    alignment: "center",
                    width: '20%'
                },
                {
                    dataField: "accion",
                    caption: "Acción",
                    alignment: "center",
                    width: '20%'
                },
                {
                    dataField: "fecha",
                    caption: "Fecha",
                    alignment: "right",
                    dataType: "datetime",
                    width: '25%',
                    calculateFilterExpression: function (value, selectedFilterOperations, target) {
                        if (target === "headerFilter" && value === "weekends") {
                            return [[getOrderDay, "=", 0], "or", [getOrderDay, "=", 6]];
                        }
                        return this.defaultCalculateFilterExpression.apply(this, arguments);
                    }
                },
                {
                    dataField: "Opciones",
                    caption: "Opciones",
                    alignment: "center",
                    allowHeaderFiltering: false,
                    width: '10%',
                    cellTemplate: function (container, options) {

                        var idEnc = options.data.id;
                        var contenido = '<a href="' + root +'Logs/Details/' + idEnc + '" title="Detalles" class="btn btn-outline-info btn-xs ml-1" ><i class="fas fa-file-alt"></i></a>'

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
                    showInColumn: "usuario",
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
                    });
            }
        }).dxDataGrid('instance');
    },

    init: function () {
        // Carga las variables de configuración.
        root = $('#Root').val();
        source = root + "Logs/Ajax/";
        funcLogs.instanceDataGrid();
    }
};

//************************************** ON READY **********************************************
$(function () {

    DevExpress.localization.locale("es-US");
    funcLogs.init();
});

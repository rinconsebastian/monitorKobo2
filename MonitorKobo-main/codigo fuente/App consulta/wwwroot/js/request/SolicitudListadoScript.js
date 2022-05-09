//*********************************** VARIABLES ******************************************
var root;
var dataGrid;
var source = "";


//*********************************** funcLE ******************************************

var funcLE = {


    instanceDataGrid: function() {
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
                    dataField: "Opciones",
                    hidingPriority: 6,
                    caption: "Opciones",
                    alignment: "center",
                    allowHeaderFiltering: false,
                    width: 80,
                    cellTemplate: function (container, options) {

                        var id = options.data.id;
                        var contenido = "";
                        if (options.data.alerta) {
                            contenido = '<a href="' + root + 'Solicitud/DetailsAlert/' + id + '" title="Ver detalle" class="btn btn-outline-warning btn-xs ml-1" ><i class="fas fa-bell alert-bell"></i></a>';
                        } else {
                            contenido = '<a href="' + root + 'Solicitud/Details/' + id + '" title="Ver detalle" class="btn btn-outline-info btn-xs ml-1" ><i class="fas fa-file-alt"></i></a>';
                        }


                        $("<div class='preventSelection'>")
                            .append(contenido)
                            .appendTo(container);
                    }

                },
                
                {
                    dataField: "estado",
                    caption: "Estado",
                    alignment: "center",
                    width: '120',
                    hidingPriority: 5,
                    cellTemplate: function (container, options) {
                        var nombre = "";
                        var color = "";
                        switch (options.data.estado) {
                            case 0:
                                color = "bg-warning text-dark";
                                nombre = "Nuevo";
                                break;
                            case 1:
                                color = "bg-success text-white";
                                nombre = "Terminado";
                                break;
                            case 3:
                                color = "bg-danger text-white";
                                nombre = "Cancelado";
                                break;
                            case 2:
                                color = "bg-info text-white";
                                nombre = "En proceso";
                                break;
                        }
                        contenido = '<h6 class="mb-0"><span class="badge ' + color + '">' + nombre + '</span></h6>';
                        $("<div class='preventSelection'>")
                            .append(contenido)
                            .appendTo(container);
                    }
                },
                {
                    dataField: "fecha",
                    caption: "Fecha",
                    alignment: "center",
                    width: '220',
                    hidingPriority: 1,
                    dataType: "datetime",
                    calculateFilterExpression: function (value, selectedFilterOperations, target) {
                        if (target === "headerFilter" && value === "weekends") {
                            return [[getOrderDay, "=", 0], "or", [getOrderDay, "=", 6]];
                        }
                        return this.defaultCalculateFilterExpression.apply(this, arguments);
                    }
                },
                {
                    dataField: "formalizacion",
                    caption: "Formalización",
                    alignment: "center",
                    width: 130,
                    hidingPriority: 3
                }, {
                    dataField: "adjunto",
                    caption: "Adjunto",
                    alignment: "center",
                    width: 130,
                    hidingPriority: 2
                },
                {
                    dataField: "asunto",
                    caption: "Asunto",
                    alignment: "center",
                    width: 150,
                    hidingPriority: 4
                },
                
            ],
            summary: {
                totalItems: [
                    {
                        column: "id",
                        summaryType: "count",
                        showInColumn: "Opciones",
                        displayFormat: "Total: {0}",
                    },
                ],

            },

            onToolbarPreparing: function(e) {
                var dataGrid = e.component;
                e.toolbarOptions.items.unshift({
                    location: "after",
                    widget: "dxButton",
                    options: {
                        icon: "refresh",
                        hint: "Actualizar",
                        onClick: function() {
                            dataGrid.refresh();
                        }
                    }
                });
            }
        }).dxDataGrid('instance');
    },

    init: function() {
        // Carga las variables de configuración.
        root = $('#Root').val();
        source = root + "Solicitud/ListAjax/";
        
        funcLE.instanceDataGrid();
    }
};

//************************************** ON READY **********************************************
$(function() {

    DevExpress.localization.locale("es-US");
    funcLE.init();
});
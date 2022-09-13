//*********************************** VARIABLES ******************************************
var root;
var allowExport = false;

var dataGridInst;

//*********************************** funcL ******************************************

var funcL = {
  
    instanceDataGrid: function (gridName, reportName, columns, source) {
        dataGridInst = $(gridName).dxDataGrid({
            dataSource: source,
            selection: {
                mode: "none",
                showCheckBoxesMode: "always",
                selectAllMode: "allPages"
            },
            noDataText: "No hay datos disponibles.",
            export: {
                enabled: allowExport,
                fileName: "Listado_" + reportName + "_" + moment().format("DD-MM-YYYY_hh-mm-ss"),
                allowExportSelectedData: false
            },
            onExporting: function (e) {
                e.component.beginUpdate();
                e.component.columnOption(reportName + "_Opciones", "visible", false);
            },
            onExported: function (e) {
                e.component.columnOption(reportName + "_Opciones", "visible", true);
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
            columns: columns,
            summary: {
                totalItems: [{
                    column: "name",
                    summaryType: "count",
                    showInColumn: "name",
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
                );
            },
            onRowPrepared: function (e) {
                if (e.rowType === "data") {
                    if (e.data.state_name == "Pendiente") {
                        e.rowElement.css('background-color', '#ffe6de');
                    }
                }

            }
        }).dxDataGrid('instance');
    },
    loadColumns: function (columnsList, reportName, projectId) {
        var resp = []
        for (var i = 0; i < columnsList.length; i++) {
            var f = columnsList[i];
            var item = {
                dataField: f.data,
                name: reportName + "_" + f.data,
                caption: f.caption,
                alignment: "center",
                width: f.width,
                hidingPriority: f.priority
            }

            if (f.caption.includes("\r")) {
                item.headerCellTemplate = function (header, info) {
                    $("<div>").html(info.column.caption.replace(/\r\n/g, "<br/>")).appendTo(header);
                }
            }
            if (f.type == "date") {
                item.dataType = "date";
                item.calculateFilterExpression = function (value, selectedFilterOperations, target) {
                    if (target === "headerFilter" && value === "weekends") {
                        return [[getOrderDay, "=", 0], "or", [getOrderDay, "=", 6]];
                    }
                    return this.defaultCalculateFilterExpression.apply(this, arguments);
                };
            } else if (f.type == "options") {
               
                item.allowHeaderFiltering = false;
                item.cellTemplate = function (container, options) {
                    var id = options.data._id;
                    var hidden = options.data.hidden;
                    var formato = options.data.formato;

                    var contenido = '';
                    contenido += '<a href="' + root + 'KoboData/Complete/' + id + '?project=' + projectId + '" title="Cargar" class="btn btn-outline-danger btn-xs ml-1" ><i class="fas fa-download"></i></a>'
                    if (hidden) {
                        contenido += '<a href="' + root + 'KoboData/Toggle/' + id + '?project=' + projectId + '" title="Mostrar" class="btn btn-outline-info btn-xs ml-1" ><i class="far fa-eye"></i></a>'
                    } else {
                        contenido += '<a href="' + root + 'KoboData/Toggle/' + id + '?project=' + projectId + '" title="Ocultar" class="btn btn-outline-warning btn-xs ml-1" ><i class="far fa-eye-slash"></i></a>'
                    }
                    
                    if (formato == 1) {
                        contenido += '<a href="' + root + 'Acuicultura/Details/' + id + '?project=2" title="Detalles" class="btn btn-outline-info btn-xs ml-1" ><i class="fas fa-print"></i></a>'
                    }
                    $("<div class='preventSelection'>").append(contenido).appendTo(container);
                };
            }
            resp.push(item);
        }
        return resp;
    },
    init: function () {
        // Carga las variables de configuración.
        root = $('#Root').val();
        allowExport = $('#allowExport').val() == 1;

        if (typeof myConfig !== "undefined") {
            for (var i = 0; i < myConfig.length; i++) {
                var conf = myConfig[i];
                funcL.instanceDataGrid(
                    conf.grid,
                    conf.reportName,
                    funcL.loadColumns(conf.columns, conf.grid, conf.projectId),
                    root + conf.source
                );
            }
        }
    }
};

//************************************** ON READY **********************************************
$(function () {

    DevExpress.localization.locale("es-US");
    funcL.init();
});
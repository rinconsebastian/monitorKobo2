//*********************************** VARIABLES ******************************************
var root;

var allowExport = false;
var allowSee = false;
var allowPrint = false;

var urlPrint = "";
var tablename = "acua_046";

//*********************************** funcL ******************************************

var funcL = {
    imprimirMultiple: function (seleccion) {
        if (seleccion.length > 0) {
            var ids = $.map(seleccion, function (n, i) {
                return n._id + "";
            });
            var url = urlPrint + "?ids=" + ids.join('&ids=') + "&project=" + projectPrint;
            var myWindow = window.open(url, "_blank", "toolbar=no,titlebar=no,menubar=no,scrollbars=yes,resizable=no,top=0,left=386,width=1000,height=700");
        } else {
            DevExpress.ui.notify("No hay formalizaciones seleccionadas.", 'warning', 3000);
        }
    },
    instanceDataGrid: function () {
        var dataGrid = $("#grid-ac-046").dxDataGrid({
            dataSource: source,
            selection: {
                mode: allowPrint ? "multiple" : "none",
                showCheckBoxesMode: "always",
                selectAllMode: "allPages"
            },
            noDataText: "No hay datos disponibles.",
            export: {
                enabled: allowExport,
                fileName: "Listado_ft-iv-046_" + moment().format("DD-MM-YYYY_hh-mm-ss"),
                allowExportSelectedData: false
            },
            onExporting: function (e) {
                e.component.beginUpdate();
                if (allowLoadValidate) {
                    e.component.columnOption(reportName + "_Opciones", "visible", false);
                    e.component.columnOption(reportName + "_state_name", "visible", true);
                }
                if (!showUser) {
                    e.component.columnOption(reportName + "_user", "visible", true);
                    e.component.columnOption(reportName + "_user_name", "visible", true);
                }
            },
            onExported: function (e) {
                if (allowLoadValidate) {
                    e.component.columnOption(reportName + "_Opciones", "visible", true);
                    e.component.columnOption(reportName + "_state_name", "visible", false);
                }
                if (!showUser) {
                    e.component.columnOption(reportName + "_user", "visible", false);
                    e.component.columnOption(reportName + "_user_name", "visible", false);
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
                    name: tablename + "kobo_id",
                    dataField: "kobo_id",
                    caption: "No\r\nRegistro",
                    headerCellTemplate: function (header, info) {
                        $("<div>").html(info.column.caption.replace(/\r\n/g, "<br/>")).appendTo(header);
                    },
                    alignment: "center",
                    width: '150',
                    hidingPriority: 5
                },
                {
                    name: tablename + "name",
                    dataField: "i_05",
                    caption: "Nombre\r\nEncuestado",
                    headerCellTemplate: function (header, info) {
                        $("<div>").html(info.column.caption.replace(/\r\n/g, "<br/>")).appendTo(header);
                    },
                    alignment: "center",
                    width: '280',
                    hidingPriority: 4
                }, {
                    name: tablename + "datetime",
                    dataField: "a_04",
                    caption: "Fecha",
                    alignment: "center",
                    width: '100',
                    hidingPriority: 3,
                    dataType: "date",
                    calculateFilterExpression: function (value, selectedFilterOperations, target) {
                        if (target === "headerFilter" && value === "weekends") {
                            return [
                                [getOrderDay, "=", 0], "or", [getOrderDay, "=", 6]
                            ];
                        }
                        return this.defaultCalculateFilterExpression.apply(this, arguments);
                    }
                }, {
                    name: tablename + "user",
                    dataField: "user",
                    caption: "Cedula\r\nEncuestador",
                    headerCellTemplate: function (header, info) {
                        $("<div>").html(info.column.caption.replace(/\r\n/g, "<br/>")).appendTo(header);
                    },
                    alignment: "center",
                    width: '120',
                    hidingPriority: 2
                }, {
                    name: tablename + "username",
                    dataField: "userName",
                    caption: "Nombre\r\nEncuestador",
                    headerCellTemplate: function (header, info) {
                        $("<div>").html(info.column.caption.replace(/\r\n/g, "<br/>")).appendTo(header);
                    },
                    alignment: "center",
                    width: '280',
                    hidingPriority: 1
                }, {
                    name: tablename + "status",
                    dataField: "state_name",
                    caption: "Estado",
                    alignment: "center",
                    width: '120',
                    hidingPriority: 9
                }, {
                    name: tablename + "opciones",
                    caption: "Opciones",
                    alignment: "center",
                    hidingPriority: 10,
                    width: '100',
                    cellTemplate: function (container, options) {
                        var id = options.data._id;
                        var state = options.data.state;
                        var formato = options.data.formato;
                        var contenido = '';
                        contenido += '<a href="' + root + 'KoboData/Complete/' + id + '?project=2" title="Cargar" class="btn btn-outline-danger btn-xs ml-1" ><i class="fas fa-download"></i></a>'
                        if (formato == 1) {
                            contenido += '<a href="' + root + 'Acuicultura/Details/' + id + '?project=2" title="Detalles" class="btn btn-outline-info btn-xs ml-1" ><i class="fas fa-print"></i></a>'
                        }     
                        $("<div class='preventSelection'>").append(contenido).appendTo(container);
                    }
                }],
            summary: {
                totalItems: [{
                    column: "name",
                    summaryType: "count",
                    showInColumn: tablename + "kobo_id",
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
                            visible: allowPrint,
                            onClick: function () {
                                var seleccion = dataGrid.option('selectedRowKeys');
                                funcL.imprimirMultiple(seleccion);
                            }
                        }
                    }
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
    init: function () {
        // Carga las variables de configuración.
        root = $('#Root').val();

        allowExport = $('#allowExport').val() == 1;
        allowSee = $('#allowSee').val() == 1;
        allowPrint = $('#allowPrint').val() == 1;

        urlPrint = $('#urlPrint').val();

        source = root + "Acuicultura/Ajax/";

        funcL.instanceDataGrid();
    }
};

//************************************** ON READY **********************************************
$(function () {

    DevExpress.localization.locale("es-US");
    funcL.init();
});
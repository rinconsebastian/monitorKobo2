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
                    dataField: "Opciones",
                    hidingPriority: 11,
                    caption: "Opciones",
                    alignment: "center",
                    allowHeaderFiltering: false,
                    width: 80,
                    cellTemplate: function (container, options) {

                        var id = options.data.id;
                        var contenido = '<a href="' + root + 'Solicitud/Edit/' + id + '" title="Validar" class="btn btn-outline-success btn-xs ml-1" ><i class="fas fa-user-edit"></i></a>'
                        
                        $("<div class='preventSelection'>")
                            .append(contenido)
                            .appendTo(container);
                    }

                },
                
                {
                    dataField: "state",
                    caption: "Estado",
                    alignment: "center",
                    width: '100',
                    hidingPriority: 10,
                    cellTemplate: function (container, options) {
                        var nombre = "";
                        var color = "";
                        switch (options.data.state) {
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
                    dataField: "request",
                    caption: "Asunto",
                    alignment: "center",
                    width: 150,
                    hidingPriority: 9
                },
                {
                    dataField: "nameUser",
                    caption: "Nombre\r\nUsuario",
                    headerCellTemplate: function (header, info) {
                        $("<div>").html(info.column.caption.replace(/\r\n/g, "<br/>")).appendTo(header);
                    },
                    alignment: "center",
                    width: 110,
                    hidingPriority: 8
                }, 
                {
                    dataField: "createDate",
                    caption: "Fecha\r\nCreación",
                    headerCellTemplate: function (header, info) {
                        $("<div>").html(info.column.caption.replace(/\r\n/g, "<br/>")).appendTo(header);
                    },
                    alignment: "center",
                    width: '90',
                    hidingPriority: 7,
                    dataType: "date",
                    calculateFilterExpression: function (value, selectedFilterOperations, target) {
                        if (target === "headerFilter" && value === "weekends") {
                            return [[getOrderDay, "=", 0], "or", [getOrderDay, "=", 6]];
                        }
                        return this.defaultCalculateFilterExpression.apply(this, arguments);
                    }
                },
               
                {
                    dataField: "formalizationNumber",
                    caption: "Formalización",
                    alignment: "center",
                    width: 100,
                    hidingPriority: 6,
                    cellTemplate: function (container, options) {
                        var nombre = options.data.formalizationNumber;
                        var id = options.data.formalizationId;
                        
                        contenido = '<a href="' + root + 'Formalizacion/Details/' + id + '" target="_blank">' + nombre + '</a>';
                        $("<div class='preventSelection'>")
                            .append(contenido)
                            .appendTo(container);
                    }
                }, 
                {
                    dataField: "file",
                    caption: "Adjunto",
                    alignment: "center",
                    width: 70,
                    hidingPriority: 3
                },
                {
                    dataField: "alertAdmin",
                    caption: "Alerta\r\nAdmin",
                    headerCellTemplate: function (header, info) {
                        $("<div>").html(info.column.caption.replace(/\r\n/g, "<br/>")).appendTo(header);
                    },
                    alignment: "center",
                    width: 70,
                    hidingPriority: 5
                },
                {
                    dataField: "alertUser",
                    caption: "Alerta\r\nUsuario",
                    headerCellTemplate: function (header, info) {
                        $("<div>").html(info.column.caption.replace(/\r\n/g, "<br/>")).appendTo(header);
                    },
                    alignment: "center",
                    width: 70,
                    hidingPriority: 4
                },
                {
                    dataField: "adminName",
                    caption: "Nombre\r\nAdmin",
                    headerCellTemplate: function (header, info) {
                        $("<div>").html(info.column.caption.replace(/\r\n/g, "<br/>")).appendTo(header);
                    },
                    alignment: "center",
                    width: 110,
                    hidingPriority: 2
                }, 
                {
                    dataField: "validationDate",
                    caption: "Fecha\r\nValidación",
                    headerCellTemplate: function (header, info) {
                        $("<div>").html(info.column.caption.replace(/\r\n/g, "<br/>")).appendTo(header);
                    },
                    alignment: "center",
                    width: '90',
                    hidingPriority: 1,
                    dataType: "date",
                    
                    calculateFilterExpression: function (value, selectedFilterOperations, target) {
                        if (target === "headerFilter" && value === "weekends") {
                            return [[getOrderDay, "=", 0], "or", [getOrderDay, "=", 6]];
                        }
                        return this.defaultCalculateFilterExpression.apply(this, arguments);
                    }
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
            }
        }).dxDataGrid('instance');
    },

    init: function() {
        // Carga las variables de configuración.
        root = $('#Root').val();
        source = root + "Solicitud/ListAjaxAdmin/";
        
        funcLE.instanceDataGrid();
    }
};

//************************************** ON READY **********************************************
$(function() {

    DevExpress.localization.locale("es-US");
    funcLE.init();
});
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
                        var state = options.data.state;
                        var formalization = options.data.recordNumber;

                        var contenido = '<a href="' + root + 'Solicitud/Edit/' + id + '" title="Validar" class="btn btn-outline-success btn-xs ml-1" ><i class="fas fa-user-edit"></i></a>';

                        if (formalization > 0 && state == 0) {
                            contenido += '<button data-id="' + id + '" title="Respuesta rapida" class="btn btn-outline-danger btn-xs ml-1 btn-quick" ><i class="fas fa-meteor"></i></button>';
                        }
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
                        var contenido = '<h6 class="mb-0"><span class="badge ' + color + '">' + nombre + '</span></h6>';
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
                    hidingPriority: 9,
                    cellTemplate: function (container, options) {

                        var request = options.data.request;
                        var message = options.data.message;
                        var contenido = '<span style="text-decoration-line: underline;" title="' + message + '">' + request + '</span>';
                        $("<div class='preventSelection'>")
                            .append(contenido)
                            .appendTo(container);
                    }
                },
                {
                    dataField: "userName",
                    caption: "Nombre\r\nUsuario",
                    headerCellTemplate: function (header, info) {
                        $("<div>").html(info.column.caption.replace(/\r\n/g, "<br/>")).appendTo(header);
                    },
                    alignment: "center",
                    width: 120,
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
                    dataField: "recordNumber",
                    caption: "Registro",
                    alignment: "center",
                    width: 100,
                    hidingPriority: 6,
                    cellTemplate: function (container, options) {
                        var nombre = options.data.recordNumber;
                        var id = options.data.recordId;
                        var project = options.data.recordProject;
                        var contenido = "";
                        if (nombre != null && nombre != "") {
                            contenido = '<a href="' + root + 'Validation/Details/' + id + '?project=' + project + '" target="_blank">' + nombre + '</a>';
                        }
                        $("<div class='preventSelection'>")
                            .append(contenido)
                            .appendTo(container);
                    }
                }, 
                {
                    dataField: "file",
                    caption: "Adjunto",
                    alignment: "center",
                    width: 100,
                    hidingPriority: 3
                },
                {
                    dataField: "alertAdmin",
                    caption: "Alerta\r\nAdmin",
                    headerCellTemplate: function (header, info) {
                        $("<div>").html(info.column.caption.replace(/\r\n/g, "<br/>")).appendTo(header);
                    },
                    alignment: "center",
                    width: 80,
                    hidingPriority: 5
                },
                {
                    dataField: "alertUser",
                    caption: "Alerta\r\nUsuario",
                    headerCellTemplate: function (header, info) {
                        $("<div>").html(info.column.caption.replace(/\r\n/g, "<br/>")).appendTo(header);
                    },
                    alignment: "center",
                    width: 80,
                    hidingPriority: 4
                },
                {
                    dataField: "alertEmail",
                    caption: "Alerta\r\nEmail",
                    headerCellTemplate: function (header, info) {
                        $("<div>").html(info.column.caption.replace(/\r\n/g, "<br/>")).appendTo(header);
                    },
                    alignment: "center",
                    width: 80,
                    hidingPriority: 4
                },
                {
                    dataField: "adminName",
                    caption: "Nombre\r\nAdmin",
                    headerCellTemplate: function (header, info) {
                        $("<div>").html(info.column.caption.replace(/\r\n/g, "<br/>")).appendTo(header);
                    },
                    alignment: "center",
                    width: 130,
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
    loadQuickResponse: function () {
        $('body').on('click', '.btn-quick', function (e) {

            var button = $(this);

            var id = $(this).data('id');

            var fullurl = root + "Solicitud/QuickResponse/";

            button.find('i').addClass('fa-cog fa-spin');
            button.attr('disabled', 'disabled');

            $.post(fullurl, { id: id }).
                done(function (data) {
                    button.find('i').removeClass('fa-cog fa-spin');
                    if (data != null) {
                        if (data.success) {
                            window.location.reload();
                        } else {
                            funcGenerico.mostrarMensaje(data.message, "error");
                        }
                    } else {
                        funcGenerico.mostrarMensaje("Error en la respuesta del servidor.", "error");
                    }

                }).fail(function (data) {
                    button.find('i').removeClass('fa-cog fa-spin');
                    funcGenerico.mostrarMensaje("Error al solicitar la operación.", "error");
                });

        });
    },
    init: function() {
        // Carga las variables de configuración.
        root = $('#Root').val();
        source = root + "Solicitud/ListAjaxAdmin/";
        
        funcLE.instanceDataGrid();
        funcLE.loadQuickResponse();
    }
};

//************************************** ON READY **********************************************
$(function() {

    DevExpress.localization.locale("es-US");
    funcLE.init();
});
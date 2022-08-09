//*********************************** VARIABLES ******************************************
var root;

var allowExport = false;
var allowLoadValidate = false;
var allowSeeValidate = false;

var allowPrint = false;
var allowDelete = false;
var allowSeeSepec = false;
var urlPrint = "";
var projectPrint = 0;

var dataGridInst;

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
    borrarMultiple: function (seleccion) {
        if (seleccion.length > 0) {
            var ids = $.map(seleccion, function (n, i) {
                return n._id + "";
            });
            var fullurl = root + "Validation/Delete/";
            var idsParam = ids.join();
            $.post(fullurl, { ids: idsParam, project: projectPrint }).
                done(function (data) {
                    if (data != null) {
                        if (data == 0) {
                            funcGenerico.mostrarMensaje("Error no se encontró ningun registro.", "warning");
                        } else {
                            var mensaje = data == 1 ? "Se eliminó un registro." : "Se eliminaron " + data + " registros.";
                            funcGenerico.mostrarMensaje(mensaje, "success");
                            dataGridInst.refresh();
                        }
                    } else {
                        funcGenerico.mostrarMensaje("Error en la respuesta del servidor.", "error");
                    }
                }).fail(function (data) {
                    funcGenerico.mostrarMensaje("Error al solicitar la operación.", "error");
                });
        } else {
            DevExpress.ui.notify("No hay elementos seleccionados.", 'warning', 3000);
        }
    },
    instanceDataGrid: function (gridName, reportName, columns, showUser, source) {
        dataGridInst = $(gridName).dxDataGrid({
            dataSource: source,
            selection: {
                mode: allowPrint || allowDelete ? "multiple" : "none",
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
                    },
                    {
                        location: "after",
                        widget: "dxButton",
                        options: {
                            icon: "trash",
                            hint: "Borrar",
                            visible: allowDelete,
                            onClick: function () {
                                var seleccion = dataGrid.option('selectedRowKeys');
                                funcL.borrarMultiple(seleccion);
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

    loadValidation: function () {
        $('body').on('click', '.load-formlz', function (e) {
            var id = $(this).data('id');
            var project = $(this).data('project');
            var btn = $(this);
            $(this).find('i').removeClass('fa-download');
            $(this).find('i').addClass('fa-cog fa-spin');
            $(this).attr('disabled', 'disabled');

            var fullurl = root + "Validation/Load/";

            $.post(fullurl, { id: id, project: project }).
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

    loadColumns: function (columnsList, showUser, reportName, projectId) {
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
            } else if (f.type == "show_user") {
                item.visible = showUser;
            } else if (f.type == "show_valid") {
                item.visible = allowSeeValidate;
            } else if (f.type == "options") {
                item.visible = allowSeeValidate;
                item.allowHeaderFiltering = false;
                item.cellTemplate = function (container, options) {

                    var state = options.data.state;
                    var itemID = options.data._id;
                    var formato = options.data.formato;

                    var contenido = "";
                    if (allowSeeSepec && formato == 1) {
                        contenido += '<a href="' + root + 'Acuicultura/Details/' + itemID + '?project=' + projectId + '" title="FT-IV-046" class="btn btn-outline-dark btn-xs ml-1" ><i class="fab fa-wpforms"></i></a>'
                    }
                    if (state > 2) {
                        contenido += '<a href="' + root + 'Validation/Details/' + itemID + '?project=' + projectId +  '" title="Detalles" class="btn btn-outline-info btn-xs ml-1" ><i class="fas fa-file-alt"></i></a>'
                    }

                    if (state == 2 && allowLoadValidate) {
                        contenido += '<button class="btn btn-outline-success btn-xs ml-1 load-formlz" data-id="' + itemID + '" data-project="' + projectId + '" title="Cargar datos"><i class="fas fa-download"></i></button>';
                    } else if (state == 3 && allowLoadValidate) {
                        contenido += '<a href="' + root + 'Validation/Edit/' + itemID + '?project=' + projectId + '" title="Editar " class="btn btn-outline-warning btn-xs ml-1" ><i class="fas fa-edit"></i></a>'
                    }
                    

                    $("<div class='preventSelection'>")
                        .append(contenido)
                        .appendTo(container);
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
        allowSeeValidate = $('#allowSeeValidate').val() == 1;
        allowLoadValidate = $('#allowLoadValidate').val() == 1;
        allowPrint = $('#allowPrint').val() == 1;
        allowDelete = $('#allowDelete').val() == 1;
        allowSeeSepec = $('#allowSeeSepec').val() == 1;
        urlPrint = $('#urlPrint').val();
        projectPrint = $('#projectPrint').val();

        if (typeof myConfig !== "undefined") {
            for (var i = 0; i < myConfig.length; i++) {
                var conf = myConfig[i];
                funcL.instanceDataGrid(
                    conf.grid,
                    conf.reportName,
                    funcL.loadColumns(conf.columns, conf.showUser, conf.grid, conf.projectId),
                    conf.showUser,
                    root + conf.source
                );
            }
        }
        funcL.loadValidation();
    }
};

//************************************** ON READY **********************************************
$(function () {

    DevExpress.localization.locale("es-US");
    funcL.init();
});
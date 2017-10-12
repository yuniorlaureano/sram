var doneAudit = {
    _Auditor: "",
    _SalesDate: "",
    _CreationDate: "",
    _SubscrId: "",
    _CallId: "",
    _PhoneNo: ""
};

$(function () {

    
    /*MENU PARA EL MODULO DE ASIGNACIONES*/
    menuAssimentModule();

    /*ASSIMGENT MODULE*/
    AssignmentModule.init();
    AssignmentModule.getAuditors();
    AssignmentModule.getPedding();
    AssignmentModule.Constructor();
    AssignmentModule.assignAudit();
    AssignmentModule.getPendingAudit();
    AssignmentModule.reassignAudit();
    AssignmentModule.GetAcctInfoByAssignment();
    AssignmentModule.audit();
    AssignmentModule.reportAdm();
    AssignmentModule.getDoneAudit();
    
    //---------------------------End assigmentModule Implementatio
    
    toggleFootable('#foo-table-table', 'td:nth-child(1),td:nth-child(12)');
    toggleFootable('#done-audit-foo-table', 'td:nth-child(11),td:nth-child(12)');
    expandAndCollapseFooTable('#foo-table-table', '#expand-foo-table', '#collapse-foo-table');
    expandAndCollapseFooTable('#done-audit-foo-table', '#done-audit-expand-foo-table', '#done-audit-collapse-foo-table');
});

function toggleFootable(table,exceptionTd) {
    //shower-foo-table//td:nth-child(11),td:nth-child(12)
    $(table).on('click', '.shower-foo-table td:not('+exceptionTd+')', function () {
        

        $(this).parent("tr").toggleClass("footable-detail-show").next().toggleClass("hide-tr-footable");
    });
}

//expand-foo-table colapse-foo-table
function expandAndCollapseFooTable(table, expander, collapser) {


    $(collapser).on('click', function () {
        $(table +' .shower-foo-table').each(function (key,value) {
            if ($(value).hasClass("footable-detail-show")) {
                $(value).removeClass("footable-detail-show").next().toggleClass("hide-tr-footable");
            }
        });
        //$('.shower-foo-table').toggleClass("footable-detail-show").next().toggleClass("hide-tr-footable");
    });

    $(expander).on('click', function () {
        $(table +' .shower-foo-table').each(function (key, value) {
            if (!$(value).hasClass("footable-detail-show")) {
                $(value).addClass("footable-detail-show").next().toggleClass("hide-tr-footable");
            }
        });
        //$('.shower-foo-table').toggleClass("footable-detail-show").next().toggleClass("hide-tr-footable");
    });
}

/*******MENU PARA EL MANEJO DEL MODULO DE ASSIGNACIONES*********/
function menuAssimentModule() {
    $('#nav-asignar-link').on('click', function () {

        AssignmentModule.getPedding();//Obtiene las asignaciones pendientes

        $('#frm-panel-auditoria').hide();
        $('#frm-panel-asignaciones-pendientes').hide();
        $('#form-panel-asignaciones').show();
        $('#frm-panel-auditorias-realizadas').hide();
        $('#frm-panel-reportes').hide();

        $('#selectedDate').val("");
    });

    //frm-panel-asignaciones-pendientes
    $('#nav-pendientes-auditar-link').on('click', function () {

        $('#frm-panel-asignaciones-pendientes').show();
        $('#form-panel-asignaciones').hide();
        $('#frm-panel-auditoria').hide();
        $('#frm-panel-auditorias-realizadas').hide();
        $('#frm-panel-reportes').hide();

        $('#frm-buscar-auditor').val("none");
        $('#frm-buscar-selectedDate').val("");

        AssignmentModule.preDisplayPenddingAudit();
        
    });

    //frm-panel-auditoria
    $('#nav-buscar-auditoria-link').on('click', function () {

        $('#frm-panel-auditoria').hide();
        $('#frm-panel-asignaciones-pendientes').hide();
        $('#form-panel-asignaciones').hide();
        $('#frm-panel-auditorias-realizadas').show();
        $('#frm-panel-reportes').hide();

        $('#frm-search-auditor').val("none");
        $('#frm-buscar-selected-date-rdv').val("");
        $('#frm-buscar-selected-date-auditoria').val("");
        $('#frm-search-subscriber-id').val("");
        $('#frm-search-call-id').val("");
        $('#frm-search-telefono').val("");
    });

    //frm-panel-auditorias-realizadas
    $('#nav-asignar-reportes-link').on('click', function () {

        $('#frm-panel-auditorias-realizadas').hide();
        $('#frm-panel-auditoria').hide();
        $('#frm-panel-asignaciones-pendientes').hide();
        $('#form-panel-asignaciones').hide();
        $('#frm-panel-reportes').show();
        
        $('#frm-repord-desde').val("");
        $('#frm-report-hasta').val("");
        $("#select-control-type-of-report").val("none");

    });
}



var AssignmentModule = (function () {

    function init() {
        reaudit();
        deletee();
    }

    function AssigmentModuleConstructor() {

        if (userCredentials.Group == "adm") {
            $('#refrescar-pendiente-auditar').fadeOut();
        } else {
            $('#refrescar-pendiente-auditar').fadeIn();
        }

        preDisplayPenddingAudit();
        
        $('#refrescar-pendiente-auditar').on('click', function () {

            var _userCode = userCredentials.Group == "adm" ? $('#frm-buscar-auditor').val() : userCredentials.Code;
            var _salesDate = $('#frm-buscar-selectedDate').val();

            createHtmlForPenddingAudit(_userCode, _salesDate);

        });

        $("#close-amount-audited-shower").on("click", function () {
            $("#total-de-cuentas-asignadas").hide();
        });
    };
    
    //Obtiene los auditores desde la base de datos.
    function getAuditorsFromDb() {

        var selectAuditorsElement = "<option value='none'>Todo</option>";//Desplega todos los auditores
        var selectLimitedAuditorsElement = "<option value='none'>Todo</option>";//desplega solo el auditor de la session actual
        var selectMultipleAuditorsElement = "";//Desplega los auditores de la lista de seleccion multiple.
        var name = "";

        $.ajax({
            type: 'GET',
            application: 'JSON',
            url: '../Assingment/GetAuditors',
            success: function (data) {

                $(data.auditors).each(function (key, value) {

                    name = String(value.Name).toLowerCase();

                    selectAuditorsElement += "<option value='" + value.UserCode + "'>" + name + "</option>";
                    selectMultipleAuditorsElement += "<option value='" + value.UserCode + "'>" + name + "</option>";

                    if (value.UserCode == userCredentials.Code) {
                        selectLimitedAuditorsElement += "<option value='" + value.UserCode + "'>" + name + "</option>";
                        
                    }

                });
                
                if (userCredentials.Group == "adm") {
                    $('.auditor').html(selectAuditorsElement);//asigna los aditores a los selecte elements que contengan la clase .auditor
                } else {
                    $('.auditor').html(selectLimitedAuditorsElement);//asigna los aditores a los selecte elements que contengan la clase .auditor
                }

                $('.auditor-multiple').html(selectMultipleAuditorsElement);//asigna los auditories a la lista multiple.
                $('.auditor-adm').html(selectAuditorsElement);//asigna el auditor de la session actual al dropdown .
            }
        });
    }

    /*******OBTIENE LAS ASIGNACIONES PENDIENTES, O SEA LAS QUE NO SE HAN ASIGNADO AUN*********/
    function getPeddingAssignMentsFromDb() {

        var total = 0;
        showColorLoading();
        var tr = "";
        $.ajax({
            type: 'GET',
            application: 'JSON',
            url: '../Assingment/unassignedAccounts',
            success: function (data) {

                total = 0;

                $(data.peddingAssignment).each(function (key, value) {
                    tr += "<tr><td>" + value.SalesDate + "</td><td>" + value.Cantidad + "</td></tr>";
                    total += Number(value.Cantidad);
                });

                $('#pedding-assignment tbody').html(tr);
                $('#total-pending').text(total);
                
                if (total > 0) {
                    alertNoty("Hay "+total+" asignaciones pendientes","Información","info");
                }
                hideColorLoading();
            }
        });
    }
    
    /*******PERMITE ASINAR UNA CUENTA*********/
    function assignAudit() {

        $('#asignar-auditor').on('click', function () {
           
            showColorLoading();

            var _auditor = $('#auditor').val(); //toma el auditor desde el select.
            var _selectedDate = $('#selectedDate').val(); //toma la fecha.
            var _userCode = userCredentials.Code;//el user code, esta almacenado en una varibale gloval, seatiada desde Layout.cshtml
            
            if (_auditor == "" || _auditor == null || _selectedDate == "" || _userCode == "") {
                alertNoty("Campos Requeridos: Fecha Y Auditor.", "Información.", "warning");
                return false;//si los campos estan vacios, termita e
            }

            $.ajax({
                type: 'POST',
                application: 'JSON',
                url: '../Assingment/AssignAudit',
                data: { salesDate: _selectedDate, selectedAuditor: String(_auditor), userCode: _userCode },
                success: function (data) {
                    
                    if (data.total != undefined) {
                        alertNoty("RDV asignado exitosamente. Se asignaron un total de " + data.total + " cuentas.", "Exito", 'info');
                        $("#total-de-cuentas-asignadas span#contenido-asignadas").html("RDV asignado exitosamente. Se asignaron un total de " + data.total + " cuentas.", "Exito", 'success');
                        $("#total-de-cuentas-asignadas").show();
                        getPeddingAssignMentsFromDb();
                    }

                    if (data.ERROR != undefined) {
                        alertNoty(data.ERROR, "Error",'danger');
                    }
                 
                    hideColorLoading();
                }
            });
        });
    }/*---------------------------------*/

    /*******OBTIENE LAS AUDITORIAS PENDIENTES, O SEA, LAS QUE NO SEAN REALIZADO*********/
    function getPendingAudit() {
        
        $('#buscar-pedding-audit').on('click', function () {
            
            var _userCode = userCredentials.Group == "adm" ? $('#frm-buscar-auditor').val() : userCredentials.Code;
            var _salesDate = $('#frm-buscar-selectedDate').val();

            if (_userCode == "none") {
                _userCode = "";
            }
            
            var canvCode = "";
            var canvEdition = 0;
            var subscriber = "";
            var chargeIn = "";

            createHtmlForPenddingAudit(_userCode, _salesDate);
            
        });
    }

    /*******DESPLEGA LAS Y GENERA EL HTML PARA MOSTRAR LAS AUDITORIAS PENDIENTES*********/
    function preDisplayPenddingAudit() {

        var _userCode = userCredentials.Group == "adm" ? '': userCredentials.Code;
        var _salesDate = $('#frm-buscar-selectedDate').val();

        createHtmlForPenddingAudit(_userCode, _salesDate);
    }

    /*******CREA EL HTML PARA DESPLEGAR LAS AUDITORIAS PENDIENTES*********/
    function createHtmlForPenddingAudit(_userCode, _salesDate) {

        var panelContainerHtml = $('#foo-table-table tbody');
        
        var panelDetailHtml = "";
        var displayPreviousId = "";
        var tr = "";
        var total = 0;
        showColorLoading();
		$.ajax({
            type: 'GET',
            application: 'JSON',
            url: '../Assingment/GetPendingAudResume',
            data: { userCode: _userCode, salesDate: _salesDate },
            success: function (data) {
                $(data.pending).each(function (key, value) {
                   
                    tr += "<tr><td>" + value.Name + "</td><td>" + value.Count + "</td></tr>";
                    total += value.Count;
                });

                $('#pedding-audit tbody').html(tr);
                $('#pedding-audit tfoot td.total-auditar').text(total);
                
            }
        });


        $.ajax({
            type: 'GET',
            application: 'JSON',
            url: '../Assingment/GetPendingAud',
            data: { userCode: _userCode, salesDate: _salesDate },
            success: function (data) {

                if (data.pending.length > 0) {
                    $('#pendientes-auditar-reasignar').css({ 'visibility': 'visible' });
                } else {
                    $('#pendientes-auditar-reasignar').css({ 'visibility': 'hidden' });
                }

                $(panelContainerHtml).html("");

                $(data.pending).each(function (key, value) {

                    displayPreviousId = "visible";
                    if (value.PrevioCallId == 'X') {
                        displayPreviousId = "hidden";
                    }
                    
                    //--------------------------------------
                    panelDetailHtml += '<tr class="shower-foo-table">';
                    panelDetailHtml += '<td class="footable-visible"><label><input type="checkbox" name="assigment-id-reassigned" value="' + value.AssignmentId + '"><label></td>';
                    panelDetailHtml += '<td class="footable-visible footable-first-column"><span class="footable-toggle"></span>' + value.RazonSocial + '</td>';
                    panelDetailHtml += '<td class="footable-visible">' + value.SubscriberId + '</td>';
                    panelDetailHtml += '<td class="footable-visible">' + value.Telefono + '</td>';
                    panelDetailHtml += '<td class="footable-visible">' + value.Canvass + '</td>';
                    panelDetailHtml += '<td class="footable-visible">' + value.BookCode + '</td>';
                    panelDetailHtml += '<td class="footable-visible">' + value.Ejecutivo + '</td>';
                    panelDetailHtml += '<td class="footable-visible">' + value.Unidad + '</td>';
                    panelDetailHtml += '<td class="footable-visible">' + value.FechaRPC + '</td>';
                    panelDetailHtml += '<td class="footable-visible">' + value.Cargo + '</td>';
                    panelDetailHtml += '<td class="footable-visible">' + value.CallId + '</td>';
                    panelDetailHtml += '<td class="footable-visible footable-last-column">';
                    panelDetailHtml += '<button data-id-assignment-audit="' + value.AssignmentId + '" class="btn btn-info reassign-audit btn-xs" style="margin:0%;">Auditar</button>';
                    panelDetailHtml += '</td>';
                    panelDetailHtml += '<td style="display: none;">wer</td>';
                    panelDetailHtml += '<td style="display: none;">wer</td>';
                    panelDetailHtml += '<td style="display: none;">wert</td>';
                    panelDetailHtml += '<td style="display: none;">wert</td>';
                    panelDetailHtml += '<td style="display: none;">wert</td>';
                    panelDetailHtml += '<td style="display: none;">wer</td>';
                    panelDetailHtml += '<td style="display: none;">wert</td>';
                    panelDetailHtml += '<td style="display: none;">wt</td>';
                    panelDetailHtml += '<td style="display: none;">wert</td>';
                    panelDetailHtml += '</tr>';
                    panelDetailHtml += '<tr class="footable-row-detail hide-tr-footable">';
                    panelDetailHtml += '<td class="footable-row-detail-cell" colspan="12">';
                    panelDetailHtml += '<div class="footable-row-detail-inner">';
                    panelDetailHtml += '<div class="footable-row-detail-row" style="display:block;margin-bottom:5px;">';
                    panelDetailHtml += '<div class="footable-row-detail-name">Control Verbal Call Id:</div>';
                    panelDetailHtml += '<div class="footable-row-detail-value">' + value.ControlVerballCallId + '</div>';
                    panelDetailHtml += '</div>';
                    panelDetailHtml += '<div class="footable-row-detail-row" style="display:block;margin-bottom:5px;">';
                    panelDetailHtml += '<div class="footable-row-detail-name">Conmentario Ejecutivo:</div>';
                    panelDetailHtml += '<div class="footable-row-detail-value">' + value.ComentarioEjecutivo + '</div>';
                    panelDetailHtml += '</div>';
                    panelDetailHtml += '<div class="footable-row-detail-row" style="display:block;margin-bottom:5px;">';
                    panelDetailHtml += '<div class="footable-row-detail-name">PDC Dimmas:</div>';
                    panelDetailHtml += '<div class="footable-row-detail-value">' + value.PDCDimmas + '</div>';
                    panelDetailHtml += '</div>';
                    panelDetailHtml += '<div class="footable-row-detail-row" style="display:block;margin-bottom:5px;">';
                    panelDetailHtml += '<div class="footable-row-detail-name">PDC Codetel:</div>';
                    panelDetailHtml += '<div class="footable-row-detail-value">' + value.PDCCodetel + '</div>';
                    panelDetailHtml += '</div>';
                    panelDetailHtml += '<div class="footable-row-detail-row" style="display:block;margin-bottom:5px;">';
                    panelDetailHtml += '<div class="footable-row-detail-name">Compañía:</div>';
                    panelDetailHtml += '<div class="footable-row-detail-value">' + value.Compania + '</div>';
                    panelDetailHtml += '</div>';
                    panelDetailHtml += '<div class="footable-row-detail-row" style="display:block;margin-bottom:5px;">';
                    panelDetailHtml += '<div class="footable-row-detail-name">Tipo de Servicio:</div>';
                    panelDetailHtml += '<div class="footable-row-detail-value">' + value.TipoDeServicio + '</div>';
                    panelDetailHtml += '</div>';
                    panelDetailHtml += '<div class="footable-row-detail-row" style="display:block;margin-bottom:5px;">';
                    panelDetailHtml += '<div class="footable-row-detail-name">Auditor Asignado:</div>';
                    panelDetailHtml += '<div class="footable-row-detail-value">' + value.AuditorAsignado + '</div>';
                    panelDetailHtml += '</div>';
                    panelDetailHtml += '<div class="footable-row-detail-row" style="display:block;margin-bottom:5px;">';
                    panelDetailHtml += '<div class="footable-row-detail-name">Venta:</div>';
                    panelDetailHtml += '<div class="footable-row-detail-value">' + value.Venta + '</div>';
                    panelDetailHtml += '</div>';
                    panelDetailHtml += '<div class="footable-row-detail-row label-info" style="visibility:' + displayPreviousId + ';display:block;margin:5px;">';
                    panelDetailHtml += '<div class="footable-row-detail-name" >Previo Call Id:</div>';
                    panelDetailHtml += '<div class="footable-row-detail-value">' + value.PrevioCallId + '</div>';
                    panelDetailHtml += '</div>';


                    panelDetailHtml += '<div class="footable-row-detail-row">';
                    panelDetailHtml += '<div class="footable-row-detail-name" ></div>';
                    panelDetailHtml += '<div class="footable-row-detail-value" id="subscriberClaims' + key + '">';
                    panelDetailHtml += '</div>';
                    panelDetailHtml += '</div>';

                    panelDetailHtml += '<div class="footable-row-detail-row">';
                    panelDetailHtml += '<div class="footable-row-detail-name" ></div>';
                    panelDetailHtml += '<div class="footable-row-detail-value" id="subscriberClaimsWithClaims' + key + '">';
                    panelDetailHtml += '</div>';
                    panelDetailHtml += '</div>';


                    panelDetailHtml += '</div>';
                    panelDetailHtml += '</td>';
                    panelDetailHtml += '</tr>';
                    //---------------------------------------

                    $(panelContainerHtml).append(panelDetailHtml);
                    panelDetailHtml = "";

                    displaySbscriberClaimsWithCredit(value.SubscriberId, value.Canvass, value.CanvEdition, key);
                    displaySbscriberClaimsWithClaims(value.SubscriberId, value.Canvass, value.CanvEdition, key);
                    
                });
                hideColorLoading();
            }
        });       
    }
    
    /*******DESPLEGA LOS RECLAMOS DE LOS SUBSCRIPTORES CON CREDITO*********/
    function displaySbscriberClaimsWithCredit(_SubscrId, _CanvCode, _CanvEdition,idSubscriber) {

        var table = "";

        $.ajax({
            type: 'POST',
            application: 'JSON',
            url: '../Assingment/GetSubscrCanvBooks',
            data: { SubscrId: _SubscrId, CanvCode: _CanvCode, CanvEdition: _CanvEdition },
            success: function (data) {


                table += '<table class="table"> <caption class"text-dark"><span class="text-dark">Informacion de Reclamos</span></caption>';
                table += '<thead>';
                table += '<tr>';
                table += '<th>No. Reclamo</th>';
                table += '<th>Libro</th>';
                table += '<th>Comentario Cliente</th>';
                table += '<th>Info Desc</th>';
                table += '</tr>';
                table += '</thead>';
                table += '<tbody id="tbdoy-claims-' + idSubscriber + '">';

                table += '</tbody>';
                table += '</table>';

                $('#foo-table-table tbody #subscriberClaims' + idSubscriber).html(table);
                $('#foo-table-table #subscriberClaims' + idSubscriber).hide();

                $.each(data.subscriberBook, function (key, value) {
                                        
                    GetSubscriberWithClaimCredit(_SubscrId, _CanvEdition, _CanvCode, value.BookCode, idSubscriber, '#tbdoy-claims-', '#foo-table-table #subscriberClaims');

                });

               
                //'#pedingAuditDetail #subscriberClaims'
                //'#tbdoy-claims-'
            }
        });
    }

    /*******DESPLEGA LOS RECLAMOS DE LOS SUBSCRIPTORES CON CREDITO PARA EL FORMULARIO DE BUSQUEDA*********/
    function displaySbscriberClaimsWithCreditForSearchForm(_SubscrId, _CanvCode, _CanvEdition, idSubscriber) {

        var table = "";

        $.ajax({
            type: 'POST',
            application: 'JSON',
            url: '../Assingment/GetSubscrCanvBooks',
            data: { SubscrId: _SubscrId, CanvCode: _CanvCode, CanvEdition: _CanvEdition },
            success: function (data) {
                
                table += '<table class="table"> <caption class"text-dark"><span class="text-dark">Informacion de Reclamos</span></caption>';
                table += '<thead>';
                table += '<tr>';
                table += '<th>No. Reclamo</th>';
                table += '<th>Libro</th>';
                table += '<th>Comentario Cliente</th>';
                table += '<th>Info Desc</th>';
                table += '</tr>';
                table += '</thead>';
                table += '<tbody id="tbdoy-claims-credit-search-' + idSubscriber + '">';
               
                table += '</tbody>';
                table += '</table>';

                $('#done-audit-foo-table #subscriberClaimsWithCreditForSearch' + idSubscriber).html(table);
                $('#done-audit-foo-table #subscriberClaimsWithCreditForSearch' + idSubscriber).hide();

                $.each(data.subscriberBook, function (key, value) {

                    GetSubscriberWithClaimCredit(_SubscrId, _CanvEdition, _CanvCode, value.BookCode, idSubscriber, '#tbdoy-claims-credit-search-', '#done-audit-foo-table #subscriberClaimsWithCreditForSearch');

                });

                

            }
        });
    }

    /*******DESPLEGA LOS RECLAMOS DE LOS SUBSCRIPTORES*********/
    function displaySbscriberClaimsWithClaims(_SubscrId, _CanvCode, _CanvEdition, idSubscriber) {

        var table = "";

        $.ajax({
            type: 'POST',
            application: 'JSON',
            url: '../Assingment/GetSubscrCanvBooks',
            data: { SubscrId: _SubscrId, CanvCode: _CanvCode, CanvEdition: _CanvEdition },
            success: function (data) {
                
                table += '<table class="table"> <caption><span class="text-dark">Cliente con Reclamos</span></caption>';
                table += '<thead>';
                table += '<tr>';
                table += '<th>No. Reclamo</th>';
                table += '<th>Libro</th>';
                table += '<th>Descripcion Reclamo</th>';
                table += '<th>Comentario Cliente</th>';
                table += '<th>Canv Code</th>';
                table += '</tr>';
                table += '</thead>';
                table += '<tbody id="tbdoy-claimswithclaims-' + idSubscriber + '">';

                table += '</tbody>';
                table += '</table>';

                $('#foo-table-table #subscriberClaimsWithClaims' + idSubscriber).html(table);
                $('#foo-table-table #subscriberClaimsWithClaims' + idSubscriber).hide();

                $.each(data.subscriberBook, function (key, value) {
                    
                    GetSubscriberWithClaims(_SubscrId, _CanvEdition, _CanvCode, value.BookCode, idSubscriber);
                    
                });
                

                

            }
        });


    }
    
    /*******OBITENE LOS RECLAMOS DE LOS SUBSCRIPTORES*********/
    function GetSubscriberWithClaims(_SubscrId, _CanvEdition, _CanvCode, _book, idSubscriber) {

        var tr = "";

        $.ajax({
            type: 'GET',
            application: 'JSON',
            url: '../Assingment/GetSubscrClaimWithClaims',
            data: { SubscrId: _SubscrId, CanvEdition: _CanvEdition, CanvCode: _CanvCode, book: _book },
            success: function (data) {
                
                $.each(data.claim, function (key, value) {
                    tr += '<tr>';
                    tr += '<td>' + value.ClaimNumber + '</td>';
                    tr += '<td>' + value.Book + '</td>';
                    tr += '<td>' + value.ClaimDescription + '</td>';
                    tr += '<td>' + value.ClientComment + '</td>';
                    tr += '<td>' + value.CanvCode + '</td>';
                    tr += '</tr>';
                });

                //$('#tbdoy-claimswithclaims-' + idSubscriber).append(tr);
                if (data.claim.length > 0) {
                    $('#foo-table-table #subscriberClaims' + idSubscriber).show();
                    $('#tbdoy-claimswithclaims-' + idSubscriber).append(tr);
                }
            }
        });
    }

    /*******OBITENE LOS RECLAMOS DE LOS SUBSCRIPTORES CON CREDITO*********/
    function GetSubscriberWithClaimCredit(_SubscrId, _CanvEdition, _CanvCode, _book, idSubscriber,tbody,parent) {

        
        var tr = "";
        $.ajax({
            type: 'GET',
            application: 'JSON',
            url: '../Assingment/GetSubscrClaimWithCredit',
            data: { SubscrId: _SubscrId, CanvEdition: _CanvEdition, CanvCode: _CanvCode, book: _book },
            success: function (data) {
                
                $.each(data.credit, function (key, value) {
                        tr += '<tr>';
                        tr += '<td>' + value.ClaimNumber + '</td>';
                        tr += '<td>' + value.Book + '</td>';
                        tr += '<td>' + value.ClientComment + '</td>';
                        tr += '<td>' + value.InfoDescription + '</td>';
                        tr += '</tr>';
                });
                                
                //$(tbody + idSubscriber).append(tr);
                if (data.credit.length > 0) {
                    $(parent + idSubscriber).show();
                    $(tbody + idSubscriber).append(tr);
                    
                }
            }
        });
    }

    /*******PERMITE REASIGNAR UNA AUDITORIA*********/
    function reassignAudit(_AssignmentId, _Auditor, _UserCode){
            
        $('#reassing-to-auditor').click(function () {

            var _reassignedAudit = [];
            
            $('[name="assigment-id-reassigned"]:checked').each(function (key, value) {

                _reassignedAudit.push(
                        {
                            "AssignmentId": $(this).val(),
                            "Auditor": $('#frm-reasign-auditor').val(),
                            "UserCode": userCredentials.Code,
                        }
                    );
            });

            if ($('#frm-reasign-auditor').val() == 'none' || _reassignedAudit.length == 0) {
                alertNoty('Debe elejir la cuenta y el auditor.', "Información", "warning");
                return false;
            }
            showColorLoading();
            $.ajax({
                type: 'POST',
                application: 'JSON',
                contentType:"application/json; charset=utf-8",
                url: '../Assingment/ReAssign',
                data: JSON.stringify({ reassignedAudit: _reassignedAudit }),
                success: function (data) {
                    
                    if (data == "DONE") {
                        
                        alertNoty('Audiorias reasignadas.', "Exito","info");
                    }

                    if (data == "ERROR") {
                        alertNoty('Error no se pudieron reasignar las auditorias.', "Error","danger");
                    }
                    
                    preDisplayPenddingAudit();
                    hideColorLoading();
                }

            });
            
        });
    }
        
    /*******OBTIENE LA DATA PARA LLENAR EL FORMULARIO DE AUDITORIA PENDIENTES*********/
    function fillFieldsForAudition(_AssignmentId) {
                
        $(".panel-datos-transaccion input[type='checkbox']").prop("checked", false);
        $(".panel-datos-transaccion #comment-audit").val("");

        $('#datos-de-la-transacción').attr("data-assignment-id-audit", _AssignmentId);

        showColorLoading();

        var datosDeLaTransaccion = "";
        $.ajax({
            type: 'POST',
            application: 'JSON',
            data: { AssignmentId: _AssignmentId },
            url: '../Assingment/GetAcctInfoByAssignment',
            success: function (data) {
                
                $(data.transaccion).each(function (key, value) {
                    datosDeLaTransaccion += '<div class="row">';
                    datosDeLaTransaccion += '<div class="col-sm-6">';
                    datosDeLaTransaccion += '<li class="list-group-item"><span class="text-dark">Razón Social:</span> ' + value.RazonSocial + '</li>';
                    datosDeLaTransaccion += '<li class="list-group-item"><span class="text-dark">SubscrId:</span> ' + value.SubscriberId + '</li>';
                    datosDeLaTransaccion += '<li class="list-group-item"><span class="text-dark">Teléfono:</span> ' + value.Telefono + '</li>';
                    datosDeLaTransaccion += '<li class="list-group-item"><span class="text-dark">Canvass:</span> ' + value.Canvass + '</li>';
                    datosDeLaTransaccion += '<li class="list-group-item"><span class="text-dark">Book Code:</span> ' + value.BookCode + '</li>';
                    datosDeLaTransaccion += '<li class="list-group-item"><span class="text-dark">Control Verbal Call Id:</span> ' + value.ControlVerballCallId + '</li>';
                    datosDeLaTransaccion += '<li class="list-group-item"><span class="text-dark">Auditor:</span> ' + value.AuditorName + '</li>';
                    datosDeLaTransaccion += '<li class="list-group-item"><span class="text-dark">Ejecutivo:</span> ' + value.Ejecutivo + '</li>';
                    datosDeLaTransaccion += '<li class="list-group-item"><span class="text-dark">Unidad:</span> ' + value.Unidad + '</li>';
                    datosDeLaTransaccion += '</div>';
                    datosDeLaTransaccion += '<div class="col-sm-6">';
                    datosDeLaTransaccion += '<li class="list-group-item"><span class="text-dark">Fecha RCP:</span> ' + value.FechaRPC + '</li>';
                    datosDeLaTransaccion += '<li class="list-group-item"><span class="text-dark">Cargo:</span> ' + value.Cargo + '</li>';
                    datosDeLaTransaccion += '<li class="list-group-item"><span class="text-dark">Venta:</span> ' + value.Venta + '</li>';
                    datosDeLaTransaccion += '<li class="list-group-item"><span class="text-dark">Call Id:</span> ' + value.CallId + '</li>';
                    datosDeLaTransaccion += '<li class="list-group-item"><span class="text-dark">Comentario del Ejecutivo:</span> ' + value.ComentarioEjecutivo + '</li>';
                    datosDeLaTransaccion += '<li class="list-group-item"><span class="text-dark">PDC Dimmas:</span> ' + value.PDCDimmas + '</li>';
                    datosDeLaTransaccion += '<li class="list-group-item"><span class="text-dark">Compañía:</span> ' + value.Compania + '</li>';
                    datosDeLaTransaccion += '<li class="list-group-item" id="data-account-id-audit" data-account-id-audit="' + value.AccountId + '"><span class="text-dark">AccountId:</span> ' + value.AccountId + '</li>';
                    datosDeLaTransaccion += '</div>';
                    datosDeLaTransaccion += '</div>';
                });

                $('#datos-de-la-transacción').html(datosDeLaTransaccion);
                hideColorLoading();
            }
        });

    }

    /*******LLENA EL FORMULARIO PARA LA AUDITORIA*********/
    function GetAcctInfoByAssignment() {

        $('#foo-table-table').on('click', '.reassign-audit', function () {
            
            var _AssignmentId = $(this).attr('data-id-assignment-audit');
            
            fillFieldsForAudition(_AssignmentId);

            $('#frm-panel-auditoria').show();
            $('#frm-panel-asignaciones-pendientes').hide();
            $('#form-panel-asignaciones').hide();
            $('#frm-panel-auditorias-realizadas').hide();

        });
    }

    /*******REALIZA LA AUDITORIA*********/
    function audit() {
       
        $('#guardar-audit').on('click', function () {

            //comment-audit
            //account-id-audit
            var assignmentId = $('#datos-de-la-transacción').attr("data-assignment-id-audit");
            var comment = $('#comment-audit').val();
            var account = $('#data-account-id-audit').attr("data-account-id-audit");

            var answer = [
                $('#pregunta1').is(":checked") ? 1 : 0, $('#pregunta2').is(":checked") ? 1 : 0,
                $('#pregunta3').is(":checked") ? 1 : 0, $('#pregunta4').is(":checked") ? 1 : 0,
                $('#pregunta5').is(":checked") ? 1 : 0, $('#pregunta6').is(":checked") ? 1 : 0,
                $('#pregunta7').is(":checked") ? 1 : 0, $('#pregunta8').is(":checked") ? 1 : 0,
                $('#pregunta9').is(":checked") ? 1 : 0, $('#pregunta10').is(":checked") ? 1 : 0,
                $('#pregunta11').is(":checked") ? 1 : 0, $('#pregunta12').is(":checked") ? 1 : 0
            ];

            var descargoReAuditoría = $('#descargo-re-auditoría').is(":checked") ? 1 : 0;
            var descargoaAdministrativo = $('#descargo-administrativo').is(":checked") ? 1 : 0;


            /*
           Audit(int AssignmentId, int AccountId, string AuditorComments, int[] Answers, string UserCode,
           bool IsDescargaAdministrativo, bool IsDescargarReauditoria)
            */
            showColorLoading();
            //JSON.stringify
            $.ajax({
                type: 'POST',
                application: 'JSON',
                contentType:'application/json charset-utf-8',
                data: JSON.stringify({
                    AssignmentId: assignmentId, AccountId: account, AuditorComments: comment, Answers: answer, UserCode: userCredentials.Code,
                    IsDescargaAdministrativo: descargoaAdministrativo, IsDescargarReauditoria: descargoReAuditoría
                }),
                url: '../Assingment/Audit',
                success: function (data) {
                    if (data.RESUSLTSET == "DONE") {
                        alertNoty("Auditoria realizada", "Exito", "info");
                        $('#frm-panel-auditoria').hide();
                        $('#frm-panel-asignaciones-pendientes').show();
                        
                    }

                    if (data.RESUSLTSET == "ERROR") {
                        alertNoty("La auditoria no se pudo realizar", "Error","danger");
                    }

                    preDisplayPenddingAudit();
                    hideColorLoading();
                }
            });
            
        });
    }

    /*******OBTIENE LAS AUDITORIAS REALIZADAS*********/
    function getDoneAudit() {        
        $('#buscar-audited-audit').on('click', function () {
            //doneAudit
            doneAudit._Auditor = ($('#frm-search-auditor').val() == "none" ? "" : $('#frm-search-auditor').val());
            doneAudit._SalesDate = $('#frm-buscar-selected-date-rdv').val();
            doneAudit._CreationDate = $('#frm-buscar-selected-date-auditoria').val();
            doneAudit._SubscrId = $('#frm-search-subscriber-id').val();
            doneAudit._CallId = $('#frm-search-call-id').val();
            doneAudit._PhoneNo = $('#frm-search-telefono').val();

            displayDoneAudit(doneAudit);
        });        
    }

    function displayDoneAudit(doneAudit)
    {
     
        var displayPreviousId = "";
        var displayBtnEliminar = userCredentials.Group == "adm" ? "inline-block" : "none";

        if (
            $.trim(doneAudit._SubscrId) == "" && $.trim(doneAudit._Auditor) == "" && $.trim(doneAudit._SalesDate) == "" &&
            $.trim(doneAudit._CreationDate) == "" && $.trim(doneAudit._CallId) == "" && $.trim(doneAudit._PhoneNo) == ""
        ) {
            alertNoty("Debe proveer almentos, unos de los parametros", "Información", "warning");
            return false;
        }
        showColorLoading();
        $.ajax({
            type: 'GET',
            application: 'JSON',
            url: '../Assingment/GetDoneAudits',
            data: {
                SubscrId: doneAudit._SubscrId,
                Auditor: doneAudit._Auditor,
                SalesDate: doneAudit._SalesDate,
                CreationDate: doneAudit._CreationDate,
                CallId: doneAudit._CallId,
                PhoneNo: doneAudit._PhoneNo
            },
            success: function (data) {

                $(data.transaccion).each(function (key, value) {

                    displayPreviousId = "inline-block";
                    if (value.PrevioCallId == 'X') {
                        displayPreviousId = "none";
                    }

                    audit += '<tr class="shower-foo-table">';
                    audit += '<td class="footable-visible footable-first-column"><span class="footable-toggle"></span>' + value.RazonSocial + '</td>';
                    audit += '<td class="footable-visible">' + value.SubscriberId + '</td>';
                    audit += '<td class="footable-visible">' + value.Telefono + '</td>';
                    audit += '<td class="footable-visible">' + value.Canvass + '</td>';
                    audit += '<td class="footable-visible">' + value.BookCode + '</td>';
                    audit += '<td class="footable-visible">' + value.Ejecutivo + '</td>';
                    audit += '<td class="footable-visible">' + value.Unidad + '</td>';
                    audit += '<td class="footable-visible">' + value.FechaRPC + '</td>';
                    audit += '<td class="footable-visible">' + value.Cargo + '</td>';
                    audit += '<td class="footable-visible">' + value.CallId + '</td>';
                    audit += '<td class="footable-visible footable-last-column">';
                    audit += '<button class="btn btn-info reaudit-form-search" data-assignment-id="' + value.AssignmentId + '">Re-Auditar</button>';
                    audit += '</td>';
                    audit += '<td class="footable-visible footable-last-column">';
                    audit += '<button class="btn btn-danger fa fa-trash delete-form-search" style="display:' + displayBtnEliminar + ';" data-audit-id="' + value.AuditId + '"/>';
                    audit += '</td>';
                    audit += '<td style="display: none;">wer</td>';
                    audit += '<td style="display: none;">wer</td>';
                    audit += '<td style="display: none;">wert</td>';
                    audit += '<td style="display: none;">wert</td>';
                    audit += '<td style="display: none;">wert</td>';
                    audit += '<td style="display: none;">wer</td>';
                    audit += '<td style="display: none;">wert</td>';
                    audit += '<td style="display: none;">wt</td>';
                    audit += '<td style="display: none;">wert</td>';
                    audit += '</tr>';

                    audit += '<tr class="footable-row-detail hide-tr-footable">';
                    audit += '<td class="footable-row-detail-cell" colspan="12">';
                    audit += '<div class="footable-row-detail-inner">';
                    audit += '<div class="footable-row-detail-row" style="display:block;margin-bottom:5px;">';
                    audit += '<div class="footable-row-detail-name" style="width:146px;">Control Verbal Call Id:</div>';
                    audit += '<div class="footable-row-detail-value">' + value.ControlVerballCallId + '</div>';
                    audit += '</div>';
                    audit += '<div class="footable-row-detail-row" style="display:block;margin-bottom:5px;">';
                    audit += '<div class="footable-row-detail-name">Conmentario Ejecutivo:</div>';
                    audit += '<div class="footable-row-detail-value">' + value.ComentarioEjecutivo + '</div>';
                    audit += '</div>';
                    audit += '<div class="footable-row-detail-row" style="display:block;margin-bottom:5px;">';
                    audit += '<div class="footable-row-detail-name">PDC Dimmas:</div>';
                    audit += '<div class="footable-row-detail-value">' + value.PDCDimmas + '</div>';
                    audit += '</div>';
                    audit += '<div class="footable-row-detail-row" style="display:block;margin-bottom:5px;">';
                    audit += '<div class="footable-row-detail-name">PDC Codetel:</div>';
                    audit += '<div class="footable-row-detail-value">' + value.PDCCodetel + '</div>';
                    audit += '</div>';
                    audit += '<div class="footable-row-detail-row" style="display:block;margin-bottom:5px;">';
                    audit += '<div class="footable-row-detail-name">Compañía:</div>';
                    audit += '<div class="footable-row-detail-value">' + value.Compania + '</div>';
                    audit += '</div>';
                    audit += '<div class="footable-row-detail-row" style="display:block;margin-bottom:5px;">';
                    audit += '<div class="footable-row-detail-name">Status de Auditoria:</div>';
                    audit += '<div class="footable-row-detail-value">' + value.Status + '</div>';
                    audit += '</div>';
                    audit += '<div class="footable-row-detail-row"  style="display:block;margin-bottom:5px;">';
                    audit += '<div class="footable-row-detail-name">Fecha de Auditoria:</div>';
                    audit += '<div class="footable-row-detail-value">' + value.AuditCreationDate + '</div>';
                    audit += '</div>';

                    audit += '<div class="footable-row-detail-row" style="display:block;margin-bottom:5px;">';
                    audit += '<div class="footable-row-detail-name">Auditor:</div>';
                    audit += '<div class="footable-row-detail-value">' + value.AuditorName + '</div>';
                    audit += '</div>';

                    audit += '<div class="footable-row-detail-row" style="display:block;margin-bottom:5px;">';
                    audit += '<div class="footable-row-detail-name">Venta:</div>';
                    audit += '<div class="footable-row-detail-value">' + value.Venta + '</div>';
                    audit += '</div>';
                    audit += '<div class="footable-row-detail-row" style="display:block;margin-bottom:5px;">';
                    audit += '<div class="footable-row-detail-name">Resultado:</div>';
                    audit += '<div class="footable-row-detail-value">' + value.AuditResult + '</div>';
                    audit += '</div>';
                    audit += '<div class="footable-row-detail-row" style="display:block;margin-bottom:5px;">';
                    audit += '<div class="footable-row-detail-name">Preguntas Invalidas:</div>';
                    audit += '<div class="footable-row-detail-value">' + value.InvalidQuestions + '</div>';
                    audit += '</div>';
                    audit += '<div class="footable-row-detail-row" style="display:block;margin-bottom:5px;">';
                    audit += '<div class="footable-row-detail-name">Comentario del Auditor:</div>';
                    audit += '<div class="footable-row-detail-value">' + value.ComentarioAuditor + '</div>';
                    audit += '</div>';

                    audit += '<div class="footable-row-detail-row label-info" style="display:' + displayPreviousId + ';margin-bottom:5px;">';
                    audit += '<div class="footable-row-detail-name" >Previo Call Id:</div>';
                    audit += '<div class="footable-row-detail-value">' + value.PrevioCallId + '</div>';
                    audit += '</div>';


                    audit += '<div class="footable-row-detail-row">';
                    audit += '<div class="footable-row-detail-name" ></div>';
                    audit += '<div class="footable-row-detail-value" id="subscriberClaimsWithCreditForSearch' + key + '">';
                    audit += '</div>';
                    audit += '</div>';

                    audit += '</div>';
                    audit += '</td>';
                    audit += '</tr>';

                    displaySbscriberClaimsWithCreditForSearchForm(value.SubscriberId, value.Canvass, value.CanvEdition, key);

                });
                
                $('#done-audit-foo-table tbody').html(audit);                
                audit = '';
                hideColorLoading();
            }
        });
    }

    /*******PERMITE REAUDITAR UNA CUENTA*********/
    function reaudit() {

        $('#done-audit-foo-table').on('click', '.reaudit-form-search', function () {

            var _AssignmentId = $(this).attr('data-assignment-id');
            
            fillFieldsForAudition(_AssignmentId);

            $('#frm-panel-auditoria').show();
            $('#frm-panel-asignaciones-pendientes').hide();
            $('#form-panel-asignaciones').hide();
            $('#frm-panel-auditorias-realizadas').hide();

        });
    }

    /*******ELIMINA UNA AUDITORIA*********/
    function deletee() {

        //DeleteAudit(int AuditId, string UserCode)
        $('#done-audit-foo-table').on('click', '.delete-form-search', function () {
                                    
            var _AuditId = $(this).attr('data-audit-id');
            var _UserCode = userCredentials.Code;
            
            bootbox.confirm("Esta seguro de eliminar la auditoría?", function (result) {
                if (result) {

                    showColorLoading();

                    $.ajax({
                        type: 'POST',
                        application: 'JSON',
                        data: { AuditId: _AuditId, UserCode: _UserCode },
                        url: '../Assingment/DeleteAudit',
                        success: function (data) {

                            if (data.RESULTSET == "DONE") {
                                alertNoty("Auditoria eliminada", "Exito", "success");

                            }

                            if (data.RESULTSET == "ERROR") {
                                alertNoty("No se pudo eliminar la auditoria", "Error", "danger");
                            }
                            hideColorLoading();
                        }
                    });

                    displayDoneAudit(doneAudit);
                }
            })           

        });

    }

    //buscar-reporte
    //GetReportPerAssor(string DateFrom, string DateTo)
    function getReportByAssesor(dateFrom, dateTo) {
   
        $("#reportes-por-auditoria-table").hide();
        $("#reportes-por-unidad-table").hide();        
        $("#reportes-por-dato-vital-table").hide();
        var tr = "";

        showColorLoading();

        $.ajax({
            type: 'GET',
            application: 'JSON',
            url: '../Assingment/GetReportPerAssor',
            data: { DateFrom: dateFrom, DateTo: dateTo },
            success: function (data) {
                
                var count = data.RESULTSET.length - 1;

                if (count < 0) {
                    hideColorLoading();
                    alertNoty("No hay informacion desde " + dateFrom + " hasta " + dateTo + ".", "Informacíon", "warning");
                    return false;
                }
                
                var reportePorAuditoriaurl = window.location.origin + "/" + (data.path.substr(data.path.lastIndexOf("SRAM"), data.path.length));
                $('#link-reporte-por-auditoria').attr("href", reportePorAuditoriaurl);

                $(data.RESULTSET).each(function (key, value) {
                    
                    if (key == count) {
                        tr += "<tr style='border-top:solid 20px white;'><td> </td><td> </td><td> </td><td> </td><td> </td></tr>";
                        tr += "<tr style='background-color:rgb(230, 230, 230)'><td>" + value.Unidad + "</td><td>TOTAL DE TODAS LAS UNIDADES:</td><td>" + value.TotalLamadas + "</td><td>" + value.TotalInvalidas + "</td><td>" + (value.PorcentajeInvalidas + "%") + "</td></tr>";
                            
                    } else {
                        tr += "<tr class='" + (value.PorcentajeInvalidas > 20 ? "danger" : "") + "'" + "><td>" + (value.Ejecutivo == "TOTAL:" ? "" : value.Unidad) + "</td><td>" + (value.Ejecutivo == "TOTAL:" ? "TOTAL PARA LA UNIDAD:" + value.Unidad : value.Ejecutivo) + "</td><td>" + value.TotalLamadas + "</td><td>" + value.TotalInvalidas + "</td><td>" + (value.PorcentajeInvalidas + "%") + "</td></tr>";
                    }
                        
                });

                $("#reportes-por-asesor-table").show();
                $("#reportes-por-asesor-table tbody").html(tr);
                hideColorLoading();
            }
        });
    }

    function getReportByUnit(dateFrom, dateTo) {

        $("#reportes-por-auditoria-table").hide();        
        $("#reportes-por-asesor-table").hide();
        $("#reportes-por-dato-vital-table").hide();
        var tr = "";
        showColorLoading();
        $.ajax({
            type: 'GET',
            application: 'JSON',
            url: '../Assingment/GetRepordPerUnidad',
            data: { DateFrom: dateFrom, DateTo: dateTo },
            success: function (data) {
                    
                var count = data.RESULTSET.length - 1;
                    
                if (count < 0) {
                    hideColorLoading();
                    alertNoty("No hay informacion desde " + dateFrom + " hasta " + dateTo + ".", "Informacíon", "warning");
                    return false;
                }

                var reportePorAuditoriaurl = window.location.origin + "/" + (data.path.substr(data.path.lastIndexOf("SRAM"), data.path.length));
                $('#link-reporte-por-auditoria').attr("href", reportePorAuditoriaurl);
                $(data.RESULTSET).each(function (key, value) {
                        
                    if (key == count) {
                        tr += "<tr style='border-top:solid 3px black;'><td> </td><td> </td><td> </td><td> </td><td> </td></tr>";
                        tr += "<tr style='background-color:rgb(230, 230, 230)'><td>" + value.Unidad + "</td><td>TOTAL DE TODAS LAS UNIDADES:</td><td>" + value.TotalLamadas + "</td><td></td><td>" + value.NiTotal.toLocaleString('en') + "</td></tr>";
                           
                    } else {
                        tr += "<tr class='" + (value.Porcentaje > 20  ? "danger" : "") + "'><td>" + value.Unidad + "</td><td>" + value.Calificacion + "</td><td>" + value.TotalLamadas + "</td><td>" + ((value.Porcentaje < 0 ? "" : value.Porcentaje + "%")) + "</td><td>" + value.NiTotal.toLocaleString('en') + "</td></tr>";
                    }
                    
                });

                $("#reportes-por-unidad-table").show();
                $("#reportes-por-unidad-table tbody").html(tr);
                hideColorLoading();
            }
        });
    }

    //GetRepordPerAudit
    function getReportByAudit(dateFrom, dateTo) {

        
        $("#reportes-por-unidad-table").hide();
        $("#reportes-por-asesor-table").hide();
        $("#reportes-por-dato-vital-table").hide();
        var tr = "";

        showColorLoading();

        $.ajax({
            type: 'GET',
            application: 'JSON',
            url: '../Assingment/GetRepordPerAudit',
            data: { DateFrom: dateFrom, DateTo: dateTo },
            success: function (data) {

                var count = data.RESULTSET.length - 1;

                if (count < 0) {
                    hideColorLoading();
                    alertNoty("No hay informacion desde " + dateFrom + " hasta " + dateTo + ".", "Informacíon", "warning");
                    return false;
                }

                var reportePorAuditoriaurl = window.location.origin + "/" + (data.path.substr(data.path.lastIndexOf("SRAM"), data.path.length));

                $('#link-reporte-por-auditoria').attr("href", reportePorAuditoriaurl);

                $(data.RESULTSET).each(function (key, value) {
                        
                    //if (key == count) {
                    tr += "<tr><td>" + value.FechaRDV + "</td><td>" + value.RazonSocial + "</td><td>" + value.SubscriberId + "</td><td>" + value.Canva + "</td>";
                    tr += "<td>" + value.Edicion + "</td><td>" + value.cargo + "</td><td>" + value.Monto + "</td>";
                    tr += "<td>" + value.Tarjeta + "</td><td>" + value.Ejecutivo + "</td><td>" + value.Unidad + "</td><td>" + value.P1 + "</td>";
                    tr += "<td>" + value.P2 + "</td><td>" + value.P3 + "</td><td>" + value.P4 + "</td><td>" + value.P5 + "</td><td>" + value.P6 + "</td>";
                    tr += "<td>" + value.P7 + "</td><td>" + value.P8 + "</td><td>" + value.P9 + "</td><td>" + value.P10 + "</td><td>" + value.Result + "</td>";
                    tr += "<td>" + value.isCodigo34 + "</td><td>" + value.EsDescargaAdministrativa + "</td><td>" + value.EsDescargaReauditoria + "</td><td>" + value.CallId + "</td>";


                        //tr += "<tr style='border-top:solid 3px black;'><td> </td><td> </td><td> </td><td> </td><td> </td></tr>";
                        //tr += "<tr style='background-color:rgb(230, 230, 230)'><td>" + value.Unidad + "</td><td>TOTAL DE TODAS LAS UNIDADES:</td><td>" + value.TotalLamadas + "</td><td>" + value.Porcentaje + "</td><td>" + value.NiTotal + "</td></tr>";

                    //} else {
                    //    tr += "<tr class='" + (value.Calificacion == "Total por unidad:" ? "danger" : "") + "'><td>" + value.Unidad + "</td><td>" + value.Calificacion + "</td><td>" + value.TotalLamadas + "</td><td>" + value.Porcentaje + "</td><td>" + value.NiTotal + "</td></tr>";
                    //}


                });
                $("#reportes-por-auditoria-table").show();
                $("#reportes-por-auditoria-table tbody").html(tr);
                hideColorLoading();
            }
        });
    }

    function getReportByDatoVital(dateFrom, dateTo) {

        $("#reportes-por-auditoria-table").hide();
        $("#reportes-por-unidad-table").hide();
        $("#reportes-por-asesor-table").hide();
        var p4Excede9
        //#ffa31a
        var tr = "";

        showColorLoading();
        $.ajax({
            type: 'GET',
            application: 'JSON',
            url: '../Assingment/reportePorDatoVital',
            data: { DateFrom: dateFrom, DateTo: dateTo },
            success: function (data) {

                var count = data.RESULTSET.length - 1;

                if (count < 0) {
                    hideColorLoading();
                    alertNoty("No hay informacion desde " + dateFrom + " hasta " + dateTo + ".", "Informacíon", "warning");
                    return false;
                }

                var reportePorAuditoriaurl = window.location.origin + "/" + (data.path.substr(data.path.lastIndexOf("SRAM"), data.path.length));
                $('#link-reporte-por-auditoria').attr("href", reportePorAuditoriaurl);
                $(data.RESULTSET).each(function (key, value) {
                      
                    if (key == count) {

                        tr += "<tr class='danger'><td>Total:</td>";
                        tr += "<td>" + value.TotalLlamadas + "</td>";
                        tr += "<td>" + value.CargoTotal.toLocaleString('en') + "</td>";
                        tr += "<td>" + value.Grabaciones + "</td>";
                        tr += "<td>" + value.CargoInvalido.toLocaleString('en') + "</td>";
                        tr += "<td>" + value.CargoInvalidoVsTotalCargo + "</td>";
                        tr += "<td>" + value.InvalidasVsTotalAuditadas + "</td>";
                        tr += "<td>" + value.Pregunta3 + "</td>";
                        tr += "<td style='color:" + (value.Pregunta4 > 9 ? "#ff6666;'" : "'") + ">" + value.Pregunta4 + "</td>";
                        tr += "<td style='color:" + (value.Pregunta5 > 9 ? "#ff6666;'" : "'") + ">" + value.Pregunta5 + "</td>";
                        tr += "<td style='color:" + (value.Pregunta6 > 9 ? "#ff6666;'" : "'") + ">" + value.Pregunta6 + "</td>";
                        tr += "<td style='color:" + (value.Pregunta7 > 9 ? "#ff6666;'" : "'") + ">" + value.Pregunta7 + "</td>";
                        tr += "<td style='color:" + (value.Pregunta8 > 9 ? "#ff6666;'" : "'") + ">" + value.Pregunta8 + "</td>";
                        tr += "<td style='color:" + (value.Pregunta9 > 9 ? "#ff6666;'" : "'") + ">" + value.Pregunta9 + "</td>";
                        tr += "<td>" + value.TotalDeIncidencias + "</td></tr>";

                    } else {

                        tr += "<tr><td>" + value.Unidad + "</td>";
                        tr += "<td>" + value.TotalLlamadas + "</td>";
                        tr += "<td>" + value.CargoTotal.toLocaleString('en') + "</td>";
                        tr += "<td>" + value.Grabaciones + "</td>";
                        tr += "<td>" + value.CargoInvalido.toLocaleString('en') + "</td>";
                        tr += "<td>" + value.InvalidasVsTotalAuditadas + "%</td>";
                        tr += "<td>" + value.CargoInvalidoVsTotalCargo + "%</td>";
                        tr += "<td>" + value.Pregunta3 + "</td>";
                        tr += "<td style='background-color:" + (value.TotalDeIncidencias > 9 ? "#ffa31a;'" : "'") + ">" + value.Pregunta4 + "</td>";
                        tr += "<td style='background-color:" + (value.TotalDeIncidencias > 9 ? "#ffa31a;'" : "'") + ">" + value.Pregunta5 + "</td>";
                        tr += "<td style='background-color:" + (value.TotalDeIncidencias > 9 ? "#ffa31a;'" : "'") + ">" + value.Pregunta6 + "</td>";
                        tr += "<td style='background-color:" + (value.TotalDeIncidencias > 9 ? "#ffa31a;'" : "'") + ">" + value.Pregunta7 + "</td>";
                        tr += "<td style='background-color:" + (value.TotalDeIncidencias > 9 ? "#ffa31a;'" : "'") + ">" + value.Pregunta8 + "</td>";
                        tr += "<td style='background-color:" + (value.TotalDeIncidencias > 9 ? "#ffa31a;'" : "'") + ">" + value.Pregunta9 + "</td>";
                        tr += "<td style='background-color:" + (value.TotalDeIncidencias > 9 ? "#ffa31a;'" : "'") + ">" + value.TotalDeIncidencias + "</td></tr>";
                    }

                });


                $("#reportes-por-dato-vital-table").show();
                $("#reportes-por-dato-vital-table tbody").html(tr);
                hideColorLoading();
            }
        });
    }

    function reportAdm() {

        $('#buscar-reporte').on('click', function () {

            
            
            var _DateFrom = $('#frm-repord-desde').val();
            var _DateTo = $('#frm-report-hasta').val();
            var tipoDeReporte = $("#select-control-type-of-report").val();

            if (_DateFrom == "" || _DateTo == "" || tipoDeReporte == "none") {
                alertNoty("Debe proveer las fechas y el tipo de reporte.", "Informacíon", "warning");
                return false;
            }

            switch(tipoDeReporte){
                
                case 'asesor': getReportByAssesor(_DateFrom, _DateTo);
                    break;
                case 'unidad': getReportByUnit(_DateFrom, _DateTo);
                    break;
                case 'auditoria': getReportByAudit(_DateFrom, _DateTo);
                    break;
                case 'dato-vital': getReportByDatoVital(_DateFrom, _DateTo);
                    break;
            }            
        });
    }

    /*INTERFACE QUE ENGLOBA LAS FUNCIONES DEL MODULO [AssignmentModule]*/
    /*******HACE VISIBLE TODAS LA FUNCIONALIDADES QUE CONTIENE EL MODULO*********/
    return {
     /*->*/   Constructor:AssigmentModuleConstructor,
     /*->*/   getAuditors: getAuditorsFromDb,
     /*->*/   getPedding: getPeddingAssignMentsFromDb,
     /*->*/   assignAudit: assignAudit,
     /*->*/   getPendingAudit: getPendingAudit,
     /*->*/   GetSubscriberWithClaims: GetSubscriberWithClaims,
     /*->*/   GetSubscriberWithClaimCredit: GetSubscriberWithClaimCredit,
     /*->*/   reassignAudit: reassignAudit,
     /*->*/   GetAcctInfoByAssignment: GetAcctInfoByAssignment,
     /*->*/   audit: audit,
     /*->*/   getDoneAudit: getDoneAudit,
     /*->*/   reportAdm: reportAdm,
     /*->*/   init: init,
              preDisplayPenddingAudit: preDisplayPenddingAudit
     /*->*/  
    };
    /**********************..........................*****************************
     .......***************ASSIGMENT MODULE INTERFACE********************.........
     **********************..........................*****************************
     *****************************************************************************/
}());
$(function () {
    
    var pendingTable = $("#pending-table").footable();

    $('#demo-foo-collapse').on('click', function () {
        pendingTable.trigger('footable_collapse_all');
    });

    $('#demo-foo-expand').on('click', function () {
        pendingTable.trigger('footable_expand_all');
    });

    $('#demo-show-entries').change(function (e) {
        e.preventDefault();
        var pageSize = $(this).val();
        pendingTable.data('page-size', pageSize);
        pendingTable.trigger('footable_initialized');
    });

    $('#demo-foo-search').on('input', function (e) {
        e.preventDefault();
        pendingTable.trigger('footable_filter', { filter: $(this).val() });
    });

    $("#pending-table").on('click', '.show-credit', function () {
        $('#credit-modal').modal().show('show');
    });

    $("#pending-table").on('click', '.show-claim', function () {
        $('#claim-modal').modal('show');
    });

    PendingModule.Init();

});

var PendingModule = (function () {

    var self = {};
    var templates = {
        summary: Handlebars.compile($("#pending-summary").html())
    };

    function Init() {
        audit(function () { window.location = pendingUrl; });
        getAuditor();
        getPendingSummary();
        reasignar(function () { window.location = pendingUrl; });
        refreshPending();
    }

    function audit(onSucess) {

        $('#pending-table').on('click', '.audit-command-button', function () {

            self.assigmentId = $(this).attr("data-assigment-id");
            self.accoutId = $(this).attr("data-account-id");
            
            fillFieldsForAudition($(this).attr("data-url"), self.assigmentId);
            $("#audit-modal").modal('show');
        });

        $(document).on('click', '#guardar-auditoria', function () {
                      
            var comment = $('#comment-audit').val();           
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

            showColorLoading();

            var data = {
                AssignmentId: self.assigmentId, AccountId: self.accoutId, AuditorComments: comment, Answers: answer,
                IsDescargaAdministrativo: descargoaAdministrativo, IsDescargarReauditoria: descargoReAuditoría
            };

            var auditory = new Auditory();
            auditory.audit($(this).data("url"), data).done(function (response) {
                if (response.OK) {
                    alertNoty(response.OK, "informacion", "success");
                    onSucess();
                }

                if (response.ERROR) {
                    alertNoty(response.ERROR, "Error", "danger");
                }

                $('input:checked').prop("checked", false);
                $("#audit-modal").modal('hide');
                hhideColorLoading();
               
            });
        });
    }

    function getAuditor() {
        var auditor = new Auditor();
        var url = $("#frm-reasign-auditor").data("url");
        var option = "<option value='none'>Auditor</option>";

        auditor.getAuditor(url).done(function (response) {
            
            $(response.auditors).each(function (key,value) {
                option += "<option value='" + value.UserCode + "'>" + value.Name + "</option>";
            });

            $("#frm-reasign-auditor").html(option);
            $("#frm-buscar-auditor").html(option);
        });
    }

    function getPendingSummary() {
        var auditor = new Auditor();
        var url = $("#pedding-audit").data("url");
        
        
        auditor.getPendingResume(url).done(function (response) {

            if (response.pending) {
                var output = templates.summary({ summary: response.pending });
                $("#pending-summary-count").html(output);
                $(".total-auditar").text(sumSummary(response.pending));
            }

        });
    }

    function sumSummary(summary) {
        
        var count = 0;
        $(summary).each(function (key, value) {
            count += value.Count;           
        });
        return count;
    }

    function reasignar(onSuccess) {
                
        $("#reassing-to-auditor").on("click", function () {
            
            var auditory = new Auditory();
            var assigments = [];
            var user = $('#frm-reasign-auditor').val();
            var url = $(this).data("url");

            $(".checked-assigment:checked").each(function (key, value) {                
                assigments.push($(value).val());                
            });

            if (assigments.length > 0) {
                auditory.reassign(url, JSON.stringify({ Assigments: assigments, User: user })).done(function (response) {
                    if (response.WARNING) {
                        alertNoty(response.WARNING, "informacion", "warning");
                    }

                    if (response.ERROR) {
                        alertNoty(response.ERROR, "Error", "danger");
                    }

                    if (response.OK) {
                        alertNoty(response.OK, "informacion", "success");
                        onSuccess();
                    }
                });
            }
            else
            {
                alertNoty("Debe seleccionar la assignacion a reasignar.", "informacion", "warning");
            }

        });
    }

    function datosDeLaTransaccion(value)
    {
        $(".trn-razon").text(value.RazonSocial);
        $(".trn-subscr").text(value.SubscriberId);
        $(".trn-tell").text(value.Telefono);
        $(".trn-canv").text(value.Canvass);
        $(".trn-book").text(value.BookCode);
        $(".trn-control").text(value.ControlVerballCallId);
        $(".trn-auditor").text(value.AuditorName);
        $(".trn-ejecutivo").text(value.Ejecutivo);
        $(".trn-account").text(value.AccountId);
        $(".trn-unidad").text(value.Unidad);
        $(".trn-fecha").text(value.FechaRPC);
        $(".trn-cargo").text(value.Cargo);
        $(".trn-venta").text(value.Venta);
        $(".trn-call-id").text(value.CallId);
        $(".trn-comentario").text(value.ComentarioEjecutivo);
        $(".trn-pdc").text(value.PDCDimmas);
        $(".trn-company").text(value.Compania);

        self.accoutId = value.AccountId;
    }

    function fillFieldsForAudition(url,_AssignmentId) {
                
        showColorLoading();
        var auditory = new Auditory();
        auditory.transactionData(url, { AssignmentId: _AssignmentId }).done(function (response) {
            $(response.transaccion).each(function (key, value) {
                datosDeLaTransaccion(value);
            });

            hideColorLoading();
        });
        

    }

    function refreshPending() {
        
        $(document).on("click", "#refrescar-pendiente-auditar", function () {
            window.location = pendingUrl;
        });
    }

    return {
        Init: Init,
        audit: audit,
        getAuditor: getAuditor,
        getPendingSummary: getPendingSummary,
        reasignar: reasignar
    };

}());
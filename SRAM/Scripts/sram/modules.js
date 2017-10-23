var PendingModule = (function () {

    var self = {};
    var templates = {};

    function Init() {

        
        templates.summary = Handlebars.compile($("#pending-summary").html());
        audit(function () { window.location = pendingUrl; });
        getAuditor();
        getPendingSummary();
        reasignar(function () { window.location = pendingUrl; });
        refreshPending();

    }

    function audit(onSucess) {

        $("#audit-modal").on("hidden.bs.modal", function () {
            cleanAuditComponent();
        });

        $('#pending-table').on('click', '.audit-command-button', function () {

            self.assigmentId = $(this).attr("data-assigment-id");
            self.accoutId = $(this).attr("data-account-id");

            fillFieldsForAudition($(this).attr("data-url"), self.assigmentId);
            $("#audit-modal").modal('show');
        });

        $(document).on('click', '#guardar-auditoria', function () {

            var comment = $('#comment-audit').val();
            var answer = [
                    $($("div#pregunta1 input[type='radio']:checked")[0]).val(), $($("div#pregunta2 input[type='radio']:checked")[0]).val(),
                    $($("div#pregunta3 input[type='radio']:checked")[0]).val(), $($("div#pregunta4 input[type='radio']:checked")[0]).val(),
                    $($("div#pregunta5 input[type='radio']:checked")[0]).val(), $($("div#pregunta6 input[type='radio']:checked")[0]).val(),
                    $($("div#pregunta7 input[type='radio']:checked")[0]).val(), $($("div#pregunta8 input[type='radio']:checked")[0]).val(),
                    $($("div#pregunta9 input[type='radio']:checked")[0]).val(), $($("div#pregunta10 input[type='radio']:checked")[0]).val(),
                    $($("div#pregunta11 input[type='radio']:checked")[0]).val(), $($("div#pregunta12 input[type='radio']:checked")[0]).val()
            ];

            for (var i = 0; i < answer.length; i++) {
                if (answer[i] == undefined)
                {
                    alertNoty("Debeseleccionar los datos vitales.", "Error", "danger");
                    return false;
                }
            }

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
                    cleanAuditComponent();
                    onSucess();
                }

                if (response.ERROR) {
                    alertNoty(response.ERROR, "Error", "danger");
                    cleanAuditComponent();
                }

                $('input:checked').prop("checked", false);
                $("#audit-modal").modal('hide');
                hideColorLoading();

            });
        });
    }

    function getAuditor() {

        var auditor = new Auditor();
        var url = $("#frm-reasign-auditor").data("url");
        var option = "<option value='none'>Auditor</option>";

        auditor.getAuditor(url).done(function (response) {
            //showColorLoading();
            $(response.auditors).each(function (key, value) {
                option += "<option value='" + value.UserCode + "'>" + value.Name + "</option>";
            });

            $("#frm-reasign-auditor").html(option);
            $("#frm-buscar-auditor").html(option);
            //hideColorLoading();
        });
    }

    function getPendingSummary() {
        var auditor = new Auditor();
        var url = $("#pedding-audit").data("url");


        auditor.getPendingResume(url).done(function (response) {

            showColorLoading();
            if (response.pending) {
                var output = templates.summary({ summary: response.pending });
                $("#pending-summary-count").html(output);
                $(".total-auditar").text(sumSummary(response.pending));
            }
            hideColorLoading();
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
            else {
                alertNoty("Debe seleccionar la assignacion a reasignar.", "informacion", "warning");
            }

        });
    }

    function datosDeLaTransaccion(value) {
        $(".trn-razon").text(value.RazonSocial);
        $(".trn-subscr").text(value.SubscriberId);
        $(".trn-tell").text(value.Telefono);
        $(".trn-canv").text(value.Canvass);
        $(".trn-book").text(value.BookCodeDetail);
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

    function fillFieldsForAudition(url, _AssignmentId) {

        
        var auditory = new Auditory();
        auditory.transactionData(url, { AssignmentId: _AssignmentId }).done(function (response) {

            showColorLoading();
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

    function cleanAuditComponent() {
        $($("#audit-modal input[type='radio']:checked")).prop("checked", false);
        $("#comment-audit").val("");
        $('#descargo-re-auditoría').prop("checked", false);
        $('#descargo-administrativo').prop("checked", false);
    }

    return {
        Init: Init,
        audit: audit,
        getAuditor: getAuditor,
        getPendingSummary: getPendingSummary,
        reasignar: reasignar
    };

}());


var AuditoryModule = (function () {

    function Init() {
        getAuditor();        
    }

    function getAuditor() {
        var auditor = new Auditor();
        var element = $('[name="Auditor"]');
        var url = element.data("url");
        var option = "<option value=''>Auditores</option>";

        auditor.getAuditor(url).done(function (response) {
            showColorLoading();
            $(response.auditors).each(function (key, value) {
                option += "<option value='" + value.UserCode + "'>" + value.Name + "</option>";
            });

            element.html(option);
            hideColorLoading();
            setSearchFormCurVal();
        });
    }

    function setSearchFormCurVal() {

        var form = $("#form-search-done-auditories");
        var auditor = $(form).find('[name="Auditor"]');
        var salesDate = $(form).find('[name="SalesDate"]');
        var creationDate = $(form).find('[name="CreationDate"]');
        var subscrId = $(form).find('[name="SubscrId"]');
        var callId = $(form).find('[name="CallId"]');
        var phone = $(form).find('[name="PhoneNo"]');

        auditor.val(auditor.attr("data-current-val"));
        salesDate.val(salesDate.attr("data-current-val"));
        creationDate.val(creationDate.attr("data-current-val"));
        subscrId.val(subscrId.attr("data-current-val"));
        callId.val(callId.attr("data-current-val"));
        phone.val(phone.attr("data-current-val"));

    }

    function deleteAudit(onSuccess) {

        $(document).on('click', '.btn-delete-audit', function () {

            var AuditId = $(this).attr('data-audit-id');
            var auditory = new Auditory();
            var url = $(this).attr("data-url");
            var data = { AuditId: AuditId };

            bootbox.confirm("Esta seguro de eliminar la auditoría?", function (result) {
                if (result) {

                    showColorLoading();
                    auditory.deleteAudit(url, data).done(function (response) {

                        if (response.OK) {
                            alertNoty(response.OK, "Informacion", "info");
                            onSuccess();
                        }

                        if (response.ERROR) {
                            alertNoty(response.ERROR, "Warning", "warning");
                        }

                        hideColorLoading();
                    });

                }
            })

        });

    }

    return {
        Init: Init,
        deleteAudit: deleteAudit
    };

}());


var AssignModule = (function () {

    var self = {};

    function Init() {
        getAuditor();
    }

    function getAuditor() {
        var auditor = new Auditor();
        var element = $("#auditors-select-piker");
        var url = element.data("url");
        var option = "";

        auditor.getAuditor(url).done(function (response) {
            showColorLoading();
            $(response.auditors).each(function (key, value) {
                option += "<option value='" + value.UserCode + "'>" + value.Name + "</option>";
            });

            element.html(option);
            element.selectpicker('render');
            element.selectpicker('refresh')
            hideColorLoading();
        });
    }

    function calculatePenSumaryTotal() {
        var total = 0;
        $(".pn-assig-acc").each(function (key, value) {
            total += Number($(value).text())
        });

        $("#total-pending").text(total);
    }

    function assign() {

        $(document).on("click", ".asignar-pendiente", function () {

            var from = $($("#form-assign-auditory")[0]);
            var date = $(this).parents("tr").find(".sales-date").text();
            var auditor = $($(".selectpicker")[0]).selectpicker('val');

            if ((date != "" && date != null) && (auditor != "" && auditor != null)) {
                showColorLoading();
                from.find("input[name='date']").val(date);
                from.find("input[name='auditor']").val(auditor.join(","));
                from.submit();
            }
            else {
                alertNoty("Debe proveer la fecha y, el o los auditores.", "informacion", "success");
            }

            hideColorLoading();
        });
    }

    return {
        Init: Init,
        getAuditor: getAuditor,
        calculatePenSumaryTotal: calculatePenSumaryTotal,
        assign: assign
    };

}());


var SubscriberModule = (function () {

    var self = {};
    var templates = {};

    function Init() {
        templates.claim = Handlebars.compile($("#subscriber-claim").html());
        templates.credit = Handlebars.compile($("#subscriber-credit").html());
    }

    function getClaim() {

        $("#pending-table").on('click', '.show-claim', function () {

            var subscriber = new Subscriber();
            var tr = $(this).parents("tr");
            var data = {
                SubscrId: tr.attr("data-subscriber"),
                CanvEdition: tr.attr("data-canv-edition"),
                CanvCode: tr.attr("data-canv-code"),
                Book: tr.attr("data-book-code")
            };
            
            subscriber.getClaim($(this).attr("data-url"), data).done(function (response) {
                showColorLoading();
                complileTemplate('claim', response, $("#claim-table tbody"));
                hideColorLoading();
            });

            $('#claim-modal').modal('show');
        });
    }

    function getCredit() {

        $("#pending-table").on('click', '.show-credit', function () {

            var subscriber = new Subscriber();
            var tr = $(this).parents("tr");
            var data = {
                SubscrId: tr.attr("data-subscriber"),
                CanvEdition: tr.attr("data-canv-edition"),
                CanvCode: tr.attr("data-canv-code"),
                Book: processBooks(tr.attr("data-book-code"))
            };
            
            subscriber.getClaim($(this).attr("data-url"), data).done(function (response) {
                showColorLoading();
                complileTemplate('credit', response , $("#credit-table tbody"));
                hideColorLoading();                
            });

            $('#credit-modal').modal('show');
        });
    }

    function complileTemplate(templt, data, container) {
        var output = templates[templt](data);
        container.html(output);
    }

    function processBooks(books) {
    
        var bookArray = books.split(",");
        var bookCode = [];
        var bookName = "";

        for (var i = 0; i < bookArray.length - 1; i++) {
            
            bookName = bookArray[i].split("=")[0]

            if (bookCode.indexOf(bookName) < 0) {
                bookCode.push(bookName);
            }
        }
        
        return bookCode.join(",");
    }

    return {
        Init: Init,
        getClaim: getClaim,
        getCredit: getCredit
    };

}());
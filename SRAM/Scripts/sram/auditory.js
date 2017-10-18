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

    $('.picker').datepicker({ autoclose: true });

    AuditoryModule.Init();

});

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
        });
    }

    return {
        Init: Init,
    };

}());

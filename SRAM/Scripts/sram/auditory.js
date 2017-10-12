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

    AuditoryModule.Init();

});

var AuditoryModule = (function () {

    function Init() {

        audit();
    }

    function audit() {

        $('#pending-table').on('click', '.audit-command-button', function () {
            $("#audit-modal").modal('show');
        });
    }

    return {
        Init: Init,
        audit: audit
    };

}());


function Auditory() {



}
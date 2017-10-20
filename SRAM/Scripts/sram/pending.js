$(function () {
    
    $('.picker').datepicker({ autoclose: true });

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

    SubscriberModule.Init();
    PendingModule.Init();
    
    SubscriberModule.getClaim();
    SubscriberModule.getCredit();
});
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

    $('#audit-foo-filter-status').change(function (e) {
        e.preventDefault();
        pendingTable.trigger('footable_filter', { filter: $(this).val() });
    });

    $('.picker').datepicker({ autoclose: true });


    SubscriberModule.Init();

    AuditoryModule.Init();
    AuditoryModule.deleteAudit(function () { $("#form-search-done-auditories").submit(); });

    PendingModule.audit(function () { $("#form-search-done-auditories").submit(); });

    
    SubscriberModule.getClaim();
    SubscriberModule.getCredit();
        
});

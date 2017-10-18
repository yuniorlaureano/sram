$(function () {        

    Assign.Init();
    Assign.calculatePenSumaryTotal();
    Assign.assign();

});

var Assign = (function () {

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

        $(document).on("click",".asignar-pendiente", function(){

            var from = $($("#form-assign-auditory")[0]);            
            var date = $(this).parents("tr").find(".sales-date").text();
            var auditor = $($(".selectpicker")[0]).selectpicker('val');
                        
            if ((date != "" && date != null) && (auditor != "" && auditor != null)) {
                showColorLoading();
                from.find("input[name='date']").val(date);
                from.find("input[name='auditor']").val(auditor.join(","));
                from.submit();
            }
            else
            {
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
function Auditory() {

    this.reassign = function (url, data) {
        return $.ajax({
            url: url,
            type: "POST",
            data: data,
            contentType: "application/json",
            dataType: "json"
        });
    };

    this.audit = function (url, data) {

        return $.ajax({
            type: 'POST',
            application: 'JSON',
            contentType: 'application/json charset-utf-8',
            data: JSON.stringify(data),
            url: url
        });
    };

    this.transactionData = function (url, data) {

        return $.ajax({
            type: 'POST',
            application: 'JSON',
            data: data,
            url: url
        });
    };

    this.getPeddingAssignMentSummary = function (url) {
        return $.get(url);
    };

    this.assign = function (url,data) {

        return $.ajax({
            type: 'POST',
            application: 'JSON',
            url: url,
            data: data
        });
    };
}

function Auditor() {

    this.getAuditor = function (url) {
        return $.get(url);
    };

    this.getPendingResume = function (url) {
        return $.get(url);
    };
}
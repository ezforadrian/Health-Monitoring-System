$(document).ready(function () {
    loadDTEcuList();
    loadBranchSelect();


    // functions for ECU Tab ---------------------------------------------------
    // Handle the modal showing
    $('#addDetailModal').on('show.bs.modal', function (event) {
        clearFormAndErrors();
        loadBranchSelect();
        updateFindingsSummary();
    });

    $('#addDetailModal').on('hidden.bs.modal', function () {
        clearFormAndErrors();
    });


    // Event listener for checkbox change
    $(document).on('change', '.test-detail-checkbox', function () {
        updateFindingsSummary();
    });

    $(document).on('change', '.test-detail-checkbox-edit', function () {
        updateFindingsSummaryEdit();
    });
    // --END functions for ECU Tab ---------------------------------------------------

});



function getQueryParameter(param) {
    var urlParams = new URLSearchParams(window.location.search);
    return urlParams.get(param);
}




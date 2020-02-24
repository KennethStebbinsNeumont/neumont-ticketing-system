$(document).ready(() => {
    // Toggle checkbox when the ticket entry is clicked
    $('.tableRow').click(function () {
        let chkbx = $(this).find('.checkbox');
        chkbx.prop('checked', !chkbx.prop('checked'));
    });

    // Prevent checkbox from toggling twice when it is
    // clicked directly
    $('.checkbox').click(function (event) {
        event.stopPropagation();
    });
});
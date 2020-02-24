$(document).ready(() => {
    // Toggle checkbox when the ticket entry is clicked
    $('.tableRow').click(function () {
        let chkbx = $(this).find('.checkbox');
        chkbx.prop('checked', !chkbx.prop('checked'));
    });

    // Preserve normal checkbox function
    $('.checkbox').click(function (event) {
        event.stopPropagation();
        this.checked = !this.checked;
    });
});
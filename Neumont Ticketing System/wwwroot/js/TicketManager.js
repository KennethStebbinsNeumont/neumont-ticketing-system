$(document).ready(() => {
    // Toggle checkbox when the ticket entry is clicked
    $('.tableRow').click(function () {
        let chkbx = $(this).find('.checkTicketSelected');
        chkbx.prop('checked', !chkbx.prop('checked'));
    });

    // Prevent checkbox from toggling twice when it is
    // clicked directly
    $('.checkTicketSelected').click(function (event) {
        event.stopPropagation();
    });

    // Ticket from toggling checkbox when a button is pressed
    $('.ticketBtnContainer').click((event) => event.stopPropagation());
});
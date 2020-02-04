$(document).ready(() => {
    const typesList = $('#typesList');

    const phoneNumberContainers = $('.phoneNumberContainer');

    phoneNumberContainers.each(function (index, element) {
        let inputs = $(element).find('.phoneNumberInput');
        inputs.each(function (index, element) {
            $(element).change(function () {
                console.log("Change detected!");
            });
            $(element).keypress(function () {
                console.log("Keypress!");
            });
        });
    });

    const emailAddressContainers = ('.emailAddressContainer');
});
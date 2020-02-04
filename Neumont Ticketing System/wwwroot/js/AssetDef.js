$(document).ready(() => {
    const typesList = $('#typesList');

    const phoneNumberContainers = $('.phoneNumberContainer');

    phoneNumberContainers.each(function (index, element) {
        let inputs = $(element).find('.phoneNumberInput');
        inputs.each(function (index, element) {
            $(input).change(function () {
                console.log("Heyyo!");
            });
        });
    });

    const emailAddressContainers = ('.emailAddressContainer');
});
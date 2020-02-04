$(document).ready(() => {
    const typesList = $('#typesList');

    const phoneNumberContainers = $('.phoneNumberContainer');

    const emailAddressContainers = ('.emailAddressContainer');

    for (ctr in phoneNumberContainers) {
        let inputs = ctr.find('.phoneNumberInput');
        for (input in inputs) {
            input.change(function () {
                console.log("Heyyo!");
            });
        }
    }
});
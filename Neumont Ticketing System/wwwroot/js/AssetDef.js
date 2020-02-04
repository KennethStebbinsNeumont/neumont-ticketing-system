$(document).ready(() => {
    const typesList = $('#typesList');

    const phoneNumberContainers = $('.phoneNumberContainer');

    const emailAddressContainers = ('.emailAddressContainer');

    for (let i = 0; i < phoneNumberContainers.length; i++) {
        let ctr = phoneNumberContainers[i];
        let inputs = ctr.find('.phoneNumberInput');
        for (let j = 0; j < inputs.length; j++) {
            let input = inputs[j];
            input.change(function () {
                console.log("Heyyo!");
            });
        }
    }
});
let phoneChange = function () {
    // This event is triggered when the user exits this field or
    // presses enter AND the value of this element has changed since
    // the last event call
    console.log("Pretzel");
    if (!$(input).val() &&
        $(this).parent().children().last().attr('id') !== $(input).attr('id')) {
        // If this field is now empty, remove it UNLESS it is the last
        // input box, which is supposed to be blank by default
        console.log("Doughnut");
        $(input).remove();
    }
};

let phoneKeypress = function () {
    // This event is triggered every time a key is pressed while this field
    // is in focus. This handler is only active for the last input in the list
    console.log("Banana");
    if ($(this).parent().children().last().attr('id') === $(this).attr('id') &&
        $(this).val()) {
        // If this element is the last element in the list and
        // it now has a value, create a new field
        console.log("Pineapple")

        let regexResults = $(this).attr('id').match(/mfr?(\d+)_PhoneNumbers_(\d+)/);
        let mfrIndex = parseInt(regexResults[1]);
        let thisIndex = parseInt(regexResults[2]);
        let newIndex = thisIndex + 1;

        let newInput = document.createElement('input');
        newInput.setAttribute('class', 'text-box single-line phoneNumberInput');
        newInput.setAttribute('id', `mfr${mfrIndex}_PhoneNumbers_${newIndex}`);
        newInput.setAttribute('name', `mfr${mfrIndex}.PhoneNumbers[${newIndex}]`);
        newInput.setAttribute('type', 'tel');

        $(newInput).keypress(phoneKeypress);
        $(newInput).change(phoneChange);

        $(this).parent().append(newInput);
    }
}

$(document).ready(() => {
    const typesList = $('#typesList');

    const phoneNumberContainers = $('.phoneNumberContainer');

    phoneNumberContainers.each(function (index, container) {
        let inputs = $(container).find('.phoneNumberInput');
        inputs.each(function (index, input) {
            $(input).change(phoneChange);
            $(input).keypress(phoneKeypress);
        });
    });

    const emailAddressContainers = ('.emailAddressContainer');
});
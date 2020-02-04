let expandableListOnKeypress = function () {
    // This event is triggered every time a key is pressed while this field
    // is in focus. This handler is only active for the last input in the list
    if ($(this).parent().children().last().attr('id') === $(this).attr('id')) {
        // If this element is the last element in the list and
        // it now has a value, create a new field

        let regexResults = $(this).attr('id').match(/mfr?(\d+)_(.*?)_(\d+)/);
        let mfrIndex = parseInt(regexResults[1]);
        let attributeName = regexResults[2];
        let thisIndex = parseInt(regexResults[3]);
        let newIndex = thisIndex + 1;

        let newInput = $(this).clone();
        newInput.val('');
        newInput.attr('id', `mfr${mfrIndex}_${attributeName}_${newIndex}`);
        newInput.attr('name', `mfr${mfrIndex}.${attributeName}[${newIndex}]`);

        $(newInput).change(phoneChange);
        $(newInput).blur(phoneChange);
        $(newInput).keypress(phoneKeypress);

        $(this).parent().append(newInput);
    }
};

let expandableListOnChange = function () {
    // This event is triggered when the user exits this field or
    // presses enter AND the value of this element has changed since
    // the last event call
    if (!$(this).val() &&
        $(this).parent().children().last().attr('id') !== $(this).attr('id')) {
        // If this field is now empty, remove it UNLESS it is the last
        // input box, which is supposed to be blank by default
        $(this).remove();
    }
};

$(document).ready(() => {
    const typesList = $('#typesList');

    const phoneNumberContainers = $('.phoneNumberContainer');

    phoneNumberContainers.each(function (index, container) {
        let inputs = $(container).find('.phoneNumberInput');
        inputs.each(function (index, input) {
            $(input).change(expandableListOnChange);
            $(input).blur(expandableListOnChange);
            $(input).keypress(expandableListOnKeypress);
        });
    });

    const emailAddressContainers = ('.emailAddressContainer');
});
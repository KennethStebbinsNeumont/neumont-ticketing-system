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

        $(newInput).change(expandableListOnChange);
        $(newInput).blur(expandableListOnChange);
        $(newInput).keypress(expandableListOnKeypress);

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

let btnAddListItemHandler = function () {
    console.log(`Handler called!!`)
    let btnContainer = $(this).parent();
    let list = btnContainer.parent();
    // Gets the first list item in the button's containing list to use as a template
    let template = list.children('.listItem').first();

    // Insert a clone of the template into the list, right before the button's container
    $(template.clone()).insertBefore(this);
}

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

    const emailAddressContainers = $('.emailAddressContainer');

    emailAddressContainers.each(function (index, container) {
        let inputs = $(container).find('.emailAddressInput');
        inputs.each(function (index, input) {
            $(input).change(expandableListOnChange);
            $(input).blur(expandableListOnChange);
            $(input).keypress(expandableListOnKeypress);
        });
    });

    const addListItemButtons = $('.btnAddListItem');

    addListItemButtons.each(function (index, btn) {
        console.log(`Adding handler to btn #${index}`);
        $(btn).click(btnAddListItemHandler);
    });
});
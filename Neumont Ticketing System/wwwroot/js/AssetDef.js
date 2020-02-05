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
    let btnContainer = $(this).parent();
    let list = btnContainer.parent();
    // Gets the first list item in the button's containing list to use as a template
    let template = list.children('.listItem').first();

    // Insert a clone of the template into the list, right before the button's container
    let clone = $(template.clone());
    // Clear the values of all input fields
    clone.find('input').each(function (i, e) {
        $(e).val('');
    });
    // https://stackoverflow.com/questions/1414276/how-to-make-the-first-option-of-select-selected-with-jquery
    clone.find('select').each(function (i, e) {
        let firstOption = $(e).find('option').first();
        $(e).val(firstOption.val());
    });
    $(clone).insertBefore(btnContainer);
}

$(document).ready(() => {
    const expandableListInputs = $('.expandableListInput');
    expandableListInputs.each(function (index, input) {
        $(input).change(expandableListOnChange);
        $(input).blur(expandableListOnChange);
        $(input).keypress(expandableListOnKeypress);
    });

    const typesList = $('#typesList');
    const typeNameInputs = typesList.find('.nameInput');
    let onTypeNameChange = function () {
        let newOptions = [];
        typesList.children().find('.nameInput').each(function (i, e) {
            let option = document.createElement('option');
            let typeName = $(e).val();
            let o = $(option);
            o.val(typeName);
            o.html(typeName);
            newOptions.push(o);
        });

        const typeSelectors = $('.typeSelector');
        typeSelectors.each(function (i, e) {
            const element = $(e);
            let oldValue = element.children('option:selected').val();
            // Clean out old options
            element.empty();
            for(option in newOptions)
                element.append($(option).clone());
            element.val(oldValue);
        });
    };
    typeNameInputs.each(function (i, e) {
        $(e).change(onTypeNameChange);
        $(e).blur(onTypeNameChange);
    });

    const mfrNameInputs = $('#manufacturersList').find('.nameInput');

    const addListItemButtons = $('.btnAddListItem');

    addListItemButtons.each(function (index, btn) {
        $(btn).click(btnAddListItemHandler);
    });
});
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

// Does some cloning work to clone the first item in the list and add it to the
// end, before the button. Afterwards, it calls the function in afterCloneAction, if it
// exists, with the new list item as the only parameter
let getBtnAddListItemHandler = function (afterCloneAction) {
    return function () {
        let btnContainer = $(this).parent();
        let list = btnContainer.parent();
        // Gets the first list item in the button's containing list to use as a template
        let template = list.children('.listItem').first();

        // Insert a clone of the template into the list, right before the button's container
        let clone = $(template.clone());
        // Clear the values of all input fields
        clone.find('input').each(function (i, e) {
            $(e).val('');
            $(e).attr('old-value', '');
        });
        // https://stackoverflow.com/questions/1414276/how-to-make-the-first-option-of-select-selected-with-jquery
        clone.find('select').each(function (i, e) {
            let firstOption = $(e).find('option').first();
            $(e).val(firstOption.val());
        });

        if (afterCloneAction)
            afterCloneAction(clone);

        $(clone).insertBefore(btnContainer);
    }
}

// inputsList is the list that contains (somewhere in its descendants) 
// one or more inputs of class nameInput that this method should
// search for
// getSelectorsToUpdate is a function that must resturn a
// jquery-ified selection of select elements that should have
// their values updated.
let getOnNameChange = function (inputsList, getSelectorsToUpdate) {
    return function () {
        const oldValue = $(this).attr('old-value');
        const newValue = $(this).val();
        let newOptions = [];
        inputsList.children().find('.nameInput').each(function (i, e) {
            let option = document.createElement('option');
            let typeName = $(e).val();
            let o = $(option);
            o.val(typeName);
            o.html(typeName);
            newOptions.push(o);
        });

        const selectors = getSelectorsToUpdate();
        selectors.each(function (i, e) {
            const element = $(e);
            let selection = element.children('option:selected').val();
            // If the thing's name that was just updated is the one selected,
            // update the selection to the thing's new name
            if (selection === oldValue)
                selection = newValue;
            // Clean out old options
            element.empty();
            newOptions.forEach((option) => {
                element.append($(option).clone());
            });
            element.val(selection);
        });
        $(this).attr('old-value', newValue);
    };
}

const jsonifyInputs = function () {
    let result = {
        types: [],
        manufacturers: [],
        models: []
    };

    $('.typeContainer').each(function (i, e) {
        let ele = $(e);
        let nameInput = ele.find('.nameInput');
        let descriptionInput = ele.find('.descriptionInput');
        if (nameInput.val()) {
            // Skip inputs with empty name fields
            result.types.push({
                "Name": nameInput.val(),
                "OriginalName": nameInput.attr('original-value'),
                "Description": descriptionInput.val()
            });
        }
    });

    $('.manufacturerContainer').each(function (i, e) {
        let ele = $(e);
        let nameInput = ele.find('.nameInput');

        if (nameInput.val()) {
            let phoneInputs = ele.find('.phoneNumberInput');
            let emailInputs = ele.find('.emailAddressInput');

            let phoneNumbers = [];
            phoneInputs.each(function (j, input) {
                if ($(input).val())
                    phoneNumbers.push($(input).val());
            });
            let emailAddresses = [];
            emailInputs.each(function (j, input) {
                if ($(input).val())
                    emailAddresses.push($(input).val());
            });

            result.manufacturers.push({
                "Name": nameInput.val(),
                "OriginalName": nameInput.attr('original-value'),
                "EmailAddresses": emailAddresses,
                "PhoneNumbers": phoneNumbers
            });
        }
    });

    $('.modelContainer').each(function (i, e) {
        let ele = $(e);
        let nameInput = ele.find('.nameInput');
        let typeSelector = ele.find('.typeSelector');
        let mfrSelector = ele.find('.manufacturerSelector');

        if (nameInput.val() && typeSelector.val() &&  mfrSelector.val()) {
            let modelNumberInput = ele.find('.modelNumberInput');

            result.models.push({
                "Name": nameInput.val(),
                "OriginalName":nameInput.attr('original-value'),
                "ModelNumber": modelNumberInput.val(),
                "TypeName": typeSelector.val(),
                "ManufacturerName": mfrSelector.val()
            });
        }
    });

    return result;
};

$(document).ready(() => {
    const expandableListInputs = $('.expandableListInput');
    expandableListInputs.each(function (index, input) {
        $(input).change(expandableListOnChange);
        $(input).blur(expandableListOnChange);
        $(input).keypress(expandableListOnKeypress);
    });

    const typesList = $('#typesList');
    const typeNameInputs = typesList.find('.nameInput');
    const mfrsList = $('#manufacturersList');
    const mfrsNameInputs = mfrsList.find('.nameInput');
    const modelsList = $('#modelsList');
    
    typeNameInputs.each(function (i, e) {
        $(e).change(getOnNameChange(typesList, () => $('.typeSelector')));
        $(e).blur(getOnNameChange(typesList, () => $('.typeSelector')));
    });
    mfrsNameInputs.each(function (i, e) {
        $(e).change(getOnNameChange(mfrsList, () => $('.manufacturerSelector')));
        $(e).blur(getOnNameChange(mfrsList, () => $('.manufacturerSelector')));
    });

    typesList.find('.btnAddListItem').click(getBtnAddListItemHandler(function (clone) {
        let nameInput = $(clone).find('.nameInput');
        nameInput.change(getOnNameChange(typesList, () => $('.typeSelector')));
        nameInput.blur(getOnNameChange(typesList, () => $('.typeSelector')));
    }));
    mfrsList.find('.btnAddListItem').click(getBtnAddListItemHandler(function (clone) {
        let nameInput = $(clone).find('.nameInput');
        nameInput.change(getOnNameChange(mfrsList, () => $('.manufacturerSelector')));
        nameInput.blur(getOnNameChange(mfrsList, () => $('.manufacturerSelector')));

        let phoneInput = $(clone).find('.phoneNumberInput');
        // Remove excessive input fields
        if (phoneInput.length > 1) {
            phoneInput.each(function (i, e) {
                if (i > 0)
                    $(e).remove();
            });
            phoneInput = $(clone).find('.phoneNumberInput');
        }
        phoneInput.change(expandableListOnChange);
        phoneInput.blur(expandableListOnChange);
        phoneInput.keypress(expandableListOnKeypress);

        let emailInput = $(clone).find('.emailAddressInput');
        // Remove excessive input fields
        if (emailInput.length > 1) {
            emailInput.each(function (i, e) {
                if (i > 0)
                    $(e).remove();
            });
            emailInput = $(clone).find('.emailAddressInput');
        }
        emailInput.change(expandableListOnChange);
        emailInput.blur(expandableListOnChange);
        emailInput.keypress(expandableListOnKeypress);
    }));
    modelsList.find('.btnAddListItem').click(getBtnAddListItemHandler(function (clone) { }));

    // https://www.c-sharpcorner.com/blogs/post-the-data-to-asp-net-mvc-controller-using-jquery-ajax
    $('#submit').click(function () {
        $.ajax({
            type: "POST",
            url: "/Settings/AssetDef",
            data: JSON.stringify(jsonifyInputs()),
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                console.log("Success!");
            },
            failure: function (response) {
                console.log("Failure :(");
            },
            error: function (response) {
                console.log("ERROR!!!!");
            }
        });
    });
});
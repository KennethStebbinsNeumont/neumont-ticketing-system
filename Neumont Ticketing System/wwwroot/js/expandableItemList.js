﻿const ExpandableItemList = {
    // Does some cloning work to clone the first item in the list and add it to the
    // end, before the button. Afterwards, it calls the function in afterCloneAction, if it
    // exists, with the new list item as the only parameter
    getBtnAddListItemHandler: function (afterCloneAction) {
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
                $(e).removeAttr('original-value');
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
};
const ExpandableItemList = {
    // Does some cloning work to clone the first item in the list and add it to the
    // end, before the button. Afterwards, it calls the function in afterCloneAction, if it
    // exists, with the jquery-wrapped new list item as the only parameter.
    // If an insertAction is provided, it is called after everything with the jquery-wrapped
    // new list item. If no insertAction is provided, this method will insert the
    // new list item at the end of the list (or, if the button that triggered this action
    // is the last item in the list, the new list item will be inserted immediately
    // before it).
    getBtnAddListItemHandler: function (afterCloneAction, insertAction) {
        return function () {
            let btnContainer = $(this).parent();
            let list = btnContainer.parent();
            // Gets the first list item in the button's containing list to use as a template
            let template = list.children('.cloneableListItem').first();

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
                afterCloneAction($(clone));

            if (insertAction) {
                insertAction($(clone));
            } else {
                if (list.children().last() === btnContainer)
                    $(clone).insertBefore(btnContainer);
                else
                    list.append($(clone));
            }
        }
    }
};
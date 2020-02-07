// inputsList is the list that contains (somewhere in its descendants) 
// one or more inputs of class nameInput that this method should
// search for
// getSelectorsToUpdate is a function that must resturn a
// jquery-ified selection of select elements that should have
// their values updated.
export function getOnNameChange(inputsList, getSelectorsToUpdate) {
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
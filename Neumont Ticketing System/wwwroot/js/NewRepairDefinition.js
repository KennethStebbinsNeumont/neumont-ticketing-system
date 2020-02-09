let subStepTemplate = $('#templateContainer').children().first();

let eilOnSelectorChange = function eilOnSelectorChange() {
    let selector = $(this);
    let newVal = selector.val();
    let parent = selector.parent();
    let selectorIndex = selector.index();
    let childrenLength = selector.children().length;
    if (newVal === '_none') {
        // If the user has chosen to delete this selector
        if (selectorIndex === childrenLength - 1) {
            // If this selector is the last one in the list
            selector.val('');
        } else {
            // If this selector isn't the last one in the list
            selector.remove();
        }
    } else if (newVal === '_all') {
        // If the user chose for this to apply to all types/mfrs/models
        let oldValues = [];
        // Get all of the current selections
        parent.find('.expandableListSelector').each(function (i, e) {
            let ele = $(e);
            let val = ele.val();
            if (val && val !== '_all' && !oldValues.includes(val)) {
                oldValues.push(val);
            }
        });

        // Put the _all selection at the top, preserve the other selections
        // (but set the elements to disabled), remove excess selectors
        parent.find('.expandableListSelector').each(function (i, e) {
            let ele = $(e);
            if (i === 0) {
                ele.val('_all');
                ele.attr('disabled', false);
            } else if (i < oldValues.length + 1) {
                ele.val(oldValues[i + 1]);
                ele.attr('disabled', true);
            } else {
                ele.remove();
            }
        });
    } else if (selectorIndex === childrenLength - 1) {
        // If this was a normal selection and 
        // this selector is the last one in the list
        let clone = selector.clone();
        clone.val('');
        parent.append(clone);
    }

    if (newVal !== '_all') {
        // Make sure that none of the elements are disabled, just in case
        // we're moving away from an _all selection
        parent.find('.expandableListSelector').each(function (i, e) {
            $(e).attr('disabled', false);
        });
    }
}
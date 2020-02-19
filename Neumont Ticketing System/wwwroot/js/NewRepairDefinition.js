let eslOnSelectorChange = function eslOnSelectorChange() {
    let selector = $(this);
    let newVal = selector.val();
    let parent = selector.parent();
    let selectorIndex = selector.index();
    let childrenLength = parent.children().length;
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
        // this selector is the last one in the list,
        // add a new empty selector
        let clone = selector.clone();
        clone.val('');
        clone.change(eslOnSelectorChange);
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

let eilOnNewSubStepClick = function eilOnNewSubStepClick() {
    let container = $(this).parent();

    let clone = $('#templateContainer').children('.subStepContainer').clone();

    clone.find('.btnAddSubStep').click(eilOnNewSubStepClick);

    clone.insertBefore(container);
}

let eilOnNewStepClick = function eilOnNewStepClick() {
    // Clone the first step in the repair step list
    let clone = $('#stepList').find('.stepContainer').first().clone();

    // Reset values to defaults
    clone.find('.stepName').val('');
    clone.find('.stepDescription').val('');
    clone.find('.subStepList').find('.subStepContainer').remove();

    // Add new sub-step handler to first level add sub-step button
    clone.find('.btnAddSubStep').click(eilOnNewSubStepClick);

    // Insert the new step right above the new step button
    clone.insertBefore($('#btnAddStep').parent());
}

let jsonifyInputs = function jsonifyInputs() {
    let repair = $('#repairList');
    let nameVal = repair.find('.nameInput').val();
    let descriptionVal = repair.find('.descriptionInput').val();
    let addFields = [];
    // Add all of the additional field inputs with values to the addFields array
    $('.addFieldInput').each(function (i, e) {
        if (e.value)
            addFields.push(e.value);
    });

    // Make sure that the name field isn't empty/null
    if (nameVal) {
        let appliesTo = {
            TypeNames: [],
            ManufacturerNames: [],
            ModelNames: []
        }
        let typeSelectors = repair.find('.typeSelector');
        if (typeSelectors.length >= 1 && typeSelectors.first().val() !== "_all") {
            // If the user has chosen for this repair to apply to all types, then leave the array blank
            // otherwise, add the selections to appliesTo.TypeNames
            typeSelectors.each(function (i, e) {
                if(e.value) // Don't add empty selections
                    appliesTo.TypeNames.push(e.value);
            });
        }
        let mfrSelectors = repair.find('.mfrSelector');
        if (mfrSelectors.length >= 1 && mfrSelectors.first().val() !== "_all") {
            // If the user has chosen for this repair to apply to all mfrs, then leave the array blank
            // otherwise, add the selections to appliesTo.TypeNames
            mfrSelectors.each(function (i, e) {
                if (e.value) // Don't add empty selections
                    appliesTo.ManufacturerNames.push(e.value);
            });
        }
        let modelSelectors = repair.find('.modelSelector');
        if (modelSelectors.length >= 1 && modelSelectors.first().val() !== "_all") {
            // If the user has chosen for this repair to apply to all models, then leave the array blank
            // otherwise, add the selections to appliesTo.TypeNames
            modelSelectors.each(function (i, e) {
                if (e.value) // Don't add empty selections
                    appliesTo.ModelNames.push(e.value);
            });
        }

        let getSteps = function (container) {
            let subSteps = [];
            let subStepCtrs = $(container).children('.subStepContainer');
            let stepNameVal = undefined;
            let inputContainers = undefined;
            for (let i = 0; i < subStepCtrs.length; i++) {
                inputContainers = $(subStepCtrs[i]).children('.inputContainer');
                stepNameVal = inputContainers.children('.subStepName').val();
                if (stepNameVal) {
                    subSteps.push({
                        Name: stepNameVal,
                        Description: inputContainers.children('.subStepDescription').val(),
                        SubSteps: getSteps(subStepCtrs[i])
                    });
                }
            }


            return subSteps;
        }
        let stepContainers = $('#stepList').children('.stepContainer');
        let steps = [];
        let stepNameVal = undefined;
        let inputContainers = undefined;
        for (let i = 0; i < stepContainers.length; i++) {
            inputContainers = $(stepContainers[i]).children('.inputContainer');
            stepNameVal = inputContainers.children('.stepName').val();
            if (stepNameVal) {
                steps.push({
                    Name: stepNameVal,
                    Description: inputContainers.children('.stepDescription').val(),
                    SubSteps: getSteps($(stepContainers[i]).children('.subStepList'))
                });
            }
        }


        return {
            Name: nameVal,
            Description: descriptionVal,
            AppliesTo: appliesTo,
            AdditionalFields: addFields,
            Steps: steps
        }
    }
}

$(document).ready(function () {
    // Assign the above handler to all selectors
    $('select').change(eslOnSelectorChange);

    // Assign the above handler to all add sub-step buttons
    $('.btnAddSubStep').click(eilOnNewSubStepClick);

    // Apply the eilOnNewStepClick handler to the add step button's click event
    $('#btnAddStep').click(eilOnNewStepClick);

    $('#submit').click(function () {
        // Send the repair definition to the back end
        $.ajax({
            type: "POST",
            url: "/Settings/NewRepairDefinition",
            data: JSON.stringify(jsonifyInputs()),
            contentType: "application/json",
            dataType: "json"
        }).then(json => {
            if (json.successful) {
                console.log("Success!");
            } else {
                console.error(`Query failed: ${json.message}`);
            }
        }).catch(() => {
            console.error("Unexpected error while submitting new repair.");
        });
    });

    const expandableListInputs = $('.expandableListInput');
    expandableListInputs.each(function (index, input) {
        $(input).change(ExpandableInputList.onInputChange);
        $(input).blur(ExpandableInputList.onInputBlur);
        $(input).keypress(ExpandableInputList.onInputKeypress);
    });
});
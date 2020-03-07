(function () {
    const eslOnSelectorChange = function eslOnSelectorChange() {
        let selector = $(this);
        let newVal = selector.val();
        let oldVal = selector.attr('old-value');
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
        }
        else if (newVal === '_all') {
            // If the user chose for this to apply to all types/mfrs/models
            // Put the _all selection at the top, preserve the other selections
            // (but set the elements to disabled), remove excess selectors
            parent.find('.expandableListSelector').each(function (i, e) {
                let ele = $(e);
                if (i === 0) {
                    ele.val('_all');
                    ele.attr('disabled', false);
                    ele.attr('old-value', '_all');
                } else {
                    ele.remove();
                }
            });


            // Re-enable all options in all selectors
            parent.find('option').each(function (i, e) {
                e.disabled = false;
            });
        }
        else if (selectorIndex === childrenLength - 1) {
            // If this was a normal selection and 
            // this selector is the last one in the list,
            // add a new empty selector
            let clone = selector.clone();
            clone.val('');
            clone.removeAttr('old-value');
            clone.change(eslOnSelectorChange);
            if (clone.hasClass('typeSelector') || clone.hasClass('mfrSelector')) {
                clone.change(onTypeOrMfrSelection);
            }

            parent.append(clone);
        }

        if (newVal !== '_all') {
            // Make sure that none of the elements are disabled, just in case
            // we're moving away from an _all selection
            parent.find('.expandableListSelector').each(function (i, e) {
                $(e).attr('disabled', false);
            });
        }

        if (newVal !== '_all' && newVal !== '_none') {
            // Now, disable the triggering selector's new selection option in
            // every other selector
            let child;
            parent.find('select').each(function (i, e) {
                if (e.value !== newVal) {
                    // Skip the selector that now has this value
                    for (let j = 0; j < e.children.length; j++) {
                        child = e.children[j];
                        if (child.value === newVal) {
                            // Disable the option with the same value as the new selection
                            child.disabled = true;
                        }
                    }
                }
            });
        }

        if (oldVal && oldVal !== '_all' && oldVal != '_none') {
            // If we're changing away from a normal selecton, re-enable
            // the old selection in all similar selectors
            parent.find('option').each(function (i, e) {
                if (e.value === oldVal) {
                    e.disabled = false;
                }
            });
        }

        selector.attr('old-value', newVal);
    };

    const onTypeOrMfrSelection = async function onTypeOrMfrSelection() {
        let typeSelectors = $('.typeSelector');
        let mfrSelectors = $('.mfrSelector');

        let request = {
            TypeNames: [],
            ManufacturerNames: []
        };

        if (typeSelectors.length >= 1 && typeSelectors.first().val() !== "_all") {
            // If the user has chosen for this repair to apply to all types, then leave the array blank
            // otherwise, add the selections to appliesTo.TypeNames
            typeSelectors.each(function (i, e) {
                if (e.value) // Don't add empty selections
                    request.TypeNames.push(e.value);
            });
        }

        if (mfrSelectors.length >= 1 && mfrSelectors.first().val() !== "_all") {
            // If the user has chosen for this repair to apply to all mfrs, then leave the array blank
            // otherwise, add the selections to appliesTo.TypeNames
            mfrSelectors.each(function (i, e) {
                if (e.value) // Don't add empty selections
                    request.ManufacturerNames.push(e.value);
            });
        }

        try {
            let response = await $.ajax({
                type: "POST",
                url: "/Assets/GetEncompassedModels",
                data: JSON.stringify(request),
                contentType: "application/json",
                dataType: "json"
            });

            if (response.successful) {
                let modelSelectors = $('.modelSelector');
                let encompassedModels = response.models;

                let newModelOptions = [];
                let option;

                option = document.createElement('option');
                option.value = "_none";
                option.text = "Clear selection"
                newModelOptions.push(option);

                option = document.createElement('option');
                option.value = "_all";
                option.text = "All Models";
                newModelOptions.push(option);

                let model;
                for (let i = 0; i < encompassedModels.length; i++) {
                    model = encompassedModels[i];

                    option = document.createElement('option');

                    option.value = model.normalizedName;
                    option.text = model.name;

                    newModelOptions.push(option);
                }

                let clonedOptions;
                let oldVal, newVal;
                let selectedModels = [];
                modelSelectors.each(function (i, e) {
                    oldVal = e.value;

                    let ele = $(e);
                    ele.empty();

                    clonedOptions = [];
                    newVal = null;

                    for (let j = 0; j < newModelOptions.length; j++) {
                        clonedOptions.push(newModelOptions[j].cloneNode(true));
                        if (newModelOptions[j].value === oldVal) {
                            // If this selector's old value is still in the list of valid options, select it
                            newVal = oldVal;
                            if (newModelOptions[j].value !== "_all") {
                                // If the selection isn't _all, add it to the list of selected models
                                selectedModels.push(oldVal);
                            }
                        }
                    }

                    ele.append(clonedOptions);
                    ele.val(newVal);
                });

                modelSelectors.each(function (i, e) {
                    if (i > 0) {
                        // Skip evaluating the first selector
                        if (!e.value) {
                            // If this selector no longer has a selection, remove it
                            e.remove();
                        }
                    }

                    $(e).find('option').each(function (ix, el) {
                        // Disable options that have been selected elsewhere
                        if (el.value != e.value && selectedModels.includes(el.value)) {
                            // If the current option isn't the one selected BUT has been
                            // selected somewhere else, disable it
                            el.disabled = true;
                        }
                    });
                });

                modelSelectors = $('.modelSelector');
                let firstSelector = modelSelectors[0];
                if (!firstSelector.value) {
                    if (modelSelectors.length > 1) {
                        firstSelector.remove();
                    } else {
                        firstSelector.value = "_all";
                    }
                }

                modelSelectors = $('.modelSelector');
                let clone;
                if (firstSelector.value && firstSelector.value !== "_all") {
                    // If the first element has a value (that isn't _all), that means we don't
                    // have a blank selector for the user to use. Clone the
                    // first selector and add it to the DOM
                    clone = firstSelector.cloneNode(true);
                    clone.value = null;
                    clone.onchange = eslOnSelectorChange;

                    firstSelector.parentElement.append(clone);
                }

                // Disable options that have been selected elsewhere in the first selector
                $(firstSelector).find('option').each(function (i, e) {
                    if (e.value !== firstSelector.value && selectedModels.includes(e.value)) {
                        // If the current option isn't the one selected BUT has been
                        // selected somewhere else, disable it
                        e.disabled = true;
                    }
                });

            } else {
                console.error(`Model query failed: "${response.message}"`);
            }
        } catch (e) {
            console.error("Unexpected error while waiting for a response for the model query.");
            console.error(e);
        }
    };

    const eilOnNewSubStepClick = function eilOnNewSubStepClick() {
        let container = $(this).parent();

        let clone = $('#templateContainer').children('.subStepContainer').clone();

        clone.find('.btnAddSubStep').click(eilOnNewSubStepClick);

        clone.insertBefore(container);
    };

    const eilOnNewStepClick = function eilOnNewStepClick() {
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
    };

    const jsonifyInputs = function jsonifyInputs() {
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
                    if (e.value) // Don't add empty selections
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
            };
        }
    };

    const submitRepairDefinition = async function submitRepairDefinition() {
        // Send the repair definition to the back end ONLY IF the name has been set
        if ($('.nameInput').val()) {
            try {
                let response = await $.ajax({
                    type: "POST",
                    url: "/Settings/NewRepairDefinition",
                    data: JSON.stringify(jsonifyInputs()),
                    contentType: "application/json",
                    dataType: "json"
                });

                if (response.successful) {
                    // Redirect to settings index page on submit
                    window.location.href = "/Settings";
                } else {
                    console.error(`Unexpected internal server error: ${response.message}`);
                }
            } catch (e) {
                console.error("Unexpected error while submitting definitions.");
                console.error(e);
            }
        }
    };

    $(document).ready(function () {
        // Assign the above handler to all selectors
        $('select').change(eslOnSelectorChange);
        $('.typeSelector, .mfrSelector').change(onTypeOrMfrSelection);

        // Assign the above handler to all add sub-step buttons
        $('.btnAddSubStep').click(eilOnNewSubStepClick);

        // Apply the eilOnNewStepClick handler to the add step button's click event
        $('#btnAddStep').click(eilOnNewStepClick);

        $('#btnSubmit').click(submitRepairDefinition);

        const expandableListInputs = $('.expandableListInput');
        expandableListInputs.each(function (index, input) {
            $(input).change(ExpandableInputList.onInputChange);
            $(input).blur(ExpandableInputList.onInputBlur);
            $(input).keypress(ExpandableInputList.onInputKeypress);
        });
    });
    // This is called closure. It ensures that any local variables/constants/functions
    // can't be seen by any other scripts, for simplicity and cleanliness
})();
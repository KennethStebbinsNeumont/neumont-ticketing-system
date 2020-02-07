$(document).ready(() => {
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

            if (nameInput.val() && typeSelector.val() && mfrSelector.val()) {
                let modelNumberInput = ele.find('.modelNumberInput');

                result.models.push({
                    "Name": nameInput.val(),
                    "OriginalName": nameInput.attr('original-value'),
                    "ModelNumber": modelNumberInput.val(),
                    "TypeName": typeSelector.val(),
                    "ManufacturerName": mfrSelector.val()
                });
            }
        });

        return result;
    };

    const expandableListInputs = $('.expandableListInput');
    expandableListInputs.each(function (index, input) {
        $(input).change(ExpandableInputList.onInputChange);
        $(input).blur(ExpandableInputList.onInputBlur);
        $(input).keypress(ExpandableInputList.onInputKeypress);
    });

    const typesList = $('#typesList');
    const typeNameInputs = typesList.find('.nameInput');
    const mfrsList = $('#manufacturersList');
    const mfrsNameInputs = mfrsList.find('.nameInput');
    const modelsList = $('#modelsList');
    
    typeNameInputs.each(function (i, e) {
        $(e).change(getUpdateSelectorsOnNameInputChange(typesList, () => $('.typeSelector')));
        $(e).blur(getUpdateSelectorsOnNameInputChange(typesList, () => $('.typeSelector')));
    });
    mfrsNameInputs.each(function (i, e) {
        $(e).change(getUpdateSelectorsOnNameInputChange(mfrsList, () => $('.manufacturerSelector')));
        $(e).blur(getUpdateSelectorsOnNameInputChange(mfrsList, () => $('.manufacturerSelector')));
    });

    typesList.find('.btnAddListItem').click(ExpandableItemList.getBtnAddListItemHandler(function (clone) {
        let nameInput = $(clone).find('.nameInput');
        nameInput.change(getUpdateSelectorsOnNameInputChange(typesList, () => $('.typeSelector')));
        nameInput.blur(getUpdateSelectorsOnNameInputChange(typesList, () => $('.typeSelector')));
    }));
    mfrsList.find('.btnAddListItem').click(ExpandableItemList.getBtnAddListItemHandler(function (clone) {
        let nameInput = $(clone).find('.nameInput');
        nameInput.change(getUpdateSelectorsOnNameInputChange(mfrsList, () => $('.manufacturerSelector')));
        nameInput.blur(getUpdateSelectorsOnNameInputChange(mfrsList, () => $('.manufacturerSelector')));

        let phoneInput = $(clone).find('.phoneNumberInput');
        // Remove excessive input fields
        if (phoneInput.length > 1) {
            phoneInput.each(function (i, e) {
                if (i > 0)
                    $(e).remove();
            });
            phoneInput = $(clone).find('.phoneNumberInput');
        }
        phoneInput.change(ExpandableInputList.onInputChange);
        phoneInput.blur(ExpandableInputList.onInputBlur);
        phoneInput.keypress(ExpandableInputList.onInputKeypress);

        let emailInput = $(clone).find('.emailAddressInput');
        // Remove excessive input fields
        if (emailInput.length > 1) {
            emailInput.each(function (i, e) {
                if (i > 0)
                    $(e).remove();
            });
            emailInput = $(clone).find('.emailAddressInput');
        }
        emailInput.change(ExpandableInputList.onInputChange);
        emailInput.blur(ExpandableInputList.onInputBlur);
        emailInput.keypress(ExpandableInputList.onInputKeypress);
    }));
    modelsList.find('.btnAddListItem').click(ExpandableItemList.getBtnAddListItemHandler());

    // https://www.c-sharpcorner.com/blogs/post-the-data-to-asp-net-mvc-controller-using-jquery-ajax
    $('#submit').click(function () {
        $.ajax({
            type: "POST",
            url: "/Settings/AssetDefinitions",
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
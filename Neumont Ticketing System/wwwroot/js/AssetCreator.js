$(document).ready(() => {
    const jsonfiyInputs = function jsonifyInputs() {
        let result = {
            owners: []
        };

        $('.ownerContainer').each(function (i, e) {
            let ele = $(e);
            let nameInput = ele.find('.nameInput');
            // Ignore owners without names
            if (nameInput.val()) {
                let preferredNameInput = ele.find('.preferredNameInput');
                let emailInputs = ele.find('.emailAddressInput');
                let phoneInputs = ele.find('.phoneNumberInput');

                let emailAddresses = [];
                emailInputs.each(function (j, input) {
                    if ($(input).val())
                        emailAddresses.push($(input).val());
                });
                let phoneNumbers = [];
                phoneInputs.each(function (j, input) {
                    if ($(input).val())
                        phoneNumbers.push($(input).val());
                });

                let assetContainers = ele.find('.assetContainer');

                let assets = [];
                assetContainers.each(function (j, c) {
                    let con = $(c);
                    let serialInput = con.find('.serialNumberInput');
                    // Skip assets without serial numbers
                    if (serialInput.val()) {
                        let modelSelector = con.find('.modelSelector');

                        assets.push({
                            "SerialNumber": serialInput.val(),
                            "ModelName": modelSelector.val()
                        });
                    }
                });

                result.owners.push({
                    "Name": nameInput.val(),
                    "PreferredName": preferredNameInput.val(),
                    "EmailAddresses": emailAddresses,
                    "PhoneNumbers": phoneNumbers,
                    "Assets": assets
                });
            }
        });

        return result;
    };

    $('.expandableListInput').each(function (i, e) {
        let ele = $(e);
        ele.change(ExpandableInputList.onInputChange);
        ele.blur(ExpandableInputList.onInputBlur);
        ele.keypress(ExpandableInputList.onInputKeypress);
    });

    $('.btnAddAsset').each(function (i, e) {
        let ele = $(e);
        ele.click(ExpandableItemList.getBtnAddListItemHandler(function (c) {
            // Choose the first model in the list
            c.find('.modelSelector').val(c.find('option').first().val());
        }));
    });

    let ownerList = $('#ownerList');
    ownerList.find('.btnNewOwner').click(ExpandableItemList.getBtnAddListItemHandler(
        function (clone) {
            let phoneInput = clone.find('.phoneNumberInput');
            // Remove excessive input fields
            if (phoneInput.length > 1) {
                phoneInput.each(function (i, e) {
                    if (i > 0)
                        $(e).remove();
                });
                phoneInput = clone.find('.phoneNumberInput');
            }
            phoneInput.change(ExpandableInputList.onInputChange);
            phoneInput.blur(ExpandableInputList.onInputBlur);
            phoneInput.keypress(ExpandableInputList.onInputKeypress);

            let emailInput = clone.find('.emailAddressInput');
            // Remove excessive input fields
            if (emailInput.length > 1) {
                emailInput.each(function (i, e) {
                    if (i > 0)
                        $(e).remove();
                });
                emailInput = clone.find('.emailAddressInput');
            }
            emailInput.change(ExpandableInputList.onInputChange);
            emailInput.blur(ExpandableInputList.onInputBlur);
            emailInput.keypress(ExpandableInputList.onInputKeypress);

            clone.find('.btnAddAsset').click(ExpandableItemList.getBtnAddListItemHandler(function (c) {
                // Choose the first model in the list
                c.find('.modelSelector').val(c.find('option').first().val());
            }));
    }, function (clone) {
            clone.insertAfter(ownerList.find('.btnNewOwnerContainer'));
    }));

    // https://www.c-sharpcorner.com/blogs/post-the-data-to-asp-net-mvc-controller-using-jquery-ajax
    $('#submit').click(function () {
        $.ajax({
            type: "POST",
            url: "/Settings/AssetManager",
            data: JSON.stringify(jsonfiyInputs()),
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
        })
    });
});
﻿(function () {
    const ownerIdRegex = /ownerId=(?<ownerId>.*?)(?:&|$)/;

    const jsonfiyInputs = function jsonifyInputs() {
        let ownerElement = $('.ownerContainer');
        let nameInput = ownerElement.find('.nameInput');
        // Print an error if the name is empty
        if (!nameInput.val()) {
            console.error("Owner's name cannot be empty.");
        } else {
            let emailInputs = ownerElement.find('.emailAddressInput');
            let phoneInputs = ownerElement.find('.phoneNumberInput');

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

            let assetContainers = ownerElement.find('.assetContainer');

            let assets = [];
            assetContainers.each(function (j, c) {
                let con = $(c);
                let serialInput = con.find('.serialNumberInput');
                // Skip assets without serial numbers
                if (serialInput.val()) {
                    let modelSelector = con.find('.modelSelector');

                    assets.push({
                        "Id": con.attr('asset-id'),
                        "SerialNumber": serialInput.val(),
                        "ModelName": modelSelector.val()
                    });
                }
            });

            return {
                "Id": location.href.match(ownerIdRegex).groups.ownerId,
                "Name": nameInput.val(),
                "PreferredName": {
                    "First": ownerElement.find('.preferredFNameInput').val(),
                    "Middle": ownerElement.find('.preferredMNameInput').val(),
                    "Last": ownerElement.find('.preferredLNameInput').val()
                },
                "EmailAddresses": emailAddresses,
                "PhoneNumbers": phoneNumbers,
                "Assets": assets
            };
        }
    };

    const updateOwner = async function updateOwner() {
        try {
            let response = await $.ajax({
                type: "POST",
                url: "/Settings/AssetEditor",
                data: JSON.stringify(jsonfiyInputs()),
                contentType: "application/json",
                dataType: "json"
            });

            if (response.successful) {
                // Redirect to asset manager page on success
                window.location.href = "/Settings/AssetManager";
            } else {
                console.error(`Unexpected internal server error: ${response.message}`);
            }
        } catch (e) {
            console.error("Unexpected error while submitting definitions.");
            console.error(e);
        }
    }

    $(document).ready(() => {

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

        // https://www.c-sharpcorner.com/blogs/post-the-data-to-asp-net-mvc-controller-using-jquery-ajax
        $('#btnSubmit').click(updateOwner);
    });
})();
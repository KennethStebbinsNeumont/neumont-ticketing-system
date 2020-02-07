$(document).ready(() => {
    const jsonfiyInputs = function jsonifyInputs() {

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
    ownerList.find('#btnNewOwner').click(ExpandableItemList.getBtnAddListItemHandler(function (clone) {
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

        clone.find('.btnAddAsset').click(ExpandableItemList.getBtnAddListItemHandler(function (c) {
            // Choose the first model in the list
            c.find('.modelSelector').val(c.find('option').first().val());
        }));
    }));
});
(function () {

    let applicableRepairs = [];

    let templateInput = undefined;

    /*
     * return object schema
     * 
     * [{
     *     id: <string - asset's object id>,
     *     serialNumber: <string - asset's serial number>,
     *     modelName: <string - name of asset's model>
     * }, ...]
     */
    let getOwnersAssets = function getOwnersAssets(ownerId) {
        // TODO
    }

    /*
     * return object schema
     * 
     * [{
     *     id: <string - repair's id>,
     *     name: <string - repair's name>,
     *     description: <string - repair's description>,
     *     additionalFieldNames: [<string - additional field name>, ...]
     * }, ...]
     */
    let getApplicableRepairs = function getApplicableRepairs(assetId) {
        // TODO

        return applicableRepairs;
    }

    let onOwnerChosen = function onOwnerChosen(ownerId) {
        // Clear the asset stuff since no assets are chosen anymore
        onAssetClear();
        // Clear the old assets
        $('.assetSelector').empty();

        let assets = getOwnersAssets(ownerId);
        let assetElements = [];
        let ele = undefined;
        let asset = undefined;
        for (let i = 0; i < assets.length; i++) {
            asset = assets[i];
            ele = document.createElement('option');
            ele.value = asset.id;
            ele.innerHTML = `${asset.modelName} (${asset.serialNumber})`;

            assetElements.push(ele);
        }
        $('.assetSelector').append(assetElements);
        $('.assetSelector').prop('disabled', false);
    }

    let onAssetChosen = function onAssetChosen(assetId) {
        // Clear the repair stuff since no repairs are chosen anymore
        onRepairClear();
        // Clear old repairs
        $('.repairSelector').empty();

        let repairs = getApplicableRepairs(assetId);
        let repairElements = [];
        let ele = undefined;
        let repair = undefined;
        for (let i = 0; i < repairs.length; i++) {
            repair = repairs[i];
            ele = document.createElement('option');
            ele.value = repair.id;
            ele.innerHTML = repair.name;

            repairElements.push(ele);
        }
        $('.repairSelector').append(assetElements);
        $('.repairSelector').prop('disabled', false);
    }

    let onRepairChosen = function onRepairChosen(repairId) {
        // Clear old additional fields
        $('#additionalFields').empty();

        let additionalFields = undefined;
        let repair = undefined;
        for (let i = 0; i < applicableRepairs.length; i++) {
            repair = applicableRepairs[i];
            if (repair.id === repairId) {
                additionalFields = repair.additionalFields;
                break;
            }
        }

        let addFieldElements = [];
        let ele = undefined;
        let fieldName = undefined;
        for (let i = 0; i < additionalFields.length; i++) {
            fieldName = additionalFields[i];
            ele = templateInput.clone();
            ele.find('p').text(`${fieldName}: `);
            ele.find('input').addClass(`input${fieldName}`)

            addFieldElements.push(ele);
        }
        $('#additionalFields').append(addFieldElements);
    }

    let onOwnerClear = function onOwnerClear() {
        $('.assetSelector').empty();
        $('.assetSelector').prop('disabled', true);
        onAssetClear();
    }

    let onAssetClear = function onAssetClear() {
        $('.repairSelector').empty();
        $('.repairSelector').prop('disabled', true);
        applicableRepairs = [];
        onRepairClear();
    }

    let onRepairClear = function onRepairClear() {
        $('#additionalFields').empty();
    }

    let onOwnerInputEvent = function onOwnerInputEvent(event) {
        let input = $(event.target);
        if (event instanceof InputEvent) {
            // If the user just edited the text of the input
            input.removeAttr('ownerId');
            onOwnerClear();
        } else {
            // If the user just clicked on an autocomplete option
            let optionsList = $(event.target.list.options);
            let ownerId = undefined;
            optionsList.each(function (i, e) {
                if (e.value === input.val()) {
                    // We've found the matching selection
                    ownerId = $(e).attr('ownerId');
                    input.attr('ownerId', ownerId);
                    onOwnerChosen(ownerId);
                }
            });
        }
    };

    let onOwnerInputBlur = function onOwnerInputBlur(event) {
        let optionsList = $(event.target.list.options);
        if (optionsList.length == 1) {
            // If the user clicked away when there's only one option, choose it
            let input = $(event.target);
            let onlyOption = optionsList.first();
            let ownerId = onlyOption.attr('ownerId');
            input.val(onlyOption.val());
            input.attr('ownerId', ownerId);
            onOwnerChosen(ownerId);
        }
    }

    $(document).ready(function () {
        const expandableListInputs = $('.expandableListInput');
        expandableListInputs.each(function (index, input) {
            $(input).change(ExpandableInputList.onInputChange);
            $(input).blur(ExpandableInputList.onInputBlur);
            $(input).keypress(ExpandableInputList.onInputKeypress);
        });

        $('.btnAddListItem').click(ExpandableItemList.getBtnAddListItemHandler(
            (clone) => {
                clone.find('textarea').val('');
            }));

        templateInput = $('#templateContainer').children('.inputContainer');
    });
})();
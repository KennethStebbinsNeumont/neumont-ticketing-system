(function () {

    let applicableRepairs = [];

    let templateInput = undefined;

    // The amount of time to wait after the value of the owner
    // field has been changed before making a query in onOwnerInputEvent
    const QUERY_DELAY_MS = 250;
    
    const matchedOnOwnerNameString = "Name";
    const matchedOnOwnerPreferredNameString = "PreferredName";
    const matchedOnOwnerOwnerEmailString = "EmailAddress";

    /* 
     * Returns a then-able object, which, when resolved, produces an object
     * as outlined in "Return object schema" below.
     * 
     * Return object schema
     * 
     * {
     *     successful: <bool - whether the query was successful>,
     *     message: <string - message from the server>,
     *     query: <string - the query this response was crafted from>,
     *     owners: [
     *     {
     *         id: <string - owner's objectid>,
     *         name: <string - the owner's name>,
     *         primaryEmail: <string - the owner's primary email address>,
     *         matchedOn: <string - describes what attribute this owner matched on>,
     *         score: <int - this owner's match score; higher means a better match>
     *     }, ...]
     * }
     *  
     * 
     */
    let getOwners = function getOwners(query) {
        return $.ajax({
            type: "POST",
            url: "/Assets/GetOwners",
            data: JSON.stringify({
                Query: query,
                MaxNumOfResults: 10
            }),
            contentType: "application/json",
            dataType: "json"
        });
    };

    /*
     * Returns a then-able object, which, when resolved, produces an object
     * as outlined in "Return object schema" below.
     *
     * Return object schema
     * 
     * {
     *     successful: <bool - whether the query was successful>,
     *     message: <string - message from the server>,
     *     ownerId: <string - the ID of the owner to whom these assets belong>,
     *     assets: [
     *     {
     *         id: <string - asset's object id>,
     *         serialNumber: <string - asset's serial number>,
     *         modelName: <string - name of asset's model>
     *     }, ...]
     * }
     */
    let getOwnersAssets = function getOwnersAssets(ownerId) {
        return $.ajax({
            type: "POST",
            url: "/Assets/GetOwnedAssets",
            data: JSON.stringify({
                OwnerId: ownerId
            }),
            contentType: "application/json",
            dataType: "json"
        });
    };

    /*
     * Returns a then-able object, which, when resolved, produces an object
     * as outlined in "Return object schema" below.
     *
     * Return object schema
     * 
     * {
     *     successful: <bool - whether the query was successful>,
     *     message: <string - message from the server>,
     *     assetId: <string - the ID of the asset to which these repair apply>,
     *     repairs: [
     *     {
     *         id: <string - repair's id>,
     *         name: <string - repair's name>,
     *         description: <string - repair's description>,
     *         additionalFieldNames: [<string - additional field name>, ...]
     *     }, ...]
     * }
     * 
     */
    let getApplicableRepairs = function getApplicableRepairs(assetId) {
        return $.ajax({
            type: "POST",
            url: "/Assets/GetApplicableRepairs",
            data: JSON.stringify({
                AssetId: assetId
            }),
            contentType: "application/json",
            dataType: "json"
        }).then((json) => {
            applicableRepairs = json.repairs;
            return json;
        });
    };

    let onOwnerChosen = async function onOwnerChosen(ownerId) {
        // Clear the asset stuff since no assets are chosen anymore
        onAssetClear();
        // Clear the old assets
        $('.assetSelector').empty();

        let response = await getOwnersAssets(ownerId);
        if (response.successful) {
            let assetElements = [];
            let ele = undefined;
            let asset = undefined;
            for (let i = 0; i < response.assets.length; i++) {
                asset = response.assets[i];
                ele = document.createElement('option');
                ele.value = asset.id;
                ele.innerHTML = `${asset.modelName} (${asset.serialNumber})`;

                assetElements.push(ele);
            }
            $('.assetSelector').append(assetElements);
            $('.assetSelector').prop('disabled', false);

            // Appending will automatically choose the first option,
            // so we need to manually trigger the onAssetChosen handler
            // unless there are no assets available
            if (response.assets.length > 0)
                onAssetChosen(response.assets[0].id);
        } else {
            console.error(`getOwnersAssets query unsuccessful: ${response.message}`);
        }
    };

    let onAssetChosen = async function onAssetChosen(assetId) {
        // Clear the repair stuff since no repairs are chosen anymore
        onRepairClear();
        // Clear old repairs
        $('.repairSelector').empty();

        let response = await getApplicableRepairs(assetId);
        if (response.successful) {
            let repairElements = [];
            let ele = undefined;
            let repair = undefined;
            for (let i = 0; i < response.repairs.length; i++) {
                repair = response.repairs[i];
                ele = document.createElement('option');
                ele.value = repair.id;
                ele.innerHTML = repair.name;

                repairElements.push(ele);
            }
            $('.repairSelector').append(repairElements);
            $('.repairSelector').prop('disabled', false);

            // Appending will automatically choose the first option,
            // so we need to manually trigger the onRepairChosen handler
            // unless there are no repairs available
            if (response.repairs.length > 0)
                onRepairChosen(response.repairs[0].id);
        } else {
            console.error(`getOwnersAssets query unsuccessful: ${response.message}`);
        }
    };

    let onRepairChosen = async function onRepairChosen(repairId) {
        // Clear old additional fields
        $('#additionalFields').empty();

        let additionalFields = undefined;
        let repair = undefined;
        for (let i = 0; i < applicableRepairs.length; i++) {
            repair = applicableRepairs[i];
            if (repair.id === repairId) {
                additionalFields = repair.additionalFieldNames;
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

            addFieldElements.push(ele);
        }
        $('#additionalFields').append(addFieldElements);
    };

    let onOwnerClear = function onOwnerClear() {
        $('.assetSelector').empty();
        $('.assetSelector').prop('disabled', true);
        onAssetClear();
    };

    let onAssetClear = function onAssetClear() {
        $('.repairSelector').empty();
        $('.repairSelector').prop('disabled', true);
        applicableRepairs = [];
        onRepairClear();
    };

    let onRepairClear = function onRepairClear() {
        $('#additionalFields').empty();
    };

    let onOwnerInputEvent = async function onOwnerInputEvent(event) {
        let input = $(event.target);
        if (event.originalEvent instanceof InputEvent) {
            // If the user just edited the text of the input
            input.removeAttr('ownerId');
            onOwnerClear();
            // Don't start making requests until there are at least 4 characters
            // in the owner's input
            if (input.val() && input.val().length > 3) {
                let oldVal = input.val();
                // Wait QUERY_DELAY_MS milliseconds before deciding whether to make the query
                await new Promise((resolve, reject) => setTimeout(resolve, QUERY_DELAY_MS));
                if (oldVal === input.val()) {
                    // If the value of the input hasn't changed after waiting, make the query

                    // First, though, clear the old options
                    let datalist = $('#ownerList');
                    datalist.empty();

                    // Now, make the query and add the results as options to the datalist
                    try {
                        let response = await getOwners(oldVal);
                        if (response.successful) {
                            let owner = undefined;
                            let ele = undefined;
                            let displayText = undefined;
                            let options = [];
                            for (let i = 0; i < response.owners.length; i++) {
                                owner = response.owners[i];

                                ele = document.createElement('option');
                                ele.value = `${owner.name} (${owner.primaryEmail})`;
                                ele.setAttribute('ownerId', owner.id);

                                options.push(ele);
                            }

                            datalist.append(options);
                        } else {
                            console.error(`Request failed: ${response.message}`);
                        }
                    } catch (e) {
                        console.error("A JS error occurred while trying to search for owners.");
                        console.error(e);
                    }
                }
            } else {
                // If the query is now too short for autocompletes, remove them
                $('#ownerList').empty();
            }
        } else {
            // If the user just clicked on an autocomplete option
            let options = $('#ownerList').children('option');
            let ownerId = undefined;
            let ele = undefined;
            for (let i = 0; i < options.length; i++) {
                ele = options[i];
                if (ele.value === input.val()) {
                    // We've found the matching selection
                    ownerId = $(ele).attr('ownerId');
                    input.attr('ownerId', ownerId);
                    onOwnerChosen(ownerId);
                    break;
                }
            }
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
    };

    let jsonifyInputs = function jsonifyInputs() {
        let ownerId = $('#ownerInput').attr('ownerId');
        let assetId = $('#assetSelector').val();
        let repairId = $('#repairSelector').val();
        let techId = $('#technicianSelector').val();
        let title = $('#titleInput').val();
        let description = $('#descriptionInput').val();

        let loanerIds = [];
        $('.loanerInput').each(function (i, e) {
            if(e.value) loanerIds.push(e.value);
        });

        let additionalFields = [];
        let ele;
        $('#additionalFields').children('.inputContainer').each(function (i, e) {
            ele = $(e)
            additionalFields.push({
                Name: ele.children('p').html(),
                Value: ele.children('input').val()
            })
        });

        let comments = [];
        $('.commentContainer').find('textarea').each(function (i, e) {
            if(e.value) comments.push(e.value);
        });

        return {
            OwnerId: ownerId,
            AssetId: assetId,
            RepairId: repairId,
            TechnicianId: techId,
            LoanerIds: loanerIds,
            Title: title,
            Description: description,
            AdditionalFields: additionalFields,
            Comments: comments
        }
    };

    /*
     * 
     * Return a then-able object that 
     * 
     */
    let submitTicket = async function submitTicket() {
        try {
            let response = await $.ajax({
                type: "POST",
                url: "/Tickets/NewTicket",
                data: JSON.stringify(jsonifyInputs()),
                contentType: "application/json",
                dataType: "json"
            });

            if (response.successful) {
                console.log(`Ticket submission succeeded: ${response.message}`);
                window.location.href = "/Tickets";
            } else {
                console.error(`Ticket submission failed: ${response.message}`);
            }
        } catch (e) {
            console.error(`Unexpected error [${e.name}] while submitting ticket.`);
            console.error(e);
        }
    };

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

        $('#ownerInput').bind('input', onOwnerInputEvent);
        $('#assetSelector').change(function() {
            onAssetChosen(this.value);
        });
        $('#repairSelector').change(function () {
            onRepairChosen(this.value);
        });
        $('#btnSubmit').click(submitTicket);
    });
// This is called closure. It ensures that any local variables/constants/functions
// can't be seen by any other scripts, for simplicity and cleanliness
})();
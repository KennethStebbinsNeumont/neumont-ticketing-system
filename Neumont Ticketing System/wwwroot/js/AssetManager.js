$(document).ready(() => {
    const searchInput = $('#searchInput');
    const resultTable = $('#resultTable');
    const template = $('#resultTemplate');

    const matchedOnSerialNumberString = "SerialNumber";
    const matchedOnOwnerNameString = "Name";
    const matchedOnOwnerPreferredNameString = "PreferredName";

    // The amount of time to wait after the value of the owner
    // field has been changed before making a query in onOwnerInputEvent
    const QUERY_DELAY_MS = 250;

    let getAssets = async function getAssets(query) {
        return $.ajax({
            type: "POST",
            url: "/Settings/AssetManager",
            data: JSON.stringify({
                Query: searchInput.val(),
                MaxNumOfResults: 50
            }),
            contentType: "application/json",
            dataType: "json",
            success: responseReceived,
            failure: unexpectedFailure,
            error: unexpectedFailure
        });
    }

    /* Response schema:
     * 
     * response = {
     *     Successful: <bool - whether or not the query was successful>,
     *     Message : <string - a message from the server about this query>,
     *     Query: <string - the query this response was crafted for>,
     *     Assets: [
     *         {
     *             OwnerId: <ObjectId - id of owner>,
     *             OwnerName: <name, not preferred name, of owner>,
     *             OwnerPreferredName: {
     *                 First: <owner's preferred first name>,
     *                 Middle: <owner's preferred last name>,
     *                 Last: <owner's preferred last name>
     *             },
     *             AssetId: <ObjectId of asset>,
     *             AssetSerial: <serial number of the matched asset>,
     *             AssetModelName: <name of the model of the matched asset>,
     *             AssetTypeName: <name of the type of the matched asset>,
     *             Score: <int - the "score" of the match; higher is a better match>,
     *             MatchedOn: <string - the property that this asset was matched on>
     *         }
     *     ]
     * }
     * 
     * */
    let responseReceived = function responseReceived(response) {
        try {
            if (response.successful) {
                // Make sure that the response we're about to display
                // isn't a response to an old query.
                if (searchInput.val() === response.query) {
                    // Empty the table of results
                    resultTable.find('.singleResult').remove();

                    // Create the new result elements
                    let results = [];
                    for (let i = 0; i < response.assets.length; i++) {
                        let asset = response.assets[i];
                        let singleResult = template.clone();

                        singleResult.removeAttr('id');
                        singleResult.attr('owner-id', asset.ownerId);
                        singleResult.attr('asset-id', asset.assetId)

                        singleResult.find('.ownerName').html(asset.ownerName);
                        if (asset.matchedOn === matchedOnOwnerNameString ||
                            asset.matchedOn === matchedOnOwnerPreferredNameString) {
                            singleResult.find('.ownerName').css('font-weight', 'bold');
                        }
                        singleResult.find('.serialNumber').html(asset.assetSerial);
                        if (asset.matchedOn === matchedOnSerialNumberString) {
                            singleResult.find('.serialNumber').css('font-weight', 'bold');
                        }
                        singleResult.find('.assetModel').html(asset.assetModelName);
                        singleResult.find('.assetType').html(asset.assetTypeName);

                        results.push(singleResult);
                    }

                    // Show the new result elements
                    resultTable.append(results);
                } else {
                    console.log(`A response for an old query ("${response.query}") was received.`)
                }
            } else {
                console.log(`Query was unsuccessful: \"${response.message}\"`);
            }
           
        } catch(err) {
            console.log(`Unexpected error occurred. Response: \"${response}\"`);
        }
    };

    let unexpectedFailure = function unexpectedFailure(response) {
        console.error(`Unexpected error while trying to query database for assets/owners.`);
        console.error(response);
    };

    let onQueryInputEvent = async function onQueryInputEvent(event) {
        let input = $(event.target);
        // If the user just edited the text of the input
        input.removeAttr('ownerId');
        onOwnerClear();
        // Don't start making requests until there are at least 3 characters
        // in the owner's input
        if (input.val() && input.val().length > 2) {
            let oldVal = input.val();
            // Wait QUERY_DELAY_MS milliseconds before deciding whether to make the query
            await new Promise((resolve, reject) => setTimeout(resolve, QUERY_DELAY_MS));
            if (oldVal === input.val()) {
                // If the value of the input hasn't changed after waiting, make the query
                getAssets(oldVal).then(responseReceived).catch(unexpectedFailure);
            }
        } else {
            // If the query is now too short for results, remove the old ones
            resultTable.find('.singleResult').remove();
        }
    };

    searchInput.bind('input', onQueryInputEvent);
});
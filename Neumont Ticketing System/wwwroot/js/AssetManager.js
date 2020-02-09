$(document).ready(() => {
    const searchInput = $('#searchInput');
    const resultTable = $('#resultTable');
    const template = $('#resultTemplate');

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
     *             AssetTypeName: <name of the type of the matched asset>
     *         }
     *     ]
     * }
     * 
     * */
    let responseReceied = function responseReceived(response) {
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
                        singleResult.find('.serialNumber').html(asset.assetSerial);
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

    };

    searchInput.change(function () {
        $.ajax({
            type: "POST",
            url: "/Settings/AssetManager",
            data: JSON.stringify({
                Query: searchInput.val(),
                MaxNumOfResults: 50
            }),
            contentType: "application/json",
            dataType: "json",
            success: responseReceied,
            failure: unexpectedFailure,
            error: unexpectedFailure
        });
    });
});
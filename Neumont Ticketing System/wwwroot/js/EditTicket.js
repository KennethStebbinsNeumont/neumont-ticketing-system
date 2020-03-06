(function () {

    const onRepairClear = function onRepairClear() {
        $('#additionalFields').empty();
    };

    const onRepairChosen = async function onRepairChosen(repairId) {
        // Clear old additional fields
        onRepairClear();

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

    const jsonifyInputs = function jsonifyInputs() {
        let ticketId = $('#ticketId').val();
        let repairId = $('#repairSelector').val();
        let techId = $('#technicianSelector').val();
        let title = $('#titleInput').val();
        let description = $('#descriptionInput').val();

        let loanerIds = [];
        $('.loanerInput').each(function (i, e) {
            if (e.value) loanerIds.push(e.value);
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
            // Only push new comments
            if (!e.disabled && e.value) comments.push(e.value);
        });

        return {
            TicketId: ticketId,
            RepairId: repairId,
            TechnicianId: techId,
            LoanerIds: loanerIds,
            Title: title,
            Description: description,
            AdditionalFields: additionalFields,
            Comments: comments
        }
    };

    const submitTicket = async function submitTicket() {
        try {
            let response = await $.ajax({
                type: "POST",
                url: "/Tickets/EditTicket",
                data: JSON.stringify(jsonifyInputs()),
                contentType: "application/json",
                dataType: "json"
            });

            if (response.successful) {
                console.log(`Ticket edit succeeded: ${response.message}`);
                window.location.href = "/Tickets";
            } else {
                console.error(`Ticket edit failed: ${response.message}`);
            }
        } catch (e) {
            console.error(`Unexpected error [${e.name}] while editing ticket.`);
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
                let textarea = clone.find('textarea');
                textarea.val('');
                textarea.prop('disabled', false);
            }));

        templateInput = $('#templateContainer').children('.inputContainer');

        $('#repairSelector').change(function () {
            onRepairChosen(this.value);
        });

        $('#btnSubmit').click(submitTicket);
    });
// This is called closure. It ensures that any local variables/constants/functions
// can't be seen by any other scripts, for simplicity and cleanliness
}) ();
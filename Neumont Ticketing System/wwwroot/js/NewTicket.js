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
});
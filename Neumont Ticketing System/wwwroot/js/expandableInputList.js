export default ExpandableInputList = {
    onInputKeypress: function () {
        // This event is triggered every time a key is pressed while this field
        // is in focus. This handler is only active for the last input in the list
        if ($(this).parent().children().last().attr('id') === $(this).attr('id')) {
            // If this element is the last element in the list and
            // it now has a value, create a new field

            let regexResults = $(this).attr('id').match(/(.*?)(\d+)_(.*?)_(\d+)/);
            let inputParent = regexResults[1];
            let parentIndex = parseInt(regexResults[2]);
            let attributeName = regexResults[3];
            let thisIndex = parseInt(regexResults[4]);
            let newIndex = thisIndex + 1;

            let newInput = $(this).clone();
            newInput.val('');
            newInput.attr('id', `${inputParent}${parentIndex}_${attributeName}_${newIndex}`);
            newInput.attr('name', `${inputParent}${parentIndex}.${attributeName}[${newIndex}]`);

            $(newInput).change(expandableListOnChange);
            $(newInput).blur(expandableListOnChange);
            $(newInput).keypress(expandableListOnKeypress);

            $(this).parent().append(newInput);
        }
    },
    onInputChange: function () {
        // This event is triggered when the user exits this field or
        // presses enter AND the value of this element has changed since
        // the last event call
        if (!$(this).val() &&
            $(this).parent().children().last().attr('id') !== $(this).attr('id')) {
            // If this field is now empty, remove it UNLESS it is the last
            // input box, which is supposed to be blank by default
            $(this).remove();
        }
    },
    onInputBlur: ExpandableInputList.onChange
};
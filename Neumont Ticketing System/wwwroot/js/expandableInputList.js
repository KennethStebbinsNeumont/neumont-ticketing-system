const ExpandableInputList = {};

{
    let onInputChange = function onInputChange() {
        // This event is triggered when the user exits this field or
        // presses enter AND the value of this element has changed since
        // the last event call
        if (!$(this).val() &&
            $(this).parent().children().last().attr('id') !== $(this).attr('id')) {
            // If this field is now empty, remove it UNLESS it is the last
            // input box, which is supposed to be blank by default
            $(this).remove();
        }
    };

    let onInputBlur = onInputChange;

    let onInputKeypress = function onInputKeypress() {
        // This event is triggered every time a key is pressed while this field
        // is in focus. This handler is only active for the last input in the list
        if ($(this).parent().children().last().attr('id') === $(this).attr('id')) {
            // If this element is the last element in the list and
            // it now has a value, create a new field

            let newInput = $(this).clone();
            newInput.val('');

            $(newInput).change(onInputChange);
            $(newInput).blur(onInputBlur);
            $(newInput).keypress(onInputKeypress);

            $(this).parent().append(newInput);
        }
    };

    ExpandableInputList.onInputKeypress = onInputKeypress;
    ExpandableInputList.onInputChange = onInputChange;
    ExpandableInputList.onInputBlur = onInputBlur;
}

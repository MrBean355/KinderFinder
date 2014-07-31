
$(document).ready(function () {
    // Add the on click handler to the check box on the edit tag page.
    $("#linkedBox").click(TogglePatronList);
});

/**
 * When editing a tag, and the user ticks/unticks the "Tag is linked" checkbox,
 * this script will enable/disable the patron selection box accordingly.
 */
function TogglePatronList() {
    var checked = $('#linkedBox').prop('checked');
    $("#patronList").prop('disabled', !checked)

    if (checked)
        $("#isLinked").prop('value', 'true');
    else
        $("#isLinked").prop('value', 'false');
}

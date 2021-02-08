/// <reference path="typings/jquery/jquery.d.ts" />
$(document).ready(function () {
    $("input[type=checkbox]").on("change", function (event) {
        var id = "#DefinedNames-" + $(event.target).data("id");
        var disabled = !$(event.target).prop("checked");
        var elem = $(id);
        elem.prop("disabled", disabled);
        elem.val("");
    });
});
//# sourceMappingURL=manageLayerThickness.js.map
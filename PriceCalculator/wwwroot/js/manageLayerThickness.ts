/// <reference path="typings/jquery/jquery.d.ts" />

$(document).ready(() => {
    $("input[type=checkbox]").on("change", function (event) {
        const id: string = "#DefinedNames-" + $(event.target).data("id");
        const disabled: boolean = !$(event.target).prop("checked");
        const elem: JQuery = $(id);

        elem.prop("disabled", disabled);
        elem.val("");
    });
});
/// <reference path="typings/jquery/jquery.d.ts" />

$(document).ready(() => {
    $(".input-group.date").datepicker({
        format: "dd/mm/yyyy"
    });
});
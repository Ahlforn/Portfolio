/// <reference path="typings/jquery/jquery.d.ts" />
var id = "";
$(document).ready(function () {
    var html = $("html");
    var uploadForm = $("div.fileControl");
    var fileControl = $("div.fileControl");
    var fileInput = $("input[type=file]");
    id = $("#quotationID").val();
    uploadForm.on("click", function (event) {
        event.preventDefault();
        event.stopPropagation();
        fileInput.trigger("click");
    });
    html.on("dragover", function (event) {
        event.preventDefault();
        event.stopPropagation();
    });
    fileControl.on("dragenter", function (event) {
        fileControl.addClass("hover");
    });
    fileControl.on("dragleave", function (event) {
        fileControl.removeClass("hover");
    });
    fileControl.on("drop", function (event) {
        event.preventDefault();
        event.stopPropagation();
        fileControl.removeClass("hover");
        var dragEvent = event.originalEvent;
        var files = dragEvent.dataTransfer.files;
        if (files.length > 0)
            initUpload(files, id);
    });
    fileInput.on("change", function (event) {
        event.preventDefault();
        event.stopPropagation();
        var input = fileInput[0];
        var files = input.files;
        if (files.length > 0)
            initUpload(files, id);
    });
});
var fileIndecis = [];
var modal = null;
var modal_shown = false;
var initUpload = function (files, quotationID) {
    var labels = [];
    var progressBars = [];
    var modalBody = $("<div>");
    if (files.length == 0)
        return false;
    for (var i = 0; i < files.length; i++) {
        labels[i] = $("<h5>").text(files[i].name);
        progressBars[i] = buildProgressBar(0, 100, 0);
        var data = new FormData();
        data.append("files", files[i]);
        uploadFile(data, i, progressBars[i], quotationID);
        fileIndecis.push(i);
        modalBody.append(labels[i]);
        modalBody.append(progressBars[i]);
    }
    modal = buildModal("Upload Model", modalBody);
    modal.on("shown.bs.modal", function (event) {
        modal_shown = true;
    });
    $("body").append(modal);
    modal.modal();
};
var uploadFile = function (data, index, progressBar, quotationID) {
    var token = $('input[name="__RequestVerificationToken"]').val();
    console.log("Starting download", index, fileIndecis);
    $.ajax({
        type: "POST",
        url: "./" + quotationID + "/Upload",
        headers: { "RequestVerificationToken": token },
        data: data,
        cache: false,
        contentType: false,
        processData: false,
        xhr: function () {
            var xhr = $.ajaxSettings.xhr();
            if (xhr.upload) {
                xhr.upload.addEventListener("progress", function (event) {
                    if (event.lengthComputable) {
                        var p = (event.loaded / event.total) * 100;
                        progressBar.updateValue(p);
                    }
                }, false);
            }
            return xhr;
        },
        error: function (data) {
            console.log("error!!");
            finishUpload(index, false);
        },
        success: function (data) {
            console.log("success!");
            finishUpload(index, true);
        }
    });
};
var finishUpload = function (index, success) {
    // TODO: show warning if upload failed.
    fileIndecis.pop();
    console.log("Download complete", index, success, fileIndecis);
    if (fileIndecis.length === 0) {
        console.log("All downloads complete. Hiding modal");
        if (modal_shown) {
            modal.modal("hide");
            modal_shown = false;
            window.location = window.location;
        }
        else {
            modal.on("shown.bs.modal", function (event) {
                modal.modal("hide");
                modal_shown = false;
                window.location = window.location;
            });
        }
    }
};
var buildModal = function (titleText, bodyContent) {
    var root = $("<div>")
        .addClass("modal fade")
        .attr("id", "uploadModal")
        .attr("role", "dialog")
        .attr("aria-labelledby", "uploadModalLabel")
        .attr("aria-hidden", "true");
    var dialog = $("<div>")
        .addClass("modal-dialog")
        .attr("role", "document")
        .appendTo(root);
    var content = $("<div>")
        .addClass("modal-content")
        .appendTo(dialog);
    var header = $("<div>")
        .addClass("modal-header")
        .appendTo(content);
    var title = $("<h5>")
        .addClass("modal-title")
        .attr("id", "uploadModalLabel")
        .text(titleText)
        .appendTo(header);
    var body = $("<div>")
        .addClass("modal-body")
        .appendTo(content);
    var footer = $("<div>")
        .addClass("modal-footer")
        .appendTo(content);
    //const close = $("<button>")
    //    .addClass("btn btn-secondary")
    //    .attr("type", "button")
    //    .attr("data-dismiss", "modal")
    //    .text("Close")
    //    .appendTo(footer);
    if (bodyContent) {
        body.append(bodyContent);
    }
    return root;
};
var buildProgressBar = function (min, max, val) {
    var wrapper = $("<div>")
        .addClass("progress");
    var bar = $("<div>")
        .addClass("progress-bar")
        .attr("role", "progressbar")
        .attr("aria-valuemin", min)
        .attr("aria-valuemax", max)
        .appendTo(wrapper);
    wrapper.updateValue = function (val) {
        bar.attr("style", "width: " + val + "%").attr("aria-valuenow", val);
    };
    wrapper.updateValue(val);
    return wrapper;
};
//# sourceMappingURL=upload.js.map
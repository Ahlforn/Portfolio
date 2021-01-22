/// <reference path="typings/jquery/jquery.d.ts" />
let id: string = "";

$(document).ready(() => {
    const html: JQuery = $("html");
    const uploadForm: JQuery = $("div.fileControl");
    const fileControl: JQuery = $("div.fileControl");
    const fileInput: JQuery = $("input[type=file]");

    id = $("#quotationID").val();

    uploadForm.on("click", (event) => {
        event.preventDefault();
        event.stopPropagation();
        fileInput.trigger("click");
    });

    html.on("dragover", function (event) {
        event.preventDefault();
        event.stopPropagation();
    });

    fileControl.on("dragenter", (event) => {
        fileControl.addClass("hover");
    });

    fileControl.on("dragleave", (event) => {
        fileControl.removeClass("hover");
    });

    fileControl.on("drop", (event) => {
        event.preventDefault();
        event.stopPropagation();

        fileControl.removeClass("hover");

        let dragEvent: DragEvent = event.originalEvent as DragEvent;
        let files: FileList = dragEvent.dataTransfer.files;

        if (files.length > 0)
            initUpload(files, id);
    });

    fileInput.on("change", (event) => {
        event.preventDefault();
        event.stopPropagation();
        let input: HTMLInputElement = fileInput[0] as HTMLInputElement;
        let files: FileList = input.files;

        if (files.length > 0)
            initUpload(files, id);
    });
});

let fileIndecis: number[] = [];
let modal: JQuery = null;
let modal_shown = false;

const initUpload = (files: FileList, quotationID) => {
    const labels: JQuery[] = [];
    let progressBars: JQuery[] = [];
    const modalBody: JQuery = $("<div>");

    if (files.length == 0)
        return false;

    for (let i: number = 0; i < files.length; i++) {
        labels[i] = $("<h5>").text(files[i].name);
        progressBars[i] = buildProgressBar(0, 100, 0);

        let data = new FormData();
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
}

const uploadFile = (data: FormData, index: number, progressBar: JQuery, quotationID) => {
    const token: string = $('input[name="__RequestVerificationToken"]').val();

    console.log("Starting download", index, fileIndecis);
    $.ajax({
        type: "POST",
        url: "./" + quotationID + "/Upload",
        headers: { "RequestVerificationToken": token },
        data: data,
        cache: false,
        contentType: false,
        processData: false,
        xhr: () => {
            let xhr = $.ajaxSettings.xhr();
            if (xhr.upload) {
                xhr.upload.addEventListener("progress", (event: ProgressEvent) => {
                    if (event.lengthComputable) {
                        let p = (event.loaded / event.total) * 100;
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
}

const finishUpload = (index: number, success: boolean): void => {
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
}

const buildModal = (titleText, bodyContent): JQuery => {
    const root = $("<div>")
        .addClass("modal fade")
        .attr("id", "uploadModal")
        .attr("role", "dialog")
        .attr("aria-labelledby", "uploadModalLabel")
        .attr("aria-hidden", "true");

    const dialog = $("<div>")
        .addClass("modal-dialog")
        .attr("role", "document")
        .appendTo(root);

    const content = $("<div>")
        .addClass("modal-content")
        .appendTo(dialog);

    const header = $("<div>")
        .addClass("modal-header")
        .appendTo(content);

    const title = $("<h5>")
        .addClass("modal-title")
        .attr("id", "uploadModalLabel")
        .text(titleText)
        .appendTo(header);

    const body = $("<div>")
        .addClass("modal-body")
        .appendTo(content);

    const footer = $("<div>")
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

const buildProgressBar = (min: number, max: number, val: number) => {
    const wrapper: JQuery = $("<div>")
        .addClass("progress");

    const bar: JQuery = $("<div>")
        .addClass("progress-bar")
        .attr("role", "progressbar")
        .attr("aria-valuemin", min)
        .attr("aria-valuemax", max)
        .appendTo(wrapper);

    wrapper.updateValue = (val: number) => {
        bar.attr("style", "width: " + val + "%").attr("aria-valuenow", val);
    }

    wrapper.updateValue(val);

    return wrapper;
}
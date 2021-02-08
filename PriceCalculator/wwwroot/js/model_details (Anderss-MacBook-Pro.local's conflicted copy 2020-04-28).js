/// <reference path="typings/jquery/jquery.d.ts" />
var materials = null;
var printers = null;
var postProcesses = null;
var selectedMaterial = null;
var selectedLayerThickness = null;
var selectedPrinter = null;
var selectedPostProcesses = [];
var submitUrl;
var InitStoredValues = function () {
    var materialsContainer = $("#materials");
    var layerThicknessContainer = $("#layerThicknesses");
    var printerContainer = $("#printers");
    var postProcessContainer = $("#postProcesses");
    if (materialsContainer.data("id") != "") {
        var materialID = Number(materialsContainer.data("id"));
        for (var _i = 0, materials_1 = materials; _i < materials_1.length; _i++) {
            var material = materials_1[_i];
            if (material.id == materialID)
                selectedMaterial = material;
        }
    }
    if (selectedMaterial != null) {
        if (postProcessContainer.data("ids") != "") {
            var ids = String(postProcessContainer.data("ids")).split(",");
            for (var _a = 0, ids_1 = ids; _a < ids_1.length; _a++) {
                var strId = ids_1[_a];
                var id_1 = Number(strId);
                if (id_1) {
                    for (var _b = 0, _c = GetRelatedPostProcesses(selectedMaterial); _b < _c.length; _b++) {
                        var pp = _c[_b];
                        if (pp.id == id_1)
                            selectedPostProcesses.push(pp);
                    }
                }
            }
        }
        if (layerThicknessContainer.data("id") != "") {
            var layerThicknessID = Number(layerThicknessContainer.data("id"));
            for (var _d = 0, _e = selectedMaterial.layerThicknesses; _d < _e.length; _d++) {
                var layerThickness = _e[_d];
                if (layerThickness.id == layerThicknessID)
                    selectedLayerThickness = layerThickness;
            }
        }
    }
    if (selectedMaterial != null && selectedLayerThickness != null && printerContainer.data("id") != "") {
        var printerID = Number(printerContainer.data("id"));
        for (var _f = 0, _g = GetRelatedPrinters(selectedLayerThickness); _f < _g.length; _f++) {
            var p = _g[_f];
            if (p.id === printerID) {
                selectedPrinter = p;
            }
        }
    }
};
var GenerateEmptyOption = function () {
    var option = document.createElement("option");
    option.value = "";
    option.text = "";
    return option;
};
var GenerateMaterialOption = function (material) {
    var label = $("<label/>")
        .addClass("btn btn-secondary")
        .addClass("materialOption")
        .data("materialId", material.id);
    var input = $("<input/>")
        .attr("type", "radio")
        .attr("name", "materialOption")
        .attr("autocomplete", "off")
        .attr("value", material.id);
    label.append(input);
    label.append(material.name);
    var option = document.createElement("option");
    option.value = material.id.toString();
    option.text = material.name;
    if (selectedMaterial == material) {
        option.selected = true;
        GenerateLayerThicknesses();
        GeneratePostProcesses();
    }
    return option;
};
var GenerateMaterials = function () {
    var input = document.getElementById("materials");
    var placeholder = null;
    if (selectedMaterial == null) {
        var placeholder_1 = GenerateEmptyOption();
        input.appendChild(placeholder_1);
    }
    for (var _i = 0, materials_2 = materials; _i < materials_2.length; _i++) {
        var material = materials_2[_i];
        var option = GenerateMaterialOption(material);
        input.appendChild(option);
    }
    input.addEventListener("change", function () {
        if (selectedMaterial == null) {
            input.remove(0);
            placeholder = null;
        }
        ResetPostProcesses();
        selectedPostProcesses = [];
        selectedMaterial = materials[input.selectedIndex];
        selectedLayerThickness = null;
        //selectedLayerThickness = materials[input.selectedIndex].layerThicknesses[0];
        //selectedPrinter = GetRelatedPrinters(selectedLayerThickness)[0];
        GenerateLayerThicknesses();
        GeneratePrinters();
        GeneratePostProcesses();
    });
};
var GenerateLayerThicknessOption = function (layerThickness) {
    var label = $("<label/>").addClass("btn btn-secondary");
    var input = $("<input/>").attr("type", "radio").attr("name", "layerThicknessOption").attr("id", "layerThicknessOption").attr("autocomplete", "off").attr("value", layerThickness.id);
    label.append(input);
    label.append(layerThickness.thickness + layerThickness.unit);
    var option = document.createElement("option");
    option.value = layerThickness.id.toString();
    option.text = layerThickness.thickness + layerThickness.unit;
    if (selectedLayerThickness == layerThickness) {
        option.selected = true;
        GeneratePrinters();
    }
    return option;
};
var GenerateLayerThicknesses = function () {
    if (selectedMaterial == null)
        return;
    var input = document.getElementById("layerThicknesses");
    while (input.firstChild) {
        input.removeChild(input.firstChild);
    }
    var placeholder = null;
    if (selectedLayerThickness == null) {
        var placeholder_2 = GenerateEmptyOption();
        input.appendChild(placeholder_2);
    }
    for (var _i = 0, _a = selectedMaterial.layerThicknesses; _i < _a.length; _i++) {
        var thickness = _a[_i];
        input.appendChild(GenerateLayerThicknessOption(thickness));
    }
    input.addEventListener("change", function (event) {
        if (selectedLayerThickness == null) {
            input.remove(0);
            placeholder = null;
        }
        selectedLayerThickness = selectedMaterial.layerThicknesses[input.selectedIndex];
        selectedPrinter = GetRelatedPrinters(selectedLayerThickness)[0];
        GeneratePrinters();
    });
};
var GetRelatedPrinters = function (layerThickness) {
    var related = [];
    for (var _i = 0, printers_1 = printers; _i < printers_1.length; _i++) {
        var pl = printers_1[_i];
        if (pl.layerThicknessID === layerThickness.id)
            related.push(pl.printer);
    }
    return related;
};
var GeneratePrinterOption = function (printer) {
    var label = $("<label/>").addClass("btn btn-secondary");
    var input = $("<input/>").attr("type", "radio").attr("name", "printerOption").attr("id", "printerOption").attr("autocomplete", "off").attr("value", printer.id);
    label.append(input);
    label.append(printer.name);
    var option = document.createElement("option");
    option.value = printer.id.toString();
    option.text = printer.name;
    if (selectedPrinter == printer) {
        option.selected = true;
    }
    return option;
};
var GeneratePrinters = function () {
    if (selectedLayerThickness == null)
        return;
    var input = document.getElementById("printers");
    while (input.firstChild) {
        input.removeChild(input.firstChild);
    }
    var related = GetRelatedPrinters(selectedLayerThickness);
    for (var _i = 0, related_1 = related; _i < related_1.length; _i++) {
        var p = related_1[_i];
        input.appendChild(GeneratePrinterOption(p));
        if (p == selectedPrinter)
            UpdatePrice();
    }
    input.addEventListener("change", function (event) {
        selectedPrinter = related[input.selectedIndex];
        UpdatePrice();
    });
};
var GetRelatedPostProcesses = function (material) {
    var related = [];
    for (var _i = 0, postProcesses_1 = postProcesses; _i < postProcesses_1.length; _i++) {
        var mpp = postProcesses_1[_i];
        if (mpp.materialID == material.id)
            related.push(mpp.postProcess);
    }
    return related;
};
var GeneratePostProcessOption = function (postProcess, checked) {
    var container = $("<div/>").addClass("form-check");
    var input = $("<input/>").addClass("form-check-input").attr("type", "checkbox").attr("name", "postProcess").attr("id", "PostProcess-" + postProcess.id).val(postProcess.id);
    var label = $("<label/>").addClass("form-check-label").attr("for", "PostProcess-" + postProcess.id);
    var price = $("<span/>").addClass("postProcessPrice");
    if (checked)
        input.prop("checked", true);
    price.attr("data-id", postProcess.id);
    label.append(postProcess.name);
    container.append(input);
    container.append(label);
    container.append(price);
    input.on("change", function (event) {
        UpdatePrice();
    });
    return container;
};
var ResetPostProcesses = function () {
    var container = $("#postProcesses");
    container.children().each(function (index, elem) {
        $(elem).remove();
    });
};
var GeneratePostProcesses = function () {
    if (selectedMaterial == null)
        return;
    var container = $("#postProcesses");
    for (var _i = 0, _a = GetRelatedPostProcesses(selectedMaterial); _i < _a.length; _i++) {
        var pp = _a[_i];
        container.append(GeneratePostProcessOption(pp, selectedPostProcesses.indexOf(pp) >= 0));
    }
};
var UpdatePrice = function () {
    var url = $("input#asyncPrice").val();
    var token = $('input[name="__RequestVerificationToken"]').val();
    var form = document.querySelector("form#details");
    var formData = new FormData(form);
    var req = new XMLHttpRequest();
    var amount = $("input#amount");
    var pricePerPart = $("div#pricePerPart");
    var totalPrice = $("div#totalPrice");
    var inputPricePerPart = $("input[name = pricePerPart]");
    var inputTotalPrice = $("input[name = priceTotal]");
    req.open("POST", url, true);
    req.setRequestHeader("RequestVerificationToken", token);
    req.responseType = "json";
    req.onload = function (event) {
        if (req.status == 200) {
            pricePerPart.text(Number(req.response.price).toFixed(0));
            inputPricePerPart.val(Number(req.response.price).toFixed(2));
            totalPrice.text((req.response.price * Number(amount.val())).toFixed(0));
            inputTotalPrice.val((req.response.price * Number(amount.val())).toFixed(2));
            var postProcesses_3 = $(".postProcessPrice").toArray();
            for (var _i = 0, postProcesses_2 = postProcesses_3; _i < postProcesses_2.length; _i++) {
                var elm = postProcesses_2[_i];
                var id_2 = Number($(elm).data("id"));
                var price = Number(req.response.postProcesses[id_2.toString()]);
                if (price) {
                    elm.innerHTML = " (" + price.toFixed(0) + ")";
                }
            }
        }
        else {
            pricePerPart.text("-");
            inputPricePerPart.val(0);
            totalPrice.text("-");
            inputTotalPrice.val(0);
        }
    };
    req.send(formData);
};
/*
 * Init xeogl
 */
var printDirection;
var aabb;
var scene;
var model;
var plane;
var _AABB;
var rotate = function (model, axis, degree) {
    var axes = [
        new Float32Array([1, 0, 0]),
        new Float32Array([0, 1, 0]),
        new Float32Array([0, 0, 1])
    ];
    axis = Number(axis);
    var d = xeogl.math.transformPoint3(xeogl.math.transposeMat4(model.worldMatrix), axes[axis]);
    model.rotate(d, degree);
    console.log(model.rotation, xeogl.math.quaternionToEuler(model.quaternion, "XYZ"));
};
var align = function (o1, o2) {
    o1._updateAABB();
    var d = xeogl.math.transformPoint3(xeogl.math.transposeMat4(o1.worldMatrix), new Float32Array([0, 1, 0]));
    var dist = (o1.aabb[1] - o2.aabb[4]) * -1;
    o1.translate(d, dist);
};
var getModelSnapshot = function () {
    return scene.canvas.getSnapshot({
        width: 500,
        height: 500,
        format: "png"
    });
};
var setAABB = function (aabbArg) {
    var str = aabbArg[0] + "," + aabbArg[1] + "," + aabbArg[2] + "," + aabbArg[3] + "," + aabbArg[4] + "," + aabbArg[5];
    aabb.val(str);
};
var setPrintDirection = function (eulerAngles) {
    var calc = function (num) { return num * (180 / Math.PI); };
    var str = calc(eulerAngles[0]) + "," + calc(eulerAngles[1]) + "," + calc(eulerAngles[2]);
    printDirection.val(str);
};
var getPrintDirection = function () {
    if (printDirection.val().length === 0)
        return null;
    var rotation = new Float32Array([0, 0, 0]);
    var values = printDirection.val().split(",");
    rotation[0] = parseFloat(values[0]);
    rotation[1] = parseFloat(values[1]);
    rotation[2] = parseFloat(values[2]);
    return rotation;
};
var initXeogl = function (updatePriceFunction) {
    var stlFile = $("#STLFile").val();
    printDirection = $("input[name=printDirection]");
    scene = new xeogl.Scene({
        canvas: "stl_canvas",
        webgl2: true,
        transparent: true,
        contextAttr: {
            preserveDrawingBuffer: true
        }
    });
    xeogl.setDefaultScene(scene);
    var group = new xeogl.Group({
        id: "group",
        rotation: [0, 0, 0],
        position: [0, 0, 0],
        scale: [1, 1, 1],
        aabbVisible: true,
        children: [
            new xeogl.STLModel({
                id: "model",
                src: stlFile,
                position: [0, 0, 0],
                rotation: getPrintDirection(),
                smoothNormals: true,
            }),
            new xeogl.Mesh({
                id: "plane",
                geometry: new xeogl.PlaneGeometry({
                    primitive: "triangles",
                    center: [0, -50, 0],
                    xSize: 600,
                    zSize: 600,
                    xSegments: 100,
                    zSegments: 100
                }),
                material: new xeogl.PhongMaterial({
                    shininess: 170,
                    specular: [0, -0.37, -0.6],
                    diffuse: [0.2, 0.2, 0.2],
                    xalpha: 0.99,
                    backfaces: false
                }),
                collidable: false,
                pickable: false
            })
        ]
    });
    model = group.childMap["model"];
    plane = group.childMap["plane"];
    aabb = $("input[name=aabb]");
    model.on("loaded", function () {
        setAABB(model.aabb);
        align(model, plane);
        updatePriceFunction();
    });
    new xeogl.CameraControl();
    var camera = group.scene.camera;
    camera.eye = [0, 100, -200];
    camera.up = camera.worldUp;
    new xeogl.CameraFollowAnimation({
        target: model,
        fly: false,
        fit: true,
        fitFOV: 35
    });
    new xeogl.AxisHelper({
        camera: scene.camera,
        size: [200, 200],
        visible: true,
    });
    var axisHelper = $('canvas[id^="xeogl-axisHelper-canvas"]');
    axisHelper.parent().addClass("axisHelperParent");
    axisHelper.appendTo("#canvasContainer");
    axisHelper.addClass("axisHelper");
    axisHelper.attr("style", "");
    $(".axisHelperParent").remove();
    $("#rotateUp").on("click", function () {
        rotate(model, 0, 90); // 0 = X-axis
        align(model, plane);
        setPrintDirection(model.rotation);
        setAABB(model.aabb);
        updatePriceFunction();
    });
    $("#rotateDown").on("click", function () {
        rotate(model, 0, -90); // 0 = X-axis
        align(model, plane);
        setPrintDirection(model.rotation);
        setAABB(model.aabb);
        updatePriceFunction();
    });
    $("#rotateLeft").on("click", function () {
        rotate(model, 1, 90); // 1 = Y-axis
        align(model, plane);
        setPrintDirection(model.rotation);
        setAABB(model.aabb);
        updatePriceFunction();
    });
    $("#rotateRight").on("click", function () {
        rotate(model, 1, -90); // 1 = Y-axis
        align(model, plane);
        setPrintDirection(model.rotation);
        setAABB(model.aabb);
        updatePriceFunction();
    });
};
$(document).ready(function () {
    $.get(document.URL + "/Materials")
        .done(function (data) {
        materials = data.materials;
        printers = data.printers;
        postProcesses = data.postProcesses;
        InitStoredValues();
        GenerateMaterials();
        UpdatePrice();
    });
    initXeogl(UpdatePrice);
    $("input#amount").on("change", function (event) {
        UpdatePrice();
    });
    $("button#download").on("click", function () {
        var url = $("input#asyncExcel").val();
        var token = $('input[name="__RequestVerificationToken"]').val();
        var name = $("input#name").val();
        var form = document.querySelector("form#details");
        var formData = new FormData(form);
        var req = new XMLHttpRequest();
        req.open("POST", url, true);
        req.setRequestHeader("RequestVerificationToken", token);
        req.responseType = "blob";
        req.onload = function (event) {
            if (req.status == 200) {
                var a = document.createElement("a");
                var aUrl = window.URL.createObjectURL(req.response);
                a.href = aUrl;
                a.download = name + ".xlsx";
                document.body.append(a);
                a.click();
                a.remove();
                window.URL.revokeObjectURL(aUrl);
            }
            else {
                console.log("Failed to generate and download excel file.");
            }
        };
        req.send(formData);
    });
    $("input#save").on("click", function (event) {
        var inputSnapshot = $("input[name = snapshot]");
        inputSnapshot.val(getModelSnapshot());
        $("form#details").off("submit").submit();
    });
});
//# sourceMappingURL=model_details.js.map
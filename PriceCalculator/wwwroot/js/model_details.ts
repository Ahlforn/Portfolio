/// <reference path="typings/jquery/jquery.d.ts" />

declare let xeogl: any;


interface PrintDirection {
    modal: JQuery,
    buildModal: JQuery,
    buildControl: JQuery
}

interface Printer {
    id: number,
    name: string,
    w: number,
    h: number,
    d: number,
}

interface Material {
    id: number,
    name: string,
    description: string,
    layerThicknesses: LayerThickness[]
}

interface LayerThickness {
    id: number,
    thickness: number
    unit: string
}

interface Industry {
    id: number,
    name: string
}

interface PostProcess {
    id: number,
    name: string,
    description: string
}

interface PrinterJson {
    printer: Printer,
    layerThicknessID: number
}

interface PostProcessJson {
    postProcess: PostProcess,
    materialID: number
}

interface ResponseData {
    materials: Material[],
    printers: PrinterJson[],
    industries: Industry[],
    postProcesses: PostProcessJson[]
}

let materials: Material[] = null;
let printers: PrinterJson[] = null;
let industries: Industry[] = null;
let postProcesses: PostProcessJson[] = null;
let selectedMaterial: Material = null;
let selectedLayerThickness: LayerThickness = null;
let selectedPrinter: Printer = null;
let selectedIndustry: Industry = null;
let selectedPostProcesses: PostProcess[] = [];

let submitUrl: string;

const InitStoredValues = (): void => {
    let materialsContainer = $("#materials");
    let layerThicknessContainer = $("#layerThicknesses");
    let printerContainer = $("#printers");
    let industryContainer = $("#industries");
    let postProcessContainer = $("#postProcesses");

    if (materialsContainer.data("id")!= "") {
        const materialID: Number = Number(materialsContainer.data("id"));
        for (let material of materials) {
            if (material.id == materialID)
                selectedMaterial = material;
        }
    }

    if (selectedMaterial != null) {
        if (postProcessContainer.data("ids") != "") {
            let ids: string[] = String(postProcessContainer.data("ids")).split(",");
            for (let strId of ids) {
                const id = Number(strId);

                if (id) {
                    for (let pp of GetRelatedPostProcesses(selectedMaterial)) {
                        if (pp.id == id)
                            selectedPostProcesses.push(pp);
                    }
                }
            }


        }

        if (layerThicknessContainer.data("id") != "") {
            const layerThicknessID: Number = Number(layerThicknessContainer.data("id"));
            for (let layerThickness of selectedMaterial.layerThicknesses) {
                if (layerThickness.id == layerThicknessID)
                    selectedLayerThickness = layerThickness;
            }
        }
    }

    if (selectedMaterial != null && selectedLayerThickness != null && printerContainer.data("id") != "") {
        const printerID: Number = Number(printerContainer.data("id"));
        for (let p of GetRelatedPrinters(selectedLayerThickness)) {
            if (p.id === printerID) {
                selectedPrinter = p;
            }
        }
    }

    if (industryContainer.data("id") != null) {
        const industryID: Number = Number(industryContainer.data("id"));
        for (let industry of industries) {
            if (industry.id == industryID)
                selectedIndustry = industry;
        }
    }
}

const GenerateEmptyOption = function (): HTMLOptionElement {
    let option: HTMLOptionElement = document.createElement("option") as HTMLOptionElement;
    option.value = "";
    option.text = "";

    return option;
}

const GenerateMaterialOption = function (material: Material): HTMLOptionElement {
    let label: JQuery = $("<label/>")
        .addClass("btn btn-secondary")
        .addClass("materialOption")
        .data("materialId", material.id);

    let input: JQuery = $("<input/>")
        .attr("type", "radio")
        .attr("name", "materialOption")
        .attr("autocomplete", "off")
        .attr("value", material.id)

    label.append(input);
    label.append(material.name);

    let option: HTMLOptionElement = document.createElement("option") as HTMLOptionElement;
    option.value = material.id.toString();
    option.text = material.name

    if (selectedMaterial == material) {
        option.selected = true;
        GenerateLayerThicknesses();
        GeneratePostProcesses();
    }

    return option;
}

const GenerateMaterials = (): void => {
    let input: HTMLSelectElement = document.getElementById("materials") as HTMLSelectElement;

    let placeholder: HTMLOptionElement = null;
    if (selectedMaterial == null) {
        let placeholder: HTMLOptionElement = GenerateEmptyOption();
        input.appendChild(placeholder);
    }

    for (let material of materials) {
        let option: HTMLOptionElement = GenerateMaterialOption(material);
        input.appendChild(option);
    }

    input.addEventListener("change", function () {
        if (selectedMaterial == null) {
            input.remove(0)
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
}

const GenerateLayerThicknessOption = (layerThickness: LayerThickness): HTMLOptionElement => {
    let label: JQuery = $("<label/>").addClass("btn btn-secondary");
    let input: JQuery = $("<input/>").attr("type", "radio").attr("name", "layerThicknessOption").attr("id", "layerThicknessOption").attr("autocomplete", "off").attr("value", layerThickness.id);

    label.append(input);
    label.append(layerThickness.thickness + layerThickness.unit);

    let option: HTMLOptionElement = document.createElement("option") as HTMLOptionElement;
    option.value = layerThickness.id.toString();
    option.text = layerThickness.thickness + layerThickness.unit;

    if (selectedLayerThickness == layerThickness) {
        option.selected = true;
        GeneratePrinters();
    }

    return option;
}

const GenerateLayerThicknesses = (): void => {
    if (selectedMaterial == null) return;

    let input: HTMLSelectElement = document.getElementById("layerThicknesses") as HTMLSelectElement;

    while (input.firstChild) {
        input.removeChild(input.firstChild);
    }

    let placeholder: HTMLOptionElement = null;
    if (selectedLayerThickness == null) {
        let placeholder: HTMLOptionElement = GenerateEmptyOption();
        input.appendChild(placeholder);
    }

    for (let thickness of selectedMaterial.layerThicknesses) {
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
}

const GetRelatedPrinters = (layerThickness: LayerThickness): Printer[] => {
    let related: Printer[] = [];

    for (let pl of printers) {
        if (pl.layerThicknessID === layerThickness.id)
            related.push(pl.printer);
    }

    return related;
}

const GeneratePrinterOption = (printer: Printer): HTMLOptionElement => {
    let label: JQuery = $("<label/>").addClass("btn btn-secondary");
    let input: JQuery = $("<input/>").attr("type", "radio").attr("name", "printerOption").attr("id", "printerOption").attr("autocomplete", "off").attr("value", printer.id);

    label.append(input);
    label.append(printer.name);

    let option: HTMLOptionElement = document.createElement("option") as HTMLOptionElement;
    option.value = printer.id.toString();
    option.text = printer.name;

    if (selectedPrinter == printer) {
        option.selected = true;
    }

    return option;
}

const GeneratePrinters = (): void => {
    if (selectedLayerThickness == null) return;

    let input = document.getElementById("printers") as HTMLSelectElement;

    while (input.firstChild) {
        input.removeChild(input.firstChild);
    }

    let related: Printer[] = GetRelatedPrinters(selectedLayerThickness);
    for (let p of related) {
        input.appendChild(GeneratePrinterOption(p));
        if (p == selectedPrinter)
            UpdatePrice();
    }

    input.addEventListener("change", function (event) {
        selectedPrinter = related[input.selectedIndex];
        UpdatePrice();
    });
}

const GenerateIndustryOption = (industry: Industry): HTMLOptionElement => {
    let option: HTMLOptionElement = document.createElement("option") as HTMLOptionElement;
    option.value = industry.id.toString();
    option.text = industry.name;

    if (selectedIndustry == industry) {
        option.selected = true;
        // GenerateSomething
    }

    return option;
}

const GenerateIndustries = (): void => {
    let input = document.getElementById("industries") as HTMLSelectElement;

    let placeholder: HTMLOptionElement = null;
    if (selectedIndustry == null) {
        placeholder = GenerateEmptyOption();
        input.appendChild(placeholder);
    }

    for (let industry of industries) {
        input.appendChild(GenerateIndustryOption(industry));
    }

    input.addEventListener("change", function () {
        if (selectedIndustry == null) {
            input.remove(0)
            placeholder = null;
        }
    });
}

const GetRelatedPostProcesses = (material: Material): PostProcess[] => {
    let related: PostProcess[] = [];

    for (let mpp of postProcesses) {
        if (mpp.materialID == material.id)
            related.push(mpp.postProcess);
    }

    return related;
}

const GeneratePostProcessOption = (postProcess: PostProcess, checked: boolean): JQuery => {
    let container: JQuery = $("<div/>").addClass("form-check");
    let input: JQuery = $("<input/>").addClass("form-check-input").attr("type", "checkbox").attr("name", "postProcess").attr("id", "PostProcess-" + postProcess.id).val(postProcess.id);
    let label: JQuery = $("<label/>").addClass("form-check-label").attr("for", "PostProcess-" + postProcess.id);
    let price: JQuery = $("<span/>").addClass("postProcessPrice");

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
}

const ResetPostProcesses = (): void => {
    let container = $("#postProcesses");
    container.children().each((index: number, elem: Element) => {
        $(elem).remove();
    });
}

const GeneratePostProcesses = (): void => {
    if (selectedMaterial == null) return;

    let container = $("#postProcesses");

    for (let pp of GetRelatedPostProcesses(selectedMaterial)) {
        container.append(GeneratePostProcessOption(pp, selectedPostProcesses.indexOf(pp) >= 0));
    }
}

const UpdatePrice = (): void => {
    // Validate that required information is indeed present
    const aabb: HTMLInputElement = document.querySelector("input[name = aabb]");
    const materials: HTMLSelectElement = document.querySelector("select#materials");
    const layerThicknesses: HTMLSelectElement = document.querySelector("select#layerThicknesses");
    const printers: HTMLSelectElement = document.querySelector("select#printers");

    if (
        aabb.value.length == 0 ||
        materials.selectedOptions[0].text.length == 0 ||
        layerThicknesses.selectedOptions[0].text.length == 0 ||
        printers.selectedOptions[0].text.length == 0
    )
        return;

    // Request price
    const url: string = $("input#asyncPrice").val();
    const token: string = $('input[name="__RequestVerificationToken"]').val();
    let form: HTMLFormElement = document.querySelector("form#details");
    let formData = new FormData(form);
    let req = new XMLHttpRequest();

    const amount: JQuery = $("input#amount");
    const pricePerPart: JQuery = $("div#pricePerPart");
    const totalPrice: JQuery = $("div#totalPrice");
    const inputPricePerPart: JQuery = $("input[name = pricePerPart]");
    const inputTotalPrice: JQuery = $("input[name = priceTotal]");

    req.open("POST", url, true);
    req.setRequestHeader("RequestVerificationToken", token);
    req.responseType = "json";
    req.onload = function (event) {
        if (req.status == 200) {
            pricePerPart.text(Number(req.response.price).toFixed(0));
            inputPricePerPart.val(Number(req.response.price).toFixed(2));
            totalPrice.text((req.response.price * Number(amount.val())).toFixed(0));
            inputTotalPrice.val((req.response.price * Number(amount.val())).toFixed(2));

            const postProcesses: HTMLElement[] = $(".postProcessPrice").toArray();

            for (let elm of postProcesses) {
                const id: number = Number($(elm).data("id"));
                const price: number = Number(req.response.postProcesses[id.toString()])

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
    }

    req.send(formData);
}

/*
 * Init xeogl
 */
let printDirection: JQuery;
let aabb: JQuery;
let scene;
let model;
let plane;
let _AABB: [Number, Number, Number, Number, Number, Number];

type PriceUpdateFunction = () => void;


const rotate = (model, axis, degree): void => {
    let axes =[
        new Float32Array([1, 0, 0]),
        new Float32Array([0, 1, 0]),
        new Float32Array([0, 0, 1])
    ];
    axis = Number(axis);
    const d = xeogl.math.transformPoint3(xeogl.math.transposeMat4(model.worldMatrix), axes[axis]);

    model.rotate(d, degree);

    console.log(model.rotation, xeogl.math.quaternionToEuler(model.quaternion, "XYZ"));
};

const align = (o1, o2): void => {
    o1._updateAABB();
    const d = xeogl.math.transformPoint3(xeogl.math.transposeMat4(o1.worldMatrix), new Float32Array([0, 1, 0]));
    const dist = (o1.aabb[1] - o2.aabb[4]) * -1;
    o1.translate(d, dist);
};

const getModelSnapshot = (): string => {
    const source: HTMLCanvasElement = document.getElementById("stl_canvas") as HTMLCanvasElement;
    const axisHelper: HTMLCanvasElement = document.querySelector('canvas[id^="xeogl-axisHelper-canvas"]');
    const canvas: HTMLCanvasElement = document.createElement("canvas");
    canvas.width = 500;
    canvas.height = 500;

    if (source == null || axisHelper == null)
        throw "Source canvases not provided.";

    const context = canvas.getContext("2d");

    context.drawImage(source, 0, 0);
    context.drawImage(axisHelper, 0, 300);

    return canvas.toDataURL("image/png");
}

const setAABB = (aabbArg: [Number, Number, Number, Number, Number, Number]) => {
    const str = aabbArg[0] + "," + aabbArg[1] + "," + aabbArg[2] + "," + aabbArg[3] + "," + aabbArg[4] + "," + aabbArg[5];
    aabb.val(str);
}

const setPrintDirection = (eulerAngles: Float32Array) => {
    const calc = (num) => num * (180 / Math.PI);
    let str = calc(eulerAngles[0]) + "," + calc(eulerAngles[1]) + "," + calc(eulerAngles[2]);
    printDirection.val(str);
}

const getPrintDirection = (): Float32Array => {
    if (printDirection.val().length === 0)
        return null;

    const rotation = new Float32Array([0, 0, 0]);
    const values = printDirection.val().split(",");

    rotation[0] = parseFloat(values[0]);
    rotation[1] = parseFloat(values[1]);
    rotation[2] = parseFloat(values[2]);

    return rotation;
}

const initXeogl = (updatePriceFunction: PriceUpdateFunction) => {
    const stlFile: JQuery = $("#STLFile").val();
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

    const group = new xeogl.Group({
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
                    xSize: 500,
                    zSize: 250,
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
    })

    model = group.childMap["model"];
    plane = group.childMap["plane"];
    aabb = $("input[name=aabb]");

    model.on("loaded", () => {
        setAABB(model.aabb);
        align(model, plane);
        updatePriceFunction();
    });

    new xeogl.CameraControl();

    const camera = group.scene.camera;

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

    const axisHelper = $('canvas[id^="xeogl-axisHelper-canvas"]');
    axisHelper.parent().addClass("axisHelperParent");
    axisHelper.appendTo("#canvasContainer");
    axisHelper.addClass("axisHelper");
    axisHelper.attr("style", "");

    $(".axisHelperParent").remove();

    $("#rotateUp").on("click", () => {
        rotate(model, 0, 90); // 0 = X-axis
        align(model, plane);
        setPrintDirection(model.rotation);
        setAABB(model.aabb);
        updatePriceFunction();
    })

    $("#rotateDown").on("click", () => {
        rotate(model, 0, -90); // 0 = X-axis
        align(model, plane);
        setPrintDirection(model.rotation);
        setAABB(model.aabb);
        updatePriceFunction();
    })

    $("#rotateLeft").on("click", () => {
        rotate(model, 1, 90); // 1 = Y-axis
        align(model, plane);
        setPrintDirection(model.rotation);
        setAABB(model.aabb);
        updatePriceFunction();
    })

    $("#rotateRight").on("click", () => {
        rotate(model, 1, -90); // 1 = Y-axis
        align(model, plane);
        setPrintDirection(model.rotation);
        setAABB(model.aabb);
        updatePriceFunction();
    })
}

$(document).ready(() => {
    $.get(document.URL + "/Materials")
        .done((data: ResponseData) => {
            materials = data.materials;
            printers = data.printers;
            industries = data.industries;
            postProcesses = data.postProcesses;
            InitStoredValues();
            GenerateMaterials();
            GenerateIndustries();
            UpdatePrice();
        });

    initXeogl(UpdatePrice);

    $("input#amount").on("change", function (event) {
        UpdatePrice();
    });

    $("button#download").on("click", () => {
        const url: string = $("input#asyncExcel").val();
        const token: string = $('input[name="__RequestVerificationToken"]').val();
        const name: string = $("input#name").val();

        let form: HTMLFormElement = document.querySelector("form#details");
        let formData = new FormData(form);
        let req = new XMLHttpRequest();

        req.open("POST", url, true);
        req.setRequestHeader("RequestVerificationToken", token);
        req.responseType = "blob";
        req.onload = function (event) {
            if (req.status == 200) {
                let a: HTMLAnchorElement = document.createElement("a");
                let aUrl = window.URL.createObjectURL(req.response);
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
        }

        req.send(formData);
    });

    $("input#save").on("click", function (event) {
        const inputSnapshot: JQuery = $("input[name = snapshot]");

        inputSnapshot.val(getModelSnapshot())

        $("form#details").off("submit").submit();
    });
});
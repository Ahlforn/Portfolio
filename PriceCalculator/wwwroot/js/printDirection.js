export default class PrintDirection {
    constructor(model) {
        this.model = model;
        this.scene = null;
        this.axes = [
            new Float32Array([1, 0, 0]), // X
            new Float32Array([0, 1, 0]), // Y
            new Float32Array([0, 0, 1])  // Z
        ];
    }

    get modal() {
        const content = this.buildControl();
        const title = "Set Print Direction";
        const modal = this.buildModal(title, content);
        this.initXeogl();

        return modal;
    }

    buildModal(titleText, bodyContent) {
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

        const close = $("<button>")
            .addClass("btn btn-secondary")
            .attr("type", "button")
            .attr("data-dismiss", "modal")
            .text("Close")
            .appendTo(footer);

        if (bodyContent) {
            body.append(bodyContent);
        }

        return root;
    };

    buildControl() {
        const container = $("<div>").addClass("printDirectionControl").append(
            $("<div>").addClass("upper").append(
                $("<div>").addClass("up").append(
                    $("<div>").addClass("triangle"),
                    $("<div>").addClass("shaft")
                ).on("click", this.rotateUp)
            ),
            $("<div>").addClass("middle").append(
                $("<div>").addClass("left").append(
                    $("<div>").addClass("triangle"),
                    $("<div>").addClass("shaft")
                ).on("click", this.rotateLeft),
                $("<div>").addClass("center"),
                $("<div>").addClass("right").append(
                    $("<div>").addClass("shaft"),
                    $("<div>").addClass("triangle")
                ).on("click", this.rotateRight)
            ),
            $("<div>").addClass("lower").append(
                $("<div>").addClass("down").append(
                    $("<div>").addClass("shaft"),
                    $("<div>").addClass("triangle")
                ).on("click", this.rotateDown)
            ),
            $("<canvas>").addClass("printDirectionCanvas")
        );

        return container;
    }

    initXeogl() {
        if (this.model == null) return false;

        this.scene = new xeogl.Scene({
            canvas: "printDirectionCanvas",
            webgl2: true,
            transparent: true
        });

        xeogl.setDefaultScene(scene);

        this.group = new xeogl.Group({
            id: "printDirectionGroup",
            rotation: [0, 0, 0],
            position: [0, 0, 0],
            scale: [1, 1, 1],
            aabbVisible: true,
        });

        this.group.addChild(this.model);

        let camera = this.scene.camera;

        camera.eye = [-30, 0, 0];
        camera.look = [1, 0, 0];
        camera.up = [0, 1, 0];
    }

    rotateUp() {
        if (this.model == null) return;
        this.model.rotate(this.axes[0], -90);
    }

    rotateDown() {
        if (this.model == null) return;
        this.model.rotate(this.axes[0], 90);
    }

    rotateLeft() {
        if (this.model == null) return;
        this.model.rotate(this.axes[2], -90);
    }

    rotateRight() {
        if (this.model == null) return;
        this.model.rotate(this.axes[2], 90);
    }
}
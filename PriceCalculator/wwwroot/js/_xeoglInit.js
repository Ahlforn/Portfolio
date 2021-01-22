//import * as xeogl from "./../lib/xeogl/xeogl.module.js";

export class InitXeogl {
    constructor(updatePriceFunction) {
        const stlFile = $("#STLFile").val();
        this.printDirection = $("input[name=printDirection]");

        const scene = new xeogl.Scene({
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
                    rotation: this.PrintDirection,
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
                        diffuseMap: new xeogl.Texture({
                            src: "./../../images/UVCheckerMap11-1024.png",
                            scale: [-5.0, 5.0]
                        }),
                        xalpha: 0.99,
                        backfaces: false
                    }),
                    collidable: false,
                    pickable: false
                })
            ]
        })

        const model = group.childMap["model"];
        const plane = group.childMap["plane"];
        this.aabb = $("input[name=aabb]");

        this.scene = scene;
        this.model = model;
        this.plane = plane;

        // TODO: Restore saved part rotaton on load.
        model.on("loaded", () => {
            this.AABB = model.aabb;
            this.align(model, plane);
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

        //$(".printDirectionOption").on("change", function () {
        //    let direction = $(this).val();
        //    rotateToAxis(model, direction);
        //});

        $("#rotateUp").on("click", () => {
            this.rotate(model, 0, 90); // 0 = X-axis
            this.align(model, plane);
            this.PrintDirection = model.rotation;
            this.AABB = model.aabb;
            updatePriceFunction();
        })

        $("#rotateDown").on("click", () => {
            this.rotate(model, 0, -90); // 0 = X-axis
            this.align(model, plane);
            this.PrintDirection = model.rotation;
            this.AABB = model.aabb;
            updatePriceFunction();
        })

        $("#rotateLeft").on("click", () => {
            this.rotate(model, 1, 90); // 1 = Y-axis
            this.align(model, plane);
            this.PrintDirection = model.rotation;
            this.AABB = model.aabb;
            updatePriceFunction();
        })

        $("#rotateRight").on("click", () => {
            this.rotate(model, 1, -90); // 1 = Y-axis
            this.align(model, plane);
            this.PrintDirection = model.rotation;
            this.AABB = model.aabb;
            updatePriceFunction();
        })

        $("#translate").on("click", this.alignWithGroundPlane);
    };

    // axis is an integer (0 = x, 1 = y, z = 2)
    rotate(model, axis, degree) {
        let axes = [
            new Float32Array([1, 0, 0]),
            new Float32Array([0, 1, 0]),
            new Float32Array([0, 0, 1])
        ];
        axis = Number(axis);
        const d = xeogl.math.transformPoint3(xeogl.math.transposeMat4(model.worldMatrix), axes[axis]);

        model.rotate(d, degree);

        let euler


        console.log(model.rotation, xeogl.math.quaternionToEuler(model.quaternion, "XYZ"));
    };

    align(o1, o2) {
        o1._updateAABB();
        const d = xeogl.math.transformPoint3(xeogl.math.transposeMat4(o1.worldMatrix), new Float32Array([0, 1, 0]));
        const dist = (o1.aabb[1] - o2.aabb[4]) * -1;
        o1.translate(d, dist);
    };

    get ModelSnapshot() {
        return this.scene.canvas.getSnapshot({
            width: 500,
            height: 500,
            format: "png"
        });
    }

    set AABB(aabb) {
        const str = aabb[0] + "," + aabb[1] + "," + aabb[2] + "," + aabb[3] + "," + aabb[4] + "," + aabb[5];
        this.aabb.val(str);
    }

    set PrintDirection(eulerAngles) {
        const calc = (num) => num * (180 / Math.PI);
        let str = calc(eulerAngles[0]) + "," + calc(eulerAngles[1]) + "," + calc(eulerAngles[2]);
        this.printDirection.val(str);
    }

    get PrintDirection() {
        if (this.printDirection.val().length === 0)
            return null;
        
        const rotation = [0, 0, 0];
        const values = this.printDirection.val().split(",");

        rotation[0] = parseFloat(values[0]);
        rotation[1] = parseFloat(values[1]);
        rotation[2] = parseFloat(values[2]);

        return rotation;
    }
};

export default InitXeogl;
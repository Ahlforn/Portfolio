//import * as xeogl from "./../lib/xeogl/xeogl.module.js";

"use strict";

Object.defineProperty(exports, "__esModule", {
    value: true
});

var _createClass = (function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; })();

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

var InitXeogl = (function () {
    function InitXeogl(updatePriceFunction) {
        var _this = this;

        _classCallCheck(this, InitXeogl);

        var stlFile = $("#STLFile").val();
        this.printDirection = $("input[name=printDirection]");

        var scene = new xeogl.Scene({
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
            children: [new xeogl.STLModel({
                id: "model",
                src: stlFile,
                position: [0, 0, 0],
                rotation: this.PrintDirection,
                smoothNormals: true
            }), new xeogl.Mesh({
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
            })]
        });

        var model = group.childMap["model"];
        var plane = group.childMap["plane"];
        this.aabb = $("input[name=aabb]");

        this.scene = scene;
        this.model = model;
        this.plane = plane;

        // TODO: Restore saved part rotaton on load.
        model.on("loaded", function () {
            _this.AABB = model.aabb;
            _this.align(model, plane);
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
            visible: true
        });

        var axisHelper = $('canvas[id^="xeogl-axisHelper-canvas"]');
        axisHelper.parent().addClass("axisHelperParent");
        axisHelper.appendTo("#canvasContainer");
        axisHelper.addClass("axisHelper");
        axisHelper.attr("style", "");

        $(".axisHelperParent").remove();

        //$(".printDirectionOption").on("change", function () {
        //    let direction = $(this).val();
        //    rotateToAxis(model, direction);
        //});

        $("#rotateUp").on("click", function () {
            _this.rotate(model, 0, 90); // 0 = X-axis
            _this.align(model, plane);
            _this.PrintDirection = model.rotation;
            _this.AABB = model.aabb;
            updatePriceFunction();
        });

        $("#rotateDown").on("click", function () {
            _this.rotate(model, 0, -90); // 0 = X-axis
            _this.align(model, plane);
            _this.PrintDirection = model.rotation;
            _this.AABB = model.aabb;
            updatePriceFunction();
        });

        $("#rotateLeft").on("click", function () {
            _this.rotate(model, 1, 90); // 1 = Y-axis
            _this.align(model, plane);
            _this.PrintDirection = model.rotation;
            _this.AABB = model.aabb;
            updatePriceFunction();
        });

        $("#rotateRight").on("click", function () {
            _this.rotate(model, 1, -90); // 1 = Y-axis
            _this.align(model, plane);
            _this.PrintDirection = model.rotation;
            _this.AABB = model.aabb;
            updatePriceFunction();
        });

        $("#translate").on("click", this.alignWithGroundPlane);
    }

    _createClass(InitXeogl, [{
        key: "rotate",

        // axis is an integer (0 = x, 1 = y, z = 2)
        value: function rotate(model, axis, degree) {
            var axes = [new Float32Array([1, 0, 0]), new Float32Array([0, 1, 0]), new Float32Array([0, 0, 1])];
            axis = Number(axis);
            var d = xeogl.math.transformPoint3(xeogl.math.transposeMat4(model.worldMatrix), axes[axis]);

            model.rotate(d, degree);

            var euler = undefined;

            console.log(model.rotation, xeogl.math.quaternionToEuler(model.quaternion, "XYZ"));
        }
    }, {
        key: "align",
        value: function align(o1, o2) {
            o1._updateAABB();
            var d = xeogl.math.transformPoint3(xeogl.math.transposeMat4(o1.worldMatrix), new Float32Array([0, 1, 0]));
            var dist = (o1.aabb[1] - o2.aabb[4]) * -1;
            o1.translate(d, dist);
        }
    }, {
        key: "ModelSnapshot",
        get: function get() {
            return this.scene.canvas.getSnapshot({
                width: 500,
                height: 500,
                format: "png"
            });
        }
    }, {
        key: "AABB",
        set: function set(aabb) {
            var str = aabb[0] + "," + aabb[1] + "," + aabb[2] + "," + aabb[3] + "," + aabb[4] + "," + aabb[5];
            this.aabb.val(str);
        }
    }, {
        key: "PrintDirection",
        set: function set(eulerAngles) {
            var calc = function calc(num) {
                return num * (180 / Math.PI);
            };
            var str = calc(eulerAngles[0]) + "," + calc(eulerAngles[1]) + "," + calc(eulerAngles[2]);
            this.printDirection.val(str);
        },
        get: function get() {
            if (this.printDirection.val().length === 0) return null;

            var rotation = [0, 0, 0];
            var values = this.printDirection.val().split(",");

            rotation[0] = parseFloat(values[0]);
            rotation[1] = parseFloat(values[1]);
            rotation[2] = parseFloat(values[2]);

            return rotation;
        }
    }]);

    return InitXeogl;
})();

exports.InitXeogl = InitXeogl;
;

exports["default"] = InitXeogl;


// Limited type definitions for xeogl. AHB 2019

interface xeogl {

}

interface ComponentCfg {
    id?: String;
    meta?: String;
}

interface Component {
    new(cfg: ComponentCfg): Component;
    destroy(): void;
    error(message: String): void;
    fire(event: String, value: Object, forget: Boolean): void;
    hasSubs(event: String): Boolean;
    isType(type: String | Function): Boolean;
    log(message: String): void;
    off(subId: String): void;
    on(event: String, callback: Function, scope: Object): String;
    once(event: String, callback: Function, scope: Object): String;
    warn(message: String): void;
}

interface xeoglObjectCfg {
    id?: String;
    guid?: String;
    meta?: String;
    entityType: String;
    parent?: Object;
    position?: Float32Array;
    scale?: Float32Array;
    rotation?: Float32Array;
    matrix?: Float32Array;
    visible?: Boolean;
    culled?: Boolean;
    pickable?: Boolean;
    clippable?: Boolean;
    collidable?: Boolean;
    castShadow?: Boolean;
    receiveShadow?: Boolean;
    outlined?: Boolean;
    ghosted?: Boolean;
    highlighted?: Boolean;
    selected?: Boolean;
    edges?: Boolean;
    aabbVisible?: Boolean;
    colorize?: Float32Array;
    opacity?: Number;
    children?: Array<Object>;
    inheritStates?: Boolean;
}

interface xeoglObject extends Component {
    new(owner?: Component, cfg?: xeoglObjectCfg): Object;
    removeChild(object: xeoglObject): void;
    removeChildren(): void;
    rotate(angle: Number): void;
    rotateX(angle: Number): void;
}

interface SceneCfg {
    id?: String;
    meta?: String;
    canvasId?: String;
    webgl2?: Boolean;
    components?: Array<Component>;
    ticksPerRender?: Number;
    passes?: Number;
    clearEachPass?: Boolean;
    transparent?: Boolean;
    backgroundColor?: Float32Array;
    backgroundImage?: String;
    gammaInput?: Boolean;
    gammaOutput?: Boolean;
    gammaFactor?: Number;
}

interface PickParams {
    pickSurface?: Boolean;
    canvasPos?: Float32Array;
    origin: Float32Array;
    direction: Float32Array;
    includeMeshes: Array<Object>;
    excludeMeshes: Array<Object>;
}

interface Scene extends Component {
    new(cfg: SceneCfg): Scene;
    clear(): void;
    clearClips(): void;
    clearLights(): void;
    create(cfg: ComponentCfg): Component;
    getAABB(target: String): [Number, Number, Number, Number, Number, Number];
    pick(params: PickParams): Object | null;
    render(forceRender: Boolean);
    setColorize(ids: Array<String>, colorize: Float32Array): void;
    setEdges(ids: Array<String>, edges: Float32Array): Boolean;
    setGhosted(ids: Array<String>, ghosted: Float32Array): Boolean;
    setHighlighted(ids: Array<String>, highlighted: Boolean): Boolean;
    setOpacity(ids: Array<String>, opacity: Object): void;
    setOutlined(ids: Array<String>, outlined: Float32Array): Boolean;
    setPickable(ids: Array<String>, pickable: Float32Array): Boolean;
    setSelected(ids: Array<String>, selected: Boolean): Boolean;
    setVisible(ids: Array<String>, culled: Boolean): Boolean;
    setVisible(ids: Array<String>, visible: Boolean): Boolean;
    withObjects(ids: Array<String>, callback: Function): void;
}

interface xeogl {
    Component: Component;
    Object: xeoglObject;
    Scene: Scene;
}
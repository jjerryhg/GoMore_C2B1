
import * as OBC from "../node_modules/openbim-components/src/index.js"
//import * as  OBC.SimpleScene from "../node_modules/openbim-components/src/core/SimpleScene/index"
import * as THREE from "../node_modules/three/build/three.module.js"

const viewer = new OBC.Components()
viewer.onInitialized.add(() => { })

const sceneComponent = new OBC.SimpleScene(viewer)
sceneComponent.setup()
viewer.scene = sceneComponent

const viewerContainer = document.getElementById("app") 
const rendererComponent = new OBC.PostproductionRenderer(viewer, viewerContainer)
viewer.renderer = rendererComponent
const postproduction = rendererComponent.postproduction

const cameraComponent = new OBC.OrthoPerspectiveCamera(viewer)
viewer.camera = cameraComponent

const raycasterComponent = new OBC.SimpleRaycaster(viewer)
viewer.raycaster = raycasterComponent

viewer.init()
postproduction.enabled = true

const grid = new OBC.SimpleGrid(viewer, new THREE.Color(0x666666))
postproduction.customEffects.excludedMeshes.push(grid.get())

const ifcLoader = new OBC.FragmentIfcLoader(viewer)


const highlighter = new OBC.FragmentHighlighter(viewer)
highlighter.setup()

const propertiesProcessor = new OBC.IfcPropertiesProcessor(viewer)
highlighter.events.select.onClear.add(() => {
    propertiesProcessor.cleanPropertiesList()
})

ifcLoader.onIfcLoaded.add(async (model) => {
    propertiesProcessor.process(model);
    highlighter.events.select.onHighlight.add((selection) => {
        const fragmentID = Object.keys(selection)[0];
        const expressID = Number([...selection[fragmentID]][0]);
        propertiesProcessor.renderProperties(model, expressID);
    });
    highlighter.update();
});


const mainToolbar = new OBC.Toolbar(viewer)
mainToolbar.addChild(
    ifcLoader.uiElement.get("main"),
    propertiesProcessor.uiElement.get("main")
)
viewer.ui.addToolbar(mainToolbar);

//let fragmentIfcLoader = new OBC.FragmentIfcLoader(viewer);
//fragmentIfcLoader.settings.wasm = {
//    path: "https://unpkg.com/web-ifc@0.0.44/",
//    absolute: true
//}

const Scene = require('Scene');
const Patches = require('Patches');
const Reactive = require('Reactive');
const FaceTracking = require('FaceTracking');
const DeviceMotion = require('DeviceMotion');
const Diagnostics = require('Diagnostics');

var face = FaceTracking.face(0);
var bustA = Scene.root.find('bustA');
var bustB = Scene.root.find('bustB');
var bustC = Scene.root.find('bustC');

var bustAtra = bustA.transform.toSignal();
var bustBtra = bustB.transform.toSignal();

var bustCtra = bustC.transform.toSignal();
var faceTra = face.cameraTransform.applyTo(bustAtra).applyTo(bustBtra).applyTo(bustCtra);

const FaceOffset = Reactive.point(0,0,0.535);
var neckPos = faceTra.position.add(FaceOffset).expSmooth(70);
Patches.setVectorValue('neck',neckPos);

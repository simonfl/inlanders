import { Engine } from '@babylonjs/core/Engines/engine';
import { Scene } from '@babylonjs/core/scene';
import { ArcRotateCamera } from '@babylonjs/core/Cameras/arcRotateCamera';
import { Camera } from '@babylonjs/core/Cameras/camera';
import { Vector3, Matrix } from '@babylonjs/core/Maths/math.vector';
import { Color3, Color4 } from '@babylonjs/core/Maths/math.color';
import { HemisphericLight } from '@babylonjs/core/Lights/hemisphericLight';
import { DirectionalLight } from '@babylonjs/core/Lights/directionalLight';
import { ShadowGenerator } from '@babylonjs/core/Lights/Shadows/shadowGenerator';
import '@babylonjs/core/Lights/Shadows/shadowGeneratorSceneComponent';
import '@babylonjs/core/Culling/ray';
import { MeshBuilder } from '@babylonjs/core/Meshes/meshBuilder';
import { Mesh } from '@babylonjs/core/Meshes/mesh';
import { TransformNode } from '@babylonjs/core/Meshes/transformNode';
import { StandardMaterial } from '@babylonjs/core/Materials/standardMaterial';
import type { State, Cell, View, Person, Preview } from './types';

export class VillageView {
  engine: Engine; scene: Scene; camera: ArcRotateCamera;
  private mats = new Map<string, StandardMaterial>();
  private terrain: TransformNode; private fixtures: TransformNode; private preview: TransformNode;
  private villagers = new Map<number, { node: TransformNode; legs: Mesh[]; arms: Mesh[]; cargo: Mesh; goal: Vector3; last: Vector3; arrived: number; person: Person }>();
  private signature = ''; private mapKey = ''; private state?: State; private shadows: ShadowGenerator;
  private pointers = new Map<number, { x: number; y: number }>();
  private down?: { x: number; y: number; moved: boolean }; private pinch = 0;
  private labels: HTMLDivElement;
  onPick: (cell: Cell) => void = () => {};
  onViewChange: () => void = () => {};
  constructor(private canvas: HTMLCanvasElement, private host: HTMLElement) {
    this.engine = new Engine(canvas, true, { preserveDrawingBuffer: true, stencil: true }, false);
    // Babylon divides CSS size by this level. Use the reciprocal pixel density,
    // capped at 1.5x to balance phone sharpness with the cost of drawing more pixels.
    this.engine.setHardwareScalingLevel(1 / Math.min(1.5, Math.max(1, window.devicePixelRatio || 1)));
    this.scene = new Scene(this.engine); this.scene.clearColor = Color4.FromHexString('#b9d4cdff');
    this.camera = new ArcRotateCamera('village', Math.PI / 2, .63, 42, new Vector3(-1, 0, 0), this.scene);
    this.camera.mode = Camera.ORTHOGRAPHIC_CAMERA; this.setZoom(this.defaultZoom());
    const light = new HemisphericLight('sky', new Vector3(0, 1, 0), this.scene); light.intensity = .6;
    light.groundColor = Color3.FromHexString('#88775a');
    const sun = new DirectionalLight('sun', new Vector3(-.5, -1, .45), this.scene); sun.position = new Vector3(15, 28, -15); sun.intensity = .65;
    this.shadows = new ShadowGenerator(1024, sun); this.shadows.useBlurExponentialShadowMap = true; this.shadows.blurKernel = 8; this.shadows.darkness = .28;
    this.terrain = new TransformNode('terrain', this.scene); this.fixtures = new TransformNode('village', this.scene); this.preview = new TransformNode('preview', this.scene);
    this.labels = document.createElement('div'); this.labels.className = 'world-labels'; host.append(this.labels);
    this.inputs();
    new ResizeObserver(() => { this.engine.resize(); this.setZoom(this.zoom); }).observe(host);
    this.engine.runRenderLoop(() => { this.animate(); this.scene.render(); this.placeLabels(); });
  }
  zoom = 19;
  setZoom(value: number) {
    this.zoom = Math.max(7, Math.min(26, value));
    const aspect = this.canvas.clientWidth / Math.max(1, this.canvas.clientHeight);
    // Keep a usable horizontal view in portrait, instead of cropping both shores.
    const halfWidth = this.zoom * Math.max(.7, aspect);
    this.camera.orthoLeft = -halfWidth; this.camera.orthoRight = halfWidth;
    this.camera.orthoTop = this.zoom; this.camera.orthoBottom = -this.zoom;
  }
  private defaultZoom() { return this.canvas.clientWidth / Math.max(1, this.canvas.clientHeight) > 1.4 ? 12 : 19; }
  home() { this.camera.target.set(-1, 0, 0); this.setZoom(this.defaultZoom()); this.onViewChange(); }
  focus(c: Cell) { this.camera.target.set(c.x, 0, c.z); this.setZoom(11); this.onViewChange(); }
  getView(): Pick<View, 'cameraX' | 'cameraZ' | 'zoom'> { return { cameraX: this.camera.target.x, cameraZ: this.camera.target.z, zoom: this.zoom }; }
  restore(v: View) { this.camera.target.set(v.cameraX, 0, v.cameraZ); this.setZoom(v.zoom); }
  private material(hex: string) {
    if (!this.mats.has(hex)) { const m = new StandardMaterial(hex, this.scene); m.diffuseColor = Color3.FromHexString(hex); m.specularColor.set(.06, .06, .06); this.mats.set(hex, m); }
    return this.mats.get(hex)!;
  }
  private box(name: string, x: number, y: number, z: number, w: number, h: number, d: number, color: string, parent: TransformNode, shadow = true) {
    const mesh = MeshBuilder.CreateBox(name, { width: w, height: h, depth: d }, this.scene);
    mesh.position.set(x, y, z); mesh.parent = parent; mesh.material = this.material(color); mesh.isPickable = false; mesh.receiveShadows = true;
    if (shadow) this.shadows.addShadowCaster(mesh); return mesh;
  }
  private cone(name: string, x: number, y: number, z: number, diameter: number, height: number, color: string, parent: TransformNode, top = 0) {
    const mesh = MeshBuilder.CreateCylinder(name, { diameterBottom: diameter, diameterTop: top, height, tessellation: 7 }, this.scene);
    mesh.position.set(x, y, z); mesh.parent = parent; mesh.material = this.material(color); mesh.isPickable = false; this.shadows.addShadowCaster(mesh); return mesh;
  }
  update(state: State) {
    this.state = state;
    if (this.mapKey !== state.id) {
      this.mapKey = state.id; this.terrain.dispose(false, false); this.terrain = new TransformNode('terrain', this.scene);
      const water = new Set(state.map.water.map(c => `${c.x},${c.z}`)), excluded = new Set(state.map.excluded.map(c => `${c.x},${c.z}`));
      for (let x = state.map.minX; x < state.map.minX + state.map.width; x++) for (let z = state.map.minZ; z < state.map.minZ + state.map.depth; z++) {
        if (excluded.has(`${x},${z}`)) continue;
        const wet = water.has(`${x},${z}`), colors = wet ? ['#70aeb2', '#75b1b4'] : ['#a7b77c', '#a2b378', '#acba81', '#a8b67d'];
        this.box('ground', x, wet ? -.14 : -.08, z, 1.005, wet ? .12 : .2, 1.005, colors[Math.abs(x * 7 + z * 13) % colors.length], this.terrain, false);
      }
      const merged = Mesh.MergeMeshes(this.terrain.getChildMeshes() as Mesh[], true, true, undefined, false, true);
      if (merged) { merged.parent = this.terrain; merged.isPickable = false; merged.receiveShadows = true; }
      const ground = MeshBuilder.CreateGround('picking-ground', { width: 100, height: 100 }, this.scene); ground.visibility = 0; ground.isPickable = true; ground.parent = this.terrain;
      this.signature = '';
    }
    const sig = JSON.stringify([state.trees.map(t => [t.id, t.felled, Math.round(t.growth * 5), t.managed]), state.buildings.map(b => [b.id, b.complete, Math.floor(b.construction * 4), Math.floor(b.growth * 5), b.harvest])]);
    if (sig !== this.signature) {
      this.signature = sig; this.fixtures.dispose(false, false); this.fixtures = new TransformNode('fixtures', this.scene);
      for (const t of state.trees) {
        this.cone('trunk', t.x, .35, t.z, .25, .7, '#795d42', this.fixtures, .19);
        if (!t.felled) for (let j = 0; j < 3; j++) this.cone('pine', t.x, .85 + j * .55, t.z, (1.7 - j * .35) * Math.max(.35, t.growth), 1.2, t.managed ? '#4b7652' : '#376455', this.fixtures);
        if (t.managed) this.box('grove-sign', t.x + .45, .12, t.z + .45, .24, .24, .24, '#edd39c', this.fixtures);
      }
      for (const b of state.bushes) this.cone('bush', b.x, .35, b.z, .9, .65, '#66825a', this.fixtures, .6);
      const yard = state.map.stockpile;
      this.box('timber-yard', yard.x, .04, yard.z, 1.5, .1, 1.5, '#99876b', this.fixtures);
      for (let i = 0; i < 4; i++) this.box('logs', yard.x, .18 + (i % 2) * .2, yard.z + Math.floor(i / 2) * .3 - .3, 1.2, .19, .23, '#916747', this.fixtures);
      for (const b of state.buildings) {
        const cx = b.footprint.reduce((n, c) => n + c.x, 0) / b.footprint.length, cz = b.footprint.reduce((n, c) => n + c.z, 0) / b.footprint.length;
        const root = new TransformNode(`building-${b.id}`, this.scene); root.parent = this.fixtures; root.position.set(cx, 0, cz); root.rotation.y = -b.rotation * Math.PI / 2;
        if (b.kind === 'Cottage') {
          this.box('stone-footing', 0, .12, 0, 3, .3, 2, '#8c8a75', root);
          this.box('plaster', 0, .83, 0, 2.85, 1.25, 1.9, '#e0d2ac', root);
          for (const side of [-1, 1]) { const roof = this.box('steep-roof', 0, 1.8, side * .52, 3.3, .13, 1.55, '#66534b', root); roof.rotation.x = side * .63; }
          this.box('door', -.45, .55, .97, .42, 1, .07, '#6b5142', root);
          this.box('window', .65, .88, .98, .43, .44, .08, '#e6b86f', root);
          this.box('chimney', 1, 2, -.3, .35, .85, .4, '#aaa08b', root);
        } else if (b.kind === 'Bridge') {
          this.box('bridge-deck', 0, .08, 0, .9, .18, 2.6, b.complete ? '#ac8458' : '#d8c7a1', root);
          for (const side of [-1, 1]) { this.box('handrail', side * .48, .65, 0, .08, .1, 2.5, '#80634b', root); for (const z of [-1, 0, 1]) this.box('post', side * .48, .35, z, .1, .7, .1, '#80634b', root); }
        } else {
          for (const c of b.footprint) this.box('garden-soil', c.x - cx, .05, c.z - cz, .9, .12, .9, b.complete ? '#82654c' : '#b3a381', root);
          // Crop positions use world footprint; keep their root unrotated to match valid cells.
          root.rotation.y = 0;
          if (b.complete) for (const c of b.footprint) for (const dx of [-.24, .24]) {
            const height = b.harvest > 0 ? .45 : .12 + b.growth * .33;
            this.cone('crops', c.x - cx + dx, height / 2 + .1, c.z - cz, .3, height, b.harvest > 0 ? '#c5ad5b' : '#5c8450', root, .14);
          }
          for (const x of [-1.55, 1.55]) this.box('garden-stake', x, .25, 0, .08, .5, .08, '#baa079', root);
        }
        if (!b.complete) {
          for (const c of b.footprint) this.box('plan-outline', c.x, .12, c.z, .92, .05, .92, '#e8ce8a', this.fixtures, false);
          this.box('construction-marker', cx, .6, cz, .12, 1.2, .12, '#82694c', this.fixtures);
        }
      }
    }
    for (const p of state.people) {
      let v = this.villagers.get(p.id);
      if (!v) {
        const n = new TransformNode(`person-${p.id}`, this.scene); n.position.set(p.x, 0, p.z);
        const colors = ['#9b5448', '#506a80', '#d1b181', '#60755a'];
        this.cone('coat', 0, .55, 0, .43, .54, colors[p.id % 4], n, .3);
        this.cone('head', 0, .99, 0, .24, .27, '#d4a47c', n, .22);
        this.cone('cap', 0, 1.15, 0, .3, .15, p.id % 2 ? '#4b5052' : '#e0d3b4', n, .22);
        const legs = [-1, 1].map(k => this.box('leg', k * .09, .18, 0, .13, .36, .15, '#4d4942', n));
        const arms = [-1, 1].map(k => this.box('arm', k * .24, .6, 0, .12, .43, .12, '#d0bb94', n));
        const cargo = this.box('basket', 0, .53, .32, .38, .28, .29, '#af8653', n);
        v = { node: n, legs, arms, cargo, goal: n.position.clone(), last: n.position.clone(), arrived: performance.now(), person: p }; this.villagers.set(p.id, v);
      }
      if (v.goal.x !== p.x || v.goal.z !== p.z) { v.last.copyFrom(v.node.position); v.goal.set(p.x, 0, p.z); v.arrived = performance.now(); }
      v.person = p; v.cargo.setEnabled(p.carried > 0);
    }
    this.labels.replaceChildren();
    for (const item of [{ label: 'Ripe harvest', x: -7, z: -8 }, { label: state.buildings.some(b => b.kind === 'Bridge' && b.complete) ? 'Crossing restored' : 'Washed-out crossing', x: 0, z: -3 }]) {
      const label = document.createElement('span'); label.textContent = item.label; label.dataset.x = String(item.x); label.dataset.z = String(item.z); this.labels.append(label);
    }
  }
  private animate() {
    for (const v of this.villagers.values()) {
      const t = Math.min(1, (performance.now() - v.arrived) / 400);
      Vector3.LerpToRef(v.last, v.goal, t, v.node.position);
      const delta = v.goal.subtract(v.last); const moving = delta.lengthSquared() > .001 && t < 1;
      if (moving) v.node.rotation.y = Math.atan2(delta.x, delta.z);
      const phase = (this.state?.seconds ?? 0) * 7 + v.person.id;
      const swing = !this.state?.paused && moving ? Math.sin(phase) * .55 : 0;
      v.legs[0].rotation.x = swing; v.legs[1].rotation.x = -swing;
      v.arms[0].rotation.x = v.person.carried ? -.9 : -swing; v.arms[1].rotation.x = v.person.carried ? -.9 : swing;
      if (!this.state?.paused && ['Chopping', 'Building', 'Harvesting', 'Planting'].includes(v.person.task)) v.arms[0].rotation.x = -1 + Math.sin(phase) * .6;
    }
  }
  showPreview(p: Preview | null) {
    this.preview.dispose(false, false); this.preview = new TransformNode('preview', this.scene);
    if (!p) return;
    for (const c of p.cells) this.box('preview-tile', c.x, .2, c.z, .95, .08, .95, p.valid ? '#eed27b' : '#ca7562', this.preview, false);
    if (p.entrance) this.box('entrance', p.entrance.x, .21, p.entrance.z, .5, .1, .5, '#f8f1d6', this.preview, false);
  }
  private placeLabels() {
    for (const label of this.labels.children) {
      const el = label as HTMLElement, point = Vector3.Project(new Vector3(Number(el.dataset.x), .5, Number(el.dataset.z)), Matrix.Identity(), this.scene.getTransformMatrix(), this.camera.viewport.toGlobal(this.canvas.clientWidth, this.canvas.clientHeight));
      el.style.left = `${point.x}px`; el.style.top = `${point.y}px`;
    }
  }
  private inputs() {
    const c = this.canvas;
    c.addEventListener('pointerdown', e => { c.setPointerCapture(e.pointerId); this.pointers.set(e.pointerId, { x: e.clientX, y: e.clientY }); this.down = { x: e.clientX, y: e.clientY, moved: this.pointers.size > 1 }; if (this.pointers.size === 2) { const [a, b] = [...this.pointers.values()]; this.pinch = Math.hypot(a.x - b.x, a.y - b.y); } });
    c.addEventListener('pointermove', e => {
      const previous = this.pointers.get(e.pointerId); if (!previous) return;
      this.pointers.set(e.pointerId, { x: e.clientX, y: e.clientY });
      if (this.pointers.size === 2) {
        const [a, b] = [...this.pointers.values()], distance = Math.hypot(a.x - b.x, a.y - b.y);
        if (this.pinch > 0 && distance > 0) this.setZoom(this.zoom * this.pinch / distance); this.pinch = distance;
        if (this.down) this.down.moved = true;
      } else if (this.down && Math.hypot(e.clientX - this.down.x, e.clientY - this.down.y) > 6) {
        this.down.moved = true; const scale = this.zoom * 2 / c.clientHeight;
        this.camera.target.x += (e.clientX - previous.x) * scale;
        this.camera.target.z -= (e.clientY - previous.y) * scale / Math.cos(.63);
        this.camera.target.x = Math.max(-18, Math.min(18, this.camera.target.x)); this.camera.target.z = Math.max(-18, Math.min(18, this.camera.target.z));
      }
    });
    c.addEventListener('pointerup', e => {
      const tap = this.down && !this.down.moved && this.pointers.size === 1;
      this.pointers.delete(e.pointerId); this.pinch = 0;
      if (tap) {
        const rect = c.getBoundingClientRect(), ray = this.scene.createPickingRay(e.clientX - rect.left, e.clientY - rect.top, Matrix.Identity(), this.camera);
        const distance = -ray.origin.y / ray.direction.y, point = ray.origin.add(ray.direction.scale(distance));
        this.onPick({ x: Math.round(point.x), z: Math.round(point.z) });
      }
      if (!this.pointers.size) { this.down = undefined; this.onViewChange(); }
    });
    c.addEventListener('pointercancel', () => { this.pointers.clear(); this.down = undefined; this.pinch = 0; });
    c.addEventListener('wheel', e => { e.preventDefault(); this.setZoom(this.zoom * (1 + Math.sign(e.deltaY) * .08)); this.onViewChange(); }, { passive: false });
  }
}

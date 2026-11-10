export type Cell = { x: number; z: number };
export type Building = Cell & { id: number; kind: string; name: string; rotation: number; complete: boolean; construction: number; delivered: number; required: number; growth: number; harvest: number; footprint: Cell[]; entrance: Cell; status: string };
export type Person = Cell & { id: number; name: string; status: string; task: string; fed: boolean; carried: number; cargo: string; freshMeal: boolean };
export type View = { active: boolean; panel: string; selection: string | null; cameraX: number; cameraZ: number; zoom: number; width: number; height: number; message?: string; previewCells?: Cell[]; previewValid?: boolean; goalsExpanded?: boolean };
export type State = {
  id: string; build: string; sequence: number; step: number; seconds: number; paused: boolean; speed: number; title: string; goalTitle: string; scenario: string; notice: string; complete: boolean;
  river: { boats: number; closed: boolean; targetBeds: number; reserveTarget: number; delivered: number; needed: number; sampleSeconds: number; untilEvent: number; timetable: string; earlyLabel: string; earlyProblem: string | null; stableSeconds: number; supply: string; phase: string } | null;
  resources: { food: number; logs: number; population: number; fed: number; housed: number; beds: number };
  objectives: string[]; hint: string; nextAction: string;
  map: { minX: number; minZ: number; width: number; depth: number; water: Cell[]; excluded: Cell[]; stockpile: Cell; stone: (Cell & { access: Cell })[]; clearings: Cell[] };
  trees: (Cell & { id: number; logs: number; felled: boolean; growth: number; managed: boolean; preserved: boolean })[];
  bushes: (Cell & { id: number })[]; buildings: Building[]; people: Person[]; events: string[]; view: View;
};
export type Order = { type: string; x?: number; z?: number; x2?: number; z2?: number; rotation?: number; siteId?: number; seconds?: number; untilEvent?: boolean; speed?: number; note?: string; view?: View };
export type Preview = { valid: boolean; problem: string | null; cells: Cell[]; entrance?: Cell; logs: number; label: string };
export type Frame = { sequence: number; utc: string; type: string; data: { message?: string; order?: Order } | null; state: State };

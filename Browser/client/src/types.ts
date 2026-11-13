export type Cell = { x: number; z: number };
export type Building = Cell & { id: number; kind: string; name: string; rotation: number; complete: boolean; construction: number; delivered: number; required: number; growth: number; harvest: number; footprint: Cell[]; entrance: Cell; launch: Cell | null; fishing: string | null; boat: { x: number; z: number; heading: number; phase: string; fisherId: number | null; fish: number } | null; status: string };
export type Person = Cell & { id: number; name: string; status: string; task: string; fed: boolean; carried: number; cargo: string; freshMeal: boolean; aboard: boolean; heading: number; homeId: number | null; route: Cell[] };
export type View = { active: boolean; panel: string; selection: string | null; cameraX: number; cameraZ: number; zoom: number; width: number; height: number; message?: string; previewCells?: Cell[]; previewValid?: boolean; goalsExpanded?: boolean };
export type State = {
  id: string; build: string; sequence: number; step: number; seconds: number; paused: boolean; speed: number; title: string; goalTitle: string; scenario: string; notice: string; complete: boolean;
  riverside: { phase: string; title: string; objectives: string[]; hint: string; arrivalIn: number; welcomeProblem: string | null; shortcut: Cell; shortcutBuilt: boolean; walkNow: number; walkWithShortcut: number; freshMeals: number; delivered: number; woodland: number } | null;
  river: { boats: number; closed: boolean; targetBeds: number; reserveTarget: number; delivered: number; needed: number; sampleSeconds: number; untilEvent: number; timetable: string; earlyLabel: string; earlyProblem: string | null; stableSeconds: number; supply: string; phase: string } | null;
  trade: { decision: string; decidedAt: number; packedLogs: number; arrived: boolean; nearHarvest: boolean; offerEnabled: boolean; cost: number; food: number; availableLogs: number; problem: string | null; departureIn: number; arrivalIn: number; crop: string } | null;
  resources: { food: number; logs: number; population: number; fed: number; housed: number; beds: number };
  objectives: string[]; hint: string; nextAction: string;
  map: { minX: number; minZ: number; width: number; depth: number; water: Cell[]; excluded: Cell[]; stockpile: Cell; fishing: (Cell & { id: number; name: string; available: number; capacity: number; renewal: number })[]; stone: (Cell & { access: Cell })[]; clearings: Cell[] };
  trees: (Cell & { id: number; logs: number; felled: boolean; growth: number; managed: boolean; preserved: boolean })[];
  bushes: (Cell & { id: number })[]; buildings: Building[]; people: Person[]; events: string[]; view: View;
};
export type Order = { type: string; x?: number; z?: number; x2?: number; z2?: number; rotation?: number; siteId?: number; seconds?: number; untilEvent?: boolean; speed?: number; note?: string; view?: View };
export type Preview = { route?: Cell[]; shortcutRoute?: Cell[]; valid: boolean; problem: string | null; cells: Cell[]; entrance?: Cell; launch?: Cell; logs: number; label: string };
export type Frame = { sequence: number; utc: string; type: string; data: { message?: string; order?: Order } | null; state: State };

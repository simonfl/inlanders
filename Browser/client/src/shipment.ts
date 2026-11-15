import type { State } from './types';

// Shared wording for the boat and cargo strip; sending remains server-authoritative.
export function shipmentStatus(s: State): { caption: string; detail: string } {
  const story = s.story;
  if (!story) return { caption: '', detail: '' };
  if (story.departed) return { caption: 'Help on its way', detail: '' };
  if (story.manifest === 'none') return { caption: s.scenario === 'harvest-relief' ? 'Prepare relief' : 'Cargo boat', detail: '' };
  if (story.packedLogs < story.logTarget || story.packedFood < story.foodTarget)
    return { caption: 'Packing supplies', detail: '' };
  if (!story.sendProblem) return { caption: 'Ready to send', detail: 'Ready to send' };
  if (s.resources.housed < 8) return { caption: 'Homes needed', detail: `Neighbors housed ${s.resources.housed}/8` };
  if (s.resources.fed < s.resources.population) return { caption: 'Neighbors need a meal', detail: `Neighbors fed ${s.resources.fed}/${s.resources.population}` };
  if (s.resources.food < story.foodFloor) return { caption: 'Home food needed', detail: `Food at home ${s.resources.food}/${story.foodFloor}` };
  return { caption: 'Review cargo', detail: story.sendProblem };
}

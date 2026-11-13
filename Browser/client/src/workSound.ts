import type { State, View } from './types';

export type WorkSound = 'wood' | 'hammer' | 'soil' | 'step' | 'water';
// Shared PCM generator: the browser and the developer audition use the same samples.
export function workSamples(kind: WorkSound, rate: number, seed = 17): Float32Array {
  const seconds = kind === 'water' ? 4 : kind === 'wood' ? .24 : .17;
  const samples = new Float32Array(Math.ceil(rate * seconds));
  let low = 0, smooth = 0;
  for (let i = 0; i < samples.length; i++) {
    seed = (Math.imul(seed, 1664525) + 1013904223) | 0;
    const noise = (seed >>> 0) / 2147483648 - 1, t = i / rate;
    low = .95 * low + .05 * noise; smooth = .998 * smooth + .002 * noise;
    if (kind === 'water') {
      // Soft filtered movement without sharp clicks; equal endpoints for looping.
      const edge = Math.min(1, t / .15, (seconds - t) / .15);
      samples[i] = (low * .1 + smooth * .5) * edge;
    } else {
      const attack = Math.min(1, t / .004), decay = Math.exp(-t * (kind === 'soil' ? 30 : 38));
      const tone = Math.sin(2 * Math.PI * (kind === 'wood' ? 155 : kind === 'hammer' ? 290 : 85) * t);
      samples[i] = attack * decay * (kind === 'soil' ? low * .7 : kind === 'step' ? low * .35 : tone * .13 + noise * .09);
    }
  }
  return samples;
}

export class VillageSound {
  private buffers = new Map<WorkSound, AudioBuffer>();
  private river?: AudioBufferSourceNode;
  private riverGain?: GainNode;
  private last = 0;
  private turn = 0;
  constructor(private context: AudioContext) {}
  private buffer(kind: WorkSound) {
    let buffer = this.buffers.get(kind);
    if (!buffer) {
      const samples = workSamples(kind, this.context.sampleRate);
      buffer = this.context.createBuffer(1, samples.length, this.context.sampleRate);
      buffer.getChannelData(0).set(samples); this.buffers.set(kind, buffer);
    }
    return buffer;
  }
  quiet() { this.riverGain?.gain.setTargetAtTime(0, this.context.currentTime, .12); }
  update(state: State, view: Pick<View, 'cameraX' | 'cameraZ'>, enabled: boolean) {
    if (!enabled || state.paused || document.hidden) { this.quiet(); return; }
    if (!this.river) {
      this.river = this.context.createBufferSource(); this.river.buffer = this.buffer('water'); this.river.loop = true;
      this.riverGain = this.context.createGain(); this.riverGain.gain.value = 0;
      this.river.connect(this.riverGain); this.riverGain.connect(this.context.destination); this.river.start();
    }
    this.riverGain!.gain.setTargetAtTime(.18, this.context.currentTime, .4);
    // Wall-clock pacing deliberately does not multiply at 3x/6x.
    if (performance.now() - this.last < 850) return;
    const working = state.people.filter(p => Math.hypot(p.x - view.cameraX, p.z - view.cameraZ) < 13 &&
      ['Chopping', 'Building', 'Harvesting', 'Planting', 'ClearingStump', 'ToStockpile', 'ToPantry'].includes(p.task));
    if (!working.length) return;
    const person = working[this.turn++ % working.length];
    const kind: WorkSound = person.task === 'Chopping' ? 'wood' : person.task === 'Building' ? 'hammer' : person.task.startsWith('To') ? 'step' : 'soil';
    const source = this.context.createBufferSource(), gain = this.context.createGain();
    source.buffer = this.buffer(kind); gain.gain.value = .26;
    source.connect(gain); gain.connect(this.context.destination); source.onended = () => { source.disconnect(); gain.disconnect(); }; source.start(); this.last = performance.now();
  }
}

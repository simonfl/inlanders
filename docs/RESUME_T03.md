# T03 — reliable settlement resume

September 13, 2026. No new playable checkpoint (15 remains current).

The recorded failure `artifacts/review/runs/20260913-142448-532-resume-c37009` proves the next-settlement button activated, but saving refused to finish: “Unable to remove the file to be replaced.” The game correctly kept the village open. A prior probe compared the unchanged in-memory world and falsely accepted navigation. File contention is demonstrated; attributing the lock to Dropbox is not established.

World and campaign saves now use an owned, unique temporary file and overwrite rename, with bounded retry for Windows sharing/access contention. Permanent failure preserves the previous committed save and propagates to the UI. Obsolete backup creation is removed; saves need no compatibility support. Controlled tests cover a released lock, a persistent lock, temporary-file cleanup and 40 exact replacements.

Review controls now deliver viewport-local input synchronously and record activation, menu/HUD, focus and geometry. This makes diagnostics deterministic; input dispatch was not the demonstrated save fix. Resume checks require the active HUD, paused neighborhood and exact world. Captures include UI error/notice and a semantic Markdown index.

Eight successive completed-settlement transitions pass at 960 and 1440 in `20260913-143205-727-resume-1d7f60` and `20260913-143211-182-resume-58e2b5`. These are rendered scripted controls, not native human play. The gathering scenario additionally exercises F5/F9 and the completed-flow transition.

Next: F28a, primary inspector hierarchy. The review index addresses evidence retrieval; no general replay framework is justified.

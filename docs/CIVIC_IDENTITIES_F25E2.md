# Civic identities — F25e2

September 12, 2026. Select a planned or built gathering hall and expand **Identity** to choose **Hall**, **Chapel** or **Planted court**. The default remains Hall. Choices are free and saved per venue. Dismantling retains the identity and disables changes.

All three remain gathering halls: 8 planks + 12 stone, the same footprint and entrance, eight recreation places, twelve-second visits, a two-minute interval and four-minute service memory. Campaign hall conditions still recognize them. No new religion/learning need, resource, technology or building kind was added.

The chapel has masonry footings and buttresses, plaster walls, recessed arched openings, a steep roof and raised bell gable. The court has low capped walls, an open timber shade, planted borders and an ornamental tree. Planting exists only in the visual model; it cannot produce timber or support wildlife. Neither model contains permanent visitors or seats that imply people occupy blocked ground. Both use the existing material batching and construction-stage system.

## Matched views

[Ordinary hall](images/civic-hall.png) · [Chapel](images/civic-chapel.png) · [Planted court](images/civic-court.png) · [Mixed neighborhood](images/civic-neighborhood.png) · [Construction stages](images/civic-stages.png) · [960px inspector](images/civic-inspector.png).

The three occupied-venue captures share the same paused simulation, camera and daylight. The chapel's bell was raised after visual inspection to separate it from the roof silhouette. No bell audio or new quiet-visit animation is included yet.

## Verification

`./Play.ps1 -CivicIdentitySmokeTest` builds cleanly and checks normal material delivery/construction, all four orientations, actual recreation visitors outside the footprint, every identity and inspector changes at 960/1440. Changing and resetting identity returns the exact prior world snapshot. Parallel forty-second simulation continuations, differing only by identity, become identical when the choice is reset: service, work, resources, trees, assignments and routes are unaffected.

Current saves roundtrip exactly, including ongoing visits and dismantling. Missing/non-hall/invalid identity commands are rejected; invalid enum and non-hall identity values in saves are rejected too. Canceling a plan or dismantling a venue removes its identity-edit target. The sheet shows all three models at all four construction stages; the ordinary hall model is retained.

`./Test.ps1 -CivicBudget` also passes: the four existing-system budget/service routes and partial-build pause/cancel/salvage/rebuild probe retain their previous results. The broader simulation suite was not rerun for this appearance change. No migration layer was added.

## Roadmap review

F25e2 is delivered. Promote **F25e3 — quiet civic visits**: make actual chapel/court breaks visually quieter while preserving the hall's social behavior and every existing service rule. Assess a restrained arrival cue only against actual visits, pause/reload and positional/mute behavior. Keep decorative crowds, repeated ceremonies and new need meters out of the models.

The learning/restoration concept remains revised, not implemented. Architectural appeal, human campaign enjoyment, further audio, management UI and the remaining roadmap retain their open scope.

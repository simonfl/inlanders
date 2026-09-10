# Woodland wildlife — F26c

Implemented resource prototype on fresh Three clearings maps. The living woods campaign scenario is still future work. This adds a reason to retain mature woodland while giving hunters a real food delivery loop.

## What the player gets

- Two authored habitat patches centered at (-8, -8) and (3, 11). Mature resource trees within five tiles contribute two stock capacity and 0.5 game/minute each, capped at twelve stock and three game/minute. Decorative trees do not count. Starting wildlife uses the existing mature trees; no extra forest is silently planted.
- A six-log hunting lodge with one hunter. It needs level ground and reachable wooded habitat within eight tiles. Hunters reserve up to two game from the shared patch, walk to its tracking clearing, hunt for ten work seconds and carry the actual load to the central pantry. Additional lodges do not create additional habitat stock.
- Game joins meals, edible reserves, actual-meal variety, recent deliveries, inventory and carried supply routes. Three balanced food types still suffice for full variety; game is optional. No butcher, leather, weapons inventory, mandatory cooking chain or new need meter.
- Existing pause and output-target controls manage hunting. Current outings finish when paused. Reassignment or lodge removal releases uncollected claims; carried food still returns physically. An outing reserved before tree loss can finish, but clearing cannot support new hunting above the reduced capacity.
- Hunting reduces stock, which recovers when trees remain. Felling lowers capacity and replenishment; excess unclaimed stock leaves the habitat. Replanting restores support only after trees mature. These are separate, recoverable consequences. Existing tree preservation and managed planting remain useful.

The tracking clearing is a fixed walkable destination that building/planting tools must keep open. It is not a new building or a claim over the entire patch. Habitat is an authored region whose productivity follows nearby mature tree cover, not a simulation of connected forest ecology.

## Presentation and feedback

The lodge has an open timber front, green pitched roof, log back wall and equipment rack. Hunters use a bow pose; food travels as tied parcels. A few deer graze and turn their heads near each habitat, with their visible number responding to its stock. These animals represent habitat activity; they are not individually tracked prey with autonomous routes or one-to-one food quantities. No combat or gore.

Markers show available stock/capacity; lodge placement and inspection show mature trees and recovery. Survey boundaries appear while placing a lodge or editing woodland. Clearing loss is shown in the on-map preview before clicking, including capacity and recovery changes. The first rendered review found that putting this only in the Build description hid it at 960px, so it was moved into the visible preview.

Animal poses follow village time and freeze on pause. Static animal pieces are batched by material while the head remains movable. Cargo and current habitat state reconstruct from format-30 saves; no old-save migration.

## Balance experiment and limits

A matched ten-minute Creative fixture with five mature trees compares one hunter against that same hunter plus one vegetable garden. Creative isolates output and avoids hunger penalties; it does not establish sustainable normal-play staffing or campaign duration.

| Layout | Setup materials | Workers | Delivered game | Delivered vegetables |
| --- | --- | --- | --- | --- |
| Hunting lodge | 6 logs | 1 | 33 | 0 |
| Lodge + garden | 10 logs | 2 | 33 | 38 |

The habitat finishes near 1.4/10 stock: the hunter consumes the opening reserve and thereafter depends on replenishment. Eight residents need 80 portions over ten minutes, so these layouts still need other food or reserves. This establishes hunting as a limited supplement, not a universally better garden. The next authored woodland scenario should make preserving trees versus opening useful building land an actual choice; this free-play prototype does not prove that scenario's difficulty or enjoyment.

Separate checks establish that two lodges cannot duplicate an odd three-unit stock, pausing restores stock, total clearing stops recovery, saplings give no instant replacement and mature regrowth restores hunting. A committed hunt remains valid through clearing. Game-only meals nourish residents, and a balanced game/vegetable/berry meal earns the existing variety benefit.

## Verification and next step

`./Test.ps1` includes physical construction and delivery, normal meals, exact hunting/recovery saves, interruption, stock targets, lodge removal, shared claims, active clearing, depletion and regrowth. All existing campaigns, construction, storage and food checks pass. The broad HUD suite passes with the added role/resource/building.

`./Play.ps1 -WildlifeSmokeTest` checks the model, animals, bow, real parcels, pause/reload, source/clearing feedback and inventory at 960/1440; captures are under `artifacts/f26c-*`. It also measures a decorated 16-resident scene and checks that a 600-frame paused survey neither grows nodes nor changes the save. Visual appeal and normal-play balance remain player reviews.

Local 1440px rendering sample, with 16 residents and 36 decorations: wildlife visible **38.4 ms median / 44.3 ms p95, 1,157 draws**; the same scene with wildlife hidden **37.5 / 39.6 ms, 1,083 draws**. A 600-frame paused survey measured **39.0 / 41.3 ms, 1,257 draws**, with stable nodes and unchanged save. These short samples show the added rendering cost, not a general frame-rate guarantee. Performance remains a constraint before increasing population or scene density; 24 residents remain unverified.

**F21j direct map resource survey is now implemented**: compare habitats, outcrops and fishing grounds without entering their building tools. See the [survey review](RESOURCE_SURVEY_F21J.md); small neighborhood seating is the next candidate. Keep The living woods and Built to last separate from their prototypes and incorporate human river/lake pacing feedback first. Broader building art still needs F23a acceptance. Learning, reflection, comfort and local food service remain candidates; this chunk does not justify adding another mandatory production chain or restoring seasons.

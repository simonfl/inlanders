# Construction-catalog keyboard navigation — F21o

September 12, 2026. B opens keyboard browsing in Build → Place, including when returning from a placement preview. Tab/Shift-Tab and Up/Down move through the category control and available cards. Left/Right changes the focused category; Enter/Space on that control also advances the category. A gold outline marks focus, and the adjacent guidance panel shows the focused building's actual description and cost.

Enter/Space on a card selects its existing placement tool and releases catalog focus. Pointer placement, R rotation and camera controls then work normally. B returns to catalog browsing; B again or Escape closes it. Browsing never places a building. While catalog focus is active, world keyboard shortcuts and held movement keys do not affect the village camera or ghost. Text-entry controls keep their typing behavior, and mouse clicks take over normally.

Focus scrolls into view, including after layout and resizing. The last focused/selected building is remembered when reopening, subject to the current category. Supper-disabled construction shows an explicit wait message and cannot be activated. A material shortage still permits planning through the existing placement rules.

## Verification

[960px focused card](images/catalog-keyboard-960.png) · [1440px focused card](images/catalog-keyboard-1440.png).

`./Play.ps1 -CatalogKeyboardSmokeTest` passes with a clean build at both sizes. Actual key events exercise all categories and every building type, real cost/description guidance, card scrolling, read-only preview selection, rotation after activation, Escape/B handoff, text entry, resizing and mouse placement. A paused supper-state UI fixture checks disabled activation and its explanation; it is not a new simulation supper test.

The initial disabled-control test set a button flag that the regular HUD immediately refreshed from simulation state. The fixture now uses the authoritative supper flag. The text-entry fixture was corrected to open Options rather than Economy. Neither required weakening gameplay rules.

`./Play.ps1 -HudSmokeTest` also passes. It was updated for the intentional B behavior: after canceling a placement, B restores catalog focus and the next B closes it. Its Watch check caught a hidden drawer being treated as an active catalog; catalog focus now requires visible management controls. Explicit H/V/G/O/I management shortcuts leave catalog focus and retain their existing actions. Mouse drawer toggling remains unchanged. No simulation or save-format changes were introduced, and the full simulation suite was not rerun for this UI chunk.

## Roadmap review

F21o is delivered. Landscape/Existing tools, inspectors and other management pages remain separate keyboard work; keyboard-only map placement and controller support have not been added.

Next promote **F25d1 / F25e1 — optional civic ambition design** from the existing campaign-systems plan. Recent work improves arranging and accessing the village; the next design question is what worthwhile shared project an established village can undertake. Compare learning/restoration and quiet civic-place concepts through concrete material/labor budgets, alternatives, visible outcomes and recovery before implementing another need or campaign level.

Art, human campaign enjoyment, audio/music, terrain work and other roadmap items remain open. This chunk does not complete the broader UI or game goal.

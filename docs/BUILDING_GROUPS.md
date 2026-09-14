# Building catalogue grouping

September 13, 2026. The player reported feeling overwhelmed by the building options. Replace the initial eighteen-card list with Homes-first browsing and visible category buttons: Homes, Food, Materials, Storage & bridges, Gathering, and All buildings. Remember the selected category during ordinary browsing; world changes and the founding build shortcut start at Homes.

Within Food, group gathering, growing, the farm/bakery chain, and meal distribution. Explain that grain needs a bakery and a pantry redistributes existing food. Pantry now belongs to Food, including existing-building filtering. All eighteen buildings remain available; there are no unlocks, costs or simulation changes. The existing category dropdown supports the Existing list and keyboard browsing; mouse placement browsing uses visible buttons.

Validation: clean game/test builds, scripted founding journeys at960 and1440, held clicks across six refresh frames for every category, complete category membership, pantry discovery, grouped bakery preview/cancellation, keyboard category selection, and unchanged world state while browsing. Founding completion and save/resume also pass. Inspected captured Homes views at both sizes and Food at960. This establishes functional access and label fit, not uncoached ease of use or enjoyment.

Local evidence under `artifacts/review/runs/`:

- `20260914-002443-749-founding-afe0d2` —960.
- `20260914-002531-831-founding-189055` —1440.
- In each run, capture0005 is Homes and capture0006 is Food; stdout records the grouped-catalog assertions.

One playable UI outcome: checkpoint31, next full review35. This responds to a concrete player problem; it does not reverse review30's decision against speculative management controls. F32a, a spacious working lakeside hamlet comparison, remains next. Reassess the category split after player use before adding search, locks or further navigation.

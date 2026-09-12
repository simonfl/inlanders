# F21m — Goals without horizontal scrolling

Status: planned from the [campaign review](CAMPAIGN_REVIEW_F11B3.md).

The 960px river and lake captures (`artifacts/goals-6-960.png` and `goals-7-960.png`) show a horizontal scrollbar and clipped goal content. The current navigation checks pass because they do not assert the content width. Diagnose the minimum-width contributors rather than hiding clipped content.

Scope: make phase actions, condition rows, explanations, resident/place/Plan links, meal evidence and tracking controls fit the actual drawer width. Wrap long labels or stack a row where needed. Preserve vertical scrolling, font readability, pointer isolation and all gameplay state. No smaller global UI scale or new dashboard.

Check both campaigns at 960/1440, collapsed and expanded rows, long assessment blockers, food evidence and relevant-place lists. Assert that horizontal scrolling is unnecessary and important controls remain reachable; inspect screenshots as well. Navigation must leave saves unchanged. This is the next chunk, ahead of storage presentation.

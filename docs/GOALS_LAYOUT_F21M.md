# F21m — Goals width audit

Status: verified; suspected defect ruled out. No product layout change needed.

The [campaign review](CAMPAIGN_REVIEW_F11B3.md) initially mistook the campaign progress bar for a horizontal scrollbar. Source inspection confirms it is a `ProgressBar`; horizontal scrolling is already disabled. The original claim of confirmed overflow was incorrect.

`SmokeGoals.cs` now checks the actual horizontal bounds of visible controls within the Goals content, allowing space for the vertical scrollbar. River and lake fixtures pass at 960/1440, including meal explanations and navigation. A separate lake fixture expands all condition explanations and adds an under-construction gathering hall to exercise relevant-place labels. The expanded 960px capture was also visually inspected: readable wrapped text, vertical scrolling and no horizontal overflow.

Verification: `Play.ps1 -GoalsSmokeTest` passes, including existing exact-save navigation checks. These fixtures are bounded coverage, not a claim about every possible future label. Keep the width assertions to catch regressions. F23b5 storage presentation remains next; do not invent a UI change to justify the mistaken finding.

# Main-menu keyboard navigation — F19c

September 12, 2026. The title screen and its Campaign, Free play, Creative and Sound pages now support keyboard navigation. A gold outline marks the focused control. Tab/Shift-Tab and Up/Down move between enabled controls, Enter/Space activate buttons, Left/Right adjust sound sliders, and Escape follows Back or Cancel. Escape on the root page stays in the menu.

Pages remember the last focused action, including the sound/music mute buttons when their labels change. Later campaign entries scroll into view. Returning to a newly rebuilt page reveals its remembered entry after layout establishes the scroll range; resizing the window also keeps that focus visible. Entering a settlement releases menu focus so Space and other gameplay shortcuts work normally. Mouse clicks can take over at any point.

Keyboard slider changes update and save the actual sound settings. Replay and New on an occupied sandbox slot have confirmation pages with Cancel selected initially; navigation alone does not perform those actions. Accepted replay and replacement retain the existing preceding-village recovery copies. Corrupt-save errors remain in a navigable menu. Normal gameplay buttons retain their existing focus policy.

## Verification

[Campaign focus at 960px](images/menu-keyboard-campaign.png) · [Sound slider focus](images/menu-keyboard-settings.png) · [Replay confirmation](images/menu-keyboard-confirm.png).

`./Play.ps1 -MenuKeyboardSmokeTest` uses isolated saves and actual key events at 960×640 and 1440×900. It checks disabled Continue, traversal/wrapping, visible focus, sound sliders and persistence, mute activation, long campaign scrolling, parent-focus restoration, replay/replacement cancellation and confirmation, campaign and Creative entry, gameplay Space, corrupt Continue/campaign handling and mouse takeover.

`./Play.ps1 -MenuSmokeTest` checks the existing mouse menu, settings, new/resume/replay and recovery copies, all Continue modes, save corruption/failure, Creative placement/removal, map switching and autosave/recovery behavior. Its replay/replacement clicks now explicitly complete the new confirmation pages. Save and audio fixtures are isolated temporary files.

The keyboard check found a remembered late campaign control could receive focus before the new page had a scroll range. Revealing that control after layout fixes the off-screen focus without resetting the player's selected entry. Menu buttons use one gold focus outline; the duplicate default focus border is suppressed locally.

These checks verify input, layout and persistence; no simulation rules or save format changed. The full simulation suite was not rerun for this menu change.

## Roadmap review

F19c is delivered. Promote **F21o — keyboard navigation in the construction catalog**. The same shared button helper still disables focus on in-game construction cards. Scope a useful next task: keyboard selection of a building, followed by a clear return to existing world placement, rotation and camera controls. Keep other management panels, controller support, title artwork and save-slot browsing separate.

Village art, campaign enjoyment, optional civic projects, audio/music and the remaining roadmap are still open. This delivery does not establish completion of the broader UI work.

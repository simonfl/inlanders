# Cottage construction clarity — owner correction within260

Delivered build `D7576B352202`, November10. The owner finished the playthrough and identified the marked moment as unfinished cottages appearing identical to finished ones. In session `a4bb04eaee0841df9a2da492edac3440`, bookmark133 at78.3s selected cottage7. Cottage6 had2/6 logs and cottage7 had0/6; both had zero construction but the renderer already drew plaster, roof, window and chimney.

The browser now draws stakes and marked ground for a plan, with the actual delivered timber stack. All materials delivered exposes a foundation/floor; quarter construction adds a timber frame, halfway adds rafters, and three-quarter adds half a roof. Only completion creates the finished exterior. Delivered-material changes now invalidate the rendering cache, alongside the existing quarter-build boundaries. Geometry shares the same building transform in every orientation. No simulation, costs, completion or staffing changes.

Verification: client compilation and service build pass. Public commands created four cottage plans in all four orientations in session `c27fb49164654b1eb5edabb09ed1d516`. Ordinary construction produced recorded material, foundation, frame, rafter, partial-roof and completed states at7/15/18/21/24/27s. Builder inspected the replay at402×681 with normal village-scale framing. Local images and state sequence are in `artifacts/browser/cottage-visual/`.

Independent read-only visual specialist accepts the bounded fix: pale sites and open framing differ clearly from completed homes. Adjacent houses can overlap the frame/partial-roof silhouette, making intermediate stages less distinct; this is recorded as a minor limitation, not grounds for another presentation batch. Evidence is actual browser replay stills, not continuous-motion viewing or a new physical-iPhone test.

The owner’s completion and this correction do not establish enjoyment of the trade choice. Hold gameplay expansion pending that evidence. Count remains260.

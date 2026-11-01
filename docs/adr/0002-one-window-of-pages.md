# One main window of pages

The product has one main window. Search, Multi-Rename, File operations and
Settings are pages in it with a Back arrow, not separate windows; only viewers
and editors open other windows (owner rulings, 2026-11-01). Total Commander
opens its search, rename and transfer tools as windows, but the owner wants a
Fluent 2 experience in one window. One window also means one engine session,
so two windows never compete for one recovery record.

## Considered options

- Separate tool windows, as in Total Commander: rejected by the owner.
- Extra main windows in one process, as an earlier version of this design
  planned: rejected, "I want no extra windows".
- A picker between peer pages, as in TinyTorrent2: dropped, because the
  Commander page is the only main page.

## Consequences

Starting an operation never changes the page; its progress is in the status
bar. The window always opens on the Commander page, so every other page has a
way back.

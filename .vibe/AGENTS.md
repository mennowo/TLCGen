# AGENTS.md

## Project Overview

TLCGen is a source code generator for software to control signalized intersections.
The app is a native Windows WPF app, built using NET10.

## Knowledge Notes (MCP)

This repo uses the `knowledge-notes` MCP server for durable, searchable notes
across sessions. **Always pass `project: "TLCGen"` on every call.**

### When to record a note

Record when you learn something that a fresh session could not re-derive from
`Grep`/`Glob` over the tree:

- **gotcha** — a non-obvious pitfall just bit you (or nearly did).
- **decision** — you made a design call whose *why* isn't visible in the
  resulting code ("we chose X over Y because Z"). Includes design choices.
- **convention** — a code-style or pattern norm adopted for this repo.
- **port-detail** — when porting (e.g. WPF→React), how a specific construct
  mapped, what worked, what didn't.
- **note** — general knowledge capture, default for anything that doesn't
  fit the four above.
- **todo** — a deferred task to pick up later. Capture enough context to
  resume cold: what to do, why, references to related notes (by id, in the
  body) and files (`related_paths`). List the queue with
  `list_notes(kind="todo")`.

Record in the moment, not at end-of-session — context fades fast.

```
record_note(
    project="{{PROJECT_NAME}}",
    kind="gotcha",
    topic="datagrid-flicker",
    body="Setting ItemsSource before binding fires OnCollectionChanged twice; ...",
    related_paths=["src/views/CustomerGrid.xaml"],
    tags=["wpf","datagrid"]
)
```

### When to recall

**First thing in a non-trivial task**, before diving into code:

```
recall_notes(project="{{PROJECT_NAME}}", query="<a concept phrase from the task>")
```

Then also try with any distinctive identifiers (class names, method names, error
strings). If nothing fires, browse with:

```
list_notes(project="{{PROJECT_NAME}}", kind="gotcha")
list_notes(project="{{PROJECT_NAME}}", kind="decision")
```

Default `mode='hybrid'` is almost always right. Switch to `mode='keyword'` only
for exact-identifier lookups where semantic paraphrase would add noise.

### Correcting or retiring notes

- Note is wrong but you have a correction → `supersede_note(old_id, new_body)`.
  The old note stays reachable by id but vanishes from recall.
- Note is wrong and you don't yet have a fix → `flag_stale(id, reason)`.

Do **not** delete notes. Supersede chains preserve history.

### What NOT to put in notes

- Transient task state (use the task tool).
- Code explanations a grep can produce.
- Project documentation (use the Docs MCP server).
- Anything already in this CLAUDE.md.

### Cross-project search

Rarely needed, but when a past port or pattern in a *different* project is
relevant:

```
recall_notes_global(query="<concept>")
```

Use sparingly — single-project recall is the default for a reason.
# Copy and Delete wait in separate lanes of one scheduler

Transfers (Copy, a Move to another volume, Sync, Mirror) and Deletes wait in two lanes of
`Syno.Files`' one scheduler. A Delete never waits for a transfer's disk, and a
transfer never waits for a Delete; each lane counts its own users of a disk against
Transfers per device or Deletes per device (owner rulings, 2026-11-01). The
owner requires that a waiting Delete can be reordered and stopped, and that
Copy and Delete stay separate as in gozler-ui, where a Delete starts at once
and never waits.

## Considered options

- One shared lane for Copy and Delete: rejected, because a long copy would
  hold up a Delete, which gozler-ui never does.
- A second scheduler for Deletes: rejected, because two schedulers would be
  two owners of one admission rule.

## Consequences

`Syno.Files` gives a Delete the disks of its volume and counts it in its own
lane, so reorder, pause, Stop and Cancel all work for Deletes through the same
queue code as transfers. gozler-ui is not affected; its Deletes still start at
once.

A transfer reads its sources when it runs, so a Delete that ran first would
remove entries a waiting transfer would have copied. The owner first accepted
that, then ruled instead (2026-11-01): a Delete waits for a transfer only for
the entries the person queues. When some of
its entries are in a transfer that has not ended, the one Delete question
offers to delete them now or after that transfer; there is no second
question. Its other entries are
deleted at once either way. So `Syno.Files` finds the part of each Delete's
tree that it shares with the queued transfers' trees.

require ../cell_ops.fs
require ../../test/test.fs

10 plan

s" 1.2" 1 1 grid->cell! str>cell
1 1 cell-done-edit
1 1 grid->cell 1 1 grid->cell! =ok
1 1 grid->cell  dup cell->type@ type:num =ok
cell->val f@ 1.2e f= ok

bye

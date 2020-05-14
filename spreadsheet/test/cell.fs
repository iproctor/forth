#! /usr/local/bin/gforth

require ../cell_ops.fs
require ../../test/test.fs

10 plan

s" 1.2" 1 1 grid->cell! str>cell
1 1 cell-done-edit
1 1 grid->cell 1 1 grid->cell! =ok
1 1 grid->cell  dup cell->type@ type:num =ok
cell->val f@ 1.2e f= ok

." #deps" cr
s" ={B1}" 2 2 grid->cell! str>cell
2 2 cell-done-edit
2 2 grid->cell  dup cell->type@ type:code =ok
cell->val f@ 1.2e f= ok
2 2 grid->cell cell->deps @ list->val 2@  1 =ok 1 =ok
1 1 grid->cell cell->backdeps @ list->val 2@  2 =ok 2 =ok

s" 2.4" 1 1 grid->cell! str>cell
1 1 cell-done-edit
2 2 grid->cell  cell->val f@ 2.4e f= ok

bye

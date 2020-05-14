#! /usr/local/bin/gforth

require ../../test/test.fs
require ../val.fs
require ../cell_ops.fs

10 plan

." #matrix" cr
2 3 new-matrix constant testm
testm matrix->dim 2@ 3 =ok 2 =ok
testm matrix->dim @ 3 =ok
: el ( r addr -- addr ) dup v f! cell+ ;
testm matrix->data 1e el 2e el 3e el 4e el 5e el 6e el drop
testm matrix->data f@ 1e f= ok
testm matrix->data cell+ f@ 2e f= ok
testm matrix->data 5 cells + f@ 6e f= ok
1 2 testm matrix[] 6e  f= ok
0e ' f+ testm matrix-for-each 21e f= ok

." #slice:sum" cr
s" 1" 0 0 grid->cell! str>cell  0 0 cell-done-edit
s" 2" 0 1 grid->cell! str>cell  0 1 cell-done-edit
s" =[A0:A1] sum" 0 2 grid->cell! str>cell  0 2 cell-done-edit
0 2 grid->cell! cell->val@ 3e f= ok

." #slice:mean" cr
s" =[A0:A1] mean" 0 2 grid->cell! str>cell  0 2 cell-done-edit
0 2 grid->cell! cell->val@ fdup 1.5e f= ok

free-val-mem

bye

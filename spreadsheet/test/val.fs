require ../../test/test.fs
require ../val.fs

7 plan

2 3 new-matrix constant testm
testm matrix->dim 2@ 3 =ok 2 =ok
: el ( r addr -- addr ) dup v f! cell+ ;
testm matrix->data 1e el 2e el 3e el 4e el 5e el 6e el
testm matrix->data f@ 1e f= ok
testm matrix->data cell+ f@ 2e f= ok
testm matrix->data 5 cells + f@ 6e f= ok
1 2 testm matrix[] 6e  f= ok
0e ' f+ testm matrix-for-each 21e f= ok

free-val-mem

bye

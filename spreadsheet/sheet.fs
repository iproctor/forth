\ Sheet interface

require block.fs
require grid.fs

: grid->block ( col row -- x y c-addr ) block-coords cells grid + ;
: grid->block! ( col row -- x y c-addr ) grid->block dup @ 0= IF
    allot-block swap v. ! THEN ;

: grid->cell ( col row -- c-addr ) grid->block @ dup 0= IF drop 2drop 0 0 exit THEN block->cell ;
: grid->cell! ( col row -- c-addr ) grid->block! block->cell ;

defer cell-update
: cell-backdeps-update ( c-addr -- ) cell->backdeps for-list[ list->val 2@  grid->cell @  cell-update ]for-list ;
: cell-run-code ( c-addr -- ) dup cell->val @ execute free-val-mem  dup cell->val f!  cell-backdeps-update ;
:noname ( c-addr -- ) dup cell->type @ type:code = IF cell-run-code THEN ; IS cell-update

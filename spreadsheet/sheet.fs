\ Sheet interface

require block.fs
require grid.fs

: grid->block ( col row -- x y c-addr ) block-coords cells grid + ;
: grid->block! ( col row -- x y c-addr ) grid->block dup @ 0= IF
    allot-block swap v. ! THEN ;

: grid->cell ( col row -- c-addr ) grid->block @ dup 0= IF drop 2drop 0 0 exit THEN block->cell ;
: grid->cell! ( col row -- c-addr ) grid->block! @ block->cell ;


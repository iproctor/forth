\ An NxN grid of pointers referring to blocks

require ../utils.fs
require ./block.fs

16 constant \grid
\grid dup * allotz constant grid

: block-at ( col row -- c-addr ) \grid * + grid + ;
: block-coords ( col row -- u u u ) ['] /block bi@  \grid *  v swap + ;
: grid->block ( col row -- x y c-addr ) block-coords cells grid + ;
: grid->block! ( col row -- x y c-addr ) grid->block dup @ 0= IF
    allot-block swap v. ! THEN ;
: grid->cell ( col row -- c-addr ) grid->block @ dup 0= IF drop 2drop 0 0 exit THEN block->cell ;
: grid->cell! ( col row -- c-addr ) grid->block! block->cell ;

: grid->slice ( col row -- ? ) ;

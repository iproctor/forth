\ An NxN block of M length cells

require cell.fs
require grid.fs

32 constant \block
: /block \block /mod ;
: *block-row \block * *cell ;
: allot-block ( -- c-addr ) \block dup * \cell * allotz ;

: block->cell ( col row c-addr -- c-addr ) >r v *cell *block-row + r> + ;

: block-at ( col row -- c-addr ) \grid * + grid + ;
: block-coords ( col row -- u u u ) ['] /block bi@  \grid *  v swap + ;

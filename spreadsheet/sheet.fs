\ An NxN grid of pointers referring to blocks

require ../utils.fs
require ./block.fs

16 constant \grid

: allot-grid ( -- c-addr ) \grid dup * allotz ;
: block-at ( col row grid -- c-addr ) >r \grid * + r> + ;
: block-coords ( col row -- u u u ) ['] /block bi@  \grid *  v swap + ;
: grid-to-block ( col row grid -- x y c-addr ) >r block-coords cells r> + ;
: view-cell ( col row grid -- c-addr u ) grid-to-block @ dup 0= IF drop 2drop 0 0 exit THEN block-cell cell-to-str ;

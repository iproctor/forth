\ An NxN block of M length cells

require ../utils.fs
require ../combis.fs

256 constant \cell
32 constant \block

: *cell \cell * ;
: /block \block /mod ;
: *block \block * ;

: allot-block ( -- c-addr ) \block dup * \cell * allotz ;
: cell-to-str ( c-addr -- c-addr u ) dup char+ swap c@ ;
: block-cell ( col row c-addr -- c-addr u ) >r ['] *cell ['] *block .s bi* + r> + ;
: !cell ( c-addr u c-addr -- ) v. c! char+ ! ;

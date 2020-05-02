\ An NxN block of M length cells

require ../utils.fs
require ../combis.fs
require ../ds/arrlist.fs

256 constant \cell
32 constant \block

0 constant type:string
1 constant type:num
2 constant type:code

: *cell \cell * ;
: /block \block /mod ;
: *block \block * ;
: bound-cell \cell 1- min 0 max ;

: allot-block ( -- c-addr ) \block dup * \cell * allotz ;
: block->cell ( col row c-addr -- c-addr ) >r ['] *cell ['] *block bi* + r> + ;

: cell->val ( c-addr -- c-addr ) ;
: cell->deps ( c-addr -- c-addr ) cell->val cell+ ;
: push-cell-deps ( w w c-addr -- ) dup >r cell->data push-arrlist r> ! ;
: cell->sz ( c-addr -- c-addr ) cell->deps cell+ ;
: cell->data ( c-addr -- c-addr ) cell->sz char+ ;
: cell->str ( c-addr -- c-addr u ) v cell->data  cell->sz c@ ;

: 1+cell ( c-addr -- ) dup cell->sz c@ 1+ bound-cell swap c! ;
: 1-cell ( c-addr -- ) dup cell->sz c@ 1- bound-cell swap c! ;
: str>cell ( c-addr u c-addr -- ) 2dup cell->sz c!  cell->data swap move ;

: cell-str-offset ( u c-addr -- c-addr ) cell->data swap chars + ;
: cell-rem-len ( u -- u ) cell->data \cell swap - ;
: cell-del ( u c-addr -- ) dup 1-cell  cell-str-offset  dup char+ swap  rot cell-rem-len  cmove ;
: shift-cell ( u c-addr -- ) cell-str-offset  dup char+  rot cell-rem-len  cmove> ;
: cell-ins ( c u c-addr -- ) dup 1+cell  2dup shift-cell  cell->data + c! ;

: cell-parse ( c-addr -- ) dup cell->str case
    dup blank-str? ?of type:string endof
    dup >float ?of over cell->val f!  type:num endof
    dup parse-code ?of over cell->val !  type:code endof
    type:string
  0 endof
  swap cell->type !

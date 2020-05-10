\ Cell structure definition and helpers
require ../ds/list.fs
require err.fs

256 constant \cell
: *cell \cell * ;

0 constant type:string
1 constant type:num
2 constant type:code



: cell->val ( c-addr -- c-addr ) ;
: cell->val@ ( c-addr -- c-addr ) cell->val f@ ;
: cell->code ( c-addr -- c-addr ) cell->val cell+ ;
: cell->deps ( c-addr -- c-addr ) cell->code cell+ ;
: cell->backdeps ( c-addr -- c-addr ) cell->deps cell+ ;
: cell->type ( c-addr -- c-addr ) cell->backdeps cell+ ;
: cell->type@ ( c-addr -- c ) cell->type c@ ;
: cell->sz ( c-addr -- c-addr ) cell->type char+ ;
: cell->data ( c-addr -- c-addr ) cell->sz char+ ;
: cell->str ( c-addr -- c-addr u ) v. cell->data  cell->sz c@ ;
: bound-cell \cell 1- min 0 max ;
\cell 0 cell->data - constant \cell-str

: push-cell-backdep ( u u c-addr -- ) cell->backdeps dup >r @  list-prepend2 r> ! ;
: rem-cell-backdep ( u u c-addr -- ) cell->backdeps dup >r @  list-filter-out2 r> ! ;

: 1+cell ( c-addr -- ) dup cell->sz c@ 1+ bound-cell swap c! ;
: 1-cell ( c-addr -- ) dup cell->sz c@ 1- bound-cell swap c! ;
: str>cell ( c-addr u c-addr -- ) 2dup cell->sz c!  cell->data swap move ;

: cell-str-offset ( u c-addr -- c-addr ) cell->data swap chars + ;
: cell-rem-len ( u -- u ) cell->data \cell swap - ;
: cell-del ( u c-addr -- ) dup 1-cell  cell-str-offset  dup char+ swap  rot cell-rem-len  cmove ;
: shift-cell ( u c-addr -- ) cell-str-offset  dup char+  rot cell-rem-len  cmove> ;
: cell-ins ( c u c-addr -- ) dup 1+cell  2dup shift-cell  cell->data + c! ;

: cell-is-code? ( cell -- flag ) cell->type@ type:code = ;
: cell-is-string? ( cell -- flag) cell->type@ type:string = ;

: cell-val-or-throw ( cell -- r ) dup cell->type@ type:string = IF drop err:nonval throw THEN cell->val f@ ;


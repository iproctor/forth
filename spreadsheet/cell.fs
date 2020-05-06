\ Cell structure definition and helpers
require ../ds/list.fs

256 constant \cell
: *cell \cell * ;

0 constant type:string
1 constant type:num
2 constant type:code

: cell->val ( c-addr -- c-addr ) ;
: cell->code ( c-addr -- c-addr ) cell->val cell+ ;
: cell->deps ( c-addr -- c-addr ) cell->code cell+ ;
: cell->backdeps ( c-addr -- c-addr ) cell->deps cell+ ;
: cell->type ( c-addr -- c-addr ) cell->deps cell+ ;
: cell->sz ( c-addr -- c-addr ) cell->type char+ ;
: cell->data ( c-addr -- c-addr ) cell->sz char+ ;
: cell->str ( c-addr -- c-addr u ) v. cell->data  cell->sz c@ ;
: bound-cell \cell 1- min 0 max ;

: push-cell-backdep ( u u c-addr -- ) dup >r cell->backdeps list-prepend2 r> ! ;
: rem-cell-backdep ( u u c-addr -- ) dup >r cell->backdeps list-filter-out2 >r ! ;

: 1+cell ( c-addr -- ) dup cell->sz c@ 1+ bound-cell swap c! ;
: 1-cell ( c-addr -- ) dup cell->sz c@ 1- bound-cell swap c! ;
: str>cell ( c-addr u c-addr -- ) 2dup cell->sz c!  cell->data swap move ;

: cell-str-offset ( u c-addr -- c-addr ) cell->data swap chars + ;
: cell-rem-len ( u -- u ) cell->data \cell swap - ;
: cell-del ( u c-addr -- ) dup 1-cell  cell-str-offset  dup char+ swap  rot cell-rem-len  cmove ;
: shift-cell ( u c-addr -- ) cell-str-offset  dup char+  rot cell-rem-len  cmove> ;
: cell-ins ( c u c-addr -- ) dup 1+cell  2dup shift-cell  cell->data + c! ;

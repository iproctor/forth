require ../ds/list.fs
require mem.fs
require sheet.fs
require err.fs

0 constant val:const
1 constant val:slice
2 constant val:matrix

: val->type ;

: const->value val->type cell+ ;
: const->value@ const->value f@ ;
: new-const-val ( r -- val ) 2 cells alloc-val  val:const over val->type !  dup const->value f! ;

: slice->from val->type cell+ ;
: slice->to slice->from 2 cells + ;
: new-slice-val ( u u u u -- val ) 5 cells alloc-val >r  val:slice r@ val->type !  r@ slice->to 2!  r@ slice->from 2! r> ;
: slice-start-row ( slice -- u ) slice->from @ ;
: slice-end-row ( slice -- u ) slice->to @ 1+ ;

: slice-start-col ( slice -- u ) slice->from cell+ @ ;
: slice-end-col ( slice -- u ) slice->to cell+ @ 1+ ;

: slice-size ( slice -- u ) dup v. slice-end-col slice-start-col -  swap v. slice-end-row slice-start-row -  * ;

: slice-col-range ( slice -- u u ) v. slice-end-col slice-start-col ;
: slice-row-range ( slice -- u u ) v. slice-end-row slice-start-row ;

: slice-for-each ( ... xt slice -- ... ) dup slice-col-range U+DO
    dup slice-row-range U+DO
      j i grid->cell cell-val-or-throw >r v. execute r>
    LOOP LOOP 2drop ;

: matrix->dim val->type cell+ ;
: matrix->cols matrix->dim cell+ @ ;
: matrix->data matrix->dim 2 cells + ;
: matrix[] ( u u matrix -- r ) { cols rows matrix } matrix matrix->cols rows *  cols +  cells  matrix matrix->data + f@ ;
: matrix-col-range ( matrix -- u u ) matrix->cols 0 ;
: matrix-row-range ( matrix -- u u ) matrix->dim @ 0 ;
: new-matrix ( u u -- val ) 2dup * 3 + cells alloc-val  val:matrix over val->type !  dup >r matrix->dim 2! r> ;

: matrix-for-each ( ... xt matrix -- ... ) dup matrix-col-range U+DO
    dup matrix-row-range U+DO
      dup j i rot matrix[] >r v. execute r>
    LOOP LOOP 2drop ;

: val-for-each ( ... xt val -- ... ) dup val->type @ CASE
    val:const OF const->value@ swap execute ENDOF
    val:slice OF slice-for-each ENDOF
    val:matrix OF matrix-for-each ENDOF
  ENDCASE ;

: slice->scalar ( slice -- r ) dup slice-size 1 <> IF err:nonscalar throw THEN
  slice->from 2@ grid->cell cell->val f@ ;

: matrix->scalar ( matrix -- r ) dup matrix->dim 2@ >r 1 = r> 1 = and IF
  0 0 rot matrix[] ELSE drop err:nonscalar throw THEN ;

: val->scalar ( val -- r ) dup val->type @ CASE
    val:const OF const->value@ ENDOF
    val:slice OF slice->scalar ENDOF
    val:matrix OF matrix->scalar ENDOF
  ENDCASE ;

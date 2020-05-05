require sheet.fs

0 constant val:const
1 constant val:slice
2 constant val:matrix

: val->type ;

0 VALUE val-mem-list
: set-val-mem-list ['] val-mem-list >body ! ;
: free-val-mem val-mem-list free-mem-list  0 set-val-mem-list ;
: alloc-val ( u -- c-addr ) val-mem-list alloc-push  set-val-mem-list ;


: const->value val->type cell+ ;
: const->value@ const->value f@ ;
: new-const-val ( r -- val ) 2 cells alloc-val  dup val:const swap val->type !  dup const->value f! ;

: slice->from val->type cell+ ;
: slice->to slice->from 2 cells + ;
: new-slice-val ( u u u u -- val ) 5 cells alloc-val  dup val:slice val->type !  dup slice->to 2!  dup slice->from 2! ;
: slice-start-col ( slice -- u ) slice->from @ ;
: slice-end-col ( slice -- u ) slice->to @ 1+ ;

: slice-start-row ( slice -- u ) slice->from cell+ @ ;
: slice-end-row ( slice -- u ) slice->to cell+ @ 1+ ;

: slice-col-range ( slice -- u u ) v. slice-end-col slice-start-col ;
: slice-row-range ( slice -- u u ) v. slice-end-row slice-start-row ;

: slice-for-each ( ... xt slice -- ... ) dup slice-col-range U+DO
    slice-row-range U+DO
      i j grid->cell cell->val f@ v. execute
    LOOP LOOP ;

: matrix-for-each ;

: val-for-each ( ... xt val -- ... ) dup val->type @ CASE
    val:const OF const->value@ swap execute ENDOF
    val:slice OF slice-for-each ENDOF
    val:matrix OF matrix-for-each ENDOF
  ENDCASE ;

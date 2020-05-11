require parse.fs
require sheet.fs
require mem.fs
require ../ds/list.fs
require ../utils.fs

: for-dep-list[ ]] for-list[ list->val 2@  grid->cell [[ ; immediate
: ]for-dep-list ]] ]for-list [[ ; immediate

: cell-exec-code ( cell -- r ) cell->code @ execute  val->scalar free-val-mem  ;
: cell-run-code ( cell -- ) dup cell-is-code? IF dup cell-exec-code  dup cell->val f! ELSE drop THEN ;

defer cell-update
: cell-backdeps-update ( cell -- ) cell->backdeps @  for-dep-list[ cell-update ]for-dep-list ;
:noname ( cell -- ) v. cell-run-code cell-backdeps-update ; IS cell-update

: rem-cell-from-dep-list ( u u list -- ) for-dep-list[ v 2dup rem-cell-backdep ]for-dep-list 2drop ;
: rem-cell-from-deps ( u u cell -- )  cell->deps @ rem-cell-from-dep-list ;
: free-cell-deps ( cell -- ) cell->deps dup @ free-list 0 swap ! ;

: cell-add-backdeps ( u u cell -- ) cell->deps @ for-dep-list[ v 2dup push-cell-backdep ]for-dep-list 2drop ;

: cell-done-edit ( u u -- ) 2dup grid->cell  3dup rem-cell-from-deps  dup free-cell-deps  dup cell-parse  v. cell-add-backdeps  cell-update ;

: edit-cell-str ( cell -- ) dup cell->str \cell-str swap edit-line swap cell->sz c! ;

: edit-cell ( u u -- ) 2dup grid->cell! edit-cell-str  cell-done-edit ;

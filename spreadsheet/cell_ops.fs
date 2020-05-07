require parse.fs
require sheet.fs
require mem.fs
require ../ds/list.fs

: for-dep-list[ ]] for-list[ list->val 2@  grid->cell [[ ; immediate
: ]for-dep-list ]] ]for-list [[ ; immediate

: cell-exec-code ( cell -- r ) cell->code @ execute  val->scalar  free-val-mem ;
: cell-run-code ( cell -- ) dup cell-is-code? IF dup cell-exec-code  dup cell->val f! ELSE drop THEN ;

defer cell-update
: cell-backdeps-update ( cell -- ) cell->backdeps @  for-dep-list[ cell-update ]for-dep-list ;
:noname ( cell -- ) v. cell-run-code cell-backdeps-update ; IS cell-update

: rem-cell-from-dep-list ( u u list -- ) for-dep-list[ v 2dup rem-cell-backdep ]for-dep-list 2drop ;
: rem-cell-from-deps ( u u -- ) 2dup grid->cell cell->deps @ v. rem-cell-from-dep-list free-list ;

: cell-add-backdeps ( u u -- ) 2dup grid->cell cell->deps @ for-dep-list[ v 2dup push-cell-backdep ]for-dep-list 2drop ;

: cell-done-edit ( u u -- ) 2dup rem-cell-from-deps  2dup grid->cell cell-parse  2dup cell-add-backdeps  grid->cell cell-update ;

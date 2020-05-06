require parse.fs
require sheet.fs
require mem.fs
require ../ds/list.fs

: for-dep-list[ ]] for-list[ list->val 2@  grid->cell [[ ; immediate
: ]for-dep-list ]] ]for-list [[ ; immediate

defer cell-update
: cell-backdeps-update ( cell -- ) cell->backdeps for-dep-list[ cell-update ]for-dep-list ;
: cell-exec-code ( cell -- r ) cell->code @ execute  val->scalar  free-val-mem ;
: cell-run-code ( cell -- ) dup cell-exec-code  dup cell->val f!  cell-backdeps-update ;
:noname ( cell -- ) dup cell->type @ type:code = IF cell-run-code ELSE drop THEN ; IS cell-update

: rem-cell-from-dep-list ( u u list -- ) for-dep-list[ v 2dup rem-cell-backdep ]for-dep-list 2drop ;
: rem-cell-from-deps ( u u cell -- ) cell->deps @ v. rem-cell-from-dep-list free-list ;

: cell-done-edit ( u u -- ) 2dup grid->cell  v. rem-cell-from-deps  dup cell-parse  cell-update ;

require ../combis.fs
require ../utils.fs

: list->next ( list -- c-addr ) ;
: list->next@ ( list -- c-addr ) @ ;
: list->val ( list -- c-addr ) cell+ ;
: list->val@ ( list -- c-addr ) list->val @ ;
: list-prepend ( w list -- list ) 2 cells allocate throw dup >r list->next !  r@ list->val !  r> ;
: list-prepend2 ( w w list -- list ) 3 cells allocate throw dup >r list->next !  r@ list->val 2! r> ;
: list-append ( w list -- list ) 2 cells allocate throw  dup 2 cells erase  dup >r swap list->next !  r@ list->val ! r> ;
: for-list[ ]] BEGIN >r r@ WHILE r@ [[ ; immediate
: ]for-list ]] r> list->next @ REPEAT rdrop [[ ; immediate
: for-list-val[ ]] for-list[ list->val @ [[ ; immediate
: show-list ( list -- ) for-list[ ." ( " list->val @ . ." )" ]for-list ;
: show-list2 ( list -- ) for-list[ ." ( " list->val 2@ swap . . ." )" ]for-list ;
: list-free ( list -- ) BEGIN dup WHILE v. list->next free throw @ REPEAT drop ;
: list-nth ( u list -- w ) BEGIN 2dup and WHILE v 1- list->next @ REPEAT nip ;

: list-consume-while[ ]] BEGIN
    >r r@ WHILE
    r@ [[ ; immediate

: ]list-consume-while ]] WHILE
    r> dup list->next @ swap free throw
  AGAIN THEN THEN r> [[ ; immediate

: [list-drop-until] { xt } ]] list-consume-while[ swap >r list->val [[ xt compile, ]]  r@ execute 0= r> swap ]list-consume-while nip [[ ;
: list-drop-until ( ... xt list -- list ) [ ' @ [list-drop-until] ] ;
: list-drop-until2 ( ... xt list -- list ) [ ' 2@ [list-drop-until] ] ;

: list-rm-node ( list-ptr -- ) dup @ v. list->next free throw   @ swap ! ;
: list-rm-next ( list -- ) list->next list-rm-node ;
: [list-filter] { at } ( ... xt list -- ... list ) ]] over >r [[ at [list-drop-until] ]] r> swap dup >r BEGIN
    dup WHILE
    dup list->next @ WHILE
    >r >r
    r@2 list->next @ list->val [[ at compile, ]] r@ execute IF
      r@2 list->next @ ELSE r@2 dup list-rm-next THEN
    r> rdrop swap
  AGAIN THEN THEN 2drop r> [[ ;
: list-filter [ ' @ [list-filter] ] ;
: list-filter2 [ ' 2@ [list-filter] ] ;

: 2<> ( w w w w -- w w flag ) >r >r 2dup r> r> v swap = v = and 0= ;
: list-filter-out2 ( w w list -- list ) ['] 2<> swap list-filter2  v 2drop ;

: alloc-push ( u list -- c-addr list ) swap allocate throw dup rot list-prepend ;
: free-mem-list ( list -- ) dup for-list[ list->val @ free throw ]for-list list-free ;

: new-list-anchor ( -- list-anchor ) 2 cells allocz ;
: list-anchor->list ;
: list-anchor->list@  list-anchor->list @ ;
: list-anchor->end cell+ ;
: list-anchor-prepend ( w list-anchor -- ) dup >r list-anchor->list @ list-prepend  r@ list-anchor->list !  r> dup list-anchor->end @ 0= IF dup list-anchor->list @ swap list-anchor->end ! THEN ;
: list-anchor-append ( w list-anchor -- ) dup list-anchor->end @ dup 0= IF drop list-anchor-prepend exit THEN
  rot swap list-append  swap list-anchor->end ! ;
: list-anchor-to-list ( list-anchor -- list ) v. list-anchor->list@ free throw ;
: for-list-anc-val[ ]] list-anchor->list@ for-list[ [[ ; immediate

: list-n ( list -- u ) 0 swap for-list[ drop 1+ ]for-list ;
: list-to-arr ( list -- w-addr u ) dup list-n cells allocate throw  0 rot for-list[ >r 2dup cells + r> list->val @ swap !  1+ ]for-list ;

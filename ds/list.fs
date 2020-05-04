require ../combis.fs
require ../utils.fs

: list->next ( list -- c-addr );
: list->val ( list -- c-addr ) cell+ ;
: list-prepend ( w list -- list ) 2 cells allocate throw dup >r list->next !  r@ list->val !  r> ;
: list-prepend2 ( w w list -- list ) 3 cells allocate throw dup >r list->next !  r@ list->val 2! r> ;
: for-list[ ]] BEGIN >r r@ WHILE r@ [[ ; immediate
: ]for-list ]] r> list-next @ REPEAT rdrop [[ ; immediate
: show-list ( list -- ) for-list[ ." ( " list->val @ . ." )" ]for-list ;
: show-list2 ( list -- ) for-list[ ." ( " list->val 2@ swap . . ." )" ]for-list ;
: free-list ( list -- ) BEGIN dup WHILE v. list-next free throw @ REPEAT drop ;

: list-consume-while[ ]] BEGIN
    >r r@ WHILE
    r@ [[ ; immediate

: ]list-consume-while ]] WHILE
    r> dup list->next @ swap free throw
  AGAIN THEN THEN r> [[ ; immediate

: [list-drop-until] { xt } ]] list-consume-while[ swap >r list->val [[ xt compile, ]]  r@ execute 0= r> swap ]list-consume-while nip [[ ;
: list-drop-until ( ... xt list -- list ) [ ' @ [list-drop-until] ] ;
: list-drop-until2 ( ... xt list -- list ) [ ' 2@ [list-drop-until] ] ;

: list-rm-next ( list -- ) list->next dup  @ v. list->next free throw   @ swap ! ;
: [list-filter] { at } ( ... xt list -- ... list ) ]] over >r [[ at [list-drop-until] ]] r> swap BEGIN
    dup WHILE
    dup list->next @ WHILE
    >r >r
    r@2 list->next @ list->val [[ at compile, ]] r@ execute IF
      r@2 list->next @ ELSE r@2 dup list-rm-next THEN
    r> rdrop swap
    exit
  AGAIN THEN THEN nip [[ ;
: list-filter [ ' @ [list-filter] ] ;
: list-filter2 [ ' 2@ [list-filter] ] ;

: 2<> ( w w w w -- w w flag ) >r >r 2dup r> r> v swap = v = and 0= ;
: list-filter-out2 ( w w list -- list ) ['] 2<> swap list-filter2 ;

require ../combis.fs
require ../utils.fs

: dlist>next ( list -- list ) ;
: dlist>next@ ( list -- list ) dlist>next @ ;
: dlist>prev ( list -- list ) dlist>next cell+ ;
: dlist>prev@ ( list -- list ) dlist>prev @ ;
: dlist>val ( list -- c-addr ) dlist>prev cell+ ;
: dlist>val@ ( list -- c-addr ) dlist>val @ ;
: dlist-maybe-set-next ( list list -- ) dup IF dlist>next ! ELSE 2drop THEN ;
: dlist-maybe-set-prev ( list list -- ) dup IF dlist>prev ! ELSE 2drop THEN ;

: dlist-new ( w prev-list next-list -- list ) 3 cells allocz >r  r@ dlist>next !  r@ dlist>prev !  r@ dlist>val !  r@ r@ dlist>next@ dlist-maybe-set-prev  r@ r@ dlist>prev@ dlist-maybe-set-next r> ;
: dlist-ins-after ( w list -- ) dup dlist>next@ dlist-new drop ;
: dlist-rm-node ( list -- ) dup v. dlist>next@ dlist>prev@ 2dup  dlist-maybe-set-next  swap dlist-maybe-set-prev  free throw ;
: dlist-new-head 0 0 0 dlist-new ;

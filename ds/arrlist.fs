 32 constant \arrlist
: allot-arrlist ( -- arrlist ) \arrlist 1+ cells allotz  dup \arrlist swap ! ;
: copy-arrlist ( arrlist arrlist -- ) swap dup @ cells  v swap  cmove ;
: grow-arrlist ( arrlist -- arrlist ) dup @ 2* tuck cells allotz tuck  copy-arrlist  v. ! ;
: push-arrlist ( w arrlist -- arrlist )
  dup 0= IF drop allot-arrlist THEN
  dup @ 1+ 1 U+DO
    dup i cells + dup
    ( w arrlist c-addr c-addr )
    @ 0= IF swap v ! UNLOOP exit THEN
    @ v over = IF 2drop UNLOOP exit THEN
  LOOP
  dup @ cells  swap grow-arrlist  v. ! ;

 : rm-arrlist ( w arrlist -- ) dup @ 1 U+DO
    2dup i cells + @ = IF i cells + 0 ! drop UNLOOP exit THEN
   LOOP 2drop ;


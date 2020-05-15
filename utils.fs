require combis.fs

: allotz ( u -- c-addr ) here dup rot dup allot erase ;
: blank-str? ( c-addr u -- flag ) 0 U+DO dup i chars + c@ bl <> IF drop unloop false exit THEN LOOP drop true ;

: r@2 ]] r> r@ swap >r [[ ; immediate
: r+ ]] r> 1+ >r [[ ; immediate

: within-range? ( n n n -- flag ) { n a b } a n <= n b <= and ;

: here-append ( c-addr u -- c-addr ) here swap v. cmove  here swap chars + ;

: 3dup 2 pick 2 pick 2 pick ;

: print-u ( n -- ) 0 <<# #s #> type #>> ;

: 2!r ( w w c-addr -- ) v swap 2! ;
: 2@r ( c-addr -- w w ) 2@ swap ;
: 3!r ( w w w c-addr -- ) >r -rot r@ 2!r r> 2 cells + ! ;

: 2elem@ ( w-addr u u -- w w ) >r over swap cells + @ swap r> cells + @ ;

: dup3rd ( w w w -- w w w w ) 2>r dup 2r> ;

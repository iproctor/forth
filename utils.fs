require combis.fs

: allotz ( u -- c-addr ) here dup rot dup allot erase ;
: blank-str? ( c-addr u -- flag ) 0 U+DO dup i chars + c@ bl <> IF drop unloop false exit THEN LOOP drop true ;

: r@2 ]] r> r@ swap >r [[ ; immediate
: r+ ]] r> 1+ >r [[ ; immediate

: within-range? ( n n n -- flag ) { n a b } a n <= n b <= and ;

: here-append ( c-addr u -- c-addr ) here swap v. cmove  here swap chars + ;



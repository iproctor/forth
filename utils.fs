require combis.fs

: allotz ( u -- c-addr ) here dup rot dup allot erase ;
: blank-str? ( c-addr u -- flag ) 0 U+DO dup i chars c@ 32 <> IF drop unloop false exit THEN LOOP drop true ;

: r@2 ]] r> r@ swap >r [[ ; immediate

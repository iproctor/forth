require val.fs

: ss:dup dup ;
: ss:swap swap ;
: ss:sum ( val -- val ) 0e ['] f+ swap val-for-each  new-const-val ;
: sum-count ( u r r -- u r ) 1+ f+ ;
: ss:mean ( val -- val ) 0 0e ['] sum-count rot val-for-each  s>f f/ new-const-val ;

require instrument.fs

\ Active set - linked list of voice
\ Voice - xt for generating nth sample, xt for destructor, other space

struct
  cell% field voice>gen-xt
  cell% field voice>destr-xt
  cell% field voice>t0
  cell% field voice>instrument
end-struct voice%

: voice>t0@ ( voice -- c-addr ) voice>t0 @ ;

: scale-sample ( n r -- n ) s>f f* f>s ;
: scale-samples ( n n r -- n n ) fdup v scale-sample scale-sample ;
: voice-scale ( n n voice -- n n ) voice>instrument @ instrument>gain@ scale-samples ;
: voice>gen ( u voice -- n n flag ) dup >r voice>t0 @ - r@ r@ voice>gen-xt @ execute
  r> swap v voice-scale ;
: voice-free ( voice -- ) dup dup voice>destr-xt @ execute  free throw ;
: voice-init ( instrument gen-xt destr-xt voice -- ) >r 0 r@ voice>t0 ! r@ voice>destr-xt ! r@ voice>gen-xt ! r> voice>instrument ! ;

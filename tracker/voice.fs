require instrument.fs

\ Active set - linked list of voice
\ Voice - xt for generating nth sample, xt for destructor, other space

: voice>gen-xt ( voice -- c-addr ) ;
: voice>destr-xt ( voice -- c-addr ) voice>gen-xt cell+ ;
: voice>t0 ( voice -- c-addr ) voice>destr-xt cell+ ;
: voice>instrument ( voice -- c-addr ) voice>t0 cell+ ;
: scale-sample ( n r -- n ) s>f f* f>s ;
: scale-samples ( n n r -- n n ) fdup v scale-sample scale-sample ;
: voice-scale ( n n voice -- n n ) voice>instrument @ instrument>gain scale-samples ;
: voice>gen ( u voice -- n n flag ) dup >r voice>t0 @ - r@ r@ voice>gen-xt @ execute
  r> swap v voice-scale ;
: voice-free ( voice -- ) dup dup voice>destr-xt @ execute  free throw ;

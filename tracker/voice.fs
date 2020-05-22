require ../utils.fs
require ../audio/wav.fs

\ Active set - linked list of voice
\ Voice - xt for generating nth sample, xt for destructor, other space

: voice>gen-xt ( voice -- c-addr ) ;
: voice>destr-xt ( voice -- c-addr ) voice>gen-xt cell+ ;
: voice>t0 ( voice -- c-addr ) voice>destr-xt cell+ ;
: voice>data ( voice -- c-addr ) voice>t0 cell+ ;
: voice>gen ( u voice -- n n flag ) dup >r voice>t0 @ - r@ r> voice>gen-xt @ execute ;
: voice-free ( voice -- ) dup dup voice>destr-xt @ execute  free throw ;

: mk-wav-gen-body ( compilation: wav -- runtime: u voice -- n n flag ) { wav } ]] drop dup [[ wav wav->n ]] literal >= IF
  drop 0 0 true ELSE [[ wav mk-wav->stereo-sample ]] false THEN [[ ;
: mk-wav-gen ( wav -- xt ) >r :noname r> mk-wav-gen-body POSTPONE ; ;
: wav-destr ;

: new-wav-voice ( xt -- voice ) 3 cells allocz >r  r@ voice>gen-xt !  ['] wav-destr r@ voice>destr-xt !  r> ;

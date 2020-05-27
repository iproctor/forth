require ../combis.fs
require ../audio/ring.fs
require sequence.fs

1 15 lshift 1- constant max-16bit-sample

\ assume 16bit samples
: interp-components ( len i len' -- m r:l ) 1- >r v 1- * s>f r> s>f f/ fdup floor fdup f>s f- ;
: sample-at ( frames u -- n ) 2* + sw@ ;
: sample-delta ( frames u -- n ) 2dup 1+ sample-at v sample-at swap - ;
: interpolate ( frames len i len' -- n ) 2dup 1- = IF 2drop 1- sample-at exit THEN
  interp-components 2dup sample-delta s>f f* sample-at s>f f+ f>s ;

: pitch-of-note ( u -- r ) 48 - 440e 1.059463094359e s>f f** f* ;
: pitch-to-samples ( r -- u ) sample-rate s>f fswap f/ f>s ;

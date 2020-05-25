require ../combis.fs
require ../audio/ring.fs
require sequence.fs

1 15 lshift constant max-16bit-sample

\ assume 16bit samples
: interp-components ( len i len' -- m r:l ) 1- >r v 1- * s>f r> s>f f/ fdup floor fdup f>s f- ;
: sample-at ( frames u -- n ) 2* + sw@ ;
: sample-delta ( frames u -- n ) 2dup 1+ sample-at v sample-at swap - ;
: interpolate ( frames len i len' -- n ) 2dup 1- = IF 2drop 1- sample-at exit THEN
  interp-components 2dup sample-delta s>f f* sample-at s>f f+ f>s ;
: interpolate-stereo ( frames len i len' -- n n ) ;

: pitch-of-note ( u u -- r ) note-to-note-index 48 - 440e 1.059463094359e s>f f** f* ;
: pitch-to-samples ( r -- u ) sample-rate s>f fswap f/ f>s ;

create sin-sample 128 2* allot
128 constant \sin-sample
: fill-sin 128 0 U+DO
    i s>f 128e f/ 3.14159e f2* f* fsin max-16bit-sample s>f f* f>s sin-sample i 2* + w!
  LOOP ;
fill-sin

create square-sample 1e f, 0e f,
2 constant \square-sample

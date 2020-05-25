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

: pitch-of-note ( u u -- r ) note-to-note-index 48 - 440e 1.059463094359e s>f f** f* ;
: pitch-to-samples ( r -- u ) sample-rate s>f fswap f/ f>s ;

512 constant \sin-sample
create sin-sample \sin-sample 2* allot
: fill-sin \sin-sample 0 U+DO
    i s>f \sin-sample s>f f/ 3.141592e f2* f* fsin max-16bit-sample s>f f* f>s sin-sample i 2* + w!
  LOOP ;
fill-sin

create square-sample 64 2* allot
64 constant \square-sample
: fill-sq \square-sample 0 U+DO
    max-16bit-sample  i \square-sample 2 / < IF negate THEN  square-sample i 2* + w!
  LOOP ;
fill-sq

64 constant \saw-sample
create saw-sample \saw-sample 2* allot
: fill-saw \saw-sample 0 U+DO
    i s>f \saw-sample s>f f/ f2* 1e f- max-16bit-sample s>f f* f>s saw-sample i 2* + w!
  LOOP ;
fill-saw

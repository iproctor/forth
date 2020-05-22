\ gonna use the ring sample rate
require ../audio/ring.fs

\ Rounds down because used by 64ths clock
\ samps * bpm * 16 / 60  / sr
: samples->64ths ( u bpm -- u ) * 16 * 60 / sample-rate / ;

\ Rounds up
\ 64ths / 16 / bpm * 60 * sr
: 64ths->samples ( u bpm -- u ) >r sample-rate * 60 *  16 r> * /up ;

\ gonna use the ring sample rate
require ../audio/ring.fs
require ../utils.fs

\ Rounds up
\ 64ths / 16 / bpm * 60 * sr
: 64ths->samples ( u bpm -- u ) >r sample-rate * 60 *  16 r> * /up ;


\ gonna use the ring sample rate
require ../audio/ring.fs
require ../utils.fs

\ Flag if even
\ samps * bpm * 16 / 60  / sr
: samples->64ths ( u bpm -- u flag ) * 16 *  60 sample-rate * /mod swap 0= ;

\ Rounds up
\ 64ths / 16 / bpm * 60 * sr
: 64ths->samples ( u bpm -- u ) over max-int = IF nip exit THEN
  >r sample-rate * 60 *  16 r> * /up ;

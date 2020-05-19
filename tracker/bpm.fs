\ gonna use the ring sample rate
require ../audio/ring.fs

: samples->64ths ( u bpm -- u ) * 64 * 60 / sample-rate / ;

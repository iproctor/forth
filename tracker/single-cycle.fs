require pitch.fs

: single-cycle>waveform 2 cells + ;
: single-cycle>\waveform 3 cells + ;

: single-cycle-voice>sample-len voice>instrument cell+ ;
: single-cycle-voice>dur single-cycle-voice>sample-len cell+ ;

: single-cycle-voice>waveform ( scvoice -- c-addr u ) voice>instrument @ v. single-cycle>waveform single-cycle>\waveform v @ @ ;

: single-cycle-destr ;
: single-cycle-gen ( u scvoice -- n n flag ) 2dup single-cycle-voice>dur @ >= IF 2drop 0 0 true exit THEN
  >r r@ single-cycle-voice>sample-len @ v. mod r> single-cycle-voice>waveform 2swap interpolate dup false ;

: single-cycle-trigger ( u u instrument -- voice ) >r pitch-of-note pitch-to-samples r>
  6 cells allocz >r r@ voice>instrument ! r@ single-cycle-voice>sample-len !
  4000 r@ single-cycle-voice>dur !
  ['] single-cycle-gen r@ voice>gen-xt !  ['] single-cycle-destr r@ voice>destr-xt ! r> ;

: new-single-cycle-instrument ( r: gain waveform waveform-len --- instrument ) 4 cells allocz >r
  ['] single-cycle-trigger r@ !  r@ cell+ f!  r@ single-cycle>\waveform !  r@ single-cycle>waveform ! r> ;

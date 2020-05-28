require pitch.fs
require voice.fs
require ../audio/wav.fs

instrument%
  cell% field single-cycle>waveform
  cell% field single-cycle>\waveform
end-struct single-cycle%

voice%
  cell% field single-cycle-voice>sample-len
  cell% field single-cycle-voice>dur
end-struct single-cycle-voice%

: single-cycle-voice>waveform ( scvoice -- c-addr u ) voice>instrument @ v. single-cycle>waveform single-cycle>\waveform v @ @ ;

: single-cycle-destr drop ;
: single-cycle-gen ( u scvoice -- n n flag ) 2dup single-cycle-voice>dur @ >= IF 2drop 0 0 true exit THEN
  >r r@ single-cycle-voice>sample-len @ v. mod r> single-cycle-voice>waveform 2swap interpolate dup false ;

: single-cycle-trigger ( note dur instrument -- voice )
  ['] single-cycle-gen ['] single-cycle-destr single-cycle-voice% %alloc v. voice-init >r
  r@ single-cycle-voice>dur !
  pitch-of-note pitch-to-samples  r@ single-cycle-voice>sample-len !
  r> ;

: new-single-cycle-instrument ( r: gain waveform waveform-len --- instrument ) single-cycle% %alloc >r
  ['] single-cycle-trigger r@ instrument-init  r@ single-cycle>\waveform !  r@ single-cycle>waveform ! r> ;


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

: load-single-cycle ( c-addr u -- c-addr u ) slurp-file drop v. wav->n get-chunks get-data-chunk data->samps swap ;

require instrument.fs
require ../audio/ring.fs
\ Thing about delay is that it's not a pure function of the sample clock. The ring buffer
\ simply contains the last generated samples, doesn't matter where they came from.
\ Still works as expected in normal playback.

instrument%
  cell% field delay>instrument
  cell% field delay>level
  cell% field delay>feedback
  cell% field delay>period
  cell% field delay>ring
  cell% field delay>clock
end-struct delay%

: delay>level@ delay>level f@ ;
: delay>feedback@ delay>feedback f@ ;
: delay>period@ delay>period @ ;

: delay-scale-feedback ( n n delay -- n n ) v. delay>level@ delay>feedback@ f* scale-samples ;

voice%
  cell% field delay-voice>subvoice
end-struct delay-voice%
: delay-voice>ring voice>instrument @ delay>ring @ ;
: delay-voice>level@ voice>instrument @ delay>level@ ;
: delay-voice>clock@ voice>instrument @ delay>clock @ ;

: delay-voice-run-subvoice ( u delay-voice -- n n flag ) delay-voice>subvoice @ voice>gen ;
: delay-sub-clock ( u delay-voice -- u ) v. voice>t0@ delay-voice>clock@ - + ;
: delay-voice-write-sample ( n n u delay-voice -- ) 2>r r@ delay-voice>level@ scale-samples 2r> v. delay-sub-clock  delay-voice>ring  ( ." dws" .s cr ) ring-frame+! ;
: delay-gen ( u delay-voice -- n n flag ) 2>r 2r@ delay-voice-run-subvoice IF 2rdrop true exit THEN
  2dup 2r> delay-voice-write-sample false ;
: delay-destr ;

: new-delay-voice ( subvoice instrument -- voice ) swap delay-voice% %alloc >r
  r@ delay-voice>subvoice !  ['] delay-gen ['] delay-destr r> v. voice-init ;
: delay-trigger ( note-ind dur instrument -- voice ) dup >r delay>instrument @ instrument-trigger r> new-delay-voice ;

: delay-init-ring ( delay -- ) init-ring  2dup v delay>period@ ring-adv-write  swap delay>ring ! ;
: add-delay ( r:level r:feedback period instrument -- instrument ) delay% %alloc >r
  ['] delay-trigger 1e r@ instrument-init  r@ delay>instrument !  r@ delay>feedback f!  r@ delay>level f!  r@ delay>period !  r@ delay-init-ring  r> ;

: delay-adv-write ( u delay -- ) 2dup delay>ring @ ring-adv-write  delay>clock +! ;
: delay-add-feedback ( u delay ) { u delay } u 0 U+DO
    i  delay delay>ring @  ring-at-read-offset read-frame
    delay delay-scale-feedback
    i  delay delay>ring @  ring-frame+!
  LOOP ;
: delay>feedback-and-adv-write ( u delay -- ) 2dup delay-add-feedback delay-adv-write ;
: delay-effect-process ( u ring delay -- ) v over v. delay>feedback-and-adv-write  delay>ring @ swap rot ring-transfer ;

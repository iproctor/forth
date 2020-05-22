require ../audio/portaudio.fs
require ../ds/list.fs
require scene.fs
require ../audio/ring.fs
require bpm.fs
require ../utils.fs

0 value sample-clock
: advance-sample-clock ( u -- ) sample-clock + to sample-clock ;
100 value bpm

: 64ths-clock sample-clock bpm samples->64ths ;

new-list-anchor constant scene-list
: add-scene ( scene -- ) scene-list list-anchor-append ;

new-list-anchor constant active-voices
: add-active-voice ( voice -- ) dup voice>t0 sample-clock swap !  active-voices list-anchor-append ;
\ true if voice is done
: play-voice ( samps ring voice -- flag ) -rot >r 0 U+DO
    dup i swap voice>gen IF 2drop unloop true exit THEN
    i r@ ring-frame+!
  LOOP rdrop false ;
: play-voices ( u ring -- ) 2>r active-voices list-anchor->list BEGIN
    ( prev-next )
    dup @ WHILE
    dup @ list->val @ 2r@ rot play-voice
    IF dup list-rm-node ELSE list->next @ THEN
  REPEAT drop 2rdrop ;

: current-scene-and-offset ( -- 64th-offset scene ) 64ths-clock scene-list list-anchor->list@ BEGIN
    dup WHILE
    dup >r list->val@ scene-dur CASE
      -1 OF r> list->val@ exit ENDOF
      2dup < IF drop r> list->val@ exit ELSE - r> list->next@ THEN
    0 ENDCASE
  REPEAT ;
: fire-pending-triggers ( offset scene -- n ) max-int -rot  scene>slots @ for-list-anc-val[
    ( steps-to-next-trig offset scn-slot )
    v dup  scn-slot-fire-trigger-at dup IF add-active-voice ELSE drop THEN
    ( min-steps-to-next-trig offset step-to-next-trig )
    swap v min
  ]for-list drop ;

: orch-fill-ring ( ring -- flag ) BEGIN
    dup ring-capacity 1- dup 100 < IF 2drop false exit THEN
    current-scene-and-offset dup 0= IF 2drop 2drop true exit THEN
    fire-pending-triggers bpm 64ths->samples min dup >r  over play-voices
    r> v. advance-sample-clock  over ring-adv-write
  AGAIN drop false ;

: orch-fill-loop ( ring -- ) BEGIN
    dup orch-fill-ring 0= WHILE
    10 pa-sleep
  REPEAT drop ;

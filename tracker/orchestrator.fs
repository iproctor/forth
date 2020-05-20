require ../ds/list.fs
require scene.fs
require ../audio/ring.fs
require bpm.fs
require ../utils.fs

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

0 value sample-clock
: advance-sample-clock ( u -- ) sample-clock + to sample-clock ;
100 value bpm

: current-scene-and-offset ( -- 64th-offset scene ) ;
: fire-pending-triggers ( offset scene -- n ) max-int -rot  scene>slots @ for-list-anc-val[
    ( steps-to-next-trig offset scn-slot )
    v dup  scn-slot-fire-trigger-at dup IF add-active-voice ELSE drop THEN
    ( min-steps-to-next-trig offset step-to-next-trig )
    swap v min
  ]for-list drop ;

: orch-fill-ring ( ring -- ) BEGIN
    dup ring-capacity 1- dup 100 > WHILE
    current-scene-and-offset dup WHILE
    fire-pending-triggers min dup >r  over play-voices
    r> advance-sample-clock
  REPEAT 2drop ;

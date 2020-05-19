require ../ds/list.fs
require scene.fs
require ../audio/ring.fs
require bpm.fs

new-list-anchor constant scene-list
: add-scene ( scene -- ) scene-list list-anchor-append ;

new-list-anchor constant active-voices

0 value sample-clock
100 value bpm

: scene-fire-triggers-at ( offset scene ) scene>slots @ list-anchor->list@ for-list[ v dup  list->val @ seq-slot-fire-trigger-at dup IF add-active-voice ELSE drop THEN ]for-list drop ;
: current-scene-and-offset ( -- 64th-offset scene ) ;
: fire-pending-triggers ( -- ) current-scene-and-offset dup 0= IF 2drop exit THEN scene-fire-triggers-at ;

: orch-fill-ring ( ring -- ) BEGIN
    dup ring-capacity 1- dup 100 WHILE
    fire-pending-triggers
    samples-until-next-trigger min dup >r  over play-voices
    r> advance-sample-clock
  REPEAT 2drop ;

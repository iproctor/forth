require ../audio/portaudio.fs
require ../ds/list.fs
require ../ds/dlist.fs
require scene.fs
require voice.fs
require ../audio/ring.fs
require bpm.fs
require ../utils.fs

0 value sample-clock
: advance-sample-clock ( u -- ) sample-clock + to sample-clock ;
100 value bpm

: samples-per-64th 1 bpm 64ths->samples ;
: 64ths-clock ( -- u flag ) sample-clock samples-per-64th /mod swap 0= ;
: on-64th? 64ths-clock nip ;
: samples-until-64th ( -- u ) sample-clock samples-per-64th /mod-round-up drop
  dup 0= IF drop samples-per-64th THEN ;

new-list-anchor constant scene-list
: add-scene ( scene -- ) scene-list list-anchor-append ;

dlist-new-head constant active-voices
: add-active-voice ( voice -- ) dup voice>t0 sample-clock swap !  active-voices dlist-ins-after ;
: active-voices-start active-voices dlist>next@ ;
: no-active-voices? ( -- flag ) active-voices-start 0= ;
\ true if voice is done
: play-voice ( samps ring voice -- flag ) rot 0 U+DO
    dup i sample-clock + swap voice>gen IF 2drop 2drop unloop true exit THEN
    ( ring voice n n )
    2>r over 2r> rot i swap ring-frame+!
  LOOP 2drop false ;
: play-voices ( u ring -- ) 2>r active-voices-start BEGIN
    dup WHILE
    dup dlist>val@ 2r@ rot play-voice
    IF ." rm voice" cr v. dlist>next@ dlist-rm-node ELSE dlist>next@ THEN
  REPEAT drop 2rdrop ;

0 value effects
: register-effect ( data xt -- ) effects list-prepend2 to effects ;
\ For each effect, call xt with n samples and ring. Effect advances n samples and writes n samples
\ to ring.
: effects-process ( u ring -- ) effects for-list[ v 2dup list->val 2@ execute ]for-list 2drop ;

: current-scene-and-offset ( -- 64th-offset scene ) 64ths-clock drop scene-list list-anchor->list@ BEGIN
    dup WHILE
    dup >r list->val@ scene-dur CASE
      -1 OF r> list->val@ exit ENDOF
      2dup < IF drop r> list->val@ exit ELSE - r> list->next@ THEN
    0 ENDCASE
  REPEAT ;
: fire-scene-triggers ( offset scene -- ) scene>slots @ for-list-anc-val[
    v dup  scn-slot-fire-trigger-at dup IF add-active-voice ELSE drop THEN
  ]for-list drop ;
: iteration-samples ( ring -- u ) ring-capacity 1-  samples-until-64th min ;
: fire-pending-triggers ( -- flag ) current-scene-and-offset dup IF
    fire-scene-triggers false ELSE 2drop no-active-voices? THEN ;
: orch-fill-ring ( ring -- flag ) BEGIN
    dup iteration-samples dup 0= IF 2drop false exit THEN
    dup . cr
    on-64th? IF fire-pending-triggers IF 2drop true exit THEN THEN
    dup >r  over play-voices
    r> v. advance-sample-clock  over 2dup effects-process ring-adv-write
  AGAIN drop false ;

: orch-fill-loop ( ring -- ) BEGIN
    dup orch-fill-ring 0= WHILE
    10 pa-sleep
  REPEAT drop ;

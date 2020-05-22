require ../utils.fs
require ../ds/list.fs
require sequence.fs
require instrument.fs

: scn-slot>seq ;
: scn-slot>seq@ @ ;
: scn-slot>instrument cell+ ;
: scn-slot>loop 2 cells + ;
\ multiplier is power of 2 exponent on speed of playback. range is -4 (whole note steps) to 2 (64th steps. 0 is 16th steps)
: scn-slot>multiplier 3 cells + ;
: new-scn-slot ( seq inst loop? mult -- scn-slot ) 4 cells allocz v. 4!r ;

: scn-slot-scn-steps ( scn-slot -- u ) scn-slot>seq @ list-n ;
\ in 64ths
: scn-slot-step-dur ( scn-slot -- u ) scn-slot>multiplier @  2 swap -  1 swap lshift ;
\ dur in 64th steps. -1 if infinite
: scn-slot-seq-dur ( scn-slot -- u ) v. scn-slot-scn-steps scn-slot-step-dur * ;
: scn-slot-dur ( scn-slot -- n ) dup scn-slot>loop @ IF drop -1 ELSE scn-slot-seq-dur THEN ;
: scn-slot-within-dur ( 64ths scn-slot -- flag ) scn-slot-dur CASE
    -1 OF drop true ENDOF
    < 0
  ENDCASE ;

\ flag if evenly on the seq step
: scn-step-from-64ths ( 64ths scn-slot -- u flag ) scn-slot-step-dur /mod swap 0= ;

: /mod-round-up ( n n -- n n ) dup >r /mod swap dup IF r> swap - swap 1+ ELSE swap rdrop THEN ;

\ Flag is true when offset past duration
: adjust-offset-for-loop ( offset scn-slot -- offset' flag ) v. scn-slot-seq-dur scn-slot>loop @ IF mod false ELSE v dup >= THEN ;

: seek-next-trig-until-end ( node -- node u ) 0 BEGIN
    over while
    over list->val @ trigger-note? IF exit THEN
    v list->next@ 1+
  REPEAT ;
: seek-next-trig-until-node ( node node u -- node u ) BEGIN
    >r 2dup <> r> swap WHILE
    over list->val @ trigger-note? 0= WHILE
    v list->next@ 1+
  AGAIN THEN THEN nip3rd ;

: seek-next-trig ( scn-slot node -- node u )
  dup seek-next-trig-until-end ( slot nth-node node u ) over IF 2swap 2drop exit THEN
  nip rot ( nth-node u slot ) dup scn-slot>loop @ 0= IF 3drop 0 max-int exit THEN
  scn-slot>seq@ swap seek-next-trig-until-node over 0= IF drop max-int THEN ;

: scn-slot-next-trig ( u scn-slot -- node u ) tuck scn-slot>seq@ list-nth seek-next-trig ;

: scn-slot-next-index ( 64ths scn-slot -- 64ths u ) scn-slot-step-dur /mod-round-up ;

: seq-trig-after-offset ( 64ths scn-slot -- node 64ths )
  v. adjust-offset-for-loop swap IF 2drop 0 max-int exit THEN
  ( 64ths scn-slot )
  v. scn-slot-next-index
  ( 64ths idx scn-slot )
  v. scn-slot-next-trig
  ( 64ths node idx scn-lot )
  scn-slot-step-dur * rot + ;


: scn-slot-play-seq-node ( seq-node scn-slot -- voice ) v list->val@ scn-slot>instrument @ instrument-play-trigger ;

: scn-slot-steps-to-next-event ( node scn-slot -- 64ths ) dup rot list->next @ seek-next-trig
  over IF 3drop max-int ELSE nip v scn-slot-step-dur 1+ * THEN ;

: scn-slot-fire-trigger-at ( 64ths scn-slot -- 64ths voice )
  tuck seq-trig-after-offset ( scn-slot node 64ths-shortfall ) dup IF v 2drop 0 exit THEN
  drop 2dup swap scn-slot-play-seq-node ( scn-lot node voice )
  -rot swap scn-slot-steps-to-next-event ( voice steps ) swap ;

: scene>slots ;
\ dur in 64th steps. if 0 the length of the longest non looping seq, or inf
: scene>dur cell+ ;
: new-scene new-list-anchor 2 cells allocz v. ! ;
\ in 64ths
: add-scn-slot-dur ( n n -- n ) over -1 = IF 2drop -1 ELSE dup -1 = IF nip ELSE max THEN THEN ;
: scene-dur ( scene -- u ) dup scene>dur @ dup IF nip exit THEN drop  scene>slots @ list-anchor->list@  0 swap  for-list[ list->val @ scn-slot-dur add-scn-slot-dur ]for-list ;

: add-to-scene ( seq inst loop? mult scene -- ) v new-scn-slot scene>slots @ list-anchor-append ;


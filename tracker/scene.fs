require ../utils.fs
require ../ds/list.fs
require sequence.fs
require instrument.fs

struct
  cell% field scn-slot>seq
  cell% field scn-slot>instrument
  cell% field scn-slot>loop
  cell% field scn-slot>multiplier
end-struct scn-slot%

: scn-slot>seq@ scn-slot>seq @ ;
\ multiplier is power of 2 exponent on speed of playback. range is -4 (whole note steps) to 2 (64th steps. 0 is 16th steps)
: new-scn-slot ( seq inst loop? mult -- scn-slot ) scn-slot% %alloc v. 4!r ;

: scn-slot-scn-steps ( scn-slot -- u ) scn-slot>seq @ list-n ;
\ in 64ths
: scn-slot-step-dur ( scn-slot -- u ) scn-slot>multiplier @  2 swap -  1 swap lshift ;
\ dur in 64th steps. -1 if infinite
: scn-slot-seq-dur ( scn-slot -- u ) v. scn-slot-scn-steps scn-slot-step-dur * ;
: scn-slot-dur ( scn-slot -- n ) dup scn-slot>loop @ IF drop -1 ELSE scn-slot-seq-dur THEN ;

\ flag if evenly on the seq step
: scn-step-from-64ths ( 64ths scn-slot -- u flag ) scn-slot-step-dur /mod swap 0= ;

\ Flag is true when offset past duration
: adjust-offset-for-loop ( offset scn-slot -- offset' flag ) v. scn-slot-seq-dur scn-slot>loop @ IF mod false ELSE v dup >= THEN ;
: scn-slot-nth-trig ( u scn-slot -- trigger ) scn-slot>seq@ list-nth list->val@ ;
: seq-trig-at-offset ( 64ths scn-slot -- trigger )
  v. adjust-offset-for-loop swap IF 2drop 0 exit THEN
  v. scn-step-from-64ths swap 0= IF 2drop 0 exit THEN
  scn-slot-nth-trig dup trigger-type @ trigger:note <> IF drop 0 THEN ;

: scn-slot-play-trigger ( trigger scn-slot -- voice ) scn-slot>instrument @ instrument-play-trigger ;

: scn-slot-fire-trigger-at ( 64ths scn-slot -- voice )
  tuck seq-trig-at-offset dup 0= IF nip exit THEN swap scn-slot-play-trigger ;

struct
  cell% field scene>slots
  \ dur in 64th steps. if 0 the length of the longest non looping seq, or inf
  cell% field scene>dur
end-struct scene%

: new-scene new-list-anchor 0 scene% %alloc v. 2!r ;
\ in 64ths
: add-scn-slot-dur ( n n -- n ) over -1 = IF 2drop -1 ELSE dup -1 = IF nip ELSE max THEN THEN ;
: scene-dur ( scene -- u ) dup scene>dur @ dup IF nip exit THEN drop  scene>slots @ list-anchor->list@  0 swap  for-list[ list->val @ scn-slot-dur add-scn-slot-dur ]for-list ;

: add-to-scene ( seq inst loop? mult scene -- ) v new-scn-slot scene>slots @ list-anchor-append ;


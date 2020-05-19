require ../utils.fs
require ../ds/list.fs

: seq-slot>seq ;
: seq-slot>instrument cell+ ;
: seq-slot>loop 2 cells + ;
\ multiplier is power of 2 exponent on speed of playback. range is -4 (whole note steps) to 2 (64th steps. 0 is 16th steps)
: seq-slot>multiplier 3 cells + ;
: new-seq-slot ( seq inst flag mult -- seq-slot ) 4 cells allocz v. 4!r ;

: seq-slot-seq-steps ( seq-slot -- u ) seq-slot>seq @ list-n ;
\ in 64ths
: seq-slot-step-dur ( seq-slot -- u ) seq-slot>multiplier @  2 swap -  1 swap lshift ;
\ dur in 64th steps. -1 if infinite
: seq-slot-dur ( seq-slot -- n ) dup seq-slot>loop IF drop -1 ELSE v. seq-slot-seq-steps seq-slot-step-dur * THEN ;

: scene>slots ;
\ dur in 64th steps. if 0 the length of the longest non looping seq, or inf
: scene>dur cell+ ;
: new-scene new-list-anchor 2 cells allocz v. ! ;
\ in 64ths
: add-seq-slot-dur ( n n -- n ) over -1 = IF 2drop -1 ELSE + THEN ;
: scene-dur ( scene -- u ) dup scene>dur @ dup IF nip exit THEN drop  scene>slots @ list-anchor->list@  0 swap  for-list[ list->val @ seq-slot-dur add-seq-slot-dur ]for-list ;

: add-to-scene ( seq inst flag mult scene -- ) v new-seq-slot scene>slots @ list-anchor-append ;


require ../combis.fs

8 constant octaves

0 constant trigger:rest
1 constant trigger:note
32 constant default-dur \ a 16th note, implies that a sequence of 16 has total dur of 32 * 16 = 512

0 value cur-seq

: seq-trigs ( seq -- c-addr ) ;
: seq-trigs@ ( seq -- c-addr ) seq-trigs @ ;
: seq-trig-count ( seq -- c-addr ) seq-trigs cell+ ;
: seq-trig-count@ ( seq -- c-addr ) seq-trig-count @ ;
: seq-sz ( seq -- c-addr ) seq-trig-count cell+ ;
: resize-seq-if-full ( -- ) cur-seq seq-sz @  cur-seq seq-trig-count@ = IF
    cur-seq cur-seq seq-sz @ 2* dup >r resize throw  r> cur-seq seq-sz ! THEN ;
: next-seq-slot ( -- c-addr ) cur-seq v. seq-trigs@ seq-trig-count@ cells + ;
: inc-seq-trigs ( -- ) cur-seq seq-trigs 1 swap +! ;

: append-to-seq ( trigger -- ) resize-seq-if-full  next-seq-slot !  inc-seq-trigs ;

: <| ( -- ) 3 cells allocate throw to cur-seq  16 cells allocate throw cur-seq seq-trigs !  0 cur-seq seq-trig-count !  16 cur-seq seq-sz ! ;
: |> ( -- seq ) cur-seq  0 to cur-seq ;

: trigger-type ( trigger -- c-addr ) ;
: trigger-dur ( trigger -- c-addr ) cell+ ;
: trigger-notes ( trigger -- c-addr ) trigger-dur cell+ ;
: trigger-notes2! ( u u trigger -- ) trigger-notes 2! ;
: init-trig ( trigger u u -- ) >r over trigger-dur !  r> swap trigger-type ! ;
: <-> ( -- ) 2 cells allocate throw  dup trigger:rest default-dur init-trig  append-to-seq ;
: new-note ( u u -- ) 4 cells allocate throw  dup trigger:note default-dur init-trig  v. trigger-notes2! append-to-seq ;

: def-notes [CHAR] h [CHAR] a U+DO octaves 0 U+DO <<# i 0 #s j hold #> nextname : j [CHAR] a - POSTPONE literal i POSTPONE literal POSTPONE new-note POSTPONE ;  #>> LOOP LOOP ;
def-notes



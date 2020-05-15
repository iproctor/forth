require ../combis.fs
require ../ds/list.fs

8 constant octaves

0 constant trigger:rest
1 constant trigger:note
32 constant default-dur \ a 16th note, implies that a sequence of 16 has total dur of 32 * 16 = 512

0 value cur-seq

: append-to-seq ( trigger -- ) cur-seq list-anchor-append ;

: <| ( -- ) new-list-anchor to cur-seq ;
: |> ( -- seq ) cur-seq list-anchor-to-list  0 to cur-seq ;

: trigger-type ( trigger -- c-addr ) ;
: trigger-dur ( trigger -- c-addr ) cell+ ;
: trigger-notes ( trigger -- c-addr ) trigger-dur cell+ ;
: trigger-notes2! ( u u trigger -- ) trigger-notes 2! ;
: init-trig ( trigger u u -- ) >r over trigger-dur !  r> swap trigger-type ! ;
: <-> ( -- ) 2 cells allocate throw  dup trigger:rest default-dur init-trig  append-to-seq ;
: new-note ( u u -- ) 4 cells allocate throw  dup trigger:note default-dur init-trig  v. trigger-notes2! append-to-seq ;

: def-notes [CHAR] h [CHAR] a U+DO octaves 0 U+DO <<# [CHAR] > hold i 0 #s j hold [CHAR] < hold #> nextname : j [CHAR] a - POSTPONE literal i POSTPONE literal POSTPONE new-note POSTPONE ;  #>> LOOP LOOP ;
def-notes



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

create note-names s" c" 2, s" c#" 2, s" d" 2, s" d#" 2, s" e" 2, s" f" 2, s" f#" 2, s" g" 2, s" g#" 2, s" a" 2, s" a#" 2, s" b" 2,
: note-to-name ( u -- c-addr u ) 2* cells note-names + 2@ ;
: hold-str ( c-addr u -- ) 2dup type cr BEGIN dup WHILE 2dup + 1- c@ hold  1- REPEAT ;
: note-index-to-note ( u -- u u ) 12 /mod ;

: def-notes 12 0 U+DO octaves 0 U+DO <<# [CHAR] > hold i 0 #s j note-to-name hold-str [CHAR] < hold #> nextname : j POSTPONE literal i POSTPONE literal POSTPONE new-note POSTPONE ;  #>> LOOP LOOP ;
def-notes



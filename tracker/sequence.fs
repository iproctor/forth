require ../combis.fs
require ../ds/list.fs

8 constant octaves

0 constant trigger:rest
1 constant trigger:note
4 constant default-dur \ a 16th note

0 value cur-seq

: append-to-seq ( trigger -- ) cur-seq list-anchor-append ;

: <| ( -- ) new-list-anchor to cur-seq ;
: |> ( -- seq ) cur-seq list-anchor-to-list  0 to cur-seq ;

: trigger-type ( trigger -- c-addr ) ;
: trigger-dur ( trigger -- c-addr ) cell+ ;
: trigger-dur@ ( trigger -- c-addr ) trigger-dur @ ;
: trigger-notes ( trigger -- c-addr ) trigger-dur cell+ ;
: trigger-notes2! ( u u trigger -- ) trigger-notes 2! ;
: init-trig ( trigger u u -- ) >r over trigger-type !  r> swap trigger-dur ! ;
: <-> ( -- ) 2 cells allocate throw  dup trigger:rest default-dur init-trig  append-to-seq ;
: new-note ( u u -- ) 4 cells allocate throw  dup trigger:note default-dur init-trig  v. trigger-notes2! append-to-seq ;
: trigger-note? ( trigger -- flag ) trigger-type @ trigger:note = ;

create note-names s" a" 2, s" a#" 2, s" b" 2, s" c" 2, s" c#" 2, s" d" 2, s" d#" 2, s" e" 2, s" f" 2, s" f#" 2, s" g" 2, s" g#" 2,
: note-to-name ( u -- c-addr u ) 2* cells note-names + 2@ ;
: hold-str ( c-addr u -- ) BEGIN dup WHILE 2dup + 1- c@ hold  1- REPEAT 2drop ;
: note-index-to-note ( u -- u u ) 12 /mod ;
: note-to-note-index ( u u -- u ) 12 * + ;
: trigger-note-index ( trigger -- u ) trigger-notes 2@ note-to-note-index ;

: def-notes 12 0 U+DO octaves 0 U+DO <<# [CHAR] > hold i 0 #s j note-to-name hold-str [CHAR] < hold #> nextname : j POSTPONE literal i POSTPONE literal POSTPONE new-note POSTPONE ;  #>> LOOP LOOP ;
def-notes



require ../ds/list.fs
require ../ds/sort.fs
require sequence.fs

\ F - E
create note-keys CHAR a , CHAR , , CHAR o , CHAR . , CHAR e , CHAR p , CHAR u , CHAR i , CHAR f , CHAR d , CHAR g , CHAR h ,

: instrument>xt ( instrument -- xt ) @ ;
: instrument>data ( instrument -- w ) cell+ @ ;
: instrument>trigger ( note oct instrument -- voice ) v. instrument>data instrument>xt execute ;
: instrument-play-trigger ( trigger instrument -- voice ) >r triger-notes 2@ r> instrument>trigger ;

create path-buf 256 allot
0 value path-buf-len
0 value path-buf-mark
: init-path-buf ( c-addr u -- ) dup to path-buf-mark path-buf swap cmove ;
: slurp-path-buf ( -- c-addr u ) path-buf path-buf-len  slurp-file ;
: post-mark-str ( -- c-addr u ) path-buf path-buf-mark +  path-buf-len path-buf-mark - ;
: copy-after-mark ( -- c-addr u ) post-mark-str dup allocate throw swap 2dup 2>r cmove 2r> ;
: pb-wav-suffix? ( -- flag ) path-buf-len 4 > IF path-buf path-buf-len + 4 - 4 s" .wav" str= ELSE false THEN ;
: remaining-pb ( -- c-addr u ) path-buf path-buf-mark +  256 path-buf-mark - ;

: new-sample ( c-addr u c-addr -- sample ) dup mk-wav-gen  4 cells allocate throw v. 4!r ;
: new-sample-from-pb ( -- sample ) slurp-path-buf drop copy-after-mark rot new-sample ;
: sample>name ( sample -- c-addr u ) 2@r ;
: sample>wav ( sample -- c-addr ) 2 cells + @ ;
: sample>gen ( sample -- c-addr ) 3 cells + @ ;
: sample-cmp ( sample-arr u u -- sample-arr n ) dup3rd 2elem@ ['] sample>name bi@ str< IF -1 ELSE 1 THEN ;
' sample-cmp ' u-arr-swp-d sorter: sample-sort drop ;

: read-dir-to-pb ( dirid -- u flag ) remaining-pb rot read-dir throw  dup IF swap path-buf-mark + to path-buf-len THEN ;
: load-samples ( dir-str -- sample-addr u ) new-list-anchor >r  2dup init-path-buf  open-dir throw
  BEGIN
    dup read-dir-to-pb WHILE
    pb-wav-suffix? IF new-sample-from-pb r@ list-anchor-append THEN
  REPEAT
  2drop
  r> list-anchor-to-list list-to-arr 2dup sample-sort ;

\ takes current sample clock
: trigger-sample ( u sample -- voice ) sample>gen new-wav-voice ;

: samples-trigger-note ( u u samples -- voice ) >r note-index-to-note cells r> + trigger-sample ;

: new-samples-instrument ( samples -- instrument ) ['] samples-trigger-note  2 cells allocz  v 2! ;



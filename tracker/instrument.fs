require ../ds/list.fs
require ../ds/sort.fs

create path-buf 256 allot
0 value path-buf-len
0 value path-buf-mark
: init-path-buf ( c-addr u -- ) dup to path-buf-mark path-buf swap cmove ;
: slurp-path-buf ( -- c-addr u ) path-buf path-buf-len  slurp-file ;
: post-mark-str ( -- c-addr u ) path-buf path-buf-mark +  path-buf-len path-buf-mark - ;
: copy-after-mark ( -- c-addr u ) post-mark-str dup allocate throw swap 2dup 2>r cmove 2r> ;
: pb-wav-suffix? ( -- flag ) path-buf-len 4 > IF path-buf path-buf-len + 4 - 4 s" .wav" str= ELSE false THEN ;
: remaining-pb ( -- c-addr u ) path-buf path-buf-mark +  256 path-buf-mark - ;

: new-sample ( c-addr u c-addr -- sample ) 3 cells allocate throw v. 3!r ;
: new-sample-from-pb ( -- sample ) slurp-path-buf drop copy-after-mark rot new-sample ;
: sample-name ( sample -- c-addr u ) 2@r ;
: sample-wav ( sample -- c-addr ) 2 cells + @ ;
: sample-cmp ( sample-arr u u -- sample-arr n ) dup3rd 2elem@ ['] sample-name bi@ str< IF -1 ELSE 1 THEN ;
' sample-cmp ' u-arr-swp-d sorter: sample-sort drop ;

: read-dir-to-pb ( dirid -- u flag ) remaining-pb rot read-dir throw  dup IF swap path-buf-mark + to path-buf-len THEN ;
: load-samples ( dir-str -- str-addr u ) new-list-anchor >r  2dup init-path-buf  open-dir throw
  BEGIN
    dup read-dir-to-pb WHILE
    pb-wav-suffix? IF new-sample-from-pb r@ list-anchor-append THEN
  REPEAT
  2drop
  r> list-anchor-to-list list-to-arr 2dup sample-sort ;


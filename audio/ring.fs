require ../combis.fs

\ 48khz, 16 bit samples, 2 channels, 100ms
2 2 * constant \frame
48000 constant sample-rate
sample-rate 10 / constant frame-count
sample-rate \frame * constant \ring
create ring \ring allot
\ Pointers are to frames
0 value read-ptr
0 value write-ptr
: frames ( u -- u ) \frame * ;
: left-channel ( c-addr -- c-addr ) ;
: right-channel ( c-addr -- c-addr ) 2 + ;
: w+! ( n c-addr -- ) tuck sw@ + swap w! ;
: add-left ( n c-addr -- ) left-channel w+! ;
: add-right ( n c-addr -- ) right-channel w+! ;
\ Writes
: ring-capacity ( -- u ) read-ptr write-ptr < IF \ring write-ptr read-ptr - - ELSE read-ptr write-ptr - THEN \frame / ;
: frame-write-offset-index ( u -- u ) write-ptr + frame-count mod ;
: frame-at-write-offset ( u -- c-addr ) frame-write-offset-index frames ring + ;

\ Mixes a frame at offset
: ring-frame+! ( n n u -- ) frame-at-write-offset v. add-right add-left ;
: ring-adv-write ( u -- ) frame-write-offset-index  barrier  to write-ptr ;

: ring-at-read ( -- c-addr ) ring read-ptr frames + ;
\ Wont wrap
: erase-from-read ( u -- ) frames ring-at-read swap erase ;
: ring-adv-read ( u -- ) dup erase-from-read  read-ptr + frame-count mod to read-ptr ;
: frames-after-read ( -- u ) frame-count read-ptr - ;

\ Returns frames written
: read-and-adv ( c-addr u -- u ) dup >r frames  ring-at-read -rot  cmove  r@ ring-adv-read r> ;
: read-ring-to-end ( c-addr u -- u ) frames-after-read min read-and-adv ;
: offset-frames ( c-addr u u -- c-addr u ) >r r@ - swap r> frames + swap ;
: ring-read ( c-addr u -- u ) read-ptr write-ptr > IF 2dup read-ring-to-end v. offset-frames ELSE 0 THEN
  over 0= IF nip nip exit THEN
  >r write-ptr read-ptr - min  read-and-adv  r> + ;

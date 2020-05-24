require ../combis.fs
require ../utils.fs
require portaudio.fs

\ 48khz, 16 bit samples, 2 channels, 100ms
2 value channels
48000 value sample-rate
2 value sample-bytes
5000 value buf-dur-ms

: \frame channels sample-bytes * ;
: frame-count sample-rate buf-dur-ms * 1000 / ;
: \ring frame-count \frame * ;
: max-sample-mag 1 sample-bytes 8 * 1- lshift ;

: frames ( u -- u ) \frame * ;
: left-channel ( c-addr -- c-addr ) ;
: right-channel ( c-addr -- c-addr ) sample-bytes + ;
: read-frame ( c-addr -- n n ) v. left-channel right-channel v sw@ sw@ ;
: clip-sample ( n -- n ) dup abs max-sample-mag > IF ." CLIPPING " dup . .s cr THEN  max-sample-mag min  max-sample-mag negate  max ;
: w+! ( n c-addr -- ) tuck sw@ + clip-sample swap  w! ;
: add-left ( n c-addr -- ) left-channel w+! ;
: add-right ( n c-addr -- ) right-channel w+! ;

: ring>read-ptr ( ring -- c-addr ) ;
: ring>read-ptr@ ( ring -- c-addr ) ring>read-ptr @ ;
: ring>write-ptr ( ring -- c-addr ) ring>read-ptr cell+ ;
: ring>write-ptr@ ( ring -- c-addr ) ring>write-ptr @ ;
: ring>sz ( ring -- u ) ring>write-ptr cell+ @ ;
: ring>data ( ring -- c-addr ) ring>write-ptr 2 cells + ;
: init-ring ( -- ring ) 4 cells \ring + allocz ;
: read-after-write? ( ring -- flag ) v. ring>read-ptr@ ring>write-ptr@ > ;

\ Writes
: ring-capacity ( ring -- u ) dup read-after-write? IF v. ring>read-ptr@ ring>write-ptr@ - ELSE v. ring>write-ptr@ ring>read-ptr@ - frame-count swap - THEN ;
: ring-count ring-capacity frame-count swap - ;
: ring-capacity-ms ( ring -- u ) ring-capacity 1000 * sample-rate / ;
: frame-write-offset-index ( u ring -- u ) ring>write-ptr@ + frame-count mod ;
: frame-at-write-offset ( u ring -- c-addr ) dup >r frame-write-offset-index frames r> ring>data + ;

\ Mixes a frame at offset
: ring-frame+! ( n n u ring -- ) frame-at-write-offset v. add-right add-left ;
: ring-adv-write ( u ring -- ) dup >r frame-write-offset-index  barrier  r> ring>write-ptr ! ;

: ring-at-read ( ring -- c-addr ) v. ring>data ring>read-ptr@ frames + ;
: ring-read-offset ( u ring -- u ) ring>read-ptr@ + frame-count mod ;
: ring-at-read-offset ( u ring -- c-addr ) dup >r ring-read-offset frames r> ring>data + ;
\ Wont wrap
: erase-from-read ( u ring -- ) v frames ring-at-read swap erase ;
: ring-adv-read ( u ring -- ) 2dup erase-from-read  v. ring-read-offset ring>read-ptr ! ;
: frames-after-read ( ring -- u ) ring>read-ptr@ frame-count swap - ;

\ Returns frames written
: read-and-adv ( c-addr u ring -- u ) 2dup 2>r v frames ring-at-read -rot cmove  r> r@ swap ring-adv-read r> ;
: read-ring-to-end ( c-addr u ring -- u ) dup >r frames-after-read min r> read-and-adv ;
: offset-frames ( c-addr u u -- c-addr u ) >r r@ - swap r> frames + swap ;
: read-until-end ( c-addr u ring -- c-addr u u ) v 2dup read-ring-to-end  v. offset-frames ;
: ring-read ( c-addr u ring -- u ) dup read-after-write? IF dup >r read-until-end r> swap ELSE 0 THEN
  >r over 0= IF drop 2drop r> exit THEN
  dup >r v. ring>write-ptr@ ring>read-ptr@ - min  r> read-and-adv  r> + ;
: ring-read-1 ( ring -- n n ) dup ring-at-read read-frame
  rot dup 1 swap erase-from-read  1 swap ring-adv-read ;
: ring-transfer ( from-ring to-ring u -- ) 0 U+DO
    2dup swap ring-read-1 rot i swap ring-frame+!
  LOOP 2drop ;

: ring-pa-callback { ibuf obuf fpb ti sf ring -- n } obuf fpb \frame * erase  obuf fpb ring ring-read drop  ring ring-count IF 0 ELSE 1 THEN ;

' ring-pa-callback pa-stream-callback: constant ring-cb


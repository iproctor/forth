require portaudio.fs
require ../combis.fs



create saw-state 0 c, 0 c,
: inc-channel ( c-addr u -- ) over c@ + swap c! ;
: step-saw ( -- u u ) saw-state c@  saw-state 6 inc-channel  saw-state char+ c@  saw-state char+ 6 inc-channel ;

: saw { ibuf obuf fpb ti sf ud -- n } obuf fpb 0 U+DO step-saw rot v. c! char+ v. c! char+ LOOP drop 0 ;

create stream 0 ,
' saw pa-stream-callback: constant saw-cb

: main
  pa-initialize throw
  ." Initialized" cr
  stream 0 2 0x20 44100e 256 saw-cb 0 pa-open-default-stream throw
  ." Created stream" cr
  stream @ pa-start-stream throw
  ." Started stream" cr
  1000 pa-sleep
  stream @ pa-stop-stream throw
  ." Stopped stream" cr
  stream @ pa-close-stream throw
  pa-terminate
  bye ;

create saw-state-sf 0 , 0 ,
: inc-channel-sf ( c-addr r -- ) dup sf@ f+ fdup 1e f>= IF 2e f- THEN sf! ;
: step-saw-sf ( -- r r ) saw-state-sf sf@  saw-state-sf 0.03e inc-channel-sf  saw-state-sf 4 + sf@  saw-state-sf 4 + 0.01e inc-channel-sf ;

: saw-sf { ibuf obuf fpb ti sf ud -- n } obuf fpb 0 U+DO step-saw-sf v. sf! 4 +  v. sf! 4 + LOOP drop 0 ;
: saw-sf2 2drop 2drop 2drop 0e 0 ;

' saw-sf2 pa-stream-callback: constant saw-sf-cb

: main-sf
  pa-initialize throw
  ." Initialized" cr
  stream 0 2 0x1 44100e 256 saw-sf-cb 0 pa-open-default-stream throw
  ." Created stream" cr
  stream @ pa-start-stream throw
  ." Started stream" cr
  1000 pa-sleep
  stream @ pa-stop-stream throw
  ." Stopped stream" cr
  stream @ pa-close-stream throw
  pa-terminate
  bye ;

require portaudio.fs
require wav.fs
require ring.fs

s" ../tracker/samples/SnareDrum/SnareDrum0003.wav" slurp-file drop constant sample

sample get-chunks constant chunks
chunks get-fmt-chunk constant fmt-chunk
chunks get-data-chunk constant data-chunk
fmt-chunk fmt->byterate constant brate
data-chunk data->samps constant samps
data-chunk data->n constant nsamps

create samp-ptr 0 ,
: rem-samps ( -- n ) nsamps samp-ptr @ - ;
: cur-samp ( -- c-addr ) samps samp-ptr @ brate * + ;

: cb { ibuf obuf fpb ti sf ud -- n } obuf fpb brate * erase  cur-samp obuf rem-samps fpb min  dup samp-ptr +!  brate * cmove  rem-samps 0= 1 and ;

create stream 0 ,
' cb pa-stream-callback: constant wav-cb

: main
  pa-initialize throw
  ." Initialized" cr
  stream 0  fmt-chunk fmt->chans  0x8  fmt-chunk fmt->sratef  256 wav-cb  0 pa-open-default-stream throw
  ." Created stream" cr
  stream @ pa-start-stream throw
  ." Started stream" cr
  1000 pa-sleep
  stream @ pa-stop-stream throw
  ." Stopped stream" cr
  stream @ pa-close-stream throw
  pa-terminate
  bye ;

fmt-chunk fmt->srate to sample-rate
: samp++ ( -- n ) cur-samp sw@  1 samp-ptr +!  ;
init-ring constant ring
: fill-ring ring ring-capacity 1- dup 100 < IF drop exit THEN
    rem-samps min dup >r 0 U+DO
    samp++ dup i ring ring-frame+!
  LOOP
  r> ring ring-adv-write ;

: fill-loop BEGIN
    rem-samps WHILE
    fill-ring  10 pa-sleep
  REPEAT ;
: drain-ring BEGIN ring ring-count WHILE 10 pa-sleep REPEAT ;


: check-ring-count ring ring-count 256 < IF ." UNDERFLOW" cr THEN ;
: cb-r { ibuf obuf fpb ti sf ud -- n } obuf fpb brate * erase  check-ring-count  obuf fpb ring ring-read drop  ring ring-count IF 0 ELSE 1 THEN ;
' cb-r pa-stream-callback: constant ring-cb

: main-ring
  fill-ring
  ring ring-capacity . cr
  pa-initialize throw
  ." Initialized" cr
  stream 0  2  0x8  fmt-chunk fmt->sratef  256 ring-cb  0 pa-open-default-stream throw
  ." Created stream" cr
  stream @ pa-start-stream throw
  ." Started stream" cr
  fill-loop
  drain-ring
  stream @ pa-stop-stream throw
  ." Stopped stream" cr
  stream @ pa-close-stream throw
  pa-terminate
  bye ;

main-ring
\ main

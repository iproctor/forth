require portaudio.fs
require wav.fs

s" ../tracker/samples/SnareDrum/SnareDrum0005.wav" slurp-file drop constant sample

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
  4000 pa-sleep
  stream @ pa-stop-stream throw
  ." Stopped stream" cr
  stream @ pa-close-stream throw
  pa-terminate
  bye ;

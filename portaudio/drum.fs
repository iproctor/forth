require portaudio.fs
require ../utils.fs

s" ../tracker/samples/SnareDrum/SnareDrum0005.wav" slurp-file drop constant sample
: get-chunks ( c-addr -- c-addr ) 12 + ;
: skip-chunk ( c-addr -- c-addr ) dup 4 + ul@ + 8 + ;
: get-chunk ( c-addr c-addr u -- c-addr ) BEGIN
    3dup 4 -rot str= IF 2drop exit THEN
    2>r skip-chunk 2r>
  AGAIN ;
: get-fmt-chunk ( c-addr -- c-addr ) s" fmt " get-chunk ;
: fmt->fcode ( c-addr -- n ) 8 + uw@ ;
: fmt->chans ( c-addr -- n ) 10 + uw@ ;
: fmt->srate ( c-addr -- n ) 12 + ul@ ;
: fmt->bitrate ( c-addr -- n ) 22 + uw@ ;

: get-data-chunk ( c-addr -- c-addr ) s" data" get-chunk ;
: data->n ( c-addr -- n ) 4 + ul@ 2 / ;
: data->samps ( c-addr -- c-addr ) 8 + ;

sample get-chunks constant chunks
chunks get-fmt-chunk constant fmt-chunk
chunks get-data-chunk constant data-chunk
data-chunk data->samps constant samps
data-chunk data->n constant nsamps

create samp-ptr 0 ,
: rem-samps ( -- n ) nsamps samp-ptr @ - ;
: cur-samp ( -- c-addr ) samps samp-ptr @ 2 * + ;

: cb { ibuf obuf fpb ti sf ud -- n } obuf fpb 2 * erase  cur-samp obuf rem-samps fpb min  dup samp-ptr +!  2 * cmove  rem-samps 0= 1 and ;

create stream 0 ,
' cb pa-stream-callback: constant wav-cb

: main
  pa-initialize throw
  ." Initialized" cr
  stream 0  fmt-chunk fmt->chans  0x8  48000e  256 wav-cb  0 pa-open-default-stream throw
  ." Created stream" cr
  stream @ pa-start-stream throw
  ." Started stream" cr
  4000 pa-sleep
  stream @ pa-stop-stream throw
  ." Stopped stream" cr
  stream @ pa-close-stream throw
  pa-terminate
  bye ;

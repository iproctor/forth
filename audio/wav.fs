require ../utils.fs

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
: fmt->sratef ( c-addr -- n ) fmt->srate s>f ;
: fmt->bitrate ( c-addr -- n ) 22 + uw@ ;
: fmt->byterate ( c-addr -- n ) fmt->bitrate 8 / ;

: get-data-chunk ( c-addr -- c-addr ) s" data" get-chunk ;
: data->n ( c-addr -- n ) 4 + ul@ 2 / ;
: data->samps ( c-addr -- c-addr ) 8 + ;

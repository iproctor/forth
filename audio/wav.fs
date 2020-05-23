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
: data->n ( u c-addr -- n ) 4 + ul@ swap / ;
: data->samps ( c-addr -- c-addr ) 8 + ;

: wav->frame-sz ( c-addr -- u ) get-chunks get-fmt-chunk v. fmt->chans fmt->byterate * ;
: wav->n ( c-addr -- u ) dup >r wav->frame-sz r> get-chunks get-data-chunk data->n ;
: print-frame ( frame u -- ) dup 10000 mod 0= IF ." getting frame " . ." at " . cr ELSE 2drop THEN ;
: mk-wav->nth-frame ( compilation: c-addr -- runtime: u -- c-addr ) ]] [[ dup wav->frame-sz ]] literal * [[ get-chunks get-data-chunk data->samps ]] literal + [[ ;
: mk-read-sample ( compilation: u -- runtime: c-addr -- n ) CASE
  1 OF POSTPONE c@ ENDOF
  2 OF POSTPONE sw@ ENDOF
  4 OF POSTPONE @ ENDOF
  0 POSTPONE literal ENDCASE ;
: mk-read-2-samples ( compilation: u -- runtime: c-addr -- n n ) ]] dup [[ dup mk-read-sample dup ]] literal rot + [[ mk-read-sample ;
: mk-frame->as-stereo ( compilation: c-addr -- runtime: c-addr -- n n ) get-chunks get-fmt-chunk v. fmt->byterate fmt->chans 2 = IF mk-read-2-samples ELSE mk-read-sample POSTPONE dup THEN ;
: mk-wav->stereo-sample ( compilation: c-addr -- runtime: u -- n n ) v. mk-wav->nth-frame  mk-frame->as-stereo ;
